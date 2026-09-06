using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace PowerAboveAll.Tests
{
    public sealed class WorldCommandTests
    {
        private static WorldSimulation Scenario()
        {
            var a=new WorldSite{Id="paris",RegionId="ile",Position=new WorldPoint(0,0)};
            var b=new WorldSite{Id="reims",RegionId="champagne",Position=new WorldPoint(780,0)};
            var sim=WorldSimulation.Create(CampaignCore.Create(),new[]{a,b},new[]{new WorldRoad{Id="road",From=a.Id,To=b.Id,Points=new List<WorldPoint>{a.Position,b.Position}}});
            sim.SetSpeed(WorldSpeed.Normal);sim.Advance(.1);return sim;
        }
        private static void Run(WorldSimulation s,double seconds)
        {s.Advance(seconds);while(s.State.Clock.PendingMilliseconds>=100)s.Drain();}
        [Test] public void DeploymentWalksToDistinctRolesAndKeepsAReserve()
        {
            var s=Scenario();var army=s.State.Army("royal");var reserve=army.Units.Find(u=>u.Role==WorldRole.Reserve);var before=reserve.Position;
            Assert.That(army.Units.Count,Is.EqualTo(6));Assert.That(reserve.Intent,Is.EqualTo(WorldIntent.Reserve));
            Run(s,20);Assert.That(WorldPoint.Distance(reserve.Position,before),Is.InRange(1,40));
            Assert.That(WorldTerrain.Dot(reserve.Destination-army.FrontAnchor,army.Forward),Is.LessThan(-200));
            Assert.That(reserve.LastFiredAt,Is.LessThan(0));Assert.That(reserve.Fatigue,Is.LessThan(5));
        }
        [Test] public void RepeatedOrderDoesNotPostponeDeliveryAndReformationEventuallyMoves()
        {
            var s=Scenario();var u=s.State.Army("royal").Units[0];var p=u.Position+new WorldPoint(0,120);
            s.OrderUnit(u.Id,p,WorldIntent.Hold,WorldFormation.Square);long due=u.Orders[0].ReceivedAt;
            Run(s,.5);s.OrderUnit(u.Id,p,WorldIntent.Hold,WorldFormation.Square);
            Assert.That(u.Orders.Count,Is.EqualTo(1));Assert.That(u.Orders[0].ReceivedAt,Is.EqualTo(due));
            var original=u.Position;Run(s,20);Assert.That(WorldPoint.Distance(original,u.Position),Is.GreaterThan(2));
        }
        [Test] public void WoodsHideReserveAndHillsOccludeSightInsteadOfGivingGlobalBuffs()
        {
            var s=Scenario();var w=s.State;var enemy=w.Army("resistance").Units[0];enemy.Position=new WorldPoint(600,0);
            Assert.That(WorldTerrain.Visible(w,w.Army("royal"),enemy),Is.True);
            w.Terrain.Add(new WorldTerrainFeature{Id="woods",Kind=WorldTerrainKind.Woodland,Centre=new WorldPoint(300,0),Radius=180,Source="test fixture",Confidence="synthetic"});
            Assert.That(WorldTerrain.Visible(w,w.Army("royal"),enemy),Is.False);
            w.Terrain.Clear();w.Terrain.Add(new WorldTerrainFeature{Id="ridge",Kind=WorldTerrainKind.Hill,Centre=new WorldPoint(300,0),Radius=160,Height=35,Source="test fixture",Confidence="synthetic"});
            Assert.That(WorldTerrain.ClearSight(w,new WorldPoint(0,0),enemy.Position),Is.False);
            Assert.That(WorldTerrain.ClearSight(w,new WorldPoint(300,0),enemy.Position),Is.True);
        }
        [Test] public void BatteryCannotShootThroughOwnCentre()
        {
            var s=Scenario();var army=s.State.Army("royal");var gun=army.Units[5];var friend=army.Units[0];
            gun.Position=new WorldPoint(0,0);friend.Position=new WorldPoint(100,0);
            foreach(var u in army.Units)if(u!=friend&&u!=gun)u.Position=new WorldPoint(-300,500);
            Assert.That(WorldTerrain.FriendlyFireLane(army,gun,new WorldPoint(500,0)),Is.False);
            friend.Position=new WorldPoint(100,90);
            Assert.That(WorldTerrain.FriendlyFireLane(army,gun,new WorldPoint(500,0)),Is.True);
        }
        [Test] public void HeadquartersLossSlowsNewOrdersButLastIntentSurvives()
        {
            var s=Scenario();var a=s.State.Army("royal");var u=a.Units[0];var hq=s.State.Headquarters.Find(h=>h.Id==a.HeadquartersId);
            hq.Position=u.Position;long before=WorldCommand.Delay(s.State,a,u);hq.Integrity=0;
            Assert.That(WorldCommand.Delay(s.State,a,u),Is.GreaterThan(before));
            Run(s,3);Assert.That(u.Intent,Is.EqualTo(WorldIntent.Advance));Assert.That(u.Pressure.HasFlag(WorldPressure.HeadquartersLost),Is.True);
        }
        [Test] public void AmmunitionComesFromNearbyWagonAndDestroyedWagonCannotRefill()
        {
            var s=Scenario();var a=s.State.Army("royal");var u=a.Units[3];u.ManualOrder=true;u.Destination=u.Position;u.ReorganizeUntil=0;u.Quiet=20;u.Ammo=0;
            a.WagonPosition=u.Position;int stock=a.AmmunitionWagon;Run(s,.1);
            Assert.That(u.Ammo,Is.EqualTo(1));Assert.That(a.AmmunitionWagon,Is.EqualTo(stock-1));
            a.WagonIntegrity=0;u.Ammo=0;Run(s,4);Assert.That(u.Ammo,Is.Zero);Assert.That(a.AmmunitionWagon,Is.Zero);
        }
        [Test] public void RearInterceptionChangesWithdrawalStateWithoutInstantArmyDeletion()
        {
            var s=Scenario();var a=s.State.Army("royal");var e=s.State.Army("resistance").Units[4];var u=a.Units[0];
            e.Position=a.FrontAnchor-a.Forward*300;e.Destination=e.Position;e.ManualOrder=true;e.ReorganizeUntil=0;
            u.Withdrawal=WorldWithdrawal.Ordered;Run(s,.1);
            Assert.That(a.RearBlocked,Is.True);Assert.That(u.Withdrawal,Is.EqualTo(WorldWithdrawal.Disordered));Assert.That(u.Men,Is.GreaterThan(0));
        }
        [Test] public void UnseenEnemyHasNoPreciseLiveStrengthOrPositionInReport()
        {
            var s=Scenario();WorldCommand.Observe(s.State);var sight=s.State.Sightings[0];
            Assert.That(sight.Minimum,Is.LessThan(sight.Maximum));var p=sight.Position;long date=sight.ObservedAt;
            foreach(var u in s.State.Army("resistance").Units)u.Position+=new WorldPoint(10000,10000);
            WorldCommand.Observe(s.State);Assert.That(sight.Visible,Is.False);Assert.That(sight.Position.X,Is.EqualTo(p.X));Assert.That(sight.ObservedAt,Is.EqualTo(date));
        }
        [Test] public void LocalTerrainAndCommandStateRoundTripMidBattle()
        {
            var s=Scenario();s.State.Terrain.Add(new WorldTerrainFeature{Id="wood",Kind=WorldTerrainKind.Woodland,Centre=new WorldPoint(80,90),Radius=60,Source="test",Confidence="synthetic"});Run(s,30);
            var clone=new WorldSimulation(CampaignArchive.Deserialize(CampaignArchive.Serialize(s.Campaign)));Run(s,40);Run(clone,40);
            Assert.That(CampaignArchive.Serialize(clone.Campaign),Is.EqualTo(CampaignArchive.Serialize(s.Campaign)));
        }
        [Test] public void RealNaturalEarthMultipartRiversHaveUniqueSaveableFeatureIds()
        {
            var s=Scenario();var data=UnityEngine.JsonUtility.FromJson<PhysicalGeography>(UnityEngine.Resources.Load<UnityEngine.TextAsset>("World/physical").text);
            GameApp.PopulateWorldTerrain(s.State,data.rivers);
            Assert.That(s.State.Terrain.Count,Is.GreaterThan(40));
            WorldValidation.Validate(s.Campaign);
            Assert.DoesNotThrow(()=>CampaignArchive.Deserialize(CampaignArchive.Serialize(s.Campaign)));
        }
        [Test] public void CountryAccountingContinuesWhileLocalCombatIsActive()
        {
            var s=Scenario();var clock=s.State.Clock;clock.Milliseconds=WorldClock.Week-500;
            s.State.NextDayAt=s.State.NextConditionAt=s.State.NextEconomyAt=WorldClock.Week;
            s.State.Battles[0].NextTickAt=clock.Milliseconds+100;int gold=s.Campaign.Gold;
            Run(s,1);Assert.That(s.State.HasCombat,Is.True);Assert.That(s.Campaign.Week,Is.EqualTo(1));
            Assert.That(s.Campaign.Gold,Is.Not.EqualTo(gold));WorldValidation.Validate(s.Campaign);
        }
        [Test] public void RestDoesNotEraseHighMoraleAndCampaignReadsActualRegimentFatigue()
        {
            var s=Scenario();var a=s.State.Army("royal");var reserve=a.Units[3];reserve.Morale=95;reserve.Quiet=20;reserve.ManualOrder=true;reserve.Destination=reserve.Position;
            foreach(var u in a.Units)u.Fatigue=70;
            Run(s,.1);Assert.That(reserve.Morale,Is.EqualTo(95));Assert.That(s.Campaign.Fatigue,Is.GreaterThan(69));
        }
        [Test] public void EmptyRegimentSeeksItsOwnWagonAndKeepsThePlayersIntent()
        {
            var s=Scenario();var a=s.State.Army("royal");var u=a.Units[0];
            u.Ammo=0;u.Intent=WorldIntent.Hold;u.ManualOrder=true;u.Destination=u.Position;u.ReorganizeUntil=0;
            a.WagonPosition=u.Position-a.Forward*200;Run(s,.1);
            Assert.That(u.Replenishing,Is.True);Assert.That(u.Moving,Is.True);Assert.That(u.Intent,Is.EqualTo(WorldIntent.Hold));
            Assert.That(WorldPoint.Distance(u.Destination,a.WagonPosition),Is.LessThan(1));
            var clone=new WorldSimulation(CampaignArchive.Deserialize(CampaignArchive.Serialize(s.Campaign)));Run(s,5);Run(clone,5);
            Assert.That(CampaignArchive.Serialize(clone.Campaign),Is.EqualTo(CampaignArchive.Serialize(s.Campaign)));
        }
    }
}
