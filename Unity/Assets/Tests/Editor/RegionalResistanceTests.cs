#if UNITY_EDITOR
using System;
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    public sealed class RegionalResistanceTests
    {
        [Serializable] public sealed class ArrivalEvidence
        {
            public MarchPreview Arrival;
        }

        static string Snapshot(CampaignState state) => JsonUtility.ToJson(state);
        static void Success(ActionResult result) { Assert.IsTrue(result.Ok, result.Key); }
        static RegionalResistanceTerms Observe(CampaignState state, string region = "champagne")
        {
            string before = Snapshot(state);
            var terms = CampaignCore.GetRegionalResistance(state, region);
            Assert.IsNotNull(terms); Assert.AreEqual(before, Snapshot(state));
            return terms;
        }

        [Test]
        public void RealMarchArrivalSurvivesNestedJsonUtilityEvidenceRoundtripWithEveryField()
        {
            var state = CampaignCore.Create();
            var supplied = CampaignCore.PreviewMarch(state, "normandy");
            for (int i = 0; i < 9; i++) Success(CampaignCore.Act(state, "bread", CampaignCore.Regions[i].Id));
            var hungry = CampaignCore.PreviewMarch(state, "champagne");
            Assert.IsNotNull(supplied); Assert.IsFalse(supplied.Hungry); Assert.IsFalse(supplied.Difficult);
            Assert.IsNotNull(hungry); Assert.IsTrue(hungry.Hungry); Assert.IsTrue(hungry.Difficult);
            foreach (var arrival in new[] { supplied, hungry })
            {
                string json = JsonUtility.ToJson(new ArrivalEvidence { Arrival = arrival });
                StringAssert.Contains("\"Arrival\"", json);
                var restored = JsonUtility.FromJson<ArrivalEvidence>(json);
                Assert.IsNotNull(restored); Assert.IsNotNull(restored.Arrival);
                Assert.AreEqual(arrival.FoodCost, restored.Arrival.FoodCost);
                Assert.AreEqual(arrival.FoodAfter, restored.Arrival.FoodAfter);
                Assert.AreEqual(arrival.MilitarySuppliesAfter, restored.Arrival.MilitarySuppliesAfter);
                Assert.AreEqual(arrival.MovesAfter, restored.Arrival.MovesAfter);
                Assert.AreEqual(arrival.Supply, restored.Arrival.Supply);
                Assert.AreEqual(arrival.Fatigue, restored.Arrival.Fatigue);
                Assert.AreEqual(arrival.Morale, restored.Arrival.Morale);
                Assert.AreEqual(arrival.Difficult, restored.Arrival.Difficult);
                Assert.AreEqual(arrival.Hungry, restored.Arrival.Hungry);
            }
        }

        [Test]
        public void InitialBreakdownUsesLocalFactsAndReturnsDetachedReadonlyTerms()
        {
            var state = CampaignCore.Create(); var terms = Observe(state);
            Assert.AreEqual("champagne", terms.RegionId); Assert.IsTrue(terms.RequiresBattle);
            Assert.AreEqual(25, terms.BaseTax); Assert.AreEqual(750d, terms.MobilizationBase);
            Assert.AreEqual(.69d, terms.UnrestPressure, 1e-12);
            Assert.AreEqual(.395d, terms.ControlGap, 1e-12);
            Assert.AreEqual(.4d, terms.EliteOpposition, 1e-12);
            Assert.AreEqual(1114, terms.EnemyTroops);
            terms.RegionId = "ile"; terms.EnemyTroops = 1; terms.MobilizationBase = 0;
            Assert.AreEqual(1114, Observe(state).EnemyTroops);
        }

        [Test]
        public void RolesArmySizeAndNationalResourcesCannotRubberBandTheTarget()
        {
            foreach (string role in new[] { "legacy", "crown", "assembly", "army" })
            foreach (int troops in new[] { 0, 200, 1200, 1600, 100000000 })
            {
                var state = role == "legacy" ? CampaignCore.Create() : CampaignCore.Create(role);
                state.Troops = troops; state.Gold = troops; state.Food = troops;
                state.MilitarySupplies = troops; state.Manpower = troops;
                state.Power = troops == 0 ? 0 : 100; state.Supply = state.Power;
                state.Factions.Find(item => item.Id == "assembly").Approval = state.Power;
                Assert.AreEqual(1114, Observe(state).EnemyTroops, role + ":" + troops);
            }
        }

        [Test]
        public void RealPaidOfficerRecruitmentInParisLeavesChampagneStrengthUnchanged()
        {
            var state = CampaignCore.Create("army");
            Success(CampaignCore.Act(state, "recruit", "ile"));
            Success(CampaignCore.GrantOfficerCommission(state));
            Success(CampaignCore.RecruitThroughDumas(state));
            Assert.AreEqual(1600, state.Troops); Assert.AreEqual(600, state.Gold);
            Assert.AreEqual(320, state.Food); Assert.AreEqual(90, state.MilitarySupplies);
            Assert.IsTrue(state.DumasOfficerCommission); Assert.IsTrue(state.DumasExtraRecruitUsed);
            Assert.AreEqual(1114, Observe(state).EnemyTroops);
        }

        [Test]
        public void TaxAndPartialBreadReliefChangeAStillHostileForceThroughLocalPolitics()
        {
            var state = CampaignCore.Create(); var original = Observe(state);
            Success(CampaignCore.Act(state, "tax", "champagne"));
            Assert.AreEqual(1234, Observe(state).EnemyTroops);
            Success(CampaignCore.Act(state, "bread", "champagne"));
            Assert.AreEqual(66, CampaignCore.Region(state, "champagne").Unrest);
            Assert.AreEqual(1106, Observe(state).EnemyTroops);
            Assert.IsTrue(CampaignCore.CanMarch(state, "champagne").RequiresBattle);
            Assert.AreEqual(1114, original.EnemyTroops, "Eski önizleme canlı duruma bağlı değişmemeli.");
        }

        [Test]
        public void TaxHolidayKeepsItsFourSettlementsAndCanEitherReduceOrRemoveResistance()
        {
            var taxed = CampaignCore.Create();
            Success(CampaignCore.Act(taxed, "tax", "champagne"));
            Success(CampaignCore.GrantRegionalAccord(taxed, "champagne"));
            Assert.AreEqual(1136, Observe(taxed).EnemyTroops);
            Assert.AreEqual(4, taxed.AccordUntilWeek); Assert.IsTrue(CampaignCore.HasRegionalAccord(taxed));
            var calm = CampaignCore.Create();
            Success(CampaignCore.GrantRegionalAccord(calm, "champagne"));
            var terms = Observe(calm); Assert.IsFalse(terms.RequiresBattle); Assert.AreEqual(0, terms.EnemyTroops);
            var march = CampaignCore.CanMarch(calm, "champagne"); Assert.IsTrue(march.Ok); Assert.IsFalse(march.RequiresBattle);
            Success(CampaignCore.March(calm, "champagne"));
            Assert.IsEmpty(calm.ResolvedBattles); Assert.AreEqual("champagne", calm.ArmyRegionId);
        }

        [Test]
        public void SuccessfulWeekChangesUnrestAndControlBeforeTheNextEstimate()
        {
            var state = CampaignCore.Create(); Assert.AreEqual(1114, Observe(state).EnemyTroops);
            Success(CampaignCore.NextWeek(state));
            Assert.AreEqual(71, CampaignCore.Region(state, "champagne").Unrest);
            Assert.AreEqual(57.5f, CampaignCore.Region(state, "champagne").Control);
            Assert.AreEqual(1151, Observe(state).EnemyTroops);
        }

        [Test]
        public void LivingGarrisonInfluencesResistanceOnlyThroughTheExistingWeeklyRegionalEffects()
        {
            var stationed = CampaignCore.Create();
            Success(CampaignCore.GrantRegionalAccord(stationed, "champagne"));
            Success(CampaignCore.March(stationed, "champagne"));
            Success(CampaignCore.Act(stationed, "tax", "champagne"));
            // Aynı geçerli bölge durumunda garnizonun bulunmadığı sınır durumu karşılaştırılır.
            var empty = CampaignArchive.Deserialize(CampaignArchive.Serialize(stationed, false)); empty.Troops = 0;
            Assert.AreEqual(1264, Observe(stationed).EnemyTroops);
            Assert.AreEqual(1264, Observe(empty).EnemyTroops);
            Success(CampaignCore.NextWeek(stationed)); Success(CampaignCore.NextWeek(empty));
            Assert.AreEqual(1264, Observe(stationed).EnemyTroops);
            Assert.AreEqual(1301, Observe(empty).EnemyTroops);
        }

        [TestCase(64.99f, false, 0)]
        [TestCase(65f, true, 1084)]
        public void MarchAndIndependentEstimateShareTheExistingHostilityBoundary(float unrest, bool hostile, int enemy)
        {
            var state = CampaignCore.Create(); CampaignCore.Region(state, "champagne").Unrest = unrest;
            var terms = Observe(state); Assert.AreEqual(hostile, terms.RequiresBattle); Assert.AreEqual(enemy, terms.EnemyTroops);
            Assert.AreEqual(hostile, CampaignCore.CanMarch(state, "champagne").RequiresBattle);
        }

        [Test]
        public void SingleFinalRoundingUsesAwayFromZeroAndDoesNotAddAPlayerBasedMinimum()
        {
            var state = CampaignCore.Create(); var region = CampaignCore.Region(state, "poitou");
            region.Unrest = 65; region.Control = region.EliteLoyalty = 100;
            Assert.AreEqual(371, Observe(state, "poitou").EnemyTroops, "370,5 toplamı371 olmalı.");
            var paris = CampaignCore.Region(state, "ile"); paris.Unrest = 100; paris.Control = paris.EliteLoyalty = 0;
            Assert.AreEqual(4320, Observe(state, "ile").EnemyTroops);
        }

        [Test]
        public void InspectionDoesNotBypassMarchGuardsAndRemainsAvailableWithoutAnArmyOrMoves()
        {
            var state = CampaignCore.Create(); state.Troops = state.Moves = 0;
            Assert.AreEqual(1114, Observe(state).EnemyTroops);
            Assert.AreEqual("error.region", CampaignCore.CanMarch(state, "unknown").Key);
            Assert.AreEqual("error.army.empty", CampaignCore.CanMarch(state, "champagne").Key);
            state.Troops = 1200;
            Assert.AreEqual("error.moves", CampaignCore.CanMarch(state, "champagne").Key);
            Assert.AreEqual(0, Observe(state, "provence").EnemyTroops);
            state.Moves = 2;
            Assert.AreEqual("error.adjacent", CampaignCore.CanMarch(state, "provence").Key);
        }

        [Test]
        public void InvalidCampaignAndUnknownRegionReturnNoTermsWithoutMutation()
        {
            Assert.IsNull(CampaignCore.GetRegionalResistance(null, "champagne"));
            var state = CampaignCore.Create(); string before = Snapshot(state);
            Assert.IsNull(CampaignCore.GetRegionalResistance(state, "unknown"));
            Assert.IsNull(CampaignCore.GetRegionalResistance(state, null)); Assert.AreEqual(before, Snapshot(state));
            state.Gold = -1; before = Snapshot(state);
            Assert.IsNull(CampaignCore.GetRegionalResistance(state, "champagne")); Assert.AreEqual(before, Snapshot(state));
            state.Gold = 840; CampaignCore.Region(state, "champagne").Unrest = float.NaN;
            Assert.IsNull(CampaignCore.GetRegionalResistance(state, "champagne"));
        }

        [Test]
        public void ArchiveAndOverlappingPetitionMandateAccordAndCommissionKeepInspectionReadonly()
        {
            var state = CampaignCore.Create("army");
            Success(CampaignCore.IssueMandate(state, "ile"));
            Success(CampaignCore.GrantRegionalAccord(state, "champagne"));
            Success(CampaignCore.GrantOfficerCommission(state));
            Success(CampaignCore.Act(state, "recruit", "ile")); Success(CampaignCore.RecruitThroughDumas(state));
            Success(CampaignCore.NextWeek(state)); Success(CampaignCore.NextWeek(state));
            Assert.IsTrue(state.PendingPetition); Assert.IsNotNull(state.Obligation);
            Assert.AreEqual(2, state.Obligation.DueWeek); Assert.IsTrue(state.DumasOfficerCommission);
            Assert.IsTrue(CampaignCore.HasRegionalAccord(state));
            string before = CampaignArchive.Serialize(state, false); var terms = Observe(state);
            var loaded = CampaignArchive.Deserialize(before);
            Assert.AreEqual(before, CampaignArchive.Serialize(loaded, false));
            Assert.AreEqual(terms.EnemyTroops, Observe(loaded).EnemyTroops);
            Assert.AreEqual(terms.RequiresBattle, Observe(loaded).RequiresBattle);
            var refused = CampaignCore.NextWeek(loaded); Assert.IsFalse(refused.Ok);
            Assert.AreEqual(before, CampaignArchive.Serialize(loaded, false));
        }
    }
}
#endif
