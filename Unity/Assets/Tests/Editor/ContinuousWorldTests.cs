using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace PowerAboveAll.Tests
{
    public sealed class ContinuousWorldTests
    {
        private static WorldSimulation World(double distance=10000)
        {
            var a=new WorldSite{Id="paris",RegionId="ile",Position=new WorldPoint(0,0)};
            var b=new WorldSite{Id="reims",RegionId="champagne",Position=new WorldPoint(distance,0)};
            return WorldSimulation.Create(CampaignCore.Create(),new[]{a,b},new[]{new WorldRoad{Id="royal-road",From=a.Id,To=b.Id,Points=new List<WorldPoint>{a.Position,b.Position}}});
        }
        [TestCase(WorldSpeed.Normal,1000L)] [TestCase(WorldSpeed.Hour,3600000L)] [TestCase(WorldSpeed.Day,86400000L)]
        public void ClockRatesDoNotDependOnRendering(WorldSpeed speed,long expected)
        {
            var s=World();s.SetSpeed(speed);for(int i=0;i<100;i++)s.Advance(.01);
            Assert.That(s.State.Clock.Milliseconds,Is.EqualTo(expected));Assert.That(s.LastStepCount,Is.LessThan(512));
            WorldValidation.Validate(s.Campaign);
        }
        [Test] public void PausedClockAndArmyRemainUnchanged()
        {
            var s=World();s.March("royal","reims");string before=CampaignArchive.Serialize(s.Campaign);
            s.Advance(100);Assert.That(CampaignArchive.Serialize(s.Campaign),Is.EqualTo(before));
        }
        [Test] public void MarchIsARealRouteAndDoesNotSpendMovesOrTeleport()
        {
            var s=World(100000);var army=s.State.Army("royal");
            Assert.That(s.March("royal","reims").Ok,Is.True);Assert.That(army.Position.X,Is.Zero);Assert.That(s.Campaign.Moves,Is.Zero);
            s.SetSpeed(WorldSpeed.Normal);s.Advance(3);Assert.That(army.Position.X,Is.Zero);
            s.Advance(10);Assert.That(army.Position.X,Is.InRange(10,12));Assert.That(army.RegionId,Is.EqualTo("ile"));
            Assert.That(WorldRouting.Remaining(army),Is.GreaterThan(99000));
        }
        [Test] public void SpeedThreeCannotSkipContactOrRunTheBattleBeforeTheAlert()
        {
            var s=World();var army=s.State.Army("royal");var original=army.Units[0];
            s.March(army.Id,"reims");s.SetSpeed(WorldSpeed.Day);s.Advance(1);
            Assert.That(s.State.Battles.Count,Is.EqualTo(1));Assert.That(s.State.Clock.Speed,Is.EqualTo(WorldSpeed.Normal));
            Assert.That(s.State.Clock.PendingMilliseconds,Is.Zero);Assert.That(s.State.Battles[0].StartedAt,Is.EqualTo(s.State.Clock.Milliseconds));
            Assert.That(WorldPoint.Distance(army.Position,s.State.Army("resistance").Position),Is.EqualTo(800).Within(.01));
            Assert.That(army.Units[0],Is.SameAs(original));Assert.That(original.Men,Is.EqualTo(original.Original));
        }
        [Test] public void ContactCanPauseAccordingToThePlayersSetting()
        {
            var s=World();s.State.BattlePolicy=BattleTimePolicy.Pause;s.March("royal","reims");s.SetSpeed(WorldSpeed.Day);s.Advance(1);
            Assert.That(s.State.HasCombat,Is.True);Assert.That(s.State.Clock.Speed,Is.EqualTo(WorldSpeed.Pause));
        }
        [Test] public void WeeklyAccountingNoLongerRequiresAButtonOrStopsForPetition()
        {
            var s=World();int gold=s.Campaign.Gold;s.SetSpeed(WorldSpeed.Day);
            for(int i=0;i<22;i++)s.Advance(1);
            Assert.That(s.Campaign.Week,Is.EqualTo(3));Assert.That(s.Campaign.Gold,Is.Not.EqualTo(gold));Assert.That(s.Campaign.PendingPetition,Is.True);
            Assert.That(CampaignCore.NextWeek(s.Campaign).Ok,Is.False);CampaignCore.Validate(s.Campaign);WorldValidation.Validate(s.Campaign);
        }
        [Test] public void MidRouteSaveRestoresPositionAndContinuesToTheSameContact()
        {
            var s=World();s.March("royal","reims");s.SetSpeed(WorldSpeed.Hour);s.Advance(1);
            var clone=new WorldSimulation(CampaignArchive.Deserialize(CampaignArchive.Serialize(s.Campaign)));
            for(int i=0;i<4;i++){s.Advance(1);clone.Advance(1);}
            Assert.That(CampaignArchive.Serialize(clone.Campaign),Is.EqualTo(CampaignArchive.Serialize(s.Campaign)));
        }
        [Test] public void UnitCommandHasDelayAndSaveKeepsItsQueue()
        {
            var s=World();s.March("royal","reims");s.SetSpeed(WorldSpeed.Day);s.Advance(1);
            var unit=s.State.Army("royal").Units[0];
            Assert.That(s.OrderUnit(unit.Id,unit.Position,WorldIntent.Hold,WorldFormation.Square).Ok,Is.True);
            Assert.That(unit.Formation,Is.EqualTo(WorldFormation.Line));
            var clone=new WorldSimulation(CampaignArchive.Deserialize(CampaignArchive.Serialize(s.Campaign)));
            s.Advance(6);clone.Advance(6);
            Assert.That(unit.Formation,Is.EqualTo(WorldFormation.Square));Assert.That(unit.Orders.Count,Is.Zero);
            Assert.That(CampaignArchive.Serialize(clone.Campaign),Is.EqualTo(CampaignArchive.Serialize(s.Campaign)));
        }
        [Test] public void RetreatLeavesBothArmiesInTheSameWorldAndMovesTheLoser()
        {
            var s=World();s.March("royal","reims");s.SetSpeed(WorldSpeed.Day);s.Advance(1);
            var army=s.State.Army("royal");s.Retreat(army.Id);var position=army.Position;
            Assert.That(s.State.Battles[0].Ended,Is.True);Assert.That(s.State.Armies.Count,Is.EqualTo(2));
            Assert.That(army.Activity,Is.EqualTo(ArmyActivity.Retreating));s.Advance(10);
            Assert.That(WorldPoint.Distance(position,army.Position),Is.GreaterThan(5));
            Assert.That(s.State.Army("resistance").Activity,Is.EqualTo(ArmyActivity.Holding));WorldValidation.Validate(s.Campaign);
        }
        [Test] public void LargeAdvanceHasABoundedWorkBudgetAndKeepsTheRemainder()
        {
            var s=World();s.SetSpeed(WorldSpeed.Day);s.Advance(30);
            Assert.That(s.LastStepCount,Is.EqualTo(WorldSimulation.FrameWorkBudget));Assert.That(s.State.Clock.PendingMilliseconds,Is.GreaterThan(0));
            while(s.State.Clock.PendingMilliseconds>=100)s.Drain();Assert.That(s.State.Clock.Milliseconds,Is.EqualTo(30*WorldClock.Day));
        }
        [Test] public void InvalidWorldSaveIsRejectedInsteadOfRepairedSilently()
        {
            var s=World();s.State.Army("royal").Position=new WorldPoint(double.NaN,0);
            Assert.Throws<ArgumentException>(()=>CampaignArchive.Serialize(s.Campaign));
        }
        [Test] public void RoadBlockPreventsRouteCreation()
        {
            var s=World();s.State.Roads[0].Blocked=true;
            Assert.That(s.March("royal","reims").Ok,Is.False);Assert.That(s.State.Army("royal").Activity,Is.EqualTo(ArmyActivity.Holding));
        }
        [Test] public void NaturalCombatResolvesWithoutOutcomeInjectionAndCanResumeMidFight()
        {
            var s=World();s.March("royal","reims");s.SetSpeed(WorldSpeed.Day);s.Advance(1);s.Advance(30);
            while(s.State.Clock.PendingMilliseconds>=100)s.Drain();
            var clone=new WorldSimulation(CampaignArchive.Deserialize(CampaignArchive.Serialize(s.Campaign)));
            for(int i=0;i<90&&s.State.HasCombat;i++){s.Advance(10);clone.Advance(10);while(s.State.Clock.PendingMilliseconds>=100)s.Drain();while(clone.State.Clock.PendingMilliseconds>=100)clone.Drain();}
            if(s.State.HasCombat)foreach(var a in s.State.Armies)foreach(var u in a.Units)
                TestContext.WriteLine($"{a.Id}/{u.Role}: men={u.Men} morale={u.Morale:0} cohesion={u.Cohesion:0} fatigue={u.Fatigue:0} ammo={u.Ammo} withdrawal={u.Withdrawal} moving={u.Moving} position={u.Position.X:0},{u.Position.Z:0} destination={u.Destination.X:0},{u.Destination.Z:0} reason={u.Pressure}");
            Assert.That(s.State.HasCombat,Is.False,"The engagement needs a natural conclusion.");
            Assert.That(CampaignArchive.Serialize(clone.Campaign),Is.EqualTo(CampaignArchive.Serialize(s.Campaign)));
            Assert.That(s.State.Armies.Count,Is.EqualTo(2));Assert.That(s.State.Armies[0].Men+s.State.Armies[1].Men,Is.LessThan(s.State.Battles[0].FirstOriginal+s.State.Battles[0].SecondOriginal));
        }
        [Test] public void ReturningLettersAreProcessedOnTheirDayWithoutWaitingForAccounting()
        {
            var s=World();CampaignCore.OpenCorrespondence(s.Campaign);s.SetSpeed(WorldSpeed.Day);s.Advance(1);
            Assert.That(CampaignCore.SendCabinetOrder(s.Campaign,"order","strict",true).Ok,Is.True);
            for(int i=0;i<8;i++)s.Advance(1);
            Assert.That(CampaignCore.Desk(s.Campaign).Orders[0].ReportReceived,Is.True);
            Assert.That(CampaignCore.CurrentDay(s.Campaign),Is.EqualTo(9));Assert.That(s.Campaign.Week,Is.EqualTo(1));
            WorldValidation.Validate(s.Campaign);CampaignCore.Validate(s.Campaign);
        }
    }
}
