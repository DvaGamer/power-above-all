using System;
using PowerAboveAll;

// Yerleşim sınırı için gerçek public komutlar; görsel veya doğal savaş kanıtı değildir.
public static class ResistanceParisLayoutProbe
{
    static int checks;
    static void Check(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static void Success(ActionResult result) { Check(result.Ok, result.Key); }
    static string Save(CampaignState state) { return CampaignArchive.Serialize(state, false); }
    public static void Main()
    {
        Console.WriteLine("kind=pure-core-probe; player-proof=false; state-mutation=public-API-only; weeks=0");
        var state = CampaignCore.Create("army");
        Success(CampaignCore.Act(state, "recruit", "ile"));
        Check(state.Troops == 1400 && state.Food == 340, "Ordinary recruitment route changed");
        Success(CampaignCore.GrantRegionalAccord(state, "ile"));
        foreach (string region in new[] { "brittany", "normandy", "picardy", "champagne", "lorraine", "burgundy", "orleans", "poitou" })
        {
            Success(CampaignCore.Act(state, "bread", region));
            Console.WriteLine("STEP,bread," + region + ",food=" + state.Food);
        }
        Check(state.Food == 20, "Eight paid bread decisions did not leave20 food");
        var outward = CampaignCore.PreviewMarch(state, "normandy");
        Check(outward != null && !outward.Hungry && outward.FoodCost == 14 && outward.MovesAfter == 1, "Outward march must remain supplied");
        Success(CampaignCore.March(state, "normandy"));
        Check(state.ArmyRegionId == "normandy" && state.Troops == 1400 && state.Moves == 1 && state.Food == 6, "Final camp or food differs");
        Check(state.SelectedRegionId == "ile", "Core route should retain its initial selection without injected selection state");
        Check(state.Week == 0 && !state.PendingPetition && state.Obligation == null, "A petition or mandate blocks layout commands");
        Check(state.Gold == 720 && state.MilitarySupplies == 100 && state.Manpower == 2200, "Unexpected army resource cost");
        Check(state.Fatigue == 10 && state.Supply == 95 && state.Morale == 76, "Unexpected outward march strain");
        var paris = CampaignCore.Region(state, "ile");
        Check(paris.Unrest == 40 && paris.Control == 74 && !paris.TaxUsed && !paris.BreadUsed, "Paris local actions differ");
        Check(CampaignCore.HasRegionalAccord(state) && state.AccordRegionId == "ile" && state.AccordUntilWeek == 4,
            "Tax holiday did not remain active in Paris");

        string before = Save(state);
        var canMarch = CampaignCore.CanMarch(state, "ile");
        var returnTrip = CampaignCore.PreviewMarch(state, "ile");
        var resistance = CampaignCore.GetRegionalResistance(state, "ile");
        var accord = CampaignCore.GetActiveRegionalAccordTerms(state);
        Check(canMarch.Ok && !canMarch.RequiresBattle, "Return route should be allowed and peaceful");
        Check(returnTrip != null && returnTrip.Hungry && returnTrip.FoodCost == 14 && returnTrip.FoodAfter == 0 && returnTrip.MovesAfter == 0,
            "Paris must show the real hungry march warning");
        Check(returnTrip.Supply == 75 && returnTrip.Fatigue == 20 && returnTrip.Morale == 68, "Hungry arrival preview differs");
        Check(resistance != null && !resistance.RequiresBattle && resistance.EnemyTroops == 0, "Peaceful Paris invented an enemy");
        Check(CampaignCore.TaxBreaksRegionalAccord(state, "ile") && accord != null && accord.RemainingWeeks == 4,
            "Paris tax action should warn about breaking its four-week holiday");
        Check(before == Save(state), "Layout previews changed the campaign");
        var loaded = CampaignArchive.Deserialize(before);
        Check(before == Save(loaded), "Worst-case state did not roundtrip");
        Check(CampaignCore.PreviewMarch(loaded, "ile").Hungry && CampaignCore.TaxBreaksRegionalAccord(loaded, "ile"), "Loaded warnings differ");
        Console.WriteLine("READY,selected=ile,camp=normandy,week=0,troops=" + state.Troops + ",food=" + state.Food +
            ",moves=" + state.Moves + ",gold=" + state.Gold + ",supplies=" + state.MilitarySupplies + ",power=" + state.Power +
            ",marchCost=" + returnTrip.FoodCost + ",hungry=" + returnTrip.Hungry + ",enemy=" + resistance.EnemyTroops +
            ",taxBreaksAccord=" + CampaignCore.TaxBreaksRegionalAccord(state, "ile") + ",accordRemaining=" + accord.RemainingWeeks);
        Console.WriteLine("PASS checks=" + checks);
    }
}
