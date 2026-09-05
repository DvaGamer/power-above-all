#if UNITY_EDITOR
using System;
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    public sealed class PatronTrustTests
    {
        static string Snapshot(CampaignState state) => JsonUtility.ToJson(state);
        static CharacterState Patron(CampaignState state) => state.Characters.Find(person => person.Id == CampaignCore.PatronIdForRole(state.RoleId));
        static void Success(ActionResult result) => Assert.IsTrue(result.Ok, result.Key);
        static void Refused(CampaignState state, Func<ActionResult> action, string reason)
        {
            string before = Snapshot(state);
            var result = action();
            Assert.IsFalse(result.Ok);
            Assert.AreEqual(reason, result.Key);
            Assert.AreEqual(before, Snapshot(state), "Reddedilen eylem günlük dahil atomik kalmalı.");
        }

        [TestCase("crown", 6)]
        [TestCase("assembly", 4)]
        [TestCase("army", 5)]
        public void BrokenTrustClosesNewAidAndResponsibilityChangesOnlyPersonalPolitics(string role, float powerCost)
        {
            var state = CampaignCore.Create(role);
            Patron(state).Relationship = 0;
            state.Factions.Find(faction => faction.Id == role).Approval = 0;
            state.Power = 31;
            string before = Snapshot(state);
            var preview = CampaignCore.GetPatronRepairTerms(state);
            Assert.AreEqual(powerCost, preview.PowerCost);
            Assert.AreEqual(4, preview.RelationshipGain);
            Success(CampaignCore.CanRepairPatronTrust(state));
            Assert.AreEqual(before, Snapshot(state), "Teklifi okumak sorumluluğu üstlenmez.");
            Refused(state, () => CampaignCore.IssueMandate(state, "ile"), "error.trust.closed");
            var expected = CampaignArchive.Deserialize(CampaignArchive.Serialize(state));
            expected.Power -= powerCost;
            Patron(expected).Relationship = 4;
            Success(CampaignCore.RepairPatronTrust(state));
            Assert.AreEqual("log.trust.repaired", state.Journal[0].Key);
            expected.Journal = state.Journal;
            Assert.AreEqual(Snapshot(expected), Snapshot(state), "Telafi bölge, kurum, stok veya takvimi değiştiremez.");
            Refused(state, () => CampaignCore.RepairPatronTrust(state), "error.trust.not_broken");
            Success(CampaignCore.CanIssueMandate(state, "ile"));
            Assert.AreEqual(Snapshot(state), Snapshot(CampaignArchive.Deserialize(CampaignArchive.Serialize(state))));
        }

        [TestCase("crown")]
        [TestCase("assembly")]
        [TestCase("army")]
        public void ExistingPromiseCanBeHonouredAtZeroTrustWithItsOriginalTerms(string role)
        {
            var state = CampaignCore.Create(role);
            Success(CampaignCore.IssueMandate(state, "ile"));
            var original = CampaignCore.GetObligationTerms(state);
            Patron(state).Relationship = 0;
            state = CampaignArchive.Deserialize(CampaignArchive.Serialize(state));
            string id = CampaignCore.MandateId(state.Obligation);
            Refused(state, () => CampaignCore.RepairPatronTrust(state), "error.trust.open");
            var saved = CampaignCore.GetObligationTerms(state);
            Assert.AreEqual(original.RegionId, saved.RegionId);
            Assert.AreEqual(original.DueWeek, saved.DueWeek);
            Assert.AreEqual(original.Fulfil.Gold, saved.Fulfil.Gold);
            Assert.AreEqual(original.Fulfil.Food, saved.Fulfil.Food);
            int gold = state.Gold, food = state.Food;
            Success(CampaignCore.ResolveMandate(state, id, "fulfil"));
            Assert.AreEqual(gold + original.Fulfil.Gold, state.Gold);
            Assert.AreEqual(food + original.Fulfil.Food, state.Food);
            Assert.AreEqual(4, Patron(state).Relationship);
            Assert.IsNull(state.Obligation);
        }

        [TestCase("crown")]
        [TestCase("assembly")]
        [TestCase("army")]
        public void PoliticalRepairHasNoResourceOrMinimumPowerLock(string role)
        {
            var state = CampaignCore.Create(role);
            state.Power = 0;
            state.Gold = state.Food = state.MilitarySupplies = state.Manpower = state.Troops = 0;
            Patron(state).Relationship = 0;
            Assert.AreEqual(0, CampaignCore.GetPatronRepairTerms(state).PowerCost);
            Success(CampaignCore.RepairPatronTrust(state));
            Assert.AreEqual(0, state.Power);
            Assert.AreEqual(4, Patron(state).Relationship);
            Refused(state, () => CampaignCore.IssueMandate(state, "ile"), "error.mandate.power");
            CampaignCore.Validate(state);
        }

        [Test]
        public void FractionalRemainingPowerIsPreviewedExactly()
        {
            var state = CampaignCore.Create("crown");
            state.Power = 2.5f;
            Patron(state).Relationship = 0;
            Assert.AreEqual(2.5f, CampaignCore.GetPatronRepairTerms(state).PowerCost);
            Success(CampaignCore.RepairPatronTrust(state));
            Assert.AreEqual(0, state.Power);
        }

        [Test]
        public void PetitionPriorityAndOrdinaryOrdersRemainIndependentOfPatronTrust()
        {
            var state = CampaignCore.Create("crown");
            Patron(state).Relationship = 0;
            Success(CampaignCore.Act(state, "tax", "burgundy"));
            Success(CampaignCore.NextWeek(state));
            Success(CampaignCore.NextWeek(state));
            Assert.IsTrue(state.PendingPetition);
            Refused(state, () => CampaignCore.RepairPatronTrust(state), "error.mandate.petition");
            Success(CampaignCore.ChoosePetition(state, "relief"));
            Success(CampaignCore.RepairPatronTrust(state));
        }

        [Test]
        public void FiveNaturalDefaultsEndUnconditionalRoyalAdvances()
        {
            var state = CampaignCore.Create("crown");
            for (int cycle = 0; cycle < 5; cycle++)
            {
                Success(CampaignCore.IssueMandate(state, "ile"));
                while (state.Week < cycle * 4 + 2) Advance(state);
                Success(CampaignCore.ResolveMandate(state, CampaignCore.MandateId(state.Obligation), "break"));
                while (state.Week < (cycle + 1) * 4) Advance(state);
                CampaignCore.Validate(state);
            }
            Assert.AreEqual(20, state.Week);
            Assert.AreEqual(0, Patron(state).Relationship);
            Assert.AreEqual(0, state.Factions.Find(faction => faction.Id == "crown").Approval);
            Assert.GreaterOrEqual(state.Power, 10);
            Refused(state, () => CampaignCore.IssueMandate(state, "ile"), "error.trust.closed");
            float previousPower = state.Power;
            Success(CampaignCore.RepairPatronTrust(state));
            Assert.AreEqual(previousPower - 6, state.Power);
            Success(CampaignCore.IssueMandate(state, "ile"));
            Assert.AreEqual(22, state.Obligation.DueWeek);
        }

        [Test]
        public void LegacyCampaignHasNoNewPatronObligation()
        {
            var state = CampaignCore.Create();
            Assert.IsNull(CampaignCore.GetPatronRepairTerms(state));
            Refused(state, () => CampaignCore.RepairPatronTrust(state), "error.role.legacy");
        }

        static void Advance(CampaignState state)
        {
            Success(CampaignCore.NextWeek(state));
            if (state.PendingPetition) Success(CampaignCore.ChoosePetition(state, "relief"));
        }
    }
}
#endif
