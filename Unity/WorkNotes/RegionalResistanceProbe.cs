using System;
using PowerAboveAll;

// Yalnız public kampanya komutları; gerçek oyuncu veya savaş sonucu kanıtı değildir.
public static class RegionalResistanceProbe
{
    static int checks;
    static void Check(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static void Success(ActionResult result) { Check(result.Ok, result.Key); }
    static string Save(CampaignState state) { return CampaignArchive.Serialize(state, false); }
    static RegionalResistanceTerms Observe(CampaignState state, string region, string label)
    {
        string before = Save(state);
        var terms = CampaignCore.GetRegionalResistance(state, region);
        Check(terms != null, "No resistance terms: " + label);
        Check(before == Save(state), "Preview mutated campaign: " + label);
        var loaded = CampaignArchive.Deserialize(before);
        var again = CampaignCore.GetRegionalResistance(loaded, region);
        Check(before == Save(loaded) && again.EnemyTroops == terms.EnemyTroops && again.RequiresBattle == terms.RequiresBattle,
            "Archive changed resistance: " + label);
        Console.WriteLine(label + " week=" + state.Week + " troops=" + state.Troops + " gold=" + state.Gold +
            " food=" + state.Food + " region=" + region + " hostile=" + terms.RequiresBattle + " enemy=" + terms.EnemyTroops);
        return terms;
    }
    public static void Main()
    {
        Console.WriteLine("kind=pure-core-probe; player-proof=false; state-mutation=public-API-only");
        var baseline = CampaignCore.Create();
        Check(Observe(baseline, "champagne", "baseline").EnemyTroops == 1114, "Initial resistance differs");
        Check(CampaignCore.CanMarch(baseline, "champagne").RequiresBattle, "Initial hostile march missing");

        var recruited = CampaignCore.Create("army");
        Success(CampaignCore.Act(recruited, "recruit", "ile"));
        Success(CampaignCore.GrantOfficerCommission(recruited));
        Success(CampaignCore.RecruitThroughDumas(recruited));
        Check(recruited.Troops == 1600 && recruited.Gold == 600 && recruited.Food == 320, "Paid recruitment route differs");
        Check(Observe(recruited, "champagne", "commission-1600").EnemyTroops == 1114, "Recruitment scaled the target");
        Check(Observe(recruited, "provence", "distant-peaceful").EnemyTroops == 0, "Peaceful region invented an enemy");
        Check(!CampaignCore.CanMarch(recruited, "provence").Ok, "Distant estimate enabled marching");

        var tax = CampaignCore.Create();
        Success(CampaignCore.Act(tax, "tax", "champagne"));
        Check(Observe(tax, "champagne", "emergency-tax").EnemyTroops == 1234, "Tax had no local consequence");
        Success(CampaignCore.Act(tax, "bread", "champagne"));
        Check(Observe(tax, "champagne", "tax-then-bread").EnemyTroops == 1106, "Partial relief did not reduce hostile strength");
        Check(CampaignCore.CanMarch(tax, "champagne").RequiresBattle, "Partial relief unexpectedly removed the battle");

        var accord = CampaignCore.Create("assembly");
        Success(CampaignCore.Act(accord, "tax", "champagne"));
        Success(CampaignCore.GrantRegionalAccord(accord, "champagne"));
        Check(Observe(accord, "champagne", "tax-then-holiday").EnemyTroops == 1136, "Holiday strength differs");
        Check(accord.AccordUntilWeek == 4, "Resistance changed holiday duration");
        for (int week = 1; week <= 4; week++)
        {
            Success(CampaignCore.NextWeek(accord));
            if (accord.PendingPetition) Success(CampaignCore.ChoosePetition(accord, "negotiate"));
            Observe(accord, "champagne", "holiday-week-" + week);
        }
        Check(!CampaignCore.HasRegionalAccord(accord), "Fourth tax settlement did not close the accord");

        var calm = CampaignCore.Create();
        Success(CampaignCore.GrantRegionalAccord(calm, "champagne"));
        Check(!Observe(calm, "champagne", "peace-through-holiday").RequiresBattle, "Peaceful compromise invented a smaller battle");
        Success(CampaignCore.March(calm, "champagne"));
        Check(calm.ResolvedBattles.Count == 0 && calm.ArmyRegionId == "champagne", "Peaceful march created a battle");
        Success(CampaignCore.NextWeek(calm));
        Check(!Observe(calm, "champagne", "garrison-after-peace").RequiresBattle, "Peaceful garrison invented an enemy");
        Console.WriteLine("PASS checks=" + checks);
    }
}
