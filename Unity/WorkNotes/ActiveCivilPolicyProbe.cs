using System;
using System.Globalization;
using PowerAboveAll;

// Sabit bir sivil politika; optimizasyon, state enjeksiyonu veya savaş sonucu yoktur.
public static class ActiveCivilPolicyProbe
{
    const int Weeks = 24, FoodBuffer = 80, SubsidyRestartFood = 240;
    static readonly string[] Priorities = { "champagne", "normandy", "picardy" };
    static int checks;
    sealed class Run
    {
        public string Name;
        public CampaignState State;
        public int BreadOrders, BreadFood, AccordOrders, SubsidyStarts, SubsidyStops, Refusals;
        public int SubsidyFoodBudgeted, TaxForgone, HungerWeeks, UnpaidWeeks, ForageFood, Released, Lost;
        public int FinalHostile, FinalPriorityHostile, FinalEnemyMax;
    }
    static string N(float value) { return value.ToString("0.###", CultureInfo.InvariantCulture); }
    static string Save(CampaignState state) { return CampaignArchive.Serialize(state, false); }
    static void Check(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static int StockAfter(int before, int change) { return (int)Math.Max(0L, Math.Min(100000000L, (long)before + change)); }

    static bool Command(Run run, string command, string regionId, Func<ActionResult> action, bool required = false)
    {
        var state = run.State; string before = Save(state);
        int week = state.Week, gold = state.Gold, food = state.Food, supplies = state.MilitarySupplies;
        int troops = state.Troops, manpower = state.Manpower; float power = state.Power;
        float urban = state.Factions.Find(item => item.Id == "urban").Approval;
        var previousHead = state.Journal[0]; var result = action();
        Console.WriteLine("COMMAND," + run.Name + "," + week + "," + command + "," + regionId + "," + result.Ok + "," + result.Key +
            "," + (state.Gold - gold) + "," + (state.Food - food) + "," + (state.MilitarySupplies - supplies) + "," +
            (state.Troops - troops) + "," + (state.Manpower - manpower) + "," + N(state.Power - power) + "," +
            N(state.Factions.Find(item => item.Id == "urban").Approval - urban));
        if (!result.Ok)
        {
            run.Refusals++; Check(before == Save(state), "Rejected command mutated state: " + command);
            Check(!required, "Required command rejected: " + result.Key); return false;
        }
        foreach (var entry in state.Journal)
        {
            if (ReferenceEquals(entry, previousHead)) break;
            Console.WriteLine("JOURNAL," + run.Name + "," + entry.Week + "," + entry.Key + "," + string.Join("|", entry.Args));
        }
        if (command == "bread") { run.BreadOrders++; run.BreadFood += food - state.Food; }
        if (command == "accord") run.AccordOrders++;
        if (command == "subsidy_on") run.SubsidyStarts++;
        if (command == "subsidy_off") run.SubsidyStops++;
        CampaignCore.Validate(state); return true;
    }

    static RegionState MostUnrest(CampaignState state, float threshold)
    {
        RegionState best = null;
        foreach (string id in Priorities)
        {
            var region = CampaignCore.Region(state, id);
            if (region.Unrest >= threshold && (best == null || region.Unrest > best.Unrest)) best = region;
        }
        return best;
    }

    static void CivilOrders(Run run)
    {
        var state = run.State;
        Check(!state.PendingPetition && state.Obligation == null, "Civil policy must resolve mandatory guards first");
        var forecast = CampaignCore.Forecast(state);
        if (state.SubsidyParis && (long)state.Food + forecast.NetFood < FoodBuffer)
            Command(run, "subsidy_off", "ile", () => CampaignCore.Act(state, "subsidy", "ile"));
        else if (!state.SubsidyParis && state.Food >= SubsidyRestartFood)
            Command(run, "subsidy_on", "ile", () => CampaignCore.Act(state, "subsidy", "ile"));

        var accordTarget = MostUnrest(state, 65);
        if (!CampaignCore.HasRegionalAccord(state) && accordTarget != null)
            Command(run, "accord", accordTarget.Id, () => CampaignCore.GrantRegionalAccord(state, accordTarget.Id));
        else Console.WriteLine("DECISION," + run.Name + "," + state.Week + ",accord," +
            (CampaignCore.HasRegionalAccord(state) ? "existing_contract" : "no_priority_at65"));

        var breadTarget = MostUnrest(state, 55);
        forecast = CampaignCore.Forecast(state);
        int retainedFood = FoodBuffer + Math.Max(0, -forecast.NetFood);
        if (breadTarget != null && state.Food >= 40L + retainedFood)
            Command(run, "bread", breadTarget.Id, () => CampaignCore.Act(state, "bread", breadTarget.Id));
        else Console.WriteLine("DECISION," + run.Name + "," + state.Week + ",bread," +
            (breadTarget == null ? "no_priority_at55" : "retain_food_buffer") + ",food=" + state.Food + ",retained=" + retainedFood);
    }

    static void Observe(Run run)
    {
        var state = run.State; string before = Save(state); int hostile = 0, priorityHostile = 0, max = 0;
        string maximumRegion = "", priorityValues = "";
        foreach (var definition in CampaignCore.Regions)
        {
            var terms = CampaignCore.GetRegionalResistance(state, definition.Id);
            Check(terms != null, "No regional preview");
            if (terms.RequiresBattle) hostile++;
            if (terms.EnemyTroops > max) { max = terms.EnemyTroops; maximumRegion = definition.Id; }
            if (Array.IndexOf(Priorities, definition.Id) >= 0)
            {
                if (terms.RequiresBattle) priorityHostile++;
                var local = CampaignCore.Region(state, definition.Id);
                priorityValues += (priorityValues.Length == 0 ? "" : ";") + definition.Id + ":" +
                    N(local.Unrest) + "/" + N(local.Control) + "/" + N(local.EliteLoyalty) + ":" + terms.EnemyTroops;
            }
        }
        Check(before == Save(state), "Regional inspection mutated campaign");
        run.FinalHostile = hostile; run.FinalPriorityHostile = priorityHostile; run.FinalEnemyMax = max;
        Console.WriteLine("STATUS," + run.Name + "," + state.Week + "," + hostile + "/12," + priorityHostile + "/3," + maximumRegion + "," + max +
            "," + state.Gold + "," + state.Food + "," + state.Troops + "," + N(state.Power) + "," +
            N(state.Factions.Find(item => item.Id == "urban").Approval) + "," + state.SubsidyParis + "," + priorityValues);
    }

    static Run Simulate(string name, bool active)
    {
        var run = new Run { Name = name, State = CampaignCore.Create() }; var state = run.State;
        Command(run, "budget1000", "ile", () => CampaignCore.SetArmyEstablishment(state, "budget", 1000), true);
        if (active) CivilOrders(run);
        Observe(run);
        for (int week = 1; week <= Weeks; week++)
        {
            string before = Save(state); var forecast = CampaignCore.Forecast(state);
            var accord = CampaignCore.GetActiveRegionalAccordTerms(state);
            var initiative = CampaignCore.GetDumasInitiativeTerms(state);
            Check(before == Save(state), "Economic terms mutated campaign");
            int gold = state.Gold, food = state.Food, troops = state.Troops, manpower = state.Manpower;
            bool hunger = (long)food + forecast.NetFood < 0, unpaid = (long)gold + forecast.NetGold < 0;
            run.SubsidyFoodBudgeted += forecast.SubsidyConsumption;
            if (accord != null) run.TaxForgone += accord.TaxForgone;
            if (hunger) run.HungerWeeks++; if (unpaid) run.UnpaidWeeks++;
            run.ForageFood += forecast.ForageFood;
            Command(run, "next_week", "ile", () => CampaignCore.NextWeek(state), true);
            Check(state.Week == week && state.Gold == StockAfter(gold, forecast.NetGold) && state.Food == StockAfter(food, forecast.NetFood), "Settlement differs from preview");
            int released = state.Manpower - manpower, lost = troops - state.Troops - released;
            Check(released >= 0 && lost >= 0, "Army accounting differs"); run.Released += released; run.Lost += lost;
            Console.WriteLine("SETTLEMENT," + name + "," + week + "," + forecast.TaxIncome + "," + forecast.ArmyCost + "," + forecast.Production +
                "," + forecast.ArmyConsumption + "," + forecast.SubsidyConsumption + "," + forecast.NetFood + "," + forecast.ForageFood +
                "," + (initiative == null ? "none" : initiative.Disposition) + "," + hunger + "," + unpaid + "," +
                (accord == null ? 0 : accord.TaxForgone) + "," + released + "," + lost);
            if (state.PendingPetition)
            {
                Check(week == 2, "Unexpected petition week");
                Command(run, "petition_negotiate", "ile", () => CampaignCore.ChoosePetition(state, "negotiate"), true);
            }
            Check(state.ArmyRegionId == "ile" && state.ResolvedBattles.Count == 0 && state.Obligation == null, "Unplanned intervention");
            if (active && week < Weeks) CivilOrders(run);
            Observe(run);
        }
        string final = Save(state); var loaded = CampaignArchive.Deserialize(final);
        Check(final == Save(loaded), "Final archive changed");
        Check(state.PetitionResolved && state.Week == Weeks, "Incomplete route");
        Console.WriteLine("SUMMARY," + name + ",gold=" + state.Gold + ",food=" + state.Food + ",troops=" + state.Troops + ",manpower=" + state.Manpower +
            ",supplies=" + state.MilitarySupplies + ",power=" + N(state.Power) + ",hostile=" + run.FinalHostile + "/12,priorityHostile=" +
            run.FinalPriorityHostile + "/3,enemyMax=" + run.FinalEnemyMax + ",breadOrders=" + run.BreadOrders + ",breadFood=" + run.BreadFood +
            ",accordOrders=" + run.AccordOrders + ",taxForgone=" + run.TaxForgone + ",subsidyStarts=" + run.SubsidyStarts + ",subsidyStops=" +
            run.SubsidyStops + ",subsidyFoodBudgeted=" + run.SubsidyFoodBudgeted + ",refusals=" + run.Refusals + ",hungerWeeks=" + run.HungerWeeks +
            ",unpaidWeeks=" + run.UnpaidWeeks + ",forageFood=" + run.ForageFood + ",released=" + run.Released + ",lost=" + run.Lost);
        return run;
    }

    public static void Main()
    {
        Console.WriteLine("kind=pure-core-probe; player-proof=false; state-mutation=public-API-only; weeks=24; role=legacy; both=budget1000; petition=negotiate");
        Console.WriteLine("heuristic=priorities:champagne/normandy/picardy;subsidy_on:food>=240;subsidy_off:food+forecastNetFood<80;accord:maxPriorityUnrest>=65;bread:maxPriorityUnrest>=55,oncePerWeek,postCostRetain80+negativeNetFood");
        Console.WriteLine("COMMAND,route,issuedWeek,command,region,ok,reason,goldDelta,foodDelta,suppliesDelta,troopsDelta,manpowerDelta,powerDelta,urbanApprovalDelta");
        Console.WriteLine("SETTLEMENT,route,week,tax,armyCost,production,armyFood,subsidyFood,netFood,forageFood,plannedDumasDisposition,hunger,unpaid,taxForgone,released,lost");
        Console.WriteLine("STATUS,route,completedWeek,hostileShare,priorityHostileShare,maxRegion,enemyMax,gold,food,troops,power,urbanApproval,subsidy,priorityUCEAndEnemy");
        var baseline = Simulate("passive_budget1000", false);
        Check(baseline.State.Gold == 2292 && baseline.State.Food == 61 && baseline.State.Troops == 1000 && baseline.State.Manpower == 2600 &&
            baseline.State.Power == 67 && baseline.FinalHostile == 11 && baseline.FinalEnemyMax == 2228, "Baseline no longer matches the previous frozen24-week receipt");
        var active = Simulate("active_civil_budget1000", true);
        Console.WriteLine("FINAL_REGION,region,passiveUCE,passiveEnemy,activeUCE,activeEnemy");
        foreach (var definition in CampaignCore.Regions)
        {
            var a = CampaignCore.Region(baseline.State, definition.Id); var b = CampaignCore.Region(active.State, definition.Id);
            Console.WriteLine("FINAL_REGION," + definition.Id + "," + N(a.Unrest) + "/" + N(a.Control) + "/" + N(a.EliteLoyalty) + "," +
                CampaignCore.GetRegionalResistance(baseline.State, definition.Id).EnemyTroops + "," + N(b.Unrest) + "/" + N(b.Control) + "/" + N(b.EliteLoyalty) +
                "," + CampaignCore.GetRegionalResistance(active.State, definition.Id).EnemyTroops);
        }
        Console.WriteLine("PASS checks=" + checks);
    }
}
