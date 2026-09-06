using System;
using System.Globalization;
using PowerAboveAll;

// Gercek public API zinciri: gıda yardımı → NPC iptali → asker aktarımı → eski borcun ödenmesi.
// Alan/state/outcome enjeksiyonu ve dosya yazımı yok; merkezi runner'ı yalnız root başlatır.
public static class ArmyMandateEstablishmentProbe
{
    static int checks;
    static void Check(bool condition, string message)
    { checks++; if (!condition) throw new Exception(message); }
    static void Success(ActionResult result) { Check(result.Ok, result.Key); }
    static string Save(CampaignState state) { return CampaignArchive.Serialize(state, false); }
    static CampaignState Reload(CampaignState state)
    {
        string json = Save(state); var loaded = CampaignArchive.Deserialize(json);
        Check(json == Save(loaded), "Archive changed linked campaign"); return loaded;
    }
    static string Number(float value) { return value.ToString("0.##", CultureInfo.InvariantCulture); }
    static void Print(string label, CampaignState state)
    {
        var f = CampaignCore.Forecast(state); var dumas = state.Characters.Find(person => person.Id == "dumas");
        var camp = CampaignCore.Region(state, state.ArmyRegionId); var debt = state.Obligation;
        Console.WriteLine(label + " week=" + state.Week + " gold=" + state.Gold + " food=" + state.Food +
            " troops=" + state.Troops + " manpower=" + state.Manpower + " supplies=" + state.MilitarySupplies +
            " power=" + Number(state.Power) + " supply=" + Number(state.Supply) + " morale=" + Number(state.Morale) +
            " camp=" + state.ArmyRegionId + " unrest=" + Number(camp.Unrest) + " elite=" + Number(camp.EliteLoyalty) +
            " dumasRel=" + Number(dumas.Relationship) + " dumasAmbition=" + Number(dumas.Ambition) +
            " policy=" + state.ArmyPolicyId + " target=" + state.ArmyTargetTroops + " reductionDue=" + state.ArmyReductionDueWeek +
            " forageDue=" + state.DumasForageDueWeek + " forageNext=" + state.DumasNextForageWeek +
            " forage=" + f.ForageFood + " tax=" + f.TaxIncome + " armyCost=" + f.ArmyCost +
            " armyFood=" + f.ArmyConsumption + " netFood=" + f.NetFood +
            " debt=" + (debt == null ? "none" : CampaignCore.MandateId(debt)) +
            " debtDue=" + (debt == null ? 0 : debt.DueWeek) + " debtGold=" + (debt == null ? 0 : debt.GoldDue) +
            " mandateNext=" + state.NextMandateWeek);
    }
    static void Advance(CampaignState state)
    {
        string before = Save(state); var f = CampaignCore.Forecast(state);
        Check(before == Save(state), "Preview mutated linked campaign");
        int gold = state.Gold, food = state.Food;
        Success(CampaignCore.NextWeek(state));
        Check(state.Gold == Math.Max(0, gold + f.NetGold), "Settlement differs from old army gold preview");
        Check(state.Food == Math.Max(0, food + f.NetFood), "Settlement differs from old army food preview");
        var log = state.Journal.Find(entry => entry.Week == state.Week && entry.Key == "log.week");
        Check(log != null && log.Args[1] == f.TaxIncome.ToString() && log.Args[2] == f.ArmyCost.ToString() &&
            log.Args[3] == f.NetFood.ToString(), "Week log is not the committed forecast");
        CampaignCore.Validate(state); Reload(state); Print("settled", state);
    }
    public static void Main()
    {
        Console.WriteLine("kind=pure-core-probe; player-proof=false; state-mutation=public-API-only; finalWeek=16");
        var state = CampaignCore.Create("army");
        Success(CampaignCore.Act(state, "subsidy", "ile"));
        for (int week = 0; week < 12; week++)
        {
            if (state.PendingPetition) Success(CampaignCore.ChoosePetition(state, "relief"));
            if (week == 0 || week == 2) Success(CampaignCore.Act(state, "recruit", "ile"));
            Advance(state);
        }
        var proposal = CampaignCore.GetDumasInitiativeTerms(state);
        Check(proposal != null && proposal.Disposition == "gather", "Known preparation no longer reaches a real gathering proposal");
        Print("before-intervention", state);
        Success(CampaignCore.IssueMandate(state, "ile"));
        string obligation = CampaignCore.MandateId(state.Obligation);
        Check(state.Obligation.RegionId == "ile" && state.Obligation.GoldDue == 80 && state.Obligation.DueWeek == 14,
            "The army supply promise has unexpected original terms");
        Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
        Check(state.ArmyReductionDueWeek == 14 && state.Obligation.DueWeek == 14, "The two distinct deadlines do not align");
        Check(CampaignCore.GetDumasInitiativeTerms(state).Disposition == "sufficient", "Actual role food did not remove gathering need");
        state = Reload(state); Print("signed-two-deadlines", state);
        int troopCount = state.Troops, reserve = state.Manpower;
        Advance(state);
        Check(state.Week == 13 && state.Troops == troopCount && state.Manpower == reserve, "Army reduced before its own deadline");
        Check(state.DumasForageDueWeek == 0 && state.ArmyReductionDueWeek == 14, "NPC cancellation changed reduction date");
        Check(CampaignCore.MandateId(state.Obligation) == obligation, "NPC cancellation removed the player's own promise");
        Advance(state);
        Check(state.Week == 14 && CampaignCore.MandateDue(state), "Expected the existing promise to become due");
        Check(state.Troops < troopCount && state.Manpower > reserve, "Newly due promise swallowed the completed army batch");
        Check(CampaignCore.MandateId(state.Obligation) == obligation && state.Obligation.GoldDue == 80 &&
            state.Obligation.RegionId == "ile", "Reduction repriced or moved the original debt");
        string blocked = Save(state); var refusal = CampaignCore.NextWeek(state);
        Check(!refusal.Ok && refusal.Key == "error.mandate.due" && blocked == Save(state), "Due-debt refusal changed any state");
        state = Reload(state);
        int beforePayment = state.Gold, deadline = state.ArmyReductionDueWeek;
        Success(CampaignCore.ResolveMandate(state, obligation, "fulfil"));
        Check(state.Gold == beforePayment - 80 && state.Obligation == null, "Original compensation was not paid exactly once");
        Check(state.ArmyReductionDueWeek == deadline, "Debt payment changed the army queue");
        Print("original-debt-paid", state);
        while (state.Week < 16) Advance(state);
        Print("linked-final", Reload(state));
        Console.WriteLine("PASS checks=" + checks + "; no-player-or-battle-proof=true");
    }
}
