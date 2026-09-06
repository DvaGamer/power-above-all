using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    public sealed class FirstCommissionTests
    {
        private static WorldSimulation Create(string role="crown")
        {
            var paris=new WorldSite{Id="paris",RegionId="ile",Position=new WorldPoint(0,0)};
            var reims=new WorldSite{Id="reims",RegionId="champagne",Position=new WorldPoint(100000,0)};
            return WorldSimulation.Create(CampaignCore.Create(role),new[]{paris,reims},new[]{new WorldRoad{Id="road",From="paris",To="reims",Points=new List<WorldPoint>{paris.Position,reims.Position}}});
        }
        private static void ToDay(WorldSimulation s,int day)
        {
            s.SetSpeed(WorldSpeed.Day);
            for(int guard=0;s.State.Clock.Milliseconds<day*WorldClock.Day&&guard<2000;guard++)
            {
                if(s.State.Clock.PendingMilliseconds>0)s.Drain();
                else s.Advance(Math.Min(1,(day*WorldClock.Day-s.State.Clock.Milliseconds)/(double)WorldClock.Day));
                if(s.State.Clock.Speed==WorldSpeed.Pause)break;
            }
            Assert.That(s.State.Clock.Milliseconds,Is.EqualTo(day*WorldClock.Day));
        }
        private static void Feed(WorldSimulation s)
        {
            if(s.State.Depots.Find(d=>d.Id=="royal-depot").Food<40)Assert.That(WorldSupply.Restock(s.Campaign,"royal-depot").Ok,Is.True);
            Assert.That(WorldSupply.Dispatch(s.Campaign,"royal-depot","royal").Ok,Is.True);
        }
        private static WorldSimulation Planned(string role)
        {
            var s=Create(role);var region=role=="assembly"?"champagne":"ile";
            Assert.That(CampaignCore.IssueMandate(s.Campaign,region).Ok,Is.True);
            Assert.That(CampaignCore.ResolveMandate(s.Campaign,CampaignCore.MandateId(s.Campaign.Obligation),"fulfil").Ok,Is.True);
            Feed(s);
            if(role=="assembly")Assert.That(CampaignCore.Act(s.Campaign,"bread","champagne").Ok,Is.True);
            ToDay(s,6);Feed(s);
            ToDay(s,13);Feed(s);
            if(role=="assembly")Assert.That(CampaignCore.Act(s.Campaign,"bread","champagne").Ok,Is.True);
            ToDay(s,14);Assert.That(CampaignCore.ChoosePetition(s.Campaign,"negotiate").Ok,Is.True);
            ToDay(s,20);Feed(s);ToDay(s,28);return s;
        }

        [TestCase("crown")][TestCase("assembly")][TestCase("army")]
        public void NormalActionsCanSatisfyEachRoleWithoutAForcedBattle(string role)
        {
            var s=Planned(role);var c=CampaignCore.Commission(s.Campaign);
            foreach(var m in c.Report)Assert.That(m.Met,Is.True,m.Id+"="+m.Value+" target="+m.Target);
            Assert.That(c.Succeeded,Is.True);Assert.That(s.State.HasCombat,Is.False);
            Assert.That(CampaignArchive.Deserialize(CampaignArchive.Serialize(s.Campaign)).Commissions[0].Succeeded,Is.True);
        }
        [TestCase("crown")][TestCase("assembly")][TestCase("army")]
        public void WaitingWithoutAnyDecisionsDoesNotFulfilTheAppointment(string role)
        {
            var s=Create(role);ToDay(s,28);var c=CampaignCore.Commission(s.Campaign);
            Assert.That(c.Succeeded,Is.False);Assert.That(c.Report.Find(m=>m.Id=="kept").Value,Is.Zero);
            if(role=="army")Assert.That(c.Report.Find(m=>m.Id=="rations").Met,Is.False);
        }
        [Test] public void HighSpeedStopsAtTheExactReportTimeAndDiscardsOldBacklog()
        {
            var s=Create();s.SetSpeed(WorldSpeed.Day);s.Advance(80);
            for(int i=0;s.State.Clock.PendingMilliseconds>0&&i<100;i++)s.Drain();
            var c=CampaignCore.Commission(s.Campaign);
            Assert.That(c.ResolvedAt,Is.EqualTo(28*WorldClock.Day));
            Assert.That(s.State.Clock.Milliseconds,Is.EqualTo(c.DueAt));
            Assert.That(s.State.Clock.Speed,Is.EqualTo(WorldSpeed.Pause));Assert.That(s.State.Clock.PendingMilliseconds,Is.Zero);
        }
        [Test] public void AnOpenPetitionDoesNotStopPromiseExpiryOrHideTheViolation()
        {
            var s=Create();CampaignCore.IssueMandate(s.Campaign,"ile");ToDay(s,14);
            Assert.That(s.Campaign.PendingPetition,Is.True);Assert.That(s.Campaign.Obligation,Is.Not.Null);
            ToDay(s,16);Assert.That(s.Campaign.Obligation,Is.Null);Assert.That(CampaignCore.Commission(s.Campaign).Broken,Is.EqualTo(1));
            Assert.That(s.Campaign.Journal.Exists(l=>l.Key=="log.commission.promise_expired"),Is.True);
            Assert.That(CampaignArchive.Deserialize(CampaignArchive.Serialize(s.Campaign)).Commissions[0].Broken,Is.EqualTo(1));
            ToDay(s,17);Assert.That(CampaignCore.Commission(s.Campaign).Broken,Is.EqualTo(1));
        }
        [Test] public void PaymentDuringGraceCountsButDefaultCannotBePaidAwayAfterwards()
        {
            var s=Create();CampaignCore.IssueMandate(s.Campaign,"ile");string id=CampaignCore.MandateId(s.Campaign.Obligation);
            ToDay(s,15);CampaignCore.ChoosePetition(s.Campaign,"negotiate");
            Assert.That(CampaignCore.ResolveMandate(s.Campaign,id,"fulfil").Ok,Is.True);ToDay(s,16);
            Assert.That(CampaignCore.Commission(s.Campaign).Kept,Is.EqualTo(1));Assert.That(CampaignCore.Commission(s.Campaign).Broken,Is.Zero);
            Assert.That(CampaignCore.ResolveMandate(s.Campaign,id,"fulfil").Ok,Is.False);
        }
        [Test] public void ReportAndRewardRemainFrozenWhenTheSameWorldContinuesOrReloads()
        {
            var s=Planned("crown");string snapshot=JsonUtility.ToJson(CampaignCore.Commission(s.Campaign));
            float power=s.Campaign.Power;var loaded=CampaignArchive.Deserialize(CampaignArchive.Serialize(s.Campaign));
            var next=new WorldSimulation(loaded);next.SetSpeed(WorldSpeed.Normal);next.Advance(10);
            Assert.That(JsonUtility.ToJson(CampaignCore.Commission(loaded)),Is.EqualTo(snapshot));Assert.That(loaded.Power,Is.EqualTo(power));
            loaded.Gold=0;Assert.That(CampaignCore.CommissionMeasures(loaded).Find(m=>m.Id=="gold").Value,Is.GreaterThanOrEqualTo(950));
        }
        [Test] public void PausedOlderWorldCannotPayAnAlreadyExpiredPromise()
        {
            var s=Create();CampaignCore.IssueMandate(s.Campaign,"ile");
            s.State.Clock.Milliseconds=16*WorldClock.Day;s.Campaign.Week=2;
            s.Campaign.PetitionResolved=true;
            Assert.That(CampaignCore.CanResolveMandate(s.Campaign,CampaignCore.MandateId(s.Campaign.Obligation),"fulfil").Ok,Is.False);
        }
        [Test] public void VersionTwelveMigratesWithoutInventingAStartedAppointment()
        {
            var s=Create("legacy");string json=CampaignArchive.Serialize(s.Campaign,false);
            json=Regex.Replace(json,"\"Version\"\\s*:\\s*13","\"Version\":12");
            json=Regex.Replace(json,"\"Commissions\"\\s*:\\s*\\[\\]\\s*,","");
            var copy=CampaignArchive.Deserialize(json);Assert.That(copy.World,Is.Not.Null);Assert.That(copy.Commissions,Is.Empty);
            Assert.That(copy.World.Clock.Milliseconds,Is.EqualTo(s.State.Clock.Milliseconds));
        }
        [Test] public void NewArchivesRequireTheirContainerAndRejectMalformedResults()
        {
            var s=Create();string json=CampaignArchive.Serialize(s.Campaign,false).Replace("\"Commissions\"","\"DiscardedCommissions\"");
            Assert.Throws<ArgumentException>(()=>CampaignArchive.Deserialize(json));
            CampaignCore.Commission(s.Campaign).DueAt++;Assert.Throws<ArgumentException>(()=>CampaignCore.Validate(s.Campaign));
        }
    }
}
