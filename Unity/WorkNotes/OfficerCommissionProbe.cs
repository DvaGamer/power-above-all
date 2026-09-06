using System;
using PowerAboveAll;

// Bu rota yalnız gerçek public komutlar kullanır; player veya savaş sonucu kanıtı değildir.
public static class OfficerCommissionProbe
{
    static int checks;
    static void Check(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static void Success(ActionResult result) { Check(result.Ok, result.Key); }
    static string Save(CampaignState state) { return CampaignArchive.Serialize(state, false); }
    static CampaignState Reload(CampaignState state)
    {
        string before = Save(state); var loaded = CampaignArchive.Deserialize(before);
        Check(before == Save(loaded), "Archive differs"); return loaded;
    }
    static void Refused(CampaignState state, Func<ActionResult> action, string reason)
    {
        string before = Save(state); var result = action();
        Check(!result.Ok && result.Key == reason && Save(state) == before, "Non-atomic refusal: " + result.Key);
    }
    static void Advance(CampaignState state)
    {
        var f = CampaignCore.Forecast(state); int gold = state.Gold, food = state.Food;
        Success(CampaignCore.NextWeek(state));
        Check(state.Gold == Math.Max(0, gold + f.NetGold) && state.Food == Math.Max(0, food + f.NetFood), "Weekly forecast differs");
        Check(!state.DumasExtraRecruitUsed, "A successful week did not reset extra recruitment");
        CampaignCore.Validate(state); Reload(state);
    }
    static void Print(string route, CampaignState state)
    {
        var f = CampaignCore.Forecast(state); var terms = CampaignCore.GetOfficerCommissionTerms(state);
        Console.WriteLine(route + " week=" + state.Week + " gold=" + state.Gold + " food=" + state.Food +
            " supplies=" + state.MilitarySupplies + " troops=" + state.Troops + " manpower=" + state.Manpower +
            " moves=" + state.Moves + " fatigue=" + state.Fatigue + " camp=" + state.ArmyRegionId +
            " active=" + terms.IsActive + " used=" + terms.ExtraRecruitUsed + " revoke=" + terms.RevokeGoldCost +
            " armyCost=" + f.ArmyCost + " armyFood=" + f.ArmyConsumption +
            " dumasLoyalty=" + state.Characters.Find(person => person.Id == "dumas").Loyalty);
    }
    public static void Main()
    {
        Console.WriteLine("kind=pure-core-probe; player-proof=false; state-mutation=public-API-only");
        var own = CampaignCore.Create();
        Success(CampaignCore.Act(own, "recruit", "ile")); Success(CampaignCore.March(own, "normandy"));
        Success(CampaignCore.Act(own, "recruit", "normandy"));
        Check(own.Troops == 1600 && own.Moves == 1 && own.Fatigue == 10, "Ordinary second-region route changed");
        Print("own-two-regions", own); Reload(own);

        var state = CampaignCore.Create("army");
        Success(CampaignCore.GrantOfficerCommission(state));
        Check(state.Characters.Find(person => person.Id == "dumas").Loyalty == 60, "Signing gave free loyalty");
        Refused(state, () => CampaignCore.RecruitThroughDumas(state), "error.commission.normal_required");
        Success(CampaignCore.Act(state, "recruit", "ile"));
        string before = Save(state); var terms = CampaignCore.GetOfficerCommissionTerms(state);
        Check(before == Save(state), "Terms mutated campaign");
        Check(terms.GoldCost == 120 && terms.FoodCost == 20 && terms.MilitarySuppliesCost == 15 && terms.ManpowerCost == 200, "Recruit price differs");
        Check(terms.LoyaltyDelta == 1 && terms.RevokeGoldCost == 117, "Expected current1400 terms");
        Success(CampaignCore.RecruitThroughDumas(state));
        Check(state.Troops == 1600 && state.Manpower == 2000 && state.Gold == 600 && state.Food == 320 && state.MilitarySupplies == 90, "Extra group not paid exactly once");
        Check(state.Moves == 2 && state.Fatigue == 0 && state.ArmyRegionId == "ile", "Commission secretly charged travel");
        Check(state.Characters.Find(person => person.Id == "dumas").Loyalty == 61, "Paid group did not create its exact loyalty");
        Print("dumas-one-camp", state); state = Reload(state);
        Refused(state, () => CampaignCore.SetArmyEstablishment(state, "budget", 1000), "error.establishment.commission");
        Success(CampaignCore.RevokeOfficerCommission(state));
        Check(state.Gold == 466 && state.Troops == 1600 && state.DumasExtraRecruitUsed, "Revoke changed troops or forgot usage");
        Success(CampaignCore.GrantOfficerCommission(state));
        Refused(state, () => CampaignCore.RecruitThroughDumas(state), "error.commission.used");
        Success(CampaignCore.March(state, "normandy"));
        Success(CampaignCore.Act(state, "recruit", "normandy"));
        Refused(state, () => CampaignCore.RecruitThroughDumas(state), "error.commission.used");
        state = Reload(state); Print("regrant-and-travel", state);
        Success(CampaignCore.RevokeOfficerCommission(state));
        Check(state.Gold == 196 && state.Troops == 1800, "Live-price revoke differs after ordinary recruitment");
        Success(CampaignCore.SetArmyEstablishment(state, "budget", 1400));
        Advance(state); Check(state.Troops == 1800 && state.ArmyReductionDueWeek == 2, "Revoke accelerated demobilization");
        Advance(state); Check(state.Troops == 1600 && state.Manpower == 2000 && state.PendingPetition, "Original reduction/petition order changed");
        Refused(state, () => CampaignCore.GrantOfficerCommission(state), "error.mandate.petition");
        Success(CampaignCore.ChoosePetition(state, "negotiate"));
        Refused(state, () => CampaignCore.GrantOfficerCommission(state), "error.commission.policy");
        Advance(state); Advance(state); Check(state.Troops == 1400 && state.ArmyReductionDueWeek == 0, "Second reduction did not settle");
        Print("rights-reclaimed-army-reduced", state); Reload(state);
        Console.WriteLine("PASS checks=" + checks);
    }
}
