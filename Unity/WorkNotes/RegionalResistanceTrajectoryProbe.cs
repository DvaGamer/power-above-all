using System;
using System.Collections.Generic;
using System.Globalization;
using PowerAboveAll;

// İki politika dışında aynı24 başarılı hafta; kaynak state'ine doğrudan yazılmaz.
public static class RegionalResistanceTrajectoryProbe
{
    const int Weeks = 24;
    static int checks;
    static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
    sealed class RegionHistory
    {
        public string Id, Transitions = "";
        public int InitialEnemy, FinalEnemy, FirstHostile = -1, PeakEnemy, PeakWeek;
        public float InitialUnrest, InitialControl, InitialElite, FinalUnrest, FinalControl, FinalElite;
        public bool PreviousHostile;
    }
    sealed class Route
    {
        public string Name;
        public CampaignState State;
        public readonly List<RegionHistory> Regions = new List<RegionHistory>();
        public int HungerWeeks, UnpaidWeeks, ForageWeeks, ForageFood, Released, Lost;
        public int PeakEnemy, PeakWeek;
        public string PeakRegion = "";
    }
    static void Check(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static void Success(ActionResult result) { Check(result.Ok, result.Key); }
    static string Save(CampaignState state) { return CampaignArchive.Serialize(state, false); }
    static string F(float value) { return value.ToString("0.###", Invariant); }
    static string Scores(float unrest, float control, float elite) { return F(unrest) + "/" + F(control) + "/" + F(elite); }
    static int StockAfter(int before, int change) { return (int)Math.Max(0L, Math.Min(100000000L, (long)before + change)); }

    static void Observe(Route route)
    {
        var state = route.State; string before = Save(state);
        int maximum = 0, hostileCount = 0; string maximumRegion = "";
        foreach (var definition in CampaignCore.Regions)
        {
            var local = CampaignCore.Region(state, definition.Id);
            var terms = CampaignCore.GetRegionalResistance(state, definition.Id);
            Check(terms != null, "Missing valid regional terms: " + definition.Id);
            Check(terms.RequiresBattle ? terms.EnemyTroops > 0 : terms.EnemyTroops == 0, "Peace/force mismatch");
            var history = route.Regions.Find(item => item.Id == definition.Id);
            if (history == null)
            {
                history = new RegionHistory { Id = definition.Id, InitialEnemy = terms.EnemyTroops,
                    InitialUnrest = local.Unrest, InitialControl = local.Control, InitialElite = local.EliteLoyalty };
                route.Regions.Add(history);
            }
            if (terms.RequiresBattle && history.FirstHostile < 0) history.FirstHostile = state.Week;
            if (state.Week == 0 || terms.RequiresBattle != history.PreviousHostile)
            {
                history.Transitions += (history.Transitions.Length == 0 ? "" : ";") + state.Week + (terms.RequiresBattle ? ":hostile" : ":peace");
                if (state.Week > 0)
                    Console.WriteLine("TRANSITION," + route.Name + "," + state.Week + "," + definition.Id + "," +
                        (terms.RequiresBattle ? "hostile" : "peace") + "," + Scores(local.Unrest, local.Control, local.EliteLoyalty) + "," + terms.EnemyTroops);
            }
            history.PreviousHostile = terms.RequiresBattle;
            history.FinalUnrest = local.Unrest; history.FinalControl = local.Control; history.FinalElite = local.EliteLoyalty;
            history.FinalEnemy = terms.EnemyTroops;
            if (terms.EnemyTroops > history.PeakEnemy) { history.PeakEnemy = terms.EnemyTroops; history.PeakWeek = state.Week; }
            if (terms.EnemyTroops > maximum) { maximum = terms.EnemyTroops; maximumRegion = definition.Id; }
            if (terms.RequiresBattle) hostileCount++;
        }
        Check(before == Save(state), "Observation mutated " + route.Name + " week" + state.Week);
        if (maximum > route.PeakEnemy) { route.PeakEnemy = maximum; route.PeakWeek = state.Week; route.PeakRegion = maximumRegion; }
        Console.WriteLine("RESISTANCE_WEEK," + route.Name + "," + state.Week + "," + hostileCount + "," + maximumRegion + "," + maximum);
    }

    static Route Run(string name, bool budget)
    {
        var route = new Route { Name = name, State = CampaignCore.Create() };
        var state = route.State;
        if (budget) Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
        Observe(route);
        for (int week = 1; week <= Weeks; week++)
        {
            string before = Save(state);
            var forecast = CampaignCore.Forecast(state);
            var initiative = CampaignCore.GetDumasInitiativeTerms(state);
            Check(before == Save(state), "Forecast/initiative mutated campaign");
            int troops = state.Troops, manpower = state.Manpower, gold = state.Gold, food = state.Food;
            bool hunger = (long)food + forecast.NetFood < 0, unpaid = (long)gold + forecast.NetGold < 0;
            Check(forecast.ForageFood == (initiative == null ? 0 : initiative.FoodGathered), "Forecast and initiative differ");
            Success(CampaignCore.NextWeek(state));
            Check(state.Week == week, "Week did not advance once");
            Check(state.Gold == StockAfter(gold, forecast.NetGold) && state.Food == StockAfter(food, forecast.NetFood), "Budget differs from shared preview");
            int released = state.Manpower - manpower, lost = troops - state.Troops - released;
            Check(released >= 0 && lost >= 0, "Created soldiers or silently discarded reserve");
            if (hunger) route.HungerWeeks++; if (unpaid) route.UnpaidWeeks++;
            if (forecast.ForageFood > 0) route.ForageWeeks++;
            route.ForageFood += forecast.ForageFood; route.Released += released; route.Lost += lost;
            if (state.PendingPetition)
            {
                Check(week == 2, "Unexpected petition timing");
                Success(CampaignCore.ChoosePetition(state, "negotiate"));
            }
            Check(state.ArmyRegionId == "ile" && state.ResolvedBattles.Count == 0 && !state.SubsidyParis, "Unplanned campaign intervention");
            CampaignCore.Validate(state);
            Console.WriteLine("ECONOMY_WEEK," + name + "," + week + "," + troops + "," + state.Troops + "," + released + "," + lost +
                "," + gold + "," + state.Gold + "," + food + "," + state.Food + "," + forecast.TaxIncome + "," + forecast.ArmyCost +
                "," + forecast.ArmyConsumption + "," + forecast.NetFood + "," + hunger + "," + unpaid + "," + forecast.ForageFood +
                "," + (initiative == null ? "none" : initiative.Disposition) + "," + F(state.Power) + "," + state.MilitarySupplies);
            Observe(route);
        }
        Check(state.PetitionResolved && state.Week == Weeks, "Incomplete route");
        string final = Save(state); var loaded = CampaignArchive.Deserialize(final);
        Check(final == Save(loaded), "Final archive does not roundtrip");
        foreach (var history in route.Regions)
            Check(CampaignCore.GetRegionalResistance(loaded, history.Id).EnemyTroops == history.FinalEnemy, "Loaded regional strength differs");
        int finalMax = 0; string finalRegion = "";
        foreach (var history in route.Regions) if (history.FinalEnemy > finalMax) { finalMax = history.FinalEnemy; finalRegion = history.Id; }
        Console.WriteLine("SUMMARY," + name + ",hungerWeeks=" + route.HungerWeeks + ",unpaidWeeks=" + route.UnpaidWeeks +
            ",forageWeeks=" + route.ForageWeeks + ",forageFood=" + route.ForageFood + ",released=" + route.Released + ",lost=" + route.Lost +
            ",troops=" + state.Troops + ",manpower=" + state.Manpower + ",gold=" + state.Gold + ",food=" + state.Food +
            ",finalEnemyMax=" + finalMax + ",finalMaxRegion=" + finalRegion + ",trajectoryPeak=" + route.PeakEnemy +
            ",peakRegion=" + route.PeakRegion + ",peakWeek=" + route.PeakWeek);
        return route;
    }

    public static void Main()
    {
        Console.WriteLine("kind=pure-core-probe; player-proof=false; state-mutation=public-API-only; weeks=24; role=legacy; petition=negotiate; optional-interventions=none");
        Console.WriteLine("RESISTANCE_WEEK,route,week,hostileRegions,maxRegion,enemyMax");
        Console.WriteLine("TRANSITION,route,week,region,status,unrest/control/elite,enemy");
        Console.WriteLine("ECONOMY_WEEK,route,week,troopsBefore,troopsAfter,released,lost,goldBefore,goldAfter,foodBefore,foodAfter,tax,armyCost,armyFood,netFood,hunger,unpaid,forageFood,plannedDumasDisposition,power,supplies");
        var campaign = Run("campaign", false); var budget = Run("budget1000", true);
        Console.WriteLine("COMPARISON,region,initialUCE,initialEnemy,campaignFinalUCE,campaignEnemy24,campaignFirstHostile,campaignTransitions,budgetFinalUCE,budgetEnemy24,budgetFirstHostile,budgetTransitions,enemyDifferenceBudgetMinusCampaign,campaignPeakAtWeek,budgetPeakAtWeek");
        foreach (var normal in campaign.Regions)
        {
            var reduced = budget.Regions.Find(item => item.Id == normal.Id);
            Check(normal.InitialEnemy == reduced.InitialEnemy && normal.InitialUnrest == reduced.InitialUnrest &&
                normal.InitialControl == reduced.InitialControl && normal.InitialElite == reduced.InitialElite, "Policy directly changed initial resistance");
            Console.WriteLine("COMPARISON," + normal.Id + "," + Scores(normal.InitialUnrest, normal.InitialControl, normal.InitialElite) +
                "," + normal.InitialEnemy + "," + Scores(normal.FinalUnrest, normal.FinalControl, normal.FinalElite) + "," + normal.FinalEnemy +
                "," + normal.FirstHostile + "," + normal.Transitions + "," + Scores(reduced.FinalUnrest, reduced.FinalControl, reduced.FinalElite) +
                "," + reduced.FinalEnemy + "," + reduced.FirstHostile + "," + reduced.Transitions + "," + (reduced.FinalEnemy - normal.FinalEnemy) +
                "," + normal.PeakEnemy + "@" + normal.PeakWeek + "," + reduced.PeakEnemy + "@" + reduced.PeakWeek);
        }
        Console.WriteLine("PASS checks=" + checks);
    }
}
