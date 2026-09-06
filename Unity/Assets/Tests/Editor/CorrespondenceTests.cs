#if UNITY_EDITOR
using System;
using NUnit.Framework;

namespace PowerAboveAll.Tests
{
    public sealed class CorrespondenceTests
    {
        static CampaignState Start()
        {var s=CampaignCore.Create();Assert.IsTrue(CampaignCore.OpenCorrespondence(s).Ok);return s;}
        static void Week(CampaignState s)
        {Assert.IsTrue(CampaignCore.NextWeek(s).Ok);if(s.PendingPetition)Assert.IsTrue(CampaignCore.ChoosePetition(s,"negotiate").Ok);CampaignCore.Validate(s);}
        static string Save(CampaignState s)=>CampaignArchive.Serialize(s,false);
        [Test] public void LegacyRegionalActionsCannotBypassCorrespondence()
        {
            var s=Start();string before=Save(s);
            Assert.AreEqual("dispatch.use_desk",CampaignCore.Act(s,"bread","guyenne").Key);
            Assert.AreEqual("dispatch.use_desk",CampaignCore.Act(s,"tax","guyenne").Key);
            Assert.AreEqual("dispatch.outside_slice",CampaignCore.GrantRegionalAccord(s,"guyenne").Key);
            Assert.AreEqual("dispatch.outside_slice",CampaignCore.CanBeginRegionalReform(s,"guyenne","compromise").Key);
            Assert.AreEqual(before,Save(s));
            Assert.IsTrue(CampaignCore.Act(s,"bread","ile").Ok);
        }
        [Test] public void SendingReservesResourcesWithoutChangingRemoteRegion()
        {
            var s=Start();var r=CampaignCore.Region(s,"guyenne");float unrest=r.Unrest,control=r.Control;
            Assert.IsTrue(CampaignCore.SendCabinetOrder(s,"bread","strict",true).Ok);
            Assert.AreEqual(320,s.Food);Assert.AreEqual(828,s.Gold);Assert.AreEqual(unrest,r.Unrest);Assert.AreEqual(control,r.Control);
            Assert.AreEqual(-4,CampaignCore.Knowledge(s,"guyenne").ObservedDay);CampaignCore.Validate(s);
        }
        [Test] public void ExecutedOrderDoesNotRevealItsResultBeforeReturn()
        {
            var s=Start();Assert.IsTrue(CampaignCore.SendCabinetOrder(s,"order","mission",true).Ok);Week(s);
            var order=CampaignCore.Desk(s).Orders[0];Assert.IsTrue(order.Executed);Assert.IsFalse(order.ReportReceived);
            Assert.AreEqual(21,CampaignCore.Region(s,"guyenne").Unrest);
            Assert.AreEqual(35,CampaignCore.Knowledge(s,"guyenne").Unrest);
            Assert.AreEqual(IntelligenceConfidence.Outdated,CampaignCore.Knowledge(s,"guyenne").Confidence);
            Assert.IsFalse(s.Journal.Exists(e=>e.Key=="log.dispatch.returned"));
        }
        [Test] public void ReturnKeepsObservationTimeAndCannotBecomeLiveTruth()
        {
            var s=Start();CampaignCore.SendCabinetOrder(s,"order","mission",true);Week(s);Week(s);
            var k=CampaignCore.Knowledge(s,"guyenne");Assert.AreEqual(21,k.Unrest);Assert.AreEqual(23,CampaignCore.Region(s,"guyenne").Unrest);
            Assert.AreEqual(7,k.ObservedDay);Assert.AreEqual(11,k.ReceivedDay);Assert.AreEqual(7,k.AgeDays);
            Assert.IsTrue(CampaignCore.Desk(s).Orders[0].ReportReceived);
        }
        [Test] public void DelegationChangesLocalMethodAndLaterPoliticalCost()
        {
            var strict=Start();var mission=Start();CampaignCore.SendCabinetOrder(strict,"order","strict",true);CampaignCore.SendCabinetOrder(mission,"order","mission",true);
            Week(strict);Week(mission);Assert.AreEqual(8,CampaignCore.Region(strict,"guyenne").Unrest-CampaignCore.Region(mission,"guyenne").Unrest);
            Assert.AreEqual(strict.Power,mission.Power);Week(strict);Week(mission);
            Assert.AreEqual(2,strict.Power-mission.Power);Assert.AreEqual(79,CampaignCore.Desk(mission).Ambition);
            Assert.AreEqual("negotiated",CampaignCore.Desk(strict).Orders[0].Outcome);Assert.AreEqual("force",CampaignCore.Desk(mission).Orders[0].Outcome);
        }
        [Test] public void RepeatedCommandIsAtomicButSeparateInquiryIsAllowed()
        {
            var s=Start();CampaignCore.SendCabinetOrder(s,"bread","strict",false);string before=Save(s);
            Assert.IsFalse(CampaignCore.SendCabinetOrder(s,"tax","mission",true).Ok);Assert.AreEqual(before,Save(s));
            Assert.IsTrue(CampaignCore.SendCabinetOrder(s,"report","strict",true).Ok);Assert.AreEqual(2,CampaignCore.Desk(s).Orders.Count);
            before=Save(s);Assert.IsFalse(CampaignCore.SendCabinetOrder(s,"report","mission",true).Ok);Assert.AreEqual(before,Save(s));
        }
        [Test] public void InFlightArchiveReplaysTheSameWorldAndKnowledge()
        {
            var s=Start();CampaignCore.SendCabinetOrder(s,"bread","mission",true);Week(s);
            var copy=CampaignArchive.Deserialize(Save(s));Assert.AreEqual(Save(s),Save(copy));Week(s);Week(copy);Assert.AreEqual(Save(s),Save(copy));
        }
        [Test] public void RegularPostHasNoExecutionOnFirstWeek()
        {
            var s=Start();CampaignCore.SendCabinetOrder(s,"order","strict",false);Week(s);
            Assert.IsFalse(CampaignCore.Desk(s).Orders[0].Executed);Assert.AreEqual(37,CampaignCore.Region(s,"guyenne").Unrest);
            Week(s);Assert.IsTrue(CampaignCore.Desk(s).Orders[0].ReportReceived);
        }
        [Test] public void TaxProceedsReachTreasuryWithReportNotAtDispatch()
        {
            var taxed=Start();var control=Start();CampaignCore.SendCabinetOrder(taxed,"tax","strict",true);CampaignCore.SendCabinetOrder(control,"report","strict",true);
            Assert.AreEqual(control.Gold,taxed.Gold);Week(taxed);Week(control);Assert.AreEqual(control.Gold,taxed.Gold);
            int before=taxed.Gold,forecast=CampaignCore.Forecast(taxed).NetGold;Week(taxed);Assert.AreEqual(before+forecast+100,taxed.Gold);
        }
        [Test] public void OlderArchiveMigratesWithoutStartingCorrespondence()
        {
            string old=Save(CampaignCore.Create()).Replace("\"Version\":9","\"Version\":8");
            var s=CampaignArchive.Deserialize(old);Assert.IsNull(CampaignCore.Desk(s));CampaignCore.Validate(s);
            Assert.Throws<ArgumentException>(()=>CampaignArchive.Deserialize(Save(Start()).Replace("\"Version\":9","\"Version\":8")));
        }
        [Test] public void CurrentArchiveRejectsNullOrMissingCorrespondence()
        {
            string json=Save(CampaignCore.Create());Assert.Throws<ArgumentException>(()=>CampaignArchive.Deserialize(json.Replace("\"Correspondence\":[]","\"Correspondence\":null")));
            Assert.Throws<ArgumentException>(()=>CampaignArchive.Deserialize(json.Replace("\"Correspondence\":[],","")));
        }
        [Test] public void InvalidFutureKnowledgeCannotBeSaved()
        {var s=Start();CampaignCore.Desk(s).LastReport.ObservedDay=7;Assert.Throws<ArgumentException>(()=>Save(s));}
        [Test] public void DeliveredFlagCannotRevealAnUnexecutedOrder()
        {var s=Start();CampaignCore.SendCabinetOrder(s,"order","strict",false);CampaignCore.Desk(s).Orders[0].ReportReceived=true;Assert.Throws<ArgumentException>(()=>Save(s));}
        [Test] public void RefusedWeekDoesNotAdvanceLetters()
        {
            var s=Start();CampaignCore.SendCabinetOrder(s,"order","strict",false);Week(s);Assert.IsTrue(CampaignCore.NextWeek(s).Ok);string before=Save(s);
            Assert.IsFalse(CampaignCore.NextWeek(s).Ok);Assert.AreEqual(before,Save(s));
        }
    }
}
#endif
