using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace PowerAboveAll.Tests
{
    public sealed class WorldSupplyTests
    {
        private static WorldSimulation Create(double distance=10000)
        {
            var a=new WorldSite{Id="paris",RegionId="ile",Position=new WorldPoint(0,0)};
            var b=new WorldSite{Id="reims",RegionId="champagne",Position=new WorldPoint(distance,0)};
            return WorldSimulation.Create(CampaignCore.Create(),new[]{a,b},new[]{new WorldRoad{Id="road",From=a.Id,To=b.Id,Points=new List<WorldPoint>{a.Position,new WorldPoint(distance/2,0),b.Position}}});
        }
        private static void Relocate(WorldState w,WorldArmy army,WorldPoint point)
        {
            var delta=point-army.Position;army.Position=point;army.WagonPosition+=delta;
            foreach(var u in army.Units){u.Position+=delta;u.Destination=u.Position;}
            var h=w.Headquarters.Find(x=>x.Id==army.HeadquartersId);h.Position+=delta;h.Destination=h.Position;
        }
        [Test] public void ReroutingFromMidRoadDoesNotReturnToOldRegionSeat()
        {
            var s=Create();var a=s.State.Army("royal");Relocate(s.State,a,new WorldPoint(7000,0));
            var route=WorldRouting.Find(s.State,a,"reims");Assert.That(route.Points[0].X,Is.EqualTo(7000));
            Assert.That(WorldRouting.Remaining(route,a.Position),Is.EqualTo(3000).Within(.001));
            Assert.That(route.Points.TrueForAll(p=>p.X>=7000),Is.True);
            var reverse=WorldRouting.Find(s.State,a,"paris");Assert.That(WorldRouting.Remaining(reverse,a.Position),Is.EqualTo(7000).Within(.001));
        }
        [Test] public void BothEndsOnSameRoadUseOnlyTheRequiredSubsegment()
        {
            var s=Create();var route=WorldRouting.Between(s.State,new WorldPoint(3000,50),new WorldPoint(7200,-50));
            Assert.That(WorldRouting.Remaining(route,route.Points[0]),Is.EqualTo(4300).Within(.001));
            Assert.That(route.Points.TrueForAll(p=>p.X>=3000&&p.X<=7200),Is.True);
            Assert.That(WorldRouting.Between(s.State,new WorldPoint(3000,6000),new WorldPoint(7000,0)),Is.Null);
        }
        [Test] public void BlockedRoadIsNotBypassedByAProjectionShortcut()
        {
            var s=Create();s.State.Roads[0].Blocked=true;
            Assert.That(WorldRouting.Between(s.State,new WorldPoint(3000,0),new WorldPoint(7000,0)),Is.Null);
        }
        [Test] public void RoadSpeedAffectsActualTravelAndBlockStopsAtCurrentSegment()
        {
            var s=Create(100000);s.State.Roads[0].SpeedFactor=.5;s.March("royal","reims");s.SetSpeed(WorldSpeed.Normal);s.Advance(13);
            Assert.That(s.State.Army("royal").Position.X,Is.EqualTo(5.5).Within(.01));
            s.State.Roads[0].Blocked=true;s.Advance(2);Assert.That(s.State.Army("royal").Position.X,Is.EqualTo(5.5).Within(.01));
        }
        [Test] public void InitialFoodIsTransferredRatherThanCreatedOrChargedTwice()
        {
            int initial=CampaignCore.Create().Food;var s=Create();var w=s.State;
            Assert.That(s.Campaign.Food+w.Depots[0].Food+w.Army("royal").Rations,Is.EqualTo(initial));
            Assert.That(CampaignCore.Forecast(s.Campaign).ArmyConsumption,Is.Zero);
            var a=w.Army("royal");double before=a.Rations;
            for(int i=0;i<96;i++)WorldSupply.QuarterHour(w);
            Assert.That(before-a.Rations,Is.EqualTo(a.Men/210d).Within(1e-7));
        }
        [Test] public void ConvoyDebitsOnceMovesAndDeliversOnce()
        {
            var s=Create();var w=s.State;var a=w.Army("royal");Relocate(w,a,new WorldPoint(4000,0));
            int stock=w.Depots[0].Food;double rations=a.Rations;
            Assert.That(WorldSupply.Dispatch(s.Campaign,"royal-depot",a.Id).Ok,Is.True);
            Assert.That(WorldSupply.Dispatch(s.Campaign,"royal-depot",a.Id).Ok,Is.False);
            var c=w.Convoys[0];Assert.That(c.Position.X,Is.Zero);Assert.That(w.Depots[0].Food,Is.EqualTo(stock-40));
            WorldSupply.Step(w,1000);Assert.That(c.Position.X,Is.EqualTo(900).Within(.001));Assert.That(a.Rations,Is.EqualTo(rations));
            WorldSupply.Step(w,4000);Assert.That(c.Status,Is.EqualTo(ConvoyStatus.Delivered));Assert.That(a.Rations,Is.EqualTo(rations+40));
            WorldSupply.Step(w,4000);Assert.That(a.Rations,Is.EqualTo(rations+40));WorldValidation.Validate(s.Campaign);
        }
        [Test] public void SpeedThreeCannotSkipAnArmyCuttingTheConvoyRoad()
        {
            var s=Create();var w=s.State;var a=w.Army("royal");var b=w.Army("resistance");
            Relocate(w,a,new WorldPoint(8000,0));Relocate(w,b,new WorldPoint(4000,0));double enemyFood=b.Rations;
            WorldSupply.Dispatch(s.Campaign,"royal-depot",a.Id);WorldSupply.Step(w,10000);
            Assert.That(w.Convoys[0].Status,Is.EqualTo(ConvoyStatus.Captured));Assert.That(w.Convoys[0].Position.X,Is.EqualTo(4000).Within(.001));
            Assert.That(b.Rations,Is.EqualTo(enemyFood+40));Assert.That(w.Convoys[0].Food,Is.Zero);
        }
        [Test] public void BlockedConvoyWaitsAndResumesWhenRoadReopens()
        {
            var s=Create();var w=s.State;Relocate(w,w.Army("royal"),new WorldPoint(7000,0));WorldSupply.Dispatch(s.Campaign,"royal-depot","royal");
            WorldSupply.Step(w,1000);double x=w.Convoys[0].Position.X;w.Roads[0].Blocked=true;WorldSupply.Step(w,10000);
            Assert.That(w.Convoys[0].Status,Is.EqualTo(ConvoyStatus.Blocked));Assert.That(w.Convoys[0].Position.X,Is.EqualTo(x));
            w.Roads[0].Blocked=false;WorldSupply.Step(w,10000);Assert.That(w.Convoys[0].Status,Is.EqualTo(ConvoyStatus.Delivered));
        }
        [Test] public void MovingEnemyAndConvoyCannotPassThroughEachOtherAtHighSpeed()
        {
            var s=Create();var w=s.State;Relocate(w,w.Army("royal"),new WorldPoint(9000,0));Relocate(w,w.Army("resistance"),new WorldPoint(4500,4000));
            WorldSupply.Dispatch(s.Campaign,"royal-depot","royal");
            WorldSupply.Step(w,10000,new List<WorldPoint>{w.Army("royal").Position,new WorldPoint(4500,-4000)});
            Assert.That(w.Convoys[0].Status,Is.EqualTo(ConvoyStatus.Captured));Assert.That(w.Convoys[0].Position.X,Is.EqualTo(4500).Within(.001));
        }
        [Test] public void DepartedArmyDoesNotReceiveCargoByTeleportation()
        {
            var s=Create();var w=s.State;var a=w.Army("royal");Relocate(w,a,new WorldPoint(4000,0));
            WorldSupply.Dispatch(s.Campaign,"royal-depot","royal");Relocate(w,a,new WorldPoint(7000,0));double food=a.Rations;
            WorldSupply.Step(w,10000);Assert.That(w.Convoys[0].Status,Is.EqualTo(ConvoyStatus.Waiting));Assert.That(a.Rations,Is.EqualTo(food));
            Relocate(w,a,new WorldPoint(4000,0));WorldSupply.Step(w,1);Assert.That(a.Rations,Is.EqualTo(food+40));
        }
        [Test] public void MidConvoySaveResumesToTheSameStockAndPosition()
        {
            var s=Create();Relocate(s.State,s.State.Army("royal"),new WorldPoint(4000,0));WorldSupply.Dispatch(s.Campaign,"royal-depot","royal");
            s.SetSpeed(WorldSpeed.Hour);s.Advance(.5);
            var clone=new WorldSimulation(CampaignArchive.Deserialize(CampaignArchive.Serialize(s.Campaign)));
            for(int i=0;i<4;i++){s.Advance(.5);clone.Advance(.5);}
            Assert.That(CampaignArchive.Serialize(clone.Campaign),Is.EqualTo(CampaignArchive.Serialize(s.Campaign)));
            Assert.That(s.State.Convoys[0].Status,Is.EqualTo(ConvoyStatus.Delivered));
        }
        [Test] public void HungerHurtsConditionBeforeDesertionAndRestUsesFiniteAmmo()
        {
            var s=Create();var a=s.State.Army("royal");a.Rations=0;int men=a.Men;float morale=a.Morale;
            for(int i=0;i<96;i++)WorldSupply.QuarterHour(s.State);
            Assert.That(a.Men,Is.EqualTo(men));Assert.That(a.Morale,Is.LessThan(morale));Assert.That(a.HungrySeconds,Is.EqualTo(86400));
            a.Rations=40;a.AmmunitionWagon=3;a.Units[0].Ammo=0;a.Units[0].Cohesion=20;
            WorldSupply.QuarterHour(s.State);Assert.That(a.Units[0].Ammo,Is.EqualTo(3));Assert.That(a.AmmunitionWagon,Is.Zero);Assert.That(a.Units[0].Cohesion,Is.GreaterThan(20));
        }
        [Test] public void UnprotectedDepotChangesHandsWithItsExistingStocks()
        {
            var s=Create();Relocate(s.State,s.State.Army("royal"),new WorldPoint(5000,0));Relocate(s.State,s.State.Army("resistance"),new WorldPoint(0,0));
            int food=s.State.Depots[0].Food;WorldSupply.QuarterHour(s.State);
            Assert.That(s.State.Depots[0].FactionId,Is.EqualTo("insurgent"));Assert.That(s.State.Depots[0].Food,Is.EqualTo(food));
            Assert.That(WorldSupply.Restock(s.Campaign,"royal-depot").Ok,Is.False);
        }
        [Test] public void CapitalRestockConservesFoodAndConsumesDeclaredEquipment()
        {
            var s=Create();int food=s.Campaign.Food+s.State.Depots[0].Food,ammo=s.State.Depots[0].Ammunition,gear=s.Campaign.MilitarySupplies;
            Assert.That(WorldSupply.Restock(s.Campaign,"royal-depot").Ok,Is.True);
            Assert.That(s.Campaign.Food+s.State.Depots[0].Food,Is.EqualTo(food));Assert.That(s.Campaign.MilitarySupplies,Is.EqualTo(gear-12));Assert.That(s.State.Depots[0].Ammunition,Is.EqualTo(ammo+96));
        }
        [Test] public void CentralCivilianShortageDoesNotStarveAProvisionedFieldArmy()
        {
            var s=Create();var a=s.State.Army("royal");a.Rations=200;s.Campaign.Food=0;s.Campaign.Gold=10000;s.Campaign.MilitarySupplies=200;
            foreach(var r in s.Campaign.Regions)r.Unrest=100;
            Assert.That(CampaignCore.Forecast(s.Campaign).NetFood,Is.LessThan(0));int men=a.Men;
            s.SetSpeed(WorldSpeed.Day);for(int i=0;i<7;i++)s.Advance(1);
            Assert.That(a.Men,Is.EqualTo(men));Assert.That(a.HungrySeconds,Is.Zero);
        }
        [Test] public void APreSupplyWorldSaveIsRejectedWithAnExplicitVersionReason()
        {
            var s=Create();string json=System.Text.RegularExpressions.Regex.Replace(CampaignArchive.Serialize(s.Campaign),"\"Schema\"\\s*:\\s*3","\"Schema\":2");
            Assert.That(json.Contains("\"Schema\":2"),Is.True);
            Assert.Throws<NotSupportedException>(()=>CampaignArchive.Deserialize(json));
        }
        [Test] public void UnfedRestingArmyActuallyBecomesExhaustedThroughTheWorldClock()
        {
            var s=Create();var a=s.State.Army("royal");a.Rations=0;a.Fatigue=0;
            s.SetSpeed(WorldSpeed.Day);s.Advance(1);
            Assert.That(a.Fatigue,Is.GreaterThan(25));Assert.That(a.HungrySeconds,Is.EqualTo(86400));
        }
        [Test] public void AnAlternativeRoadRemainsReachableFromTheCurrentJunction()
        {
            var s=Create();var w=s.State;var c=new WorldSite{Id="detour",RegionId="ile",Position=new WorldPoint(5000,5000)};w.Sites.Add(c);w.Roads[0].Blocked=true;
            w.Roads.Add(new WorldRoad{Id="west",From="paris",To=c.Id,Points=new List<WorldPoint>{w.Sites[0].Position,c.Position}});
            w.Roads.Add(new WorldRoad{Id="east",From=c.Id,To="reims",Points=new List<WorldPoint>{c.Position,w.Sites[1].Position}});
            var route=WorldRouting.Find(w,w.Army("royal"),"reims");
            Assert.That(route.RoadIds,Is.EquivalentTo(new[]{"west","east"}));Assert.That(route.Points.Exists(p=>p.Z==5000),Is.True);
        }
    }
}
