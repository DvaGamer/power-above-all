using System;
using System.Globalization;
using PowerAboveAll;

// Yalnız mevcut public komutlarla ulaşılmış kampanya; sınır değerleri state'e yazılmaz.
public static class DumasInitiativeProbe
{
    static int checks;
    static void Check(bool condition, string message)
    { checks++; if (!condition) throw new Exception(message); }
    static void Success(ActionResult result) { Check(result.Ok, result.Key); }
    static string Save(CampaignState state) { return CampaignArchive.Serialize(state, false); }
    static CampaignState Reload(CampaignState state)
    {
        string before = Save(state); var loaded = CampaignArchive.Deserialize(before);
        Check(before == Save(loaded), "Archive round trip differs"); return loaded;
    }
    static string Number(float value) { return value.ToString("0.##", CultureInfo.InvariantCulture); }
    static void Print(string label, CampaignState state)
    {
        var forecast = CampaignCore.Forecast(state); var terms = CampaignCore.GetDumasInitiativeTerms(state);
        Console.WriteLine(label + " week=" + state.Week + " gold=" + state.Gold + " food=" + state.Food +
            " troops=" + state.Troops + " supply=" + Number(state.Supply) + " morale=" + Number(state.Morale) +
            " power=" + Number(state.Power) + " camp=" + state.ArmyRegionId +
            " due=" + state.DumasForageDueWeek + " next=" + state.DumasNextForageWeek +
            " tax=" + forecast.TaxIncome + " production=" + forecast.Production + " forage=" + forecast.ForageFood +
            " netFood=" + forecast.NetFood + " disposition=" + (terms == null ? "none" : terms.Disposition) +
            " postLocalNeed=" + (terms == null ? 0 : terms.FoodShortfall));
    }
    static void AdvanceWithEquality(CampaignState state)
    {
        string before = Save(state); var forecast = CampaignCore.Forecast(state);
        var terms = CampaignCore.GetDumasInitiativeTerms(state);
        Check(before == Save(state), "Preview mutated state");
        int gold = state.Gold, food = state.Food, troops = state.Troops, due = state.DumasForageDueWeek;
        Success(CampaignCore.NextWeek(state));
        Check(state.Gold == Math.Max(0, gold + forecast.NetGold), "Preview gold differs");
        Check(state.Food == Math.Max(0, food + forecast.NetFood), "Preview food differs");
        var week = state.Journal.Find(entry => entry.Key == "log.week" && entry.Week == state.Week);
        Check(week != null && week.Args[1] == forecast.TaxIncome.ToString() &&
            week.Args[2] == forecast.ArmyCost.ToString() && week.Args[3] == forecast.NetFood.ToString(), "Week log differs from preview");
        if (terms != null)
        {
            string logKey = "log.dumas." + (terms.Disposition == "gather" ? "gathered" : terms.Disposition);
            var report = state.Journal.Find(entry => entry.Key == logKey && entry.Week == due);
            Check(report != null, "Initiative report must have its announced due date");
            Check(state.DumasForageDueWeek == 0, "Settled initiative remains pending");
            if (terms.Disposition == "gather")
            {
                Check(terms.FoodGathered > 0 && terms.FoodGathered <= 40 && forecast.ForageFood == terms.FoodGathered, "Transfer outside terms");
                Check(food + forecast.NetFood >= 0, "Accepted transfer did not fully close food gap");
                Check(state.Troops == troops, "Reachable fixture still loses soldiers despite food relief; inspect other shortages");
            }
            else Check(forecast.ForageFood == 0, "Cancelled initiative transferred food");
        }
        CampaignCore.Validate(state); Reload(state);
    }
    static CampaignState PrepareWeekEight()
    {
        var state = CampaignCore.Create("legacy");
        Success(CampaignCore.Act(state, "subsidy", "ile"));
        for (int week = 0; week < 8; week++)
        {
            Check(state.Week == week, "Unexpected week");
            if (state.PendingPetition) Success(CampaignCore.ChoosePetition(state, "relief"));
            if (week % 2 == 0) Success(CampaignCore.Act(state, "recruit", state.ArmyRegionId));
            Print("prepare", state); AdvanceWithEquality(state);
            if (week < 7) Check(!CampaignCore.HasDumasInitiative(state), "Hunger arrived before the recorded eight-week route");
        }
        Check(state.Week == 8 && state.DumasForageDueWeek == 9 && state.DumasNextForageWeek == 12, "Expected first notice at week8, due9, cooldown12");
        Print("reachable-week8", state); return Reload(state);
    }
    public static void Main()
    {
        Console.WriteLine("kind=pure-core-probe; player-proof=false; state-mutation=public-API-only");
        var fixture = PrepareWeekEight();
        var allow = Reload(fixture); var terms = CampaignCore.GetDumasInitiativeTerms(allow);
        Check(terms != null && terms.Disposition == "gather", "Eight-week route does not fit the40 limit: " + (terms == null ? "none" : terms.FoodShortfall.ToString()));
        int survivors = allow.Troops; AdvanceWithEquality(allow); Print("allowed-week9", allow);
        Check(allow.Troops == survivors, "Allow route should preserve soldiers in this fixture");

        var veto = Reload(fixture); int deadline = veto.DumasForageDueWeek;
        Success(CampaignCore.VetoDumasInitiative(veto, deadline));
        string afterVeto = Save(veto); var repeated = CampaignCore.VetoDumasInitiative(veto, deadline);
        Check(!repeated.Ok && Save(veto) == afterVeto, "Repeated veto is not atomic");
        AdvanceWithEquality(veto); Print("veto-week9", veto);
        Check(veto.Troops < survivors, "Veto route should retain actual old starvation loss");
        Check(veto.DumasNextForageWeek == 12, "Veto reset cooldown");

        var withdrawal = Reload(fixture); Success(CampaignCore.Act(withdrawal, "subsidy", "ile"));
        Print("withdrawal-preview", withdrawal); AdvanceWithEquality(withdrawal); Print("withdrawal-week9", withdrawal);

        var accord = Reload(fixture); string before = Save(accord);
        var proposal = CampaignCore.GetRegionalAccordTerms(accord, "orleans");
        Check(proposal != null && Save(accord) == before, "Accord preview mutated state");
        Success(CampaignCore.GrantRegionalAccord(accord, "orleans"));
        Check(CampaignCore.Forecast(accord).TaxIncome == proposal.ProjectedTaxIncome, "Accord and actual common plan disagree");
        Print("accord-preview", accord); AdvanceWithEquality(accord); Print("accord-week9", accord);
        Check(accord.AccordRegionId == "orleans" && accord.AccordUntilWeek == 12, "Initiative changed signed accord duration");

        var march = Reload(fixture); var check = CampaignCore.CanMarch(march, "normandy");
        Check(check.Ok && !check.RequiresBattle, "Expected real peaceful march to Normandy");
        Success(CampaignCore.March(march, "normandy"));
        Check(CampaignCore.GetDumasInitiativeTerms(march).RegionId == "normandy", "Initiative did not follow current camp");
        Check(march.DumasForageDueWeek == 9, "March changed announced deadline");
        Print("march-preview", march); AdvanceWithEquality(march); Print("march-week9", march);
        Console.WriteLine("PASS checks=" + checks + "; fixture=8realweeks; settlement=9; branches=allow/veto/withdrawal/accord/march");
    }
}
