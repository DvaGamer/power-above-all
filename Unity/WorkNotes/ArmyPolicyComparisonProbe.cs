using System;
using System.Globalization;
using PowerAboveAll;

// Altı eşit başlangıç,24 gerçek hesap. Kaynak/asker/savaş sonucu enjeksiyonu ve dosya yazımı yok.
public static class ArmyPolicyComparisonProbe
{
    sealed class Totals
    {
        public int SuccessfulWeeks, ShortageWeeks, FoodHungerWeeks, Losses, ForageWeeks, GatheredFood;
        public int Announcements, ReductionBatches, ReturnedPeople, RecruitAccepted, RecruitRefused;
        public int RecruitGold, RecruitFood, RecruitSupplies;
        public string LastShortage = "none";
    }
    static int checks;
    static void Check(bool condition, string message)
    { checks++; if (!condition) throw new Exception(message); }
    static string Save(CampaignState state) { return CampaignArchive.Serialize(state, false); }
    static string Number(float value) { return value.ToString("0.##", CultureInfo.InvariantCulture); }
    static int Count(string value) { return int.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture); }
    static CampaignState Reload(CampaignState state)
    {
        string archive = Save(state); var loaded = CampaignArchive.Deserialize(archive);
        Check(archive == Save(loaded), "Final archive did not preserve full campaign");
        return loaded;
    }
    static void Print(string id, CampaignState state, Totals totals)
    {
        string before = Save(state);
        var forecast = CampaignCore.Forecast(state); var terms = CampaignCore.GetDumasInitiativeTerms(state);
        var general = state.Characters.Find(person => person.Id == "dumas");
        Check(before == Save(state), "Read-only observation changed campaign");
        Console.WriteLine("ROW scenario=" + id + " week=" + state.Week +
            " gold=" + state.Gold + " food=" + state.Food + " troops=" + state.Troops + " manpower=" + state.Manpower +
            " avgUnrest=" + Number(CampaignCore.AverageUnrest(state)) + " power=" + Number(state.Power) +
            " dumasRelation=" + Number(general.Relationship) + " supplies=" + state.MilitarySupplies +
            " supply=" + Number(state.Supply) + " morale=" + Number(state.Morale) +
            " policy=" + state.ArmyPolicyId + " target=" + state.ArmyTargetTroops + " reductionDue=" + state.ArmyReductionDueWeek +
            " nextTax=" + forecast.TaxIncome + " nextArmyCost=" + forecast.ArmyCost + " nextArmyFood=" + forecast.ArmyConsumption +
            " nextNetFood=" + forecast.NetFood + " nextForage=" + forecast.ForageFood +
            " nextDumasDisposition=" + (terms == null ? "none" : terms.Disposition) +
            " dumasDue=" + state.DumasForageDueWeek + " dumasNext=" + state.DumasNextForageWeek +
            " actualShortageWeeks=" + totals.ShortageWeeks + " actualFoodHungerWeeks=" + totals.FoodHungerWeeks +
            " actualLosses=" + totals.Losses + " actualForageWeeks=" + totals.ForageWeeks +
            " actualGatheredFood=" + totals.GatheredFood + " lastShortage=" + totals.LastShortage);
    }
    static void Recruit(string id, CampaignState state, Totals totals)
    {
        string before = Save(state);
        int gold = state.Gold, food = state.Food, supplies = state.MilitarySupplies;
        long people = (long)state.Troops + state.Manpower;
        // Core'da ayrı CanAct yok; olağan Act retleri bütün snapshot ile atomik olarak denetlenir.
        var result = CampaignCore.Act(state, "recruit", state.ArmyRegionId);
        if (!result.Ok)
        {
            totals.RecruitRefused++;
            Check(before == Save(state), "Refused recruit changed state");
        }
        else
        {
            totals.RecruitAccepted++;
            totals.RecruitGold += gold - state.Gold;
            totals.RecruitFood += food - state.Food;
            totals.RecruitSupplies += supplies - state.MilitarySupplies;
            Check((long)state.Troops + state.Manpower == people, "Recruit created or destroyed people");
            CampaignCore.Validate(state);
        }
        Console.WriteLine("ACTION scenario=" + id + " week=" + state.Week + " command=recruit region=" + state.ArmyRegionId +
            " accepted=" + result.Ok + " reason=" + result.Key + " goldPaid=" + (gold - state.Gold) +
            " foodPaid=" + (food - state.Food) + " suppliesPaid=" + (supplies - state.MilitarySupplies));
    }
    static void Advance(string id, CampaignState state, Totals totals)
    {
        string before = Save(state); var forecast = CampaignCore.Forecast(state);
        Check(before == Save(state), "Forecast changed campaign before settlement");
        int gold = state.Gold, food = state.Food;
        long people = (long)state.Troops + state.Manpower;
        bool foodHunger = (long)food + forecast.NetFood < 0;
        var result = CampaignCore.NextWeek(state);
        if (!result.Ok)
        {
            Check(before == Save(state), "Rejected week changed campaign");
            throw new Exception("Unexpected blocked comparison: " + id + " " + result.Key);
        }
        totals.SuccessfulWeeks++;
        Check(state.Gold == Math.Max(0, gold + forecast.NetGold), "Actual gold differs from old-army forecast");
        Check(state.Food == Math.Max(0, food + forecast.NetFood), "Actual food differs from old-army forecast");
        var settlement = state.Journal.Find(entry => entry.Week == state.Week && entry.Key == "log.week");
        Check(settlement != null && Count(settlement.Args[1]) == forecast.TaxIncome &&
            Count(settlement.Args[2]) == forecast.ArmyCost && Count(settlement.Args[3]) == forecast.NetFood,
            "Weekly report differs from committed forecast");
        int losses = 0, gathered = 0, shortageEntries = 0, forageEntries = 0;
        totals.LastShortage = "none";
        foreach (var entry in state.Journal)
        {
            if (entry.Week != state.Week) continue;
            if (entry.Key == "log.shortage")
            {
                shortageEntries++; losses += Count(entry.Args[0]); totals.LastShortage = entry.Args[1];
                Check((entry.Args[1] == "shortage.food") == foodHunger, "Actual hunger reason differs from committed food balance");
            }
            else if (entry.Key == "log.dumas.gathered") { forageEntries++; gathered += Count(entry.Args[1]); }
            else if (entry.Key == "log.dumas.announced") totals.Announcements++;
            else if (entry.Key == "log.establishment.reduced")
            { totals.ReductionBatches++; totals.ReturnedPeople += Count(entry.Args[0]); }
        }
        Check(shortageEntries <= 1 && forageEntries <= 1, "Duplicate settlement consequence");
        Check(!foodHunger || shortageEntries == 1, "True food shortage lacks its actual report");
        Check(gathered == forecast.ForageFood, "Actual gathering differs from preview or was applied twice");
        Check((long)state.Troops + state.Manpower == people - losses, "Reduction or shortage created/destroyed unreported people");
        Check(state.ResolvedBattles.Count == 0, "Comparison contains an artificial battle");
        CampaignCore.Validate(state);
        totals.ShortageWeeks += shortageEntries;
        if (foodHunger) totals.FoodHungerWeeks++;
        totals.Losses += losses; totals.ForageWeeks += forageEntries; totals.GatheredFood += gathered;
        if (shortageEntries > 0 || forageEntries > 0)
            Console.WriteLine("EVENT scenario=" + id + " week=" + state.Week + " actualShortage=" + totals.LastShortage +
                " losses=" + losses + " actualGatheredFood=" + gathered);
    }
    static void Run(string id, int target, bool recruit, string baseline)
    {
        var state = CampaignCore.Create(); var totals = new Totals();
        Check(Save(state) == baseline, "Scenario did not start from identical legacy state");
        if (target >= 0) Check(CampaignCore.SetArmyEstablishment(state, "budget", target).Ok, "Budget policy refused");
        for (int week = 0; week < 24; week++)
        {
            Check(state.Week == week, "Comparison did not advance one real week");
            if (state.PendingPetition)
            {
                var reply = CampaignCore.ChoosePetition(state, "negotiate");
                Check(reply.Ok, "Common negotiate response refused");
                Console.WriteLine("ACTION scenario=" + id + " week=" + week + " command=petition-negotiate accepted=True");
            }
            if (recruit && (week == 0 || week == 2 || week == 4)) Recruit(id, state, totals);
            // Week0 satırı başlangıç emirlerinden sonra, ilk hesaptan öncedir; diğerleri biten gerçek hesaptır.
            if (week == 0) Print(id, state, totals);
            Advance(id, state, totals);
            if (state.Week == 6 || state.Week == 12 || state.Week == 24) Print(id, state, totals);
        }
        Check(totals.SuccessfulWeeks == 24 && state.Week == 24, "Scenario lacks24 successful settlements");
        Check((long)state.Troops + state.Manpower == 3600L - totals.Losses, "Final people conservation failed");
        state = Reload(state);
        Console.WriteLine("SUMMARY scenario=" + id + " successfulWeeks=" + totals.SuccessfulWeeks +
            " recruitsAccepted=" + totals.RecruitAccepted + " recruitsRefused=" + totals.RecruitRefused +
            " recruitGold=" + totals.RecruitGold + " recruitFood=" + totals.RecruitFood + " recruitSupplies=" + totals.RecruitSupplies +
            " actualShortageWeeks=" + totals.ShortageWeeks + " actualFoodHungerWeeks=" + totals.FoodHungerWeeks +
            " actualLosses=" + totals.Losses + " actualForageWeeks=" + totals.ForageWeeks + " actualGatheredFood=" + totals.GatheredFood +
            " announcements=" + totals.Announcements + " actualReductionBatches=" + totals.ReductionBatches +
            " actualReturnedPeople=" + totals.ReturnedPeople + " finalArchiveEqual=True");
        Console.WriteLine("FINAL-ARCHIVE scenario=" + id + " " + Save(state));
    }
    public static void Main()
    {
        Console.WriteLine("kind=pure-core-comparison; player-proof=false; scenarios=6; successfulWeeksEach=24; state-mutation=public-API-only");
        Console.WriteLine("scope=identical-legacy-start; petition=negotiate; subsidy=unchanged-off; no-war; no-new-mandates-or-accords; week0-row=after-opening-orders");
        string baseline = Save(CampaignCore.Create());
        Run("campaign", -1, false, baseline);
        Run("budget1000", 1000, false, baseline);
        Run("budget600", 600, false, baseline);
        Run("budget0", 0, false, baseline);
        Run("campaign-recruit024", -1, true, baseline);
        Run("budget1000-recruit024", 1000, true, baseline);
        Console.WriteLine("PASS checks=" + checks + "; policy-ranking=not-inferred; battlefield-capacity-and-future-risks-not-tested");
    }
}
