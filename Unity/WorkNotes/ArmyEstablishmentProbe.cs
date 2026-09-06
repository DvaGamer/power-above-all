using System;
using PowerAboveAll;

// Kaynak incelemesi için gerçek public komut rotası; player veya savaş kanıtı değildir.
public static class ArmyEstablishmentProbe
{
    static int checks;
    static void Check(bool condition, string message)
    { checks++; if (!condition) throw new Exception(message); }
    static void Success(ActionResult result) { Check(result.Ok, result.Key); }
    static string Save(CampaignState state) { return CampaignArchive.Serialize(state, false); }
    static CampaignState Reload(CampaignState state)
    {
        string before = Save(state); var loaded = CampaignArchive.Deserialize(before);
        Check(before == Save(loaded), "Archive changed state"); return loaded;
    }
    static void Advance(CampaignState state)
    {
        string before = Save(state); var f = CampaignCore.Forecast(state);
        var terms = CampaignCore.GetArmyEstablishmentTerms(state);
        Check(before == Save(state), "Read-only forecast mutated campaign");
        int gold = state.Gold, food = state.Food;
        Success(CampaignCore.NextWeek(state));
        Check(state.Gold == Math.Max(0, gold + f.NetGold), "Gold differs from current-army forecast");
        Check(state.Food == Math.Max(0, food + f.NetFood), "Food differs from current-army forecast");
        var log = state.Journal.Find(entry => entry.Key == "log.week" && entry.Week == state.Week);
        Check(log != null && log.Args[2] == f.ArmyCost.ToString() && log.Args[3] == f.NetFood.ToString(), "Week report differs from forecast");
        CampaignCore.Validate(state); Reload(state);
        Console.WriteLine("week=" + state.Week + " troops=" + state.Troops + " manpower=" + state.Manpower +
            " paid=" + f.ArmyCost + " armyFood=" + f.ArmyConsumption + " gold=" + state.Gold +
            " food=" + state.Food + " due=" + state.ArmyReductionDueWeek + " dumasRel=" +
            state.Characters.Find(person => person.Id == "dumas").Relationship + " beforeBatch=" + terms.NextBatchTroops);
    }
    public static void Main()
    {
        Console.WriteLine("kind=pure-core-probe; player-proof=false; state-mutation=public-API-only");
        var state = CampaignCore.Create();
        Check(!CampaignCore.HasArmyEstablishment(state), "Default policy must not reduce troops");
        Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
        var terms = CampaignCore.GetArmyEstablishmentTerms(state);
        Check(terms.DueWeek == 2 && terms.FirstReducedBudgetWeek == 3 && terms.NextBatchTroops == 200, "Wrong two-week plan");
        Check(terms.CurrentArmyCost == 136 && terms.ArmyCostAfterBatch == 120 &&
            terms.CurrentArmyConsumption == 40 && terms.ArmyConsumptionAfterBatch == 34, "Wrong shared leaf expenses");
        Check(terms.ManpowerAfterBatch == 2600 && terms.DumasRelationshipDelta == -4, "Wrong visible actual transfer");
        state = Reload(state); Advance(state);
        Check(state.Troops == 1200 && state.Manpower == 2400 && state.ArmyReductionDueWeek == 2, "Reduction happened early");
        Advance(state);
        Check(state.PendingPetition && state.Troops == 1000 && state.Manpower == 2600, "Week2 petition swallowed the earned batch");
        Check(state.ArmyReductionDueWeek == 0 && state.Characters.Find(person => person.Id == "dumas").Relationship == 46, "Reduction not settled exactly once");
        var f = CampaignCore.Forecast(state);
        Check(f.ArmyCost == 120 && f.ArmyConsumption == 34, "Next settlement does not save upkeep");
        string blocked = Save(state); Check(!CampaignCore.NextWeek(state).Ok && Save(state) == blocked, "Petition refusal mutated campaign");
        Success(CampaignCore.ChoosePetition(state, "negotiate"));
        int gold = state.Gold, food = state.Food, supplies = state.MilitarySupplies;
        Success(CampaignCore.Act(state, "recruit", "ile"));
        Check(state.Troops == 1200 && state.Manpower == 2400 && state.Gold == gold - 120 &&
            state.Food == food - 20 && state.MilitarySupplies == supplies - 15, "Returned reserve skipped old recruit price");
        Check(state.ArmyReductionDueWeek == 4, "Recruit did not schedule a new two-week departure");
        Success(CampaignCore.SetArmyEstablishment(state, "budget", 900));
        Check(state.ArmyReductionDueWeek == 4, "Target revision moved an existing due date");
        Success(CampaignCore.SetArmyEstablishment(state, "campaign", 0));
        Check(state.Troops == 1200 && state.Manpower == 2400 && state.ArmyReductionDueWeek == 0, "Cancellation created soldiers");
        Advance(state); Check(state.Troops == 1200, "Campaign policy retained a reduction");

        var zero = CampaignCore.Create(); Success(CampaignCore.SetArmyEstablishment(zero, "budget", 0));
        for (int week = 0; week < 12; week++)
        {
            if (zero.PendingPetition) Success(CampaignCore.ChoosePetition(zero, "negotiate"));
            Advance(zero);
            Check(zero.Troops == 1200 - 200 * (zero.Week / 2), "Batch cadence differs from two successful weeks");
            Check(zero.Troops + zero.Manpower == 3600, "Demobilization destroyed or created people");
        }
        Check(zero.Troops == 0 && zero.Manpower == 3600 && zero.ArmyReductionDueWeek == 0, "Zero target did not settle");
        Advance(zero); Check(zero.Week == 13, "Zero army became a hidden game over");
        Success(CampaignCore.SetArmyEstablishment(zero, "campaign", 0));
        Success(CampaignCore.Act(zero, "recruit", "ile"));
        Check(zero.Troops == 200 && zero.Manpower == 3400, "Zero army could not recover at the normal price");
        Reload(zero);
        Console.WriteLine("PASS checks=" + checks);
    }
}
