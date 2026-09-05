#if UNITY_EDITOR
using System;
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    public sealed class CampaignCoreTests
    {
        private static string Snapshot(CampaignState s) { return JsonUtility.ToJson(s); }
        private static CampaignState Clone(CampaignState s) { return JsonUtility.FromJson<CampaignState>(Snapshot(s)); }

        [Test] public void InitialStateAndGraphAreComplete()
        {
            var s=CampaignCore.Create();Assert.DoesNotThrow(()=>CampaignCore.Validate(s));
            Assert.AreEqual(12,s.Regions.Count);Assert.AreEqual(4,s.Factions.Count);Assert.AreEqual(4,s.Characters.Count);
            foreach(var region in CampaignCore.Regions)
                foreach(var neighbour in region.Neighbours)
                    CollectionAssert.Contains(Array.Find(CampaignCore.Regions,r=>r.Id==neighbour).Neighbours,region.Id);
            foreach(var faction in s.Factions)
                Assert.AreEqual(faction.Id,s.Characters.Find(c=>c.Id==faction.LeaderId).FactionId);
        }
        [Test] public void FailedActionsDoNotPartiallyMutateState()
        {
            var s=CampaignCore.Create();s.Food=39;string before=Snapshot(s);
            Assert.IsFalse(CampaignCore.Act(s,"bread","ile").Ok);Assert.AreEqual(before,Snapshot(s));
            s.Food=40;Assert.IsTrue(CampaignCore.Act(s,"bread","ile").Ok);Assert.AreEqual(0,s.Food);
            before=Snapshot(s);Assert.IsFalse(CampaignCore.Act(s,"bread","ile").Ok);Assert.AreEqual(before,Snapshot(s));
            s.Gold=120;s.Food=20;s.MilitarySupplies=14;s.Manpower=200;before=Snapshot(s);
            Assert.IsFalse(CampaignCore.Act(s,"recruit","ile").Ok);Assert.AreEqual(before,Snapshot(s));
            s.MilitarySupplies=15;Assert.IsTrue(CampaignCore.Act(s,"recruit","ile").Ok);
            Assert.AreEqual(0,s.Gold);Assert.AreEqual(0,s.Food);Assert.AreEqual(0,s.MilitarySupplies);Assert.AreEqual(0,s.Manpower);Assert.AreEqual(1400,s.Troops);
        }
        [Test] public void RecruitmentRequiresArmyAndUsesManpowerOnlyOnce()
        {
            var s=CampaignCore.Create();string before=Snapshot(s);
            Assert.IsFalse(CampaignCore.Act(s,"recruit","normandy").Ok);Assert.AreEqual(before,Snapshot(s));
            Assert.IsTrue(CampaignCore.Act(s,"recruit","ile").Ok);before=Snapshot(s);
            Assert.IsFalse(CampaignCore.Act(s,"recruit","ile").Ok);Assert.AreEqual(before,Snapshot(s));
            CampaignCore.NextWeek(s);Assert.IsFalse(CampaignCore.Region(s,"ile").RecruitUsed);
        }
        [Test] public void SubsidyConsumesFoodWeeklyAndWithdrawalHasPoliticalCost()
        {
            var control=CampaignCore.Create();var subsidised=Clone(control);
            Assert.IsTrue(CampaignCore.Act(subsidised,"subsidy","ile").Ok);
            Assert.AreEqual(20,CampaignCore.Forecast(subsidised).SubsidyConsumption);
            CampaignCore.NextWeek(control);CampaignCore.NextWeek(subsidised);
            Assert.AreEqual(control.Food-20,subsidised.Food);
            Assert.Less(CampaignCore.Region(subsidised,"ile").Unrest,CampaignCore.Region(control,"ile").Unrest);
            var urban=subsidised.Factions.Find(f=>f.Id=="urban");float approval=urban.Approval;
            Assert.IsTrue(CampaignCore.Act(subsidised,"subsidy","ile").Ok);
            Assert.IsFalse(subsidised.SubsidyParis);Assert.Less(urban.Approval,approval);
        }
        [Test] public void SupplyShortageReducesStrengthMoraleAndPoliticalPower()
        {
            var s=CampaignCore.Create();s.Gold=0;s.Food=0;s.MilitarySupplies=0;s.Troops=12000;
            float morale=s.Morale,supply=s.Supply,power=s.Power;CampaignCore.NextWeek(s);
            Assert.Less(s.Troops,12000);Assert.Less(s.Morale,morale);Assert.Less(s.Supply,supply);Assert.Less(s.Power,power);
            Assert.AreEqual(0,s.Gold);Assert.AreEqual(0,s.Food);Assert.AreEqual(0,s.MilitarySupplies);
            Assert.DoesNotThrow(()=>CampaignCore.Validate(s));
        }
        [Test] public void RegionControlAndAssemblySupportActuallyAffectTaxForecast()
        {
            var s=CampaignCore.Create();int baseline=CampaignCore.Forecast(s).TaxIncome;
            foreach(var r in s.Regions)r.Control=0;
            Assert.Less(CampaignCore.Forecast(s).TaxIncome,baseline);
            int lowControl=CampaignCore.Forecast(s).TaxIncome;s.Factions.Find(f=>f.Id=="assembly").Approval=0;
            Assert.Less(CampaignCore.Forecast(s).TaxIncome,lowControl);
        }
        [Test] public void DestroyedArmyCanRebuildMaterialReserveAndRecruit()
        {
            var s=CampaignCore.Create();s.Troops=0;s.MilitarySupplies=0;
            CampaignCore.NextWeek(s);Assert.AreEqual(18,s.MilitarySupplies);
            Assert.IsTrue(CampaignCore.Act(s,"recruit","ile").Ok);Assert.AreEqual(200,s.Troops);
        }
        [Test] public void MarchRequiresAdjacencyTroopsAndMovementAndConsumesSupplies()
        {
            var s=CampaignCore.Create();Assert.IsFalse(CampaignCore.CanMarch(s,"provence").Ok);
            Assert.IsFalse(CampaignCore.CanMarch(s,"missing").Ok);
            int food=s.Food,materials=s.MilitarySupplies;
            Assert.IsTrue(CampaignCore.March(s,"normandy").Ok);
            Assert.AreEqual("normandy",s.ArmyRegionId);Assert.Less(s.Food,food);Assert.Less(s.MilitarySupplies,materials);Assert.Greater(s.Fatigue,0);
            s.Moves=0;Assert.IsFalse(CampaignCore.CanMarch(s,"ile").Ok);
            s.Moves=2;s.Troops=0;Assert.IsFalse(CampaignCore.CanMarch(s,"ile").Ok);
        }
        [Test] public void HostileMarchCannotBypassBattleAndBattleCannotApplyTwiceAfterLoad()
        {
            var s=CampaignCore.Create();string before=Snapshot(s);
            var march=CampaignCore.March(s,"champagne");Assert.IsFalse(march.Ok);Assert.IsTrue(march.RequiresBattle);Assert.AreEqual(before,Snapshot(s));
            Assert.IsFalse(CampaignCore.ResolveBattle(s,"champagne","battle-0-1-ile-champagne",true,100,60).Ok);
            Assert.IsFalse(CampaignCore.ResolveBattle(s,"champagne","battle-0-2-ile-champagne",true,-1,60).Ok);
            Assert.IsFalse(CampaignCore.ResolveBattle(s,"champagne","battle-0-2-ile-champagne",true,100,float.NaN).Ok);
            Assert.AreEqual(before,Snapshot(s));
            Assert.IsTrue(CampaignCore.ResolveBattle(s,"champagne","battle-0-2-ile-champagne",true,100,60).Ok);
            Assert.AreEqual(1100,s.Troops);Assert.AreEqual("champagne",s.ArmyRegionId);Assert.Greater(s.Power,55);
            var loaded=Clone(s);CampaignCore.Validate(loaded);before=Snapshot(loaded);
            Assert.IsFalse(CampaignCore.ResolveBattle(loaded,"champagne","battle-0-2-ile-champagne",true,100,60).Ok);Assert.AreEqual(before,Snapshot(loaded));
        }
        [Test] public void DefeatConsumesMarchButKeepsOriginalArmyLocation()
        {
            var s=CampaignCore.Create();Assert.IsTrue(CampaignCore.ResolveBattle(s,"champagne","battle-0-2-ile-champagne",false,300,30).Ok);
            Assert.AreEqual("ile",s.ArmyRegionId);Assert.AreEqual(900,s.Troops);Assert.Less(s.Moves,2);Assert.Less(s.Power,55);Assert.Less(s.Morale,30);
        }
        [Test] public void CorruptedSavesAreRejected()
        {
            Action<CampaignState>[] corruptions={
                s=>s.Gold=-1,s=>s.Morale=float.NaN,s=>s.Supply=float.PositiveInfinity,s=>s.Moves=3,
                s=>s.SelectedRegionId="invalid",s=>s.Regions.RemoveAt(0),s=>s.Regions[0].Id=s.Regions[1].Id,
                s=>s.Factions[0].LeaderId="missing",s=>s.Characters[0].FactionId="army",s=>s.Journal=null,
                s=>s.ResolvedBattles.Add("battle-0-2-ile-provence"),
                s=>{s.ResolvedBattles.Add("battle-0-2-ile-champagne");s.ResolvedBattles.Add("battle-0-2-ile-champagne");}
            };
            foreach(var corrupt in corruptions){var s=CampaignCore.Create();corrupt(s);Assert.Throws<ArgumentException>(()=>CampaignCore.Validate(s));}
            Assert.Throws<ArgumentException>(()=>CampaignCore.Validate(null));
        }
        [Test] public void TwoHundredWeeksAreDeterministicAndRemainSaveable()
        {
            Func<CampaignState> run=()=>{
                var s=CampaignCore.Create();CampaignCore.Act(s,"subsidy","ile");
                for(int i=0;i<200;i++){CampaignCore.Act(s,"bread",CampaignCore.Regions[i%12].Id);Assert.IsTrue(CampaignCore.NextWeek(s).Ok);s=Clone(s);CampaignCore.Validate(s);}return s;
            };
            var a=run();Assert.AreEqual(200,a.Week);Assert.LessOrEqual(a.Journal.Count,40);Assert.AreEqual(Snapshot(a),Snapshot(run()));
        }
    }
}
#endif
