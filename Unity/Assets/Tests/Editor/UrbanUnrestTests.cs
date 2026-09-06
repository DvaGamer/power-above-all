#if UNITY_EDITOR
using System;
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    public sealed class UrbanUnrestTests
    {
        static FactionState Urban(CampaignState state) => state.Factions.Find(item => item.Id == "urban");
        static string Snapshot(CampaignState state) => JsonUtility.ToJson(state);
        static void Success(ActionResult result) { Assert.IsTrue(result.Ok, result.Key); }
        static CampaignState SubsidyFixture(float approval, bool hungry)
        {
            var state = CampaignCore.Create();
            Success(CampaignCore.Act(state, "subsidy", "ile"));
            Urban(state).Approval = approval;
            if (hungry) state.Food = 0;
            CampaignCore.Validate(state); return state;
        }
        static CampaignState PreviewWeek(CampaignState state)
        {
            string before = Snapshot(state);
            var preview = JsonUtility.FromJson<CampaignState>(before);
            Success(CampaignCore.NextWeek(preview));
            Assert.AreEqual(before, Snapshot(state), "Önizleme asıl kampanyayı değiştirmemeli.");
            return preview;
        }
        static void CommitMatchesAndPreservesPreview(CampaignState state, CampaignState preview)
        {
            string projected = Snapshot(preview);
            Success(CampaignCore.NextWeek(state));
            Assert.AreEqual(projected, Snapshot(state), "Gerçek hesap önizlemenin bütün sonuçlarıyla eşleşmeli.");
            Assert.AreEqual(projected, Snapshot(preview), "Gerçek hesap önceki önizleme kopyasını değiştirmemeli.");
        }

        [TestCase(0f, 2)]
        [TestCase(39.99f, 2)]
        [TestCase(40f, 0)]
        [TestCase(59.99f, 0)]
        [TestCase(60f, -1)]
        [TestCase(100f, -1)]
        public void UrbanContributionPreservesBothThresholdsAndValidEndpoints(float approval, int expected)
        { Assert.AreEqual(expected, CampaignCore.UrbanUnrestDelta(approval)); }

        [Test]
        public void InvalidApprovalCannotBePresentedAsAValidPoliticalContribution()
        {
            foreach (float approval in new[] { float.NaN, float.PositiveInfinity, float.NegativeInfinity, -.01f, 100.01f })
            {
                var error = Assert.Throws<ArgumentOutOfRangeException>(() => CampaignCore.UrbanUnrestDelta(approval));
                Assert.AreEqual("approval", error.ParamName);
            }
        }

        [TestCase(39f, 42f, 0)]
        [TestCase(59f, 62f, -1)]
        public void PaidSubsidyCrossesTheThresholdBeforeApplyingTheRegionalWeek(float beforeApproval, float afterApproval, int unrestChange)
        {
            var state = SubsidyFixture(beforeApproval, false);
            var region = CampaignCore.Region(state, "normandy");
            float beforeUnrest = region.Unrest, beforeControl = region.Control;
            Assert.AreNotEqual(CampaignCore.UrbanUnrestDelta(beforeApproval), unrestChange);
            var preview = PreviewWeek(state);
            Assert.AreEqual(afterApproval, Urban(preview).Approval);
            Assert.AreEqual(unrestChange, CampaignCore.UrbanUnrestDelta(Urban(preview).Approval));
            Assert.AreEqual(beforeUnrest + unrestChange, CampaignCore.Region(preview, "normandy").Unrest);
            Assert.AreEqual(beforeControl, CampaignCore.Region(preview, "normandy").Control);
            Assert.IsTrue(preview.Journal.Exists(entry => entry.Key == "log.subsidy.paid" && entry.Week == 1));
            Assert.AreEqual(1200, preview.Troops); Assert.Greater(preview.Food, 0);
            CommitMatchesAndPreservesPreview(state, preview);
        }

        [Test]
        public void FailedSubsidyUsesReducedApprovalAndAddsHungerSeparatelyInTheRealWeek()
        {
            var state = SubsidyFixture(45, true);
            Assert.Less(CampaignCore.Forecast(state).NetFood, 0);
            Assert.AreEqual(0, CampaignCore.UrbanUnrestDelta(Urban(state).Approval));
            float beforeUnrest = CampaignCore.Region(state, "normandy").Unrest;
            var preview = PreviewWeek(state);
            Assert.AreEqual(37, Urban(preview).Approval);
            Assert.AreEqual(2, CampaignCore.UrbanUnrestDelta(Urban(preview).Approval));
            Assert.AreEqual(beforeUnrest + 10, CampaignCore.Region(preview, "normandy").Unrest,
                "Garnizonsuz bölgede yeni şehir katkısı+2 ve açlık+8 birlikte uygulanmalı.");
            Assert.AreEqual(0, preview.Food); Assert.AreEqual(1104, preview.Troops);
            Assert.IsTrue(preview.Journal.Exists(entry => entry.Key == "log.subsidy.failed" && entry.Week == 1));
            Assert.IsFalse(preview.Journal.Exists(entry => entry.Key == "log.subsidy.paid"));
            CommitMatchesAndPreservesPreview(state, preview);
        }
    }
}
#endif
