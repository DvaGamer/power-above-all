using System;
using System.Globalization;
using PowerAboveAll;

// Yalnız public komutlar ve gerçek arşiv kopyaları; state enjeksiyonu/savaş sonucu yoktur.
public static class RegionalReformProbe
{
    static int checks;
    static string N(float value) { return value.ToString("0.###", CultureInfo.InvariantCulture); }
    static string Save(CampaignState state) { return CampaignArchive.Serialize(state, false); }
    static void Check(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static int Stock(int value, int delta) { return (int)Math.Max(0L, Math.Min(100000000L, (long)value + delta)); }
    static CampaignState Reload(CampaignState state, string label)
    {
        string saved = Save(state); var loaded = CampaignArchive.Deserialize(saved);
        Check(saved == Save(loaded), "Archive changed: " + label);
        Console.WriteLine("ARCHIVE," + label + ",v=" + CampaignArchive.CurrentVersion + ",week=" + loaded.Week +
            ",reform=" + loaded.ReformRegionId + "/" + loaded.ReformModeId + "/" + loaded.ReformStepsRemaining +
            ",accord=" + loaded.AccordRegionId + "/" + loaded.AccordUntilWeek + ",dumas=" + loaded.DumasForageDueWeek +
            "/" + loaded.DumasNextForageWeek + ",commission=" + loaded.DumasOfficerCommission);
        return loaded;
    }
    static void Command(CampaignState state, string route, string label, Func<ActionResult> action, string expectedError = null)
    {
        string before = Save(state); int gold = state.Gold, food = state.Food, troops = state.Troops;
        float power = state.Power; var result = action();
        Console.WriteLine("COMMAND," + route + ",week=" + state.Week + "," + label + ",ok=" + result.Ok + ",key=" + result.Key +
            ",goldDelta=" + (state.Gold - gold) + ",foodDelta=" + (state.Food - food) +
            ",troopsDelta=" + (state.Troops - troops) + ",powerDelta=" + N(state.Power - power));
        if (expectedError == null) Check(result.Ok, route + "/" + label + " rejected: " + result.Key);
        else Check(!result.Ok && result.Key == expectedError && Save(state) == before, "Non-atomic or unexpected refusal: " + label);
        CampaignCore.Validate(state);
    }
    static void Petition(CampaignState state, string route, string choice)
    {
        if (state.PendingPetition)
            Command(state, route, "petition_" + choice, () => CampaignCore.ChoosePetition(state, choice));
    }
    static void PrintTerms(CampaignState state, string label)
    {
        string before = Save(state); var t = CampaignCore.GetRegionalReformTerms(state);
        Check(t != null && Save(state) == before, "Terms are missing or mutable: " + label);
        Console.WriteLine("TERMS," + label + ",week=" + state.Week + ",region=" + t.RegionId + ",mode=" + t.ModeId +
            ",status=" + t.StatusId + ",remaining=" + t.StepsRemaining + ",readyNow=" + t.RegionReadyNow +
            ",unrest=" + N(t.RegionUnrest) + ",control=" + N(t.RegionControl) +
            ",earliestActivation=" + t.EarliestActivationWeek + ",earliestFirstBudget=" + t.EarliestFirstReformedBudgetWeek +
            ",taxWithout=" + t.WithoutReformTaxIncome + ",taxWith=" + t.WithReformTaxIncome +
            ",productionWithout=" + t.WithoutReformProduction + ",productionWith=" + t.WithReformProduction +
            ",forageWithout=" + t.WithoutReformForageFood + ",forageWith=" + t.WithReformForageFood +
            ",netFoodWithout=" + t.WithoutReformNetFood + ",netFoodWith=" + t.WithReformNetFood +
            ",completionRelation=" + N(t.CompletionRelationshipDelta) + ",endRelation=" + N(t.EndRelationshipDelta));
    }
    static EconomyForecast Settle(CampaignState state, string route)
    {
        string before = Save(state); var f = CampaignCore.Forecast(state);
        var initiative = CampaignCore.GetDumasInitiativeTerms(state);
        Check(Save(state) == before, "Forecast changed state: " + route);
        int gold = state.Gold, food = state.Food, troops = state.Troops, oldWeek = state.Week;
        Command(state, route, "next_week", () => CampaignCore.NextWeek(state));
        Check(state.Week == oldWeek + 1 && state.Gold == Stock(gold, f.NetGold) && state.Food == Stock(food, f.NetFood),
            "Actual settlement differs from forecast: " + route);
        var log = state.Journal.Find(entry => entry.Week == state.Week && entry.Key == "log.week");
        Check(log != null && log.Args[1] == f.TaxIncome.ToString(CultureInfo.InvariantCulture) &&
            log.Args[2] == f.ArmyCost.ToString(CultureInfo.InvariantCulture) && log.Args[3] == f.NetFood.ToString(CultureInfo.InvariantCulture),
            "Week journal differs from accepted budget: " + route);
        Console.WriteLine("SETTLEMENT," + route + ",week=" + state.Week + ",tax=" + f.TaxIncome + ",armyCost=" + f.ArmyCost +
            ",production=" + f.Production + ",armyFood=" + f.ArmyConsumption + ",subsidyFood=" + f.SubsidyConsumption +
            ",forage=" + f.ForageFood + ",netFood=" + f.NetFood + ",dumas=" + (initiative == null ? "none" : initiative.Disposition) +
            ",gold=" + state.Gold + ",food=" + state.Food + ",troops=" + state.Troops + ",lost=" + (troops - state.Troops) +
            ",power=" + N(state.Power) + ",reformRemaining=" + state.ReformStepsRemaining);
        return f;
    }
    static void Begin(CampaignState state, string route, string region, string mode)
    {
        string before = Save(state); var terms = CampaignCore.GetRegionalReformTerms(state, region, mode);
        Check(terms != null && Save(state) == before, "Proposed terms unavailable or mutable");
        int gold = state.Gold; float power = state.Power;
        Command(state, route, "begin_" + region + "_" + mode, () => CampaignCore.BeginRegionalReform(state, region, mode));
        Check(state.Gold == gold - terms.GoldCost && state.Power == power - terms.PowerCost &&
            state.ReformStepsRemaining == CampaignCore.RegionalReformPreparationWeeks, "Initial price or preparation differs");
        PrintTerms(state, route + "-signed");
    }
    static void End(CampaignState state, string route)
    {
        var terms = CampaignCore.GetRegionalReformTerms(state);
        var sponsor = state.Characters.Find(person => person.Id == terms.SponsorId);
        int gold = state.Gold, food = state.Food; float power = state.Power, relationship = sponsor.Relationship;
        Command(state, route, "end_project", () => CampaignCore.EndRegionalReform(state));
        Check(state.Gold == gold && state.Food == food && state.Power == power &&
            sponsor.Relationship == relationship + terms.EndRelationshipDelta && !CampaignCore.HasRegionalReform(state),
            "Cancellation refunded payment or differs from current relationship terms");
        PrintTerms(state, route + "-closed");
    }
    static void CompareNormandy()
    {
        var baseline = CampaignCore.Create();
        var provisioning = Reload(baseline, "normandy-provisioning-seed");
        var commerce = Reload(baseline, "normandy-commerce-seed");
        Begin(provisioning, "normandy_provisioning", "normandy", "provisioning");
        Begin(commerce, "normandy_commerce", "normandy", "commerce");
        Command(provisioning, "normandy_provisioning", "duplicate_begin", () => CampaignCore.BeginRegionalReform(provisioning, "ile", "commerce"), "error.reform.open");
        provisioning = Reload(provisioning, "normandy-pending");
        for (int week = 1; week <= 4; week++)
        {
            var a = Settle(baseline, "normandy_baseline"); var b = Settle(provisioning, "normandy_provisioning"); var c = Settle(commerce, "normandy_commerce");
            Check(a.TaxIncome == b.TaxIncome && a.TaxIncome == c.TaxIncome && a.Production == b.Production && a.Production == c.Production,
                "Project affected one of its four preparation budgets");
            Check(provisioning.ReformStepsRemaining == 4 - week && commerce.ReformStepsRemaining == 4 - week,
                "Normandy did not earn its eligible step");
            if (week == 2)
            {
                Command(provisioning, "normandy_provisioning", "end_during_petition", () => CampaignCore.EndRegionalReform(provisioning), "error.mandate.petition");
                Command(provisioning, "normandy_provisioning", "week_during_petition", () => CampaignCore.NextWeek(provisioning), "error.petition.pending");
            }
            Petition(baseline, "normandy_baseline", "negotiate"); Petition(provisioning, "normandy_provisioning", "negotiate"); Petition(commerce, "normandy_commerce", "negotiate");
            Check(provisioning.Gold == baseline.Gold - CampaignCore.RegionalReformGoldCost && commerce.Gold == provisioning.Gold &&
                provisioning.Food == baseline.Food && commerce.Food == baseline.Food, "Preparation charged hidden recurring costs");
        }
        provisioning = Reload(provisioning, "normandy-active"); commerce = Reload(commerce, "commerce-active");
        var p = CampaignCore.GetRegionalReformTerms(provisioning); var cTerms = CampaignCore.GetRegionalReformTerms(commerce);
        Check(p.StatusId == "active" && cTerms.StatusId == "active" && p.TaxIncomeDelta < 0 && p.ProductionDelta > 0 &&
            cTerms.TaxIncomeDelta > 0 && cTerms.ProductionDelta < 0, "Modes did not create distinct actual economic tradeoffs");
        PrintTerms(provisioning, "normandy-before-first-budget"); PrintTerms(commerce, "commerce-before-first-budget");
        var baseF = Settle(baseline, "normandy_baseline"); var pF = Settle(provisioning, "normandy_provisioning"); var cF = Settle(commerce, "normandy_commerce");
        Check(pF.TaxIncome - baseF.TaxIncome == p.TaxIncomeDelta && pF.Production - baseF.Production == p.ProductionDelta &&
            cF.TaxIncome - baseF.TaxIncome == cTerms.TaxIncomeDelta && cF.Production - baseF.Production == cTerms.ProductionDelta,
            "First actual budget differs from conditional terms at the same conditions");
        Console.WriteLine("COMPARISON,firstReformedBudget=5,provisioningGoldVsBaseline=" + (provisioning.Gold - baseline.Gold) +
            ",provisioningFoodVsBaseline=" + (provisioning.Food - baseline.Food) + ",commerceGoldVsBaseline=" + (commerce.Gold - baseline.Gold) +
            ",commerceFoodVsBaseline=" + (commerce.Food - baseline.Food));
        var cancelled = Reload(provisioning, "active-cancellation-seed"); End(cancelled, "cancel_active");
        cancelled = Reload(cancelled, "cancelled-no-refund");
        Check(CampaignCore.Forecast(cancelled).TaxIncome == CampaignCore.Forecast(baseline).TaxIncome &&
            CampaignCore.Forecast(cancelled).Production == CampaignCore.Forecast(baseline).Production, "Ended project still changes economy");
        Begin(cancelled, "restart_paid", "normandy", "commerce");
        End(cancelled, "cancel_pending"); Reload(cancelled, "restart-and-pending-cancellation");
    }
    static void RecoverBlockedChampagne()
    {
        var state = CampaignCore.Create(); Begin(state, "champagne_recovery", "champagne", "provisioning");
        Check(CampaignCore.GetRegionalReformTerms(state).StatusId == "blocked", "Initially hostile Champagne should wait");
        Settle(state, "champagne_recovery"); Settle(state, "champagne_recovery");
        Check(state.ReformStepsRemaining == 4, "Blocked preparation advanced");
        Petition(state, "champagne_recovery", "negotiate");
        state = Reload(state, "blocked-two-weeks"); PrintTerms(state, "blocked-before-recovery");
        Command(state, "champagne_recovery", "bread_champagne", () => CampaignCore.Act(state, "bread", "champagne"));
        Settle(state, "champagne_recovery");
        Check(state.ReformStepsRemaining == 3, "Real bread did not restore one eligible preparation step");
        Command(state, "champagne_recovery", "accord_champagne", () => CampaignCore.GrantRegionalAccord(state, "champagne"));
        PrintTerms(state, "same-region-accord-pending");
        Check(CampaignCore.GetRegionalReformTerms(state).TaxIncomeDelta == 0, "Same-region holiday preview charged exempt tax");
        while (state.Week < 6) Settle(state, "champagne_recovery");
        Check(state.ReformStepsRemaining == 0 && state.AccordUntilWeek == 7 && CampaignCore.HasRegionalAccord(state),
            "Two blocked weeks plus four eligible steps should activate at week6 while holiday remains");
        state = Reload(state, "active-reform-active-holiday"); PrintTerms(state, "recovery-before-first-budget");
        var active = CampaignCore.GetRegionalReformTerms(state); var first = Settle(state, "champagne_recovery");
        Check(first.TaxIncome == active.WithReformTaxIncome && first.Production == active.WithReformProduction &&
            !CampaignCore.HasRegionalAccord(state), "Fourth holiday settlement and first reform budget disagree");
        var expired = CampaignCore.GetRegionalReformTerms(state);
        Check(expired.TaxIncomeDelta < 0 && expired.ProductionDelta > 0, "After tax holiday expires reform tradeoff should reappear");
        PrintTerms(state, "holiday-completed-tax-tradeoff-visible"); Settle(state, "champagne_recovery"); Reload(state, "recovery-final");
    }
    static void CombinedDumasRoute()
    {
        var state = CampaignCore.Create();
        Command(state, "dumas_combined", "commission_grant", () => CampaignCore.GrantOfficerCommission(state));
        Command(state, "dumas_combined", "subsidy_on", () => CampaignCore.Act(state, "subsidy", "ile"));
        for (int week = 0; week < 8; week++)
        {
            Petition(state, "dumas_combined", "relief");
            if (week % 2 == 0) Command(state, "dumas_combined", "normal_recruit", () => CampaignCore.Act(state, "recruit", state.ArmyRegionId));
            if (week == 4) Begin(state, "dumas_combined", "normandy", "provisioning");
            Settle(state, "dumas_combined");
        }
        Check(state.Week == 8 && state.ReformStepsRemaining == 0 && state.DumasForageDueWeek == 9 && state.DumasNextForageWeek == 12 &&
            state.DumasOfficerCommission, "Known eight-week hunger route did not coexist with completed preparation and commission");
        Command(state, "dumas_combined", "accord_orleans", () => CampaignCore.GrantRegionalAccord(state, "orleans"));
        state = Reload(state, "reform-dumas-accord-commission");
        var terms = CampaignCore.GetRegionalReformTerms(state); var initiative = CampaignCore.GetDumasInitiativeTerms(state);
        Check(initiative != null && initiative.Disposition == "gather" && terms.WithReformForageFood > 0 && terms.WithoutReformForageFood > terms.WithReformForageFood &&
            terms.NetFoodDelta == 0, "This reachable food relief comparison should replace part of forage, not claim extra stored food");
        PrintTerms(state, "active-project-with-dumas-due");
        var ended = Reload(state, "combined-cancel-seed"); End(ended, "combined_end");
        Check(ended.DumasForageDueWeek == 9 && ended.DumasNextForageWeek == 12 && ended.AccordRegionId == "orleans" &&
            ended.AccordUntilWeek == 12 && ended.DumasOfficerCommission, "Ending project changed independent promises or rights");
        var without = CampaignCore.Forecast(ended);
        Check(without.TaxIncome == terms.WithoutReformTaxIncome && without.Production == terms.WithoutReformProduction &&
            without.ForageFood == terms.WithoutReformForageFood && without.NetFood == terms.WithoutReformNetFood, "Real cancellation differs from without-project lens");
        int alive = state.Troops; var withF = Settle(state, "dumas_keep_project"); var withoutF = Settle(ended, "dumas_end_project");
        Check(state.Troops == alive && ended.Troops == alive && state.DumasForageDueWeek == 0 && ended.DumasForageDueWeek == 0 &&
            state.DumasOfficerCommission && ended.DumasOfficerCommission && state.AccordRegionId == "orleans" && ended.AccordRegionId == "orleans",
            "Food relief or independent rights did not survive actual settlement");
        Console.WriteLine("COMPARISON,dumasWeek=9,projectForage=" + withF.ForageFood + ",endedForage=" + withoutF.ForageFood +
            ",projectProduction=" + withF.Production + ",endedProduction=" + withoutF.Production +
            ",projectTax=" + withF.TaxIncome + ",endedTax=" + withoutF.TaxIncome + ",survivors=" + alive);
        Reload(state, "combined-settled-project-active"); Reload(ended, "combined-settled-project-ended");
    }
    public static void Main()
    {
        Console.WriteLine("kind=pure-core-probe; player-proof=false; state-mutation=public-API-only; role=legacy; no-battles; legal-archive-branches=true");
        CompareNormandy(); RecoverBlockedChampagne(); CombinedDumasRoute();
        Console.WriteLine("PASS checks=" + checks + "; routes=normandy-paired-five-budgets/champagne-eight-week-recovery/dumas-nine-week-compound");
    }
}
