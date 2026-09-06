#if UNITY_EDITOR
using System;
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    public sealed class OfficerCommissionTests
    {
        static string Snapshot(CampaignState state) => JsonUtility.ToJson(state);
        static CharacterState Dumas(CampaignState state) => state.Characters.Find(person => person.Id == "dumas");
        static void Success(ActionResult result) { Assert.IsTrue(result.Ok, result.Key); }
        static CampaignState Reload(CampaignState state)
        {
            string before = Snapshot(state), json = CampaignArchive.Serialize(state, false);
            StringAssert.Contains("\"Version\":" + CampaignArchive.CurrentVersion, json);
            var loaded = CampaignArchive.Deserialize(json); Assert.AreEqual(before, Snapshot(loaded)); return loaded;
        }
        static void Refused(CampaignState state, Func<ActionResult> action, string key)
        {
            string before = Snapshot(state); var result = action();
            Assert.IsFalse(result.Ok); Assert.AreEqual(key, result.Key); Assert.AreEqual(before, Snapshot(state));
        }
        static CampaignState Ready()
        {
            var state = CampaignCore.Create(); Success(CampaignCore.GrantOfficerCommission(state));
            Success(CampaignCore.Act(state, "recruit", "ile")); return state;
        }
        static void Advance(CampaignState state)
        {
            var f = CampaignCore.Forecast(state); int gold = state.Gold, food = state.Food;
            Success(CampaignCore.NextWeek(state));
            Assert.AreEqual(Math.Max(0, gold + f.NetGold), state.Gold);
            Assert.AreEqual(Math.Max(0, food + f.NetFood), state.Food);
            Assert.IsFalse(state.DumasExtraRecruitUsed); CampaignCore.Validate(state);
        }

        [Test]
        public void GrantCreatesOnlyTheRightAndClosedTermsRemainReadOnly()
        {
            var state = CampaignCore.Create(); state.Power = 0; Dumas(state).Relationship = 0;
            string before = Snapshot(state); var closed = CampaignCore.GetOfficerCommissionTerms(state);
            Assert.IsFalse(closed.IsActive); Assert.AreEqual(100, closed.RevokeGoldCost);
            Assert.AreEqual(before, Snapshot(state));
            Success(CampaignCore.GrantOfficerCommission(state));
            Assert.IsTrue(CampaignCore.HasOfficerCommission(state));
            Assert.AreEqual(1200, state.Troops); Assert.AreEqual(840, state.Gold);
            Assert.AreEqual(60, Dumas(state).Loyalty); Assert.AreEqual(0, state.Power);
            Assert.AreEqual(0, Dumas(state).Relationship);
            Refused(state, () => CampaignCore.GrantOfficerCommission(state), "error.commission.active");
            Refused(state, () => CampaignCore.RecruitThroughDumas(state), "error.commission.normal_required"); Reload(state);
        }

        [Test]
        public void PaidExtraGroupHasExactlyTheNormalCostsAndLocalEffectsPlusOneActualLoyalty()
        {
            var state = Ready(); var region = CampaignCore.Region(state, "ile");
            float unrest = region.Unrest, morale = state.Morale, approval = state.Factions.Find(f => f.Id == "army").Approval;
            string before = Snapshot(state); var terms = CampaignCore.GetOfficerCommissionTerms(state);
            Assert.AreEqual(before, Snapshot(state)); Assert.AreEqual(1400, terms.CurrentTroops);
            Assert.AreEqual(200, terms.RecruitTroops); Assert.AreEqual(120, terms.GoldCost);
            Assert.AreEqual(20, terms.FoodCost); Assert.AreEqual(15, terms.MilitarySuppliesCost); Assert.AreEqual(200, terms.ManpowerCost);
            Assert.AreEqual(1600, terms.TroopsAfterRecruit); Assert.AreEqual(2000, terms.ManpowerAfterRecruit);
            Assert.AreEqual(153, terms.CurrentArmyCost); Assert.AreEqual(170, terms.ArmyCostAfterRecruit);
            Assert.AreEqual(47, terms.CurrentArmyConsumption); Assert.AreEqual(54, terms.ArmyConsumptionAfterRecruit);
            Assert.AreEqual(117, terms.RevokeGoldCost);
            terms.GoldCost = 0; terms.LoyaltyDelta = 99;
            Success(CampaignCore.RecruitThroughDumas(state));
            Assert.AreEqual(1600, state.Troops); Assert.AreEqual(2000, state.Manpower);
            Assert.AreEqual(600, state.Gold); Assert.AreEqual(320, state.Food); Assert.AreEqual(90, state.MilitarySupplies);
            Assert.AreEqual(unrest + 2, region.Unrest); Assert.AreEqual(morale - 2, state.Morale);
            Assert.AreEqual(approval + 2, state.Factions.Find(f => f.Id == "army").Approval);
            Assert.AreEqual(61, Dumas(state).Loyalty); Assert.AreEqual(80, Dumas(state).Ambition);
            Assert.AreEqual(50, Dumas(state).Relationship); Assert.AreEqual(55, state.Power);
            Assert.AreEqual(2, state.Moves); Assert.AreEqual(0, state.Fatigue);
            CollectionAssert.AreEqual(new[] { "region.ile", "200", "1" }, state.Journal[0].Args);
            Assert.AreEqual(134, CampaignCore.GetOfficerCommissionTerms(state).RevokeGoldCost); Reload(state);
        }

        [Test]
        public void OrdinaryRecruitmentKeepsItsOriginalRefusalOrderAndRegionalMovementAlternative()
        {
            var state = CampaignCore.Create(); var other = CampaignCore.Region(state, "normandy");
            other.RecruitUsed = true; state.Gold = 0; state.Troops = 100000000;
            Refused(state, () => CampaignCore.Act(state, "recruit", "normandy"), "error.used");
            other.RecruitUsed = false;
            Refused(state, () => CampaignCore.Act(state, "recruit", "normandy"), "error.recruit.location");
            Refused(state, () => CampaignCore.Act(state, "recruit", "ile"), "error.recruit.cost");
            state.Gold = 120;
            Refused(state, () => CampaignCore.Act(state, "recruit", "ile"), "error.capacity");
            var route = CampaignCore.Create(); Success(CampaignCore.Act(route, "recruit", "ile"));
            Success(CampaignCore.March(route, "normandy")); Success(CampaignCore.Act(route, "recruit", "normandy"));
            Assert.AreEqual(1600, route.Troops); Assert.AreEqual(1, route.Moves); Assert.AreEqual(10, route.Fatigue);
            Assert.AreEqual(60, Dumas(route).Loyalty); Assert.IsFalse(route.DumasOfficerCommission); Reload(route);
        }

        [Test]
        public void GlobalUseSurvivesRevocationRegrantMovementAndReloadAndResetsOnlyOnSuccessfulWeek()
        {
            var state = Ready(); Success(CampaignCore.RecruitThroughDumas(state));
            Success(CampaignCore.RevokeOfficerCommission(state));
            Assert.IsTrue(state.DumasExtraRecruitUsed); state = Reload(state);
            Success(CampaignCore.GrantOfficerCommission(state));
            Refused(state, () => CampaignCore.RecruitThroughDumas(state), "error.commission.used");
            Success(CampaignCore.March(state, "normandy")); Success(CampaignCore.Act(state, "recruit", "normandy"));
            Refused(state, () => CampaignCore.RecruitThroughDumas(state), "error.commission.used");
            Advance(state);
            Refused(state, () => CampaignCore.RecruitThroughDumas(state), "error.commission.normal_required");
            Success(CampaignCore.Act(state, "recruit", "normandy")); Success(CampaignCore.RecruitThroughDumas(state));
            Assert.AreEqual(2200, state.Troops); Assert.AreEqual(62, Dumas(state).Loyalty); Reload(state);
        }

        [Test]
        public void ThePrerequisiteIsTheCurrentCampNotAnyEarlierRecruitment()
        {
            var state = Ready(); Success(CampaignCore.March(state, "normandy"));
            Refused(state, () => CampaignCore.RecruitThroughDumas(state), "error.commission.normal_required");
            Success(CampaignCore.Act(state, "recruit", "normandy")); Success(CampaignCore.RecruitThroughDumas(state));
            Assert.AreEqual(1800, state.Troops); Assert.IsTrue(CampaignCore.Region(state, "normandy").RecruitUsed); Reload(state);
        }

        [Test]
        public void RevocationPreservesPeopleAndEarnedLoyaltyThenUnlocksTheOriginalTwoWeekReduction()
        {
            var state = Ready(); Success(CampaignCore.RecruitThroughDumas(state));
            Refused(state, () => CampaignCore.SetArmyEstablishment(state, "budget", 1600), "error.establishment.commission");
            Refused(state, () => CampaignCore.SetArmyEstablishment(state, "budget", 1000), "error.establishment.commission");
            Success(CampaignCore.RevokeOfficerCommission(state));
            Assert.AreEqual(466, state.Gold); Assert.AreEqual(1600, state.Troops); Assert.AreEqual(2000, state.Manpower);
            Assert.AreEqual(61, Dumas(state).Loyalty); Assert.IsTrue(state.DumasExtraRecruitUsed);
            Refused(state, () => CampaignCore.RevokeOfficerCommission(state), "error.commission.none");
            Success(CampaignCore.SetArmyEstablishment(state, "budget", 1400));
            Assert.AreEqual(2, state.ArmyReductionDueWeek);
            Refused(state, () => CampaignCore.GrantOfficerCommission(state), "error.commission.policy");
            Advance(state); Assert.AreEqual(1600, state.Troops);
            Advance(state); Assert.AreEqual(1400, state.Troops); Assert.AreEqual(2200, state.Manpower);
            Assert.AreEqual(61, Dumas(state).Loyalty); Reload(state);
        }

        [TestCase(99.5f, .5f)]
        [TestCase(100f, 0f)]
        public void ClampedPersonalAndRegionalEffectsRemainHonestAndTheRecruitStillHasValueAtFullLoyalty(float loyalty, float gain)
        {
            var state = Ready(); Dumas(state).Loyalty = loyalty; state.Morale = 1;
            CampaignCore.Region(state, "ile").Unrest = 99.5f; state.Factions.Find(f => f.Id == "army").Approval = 99.5f;
            var terms = CampaignCore.GetOfficerCommissionTerms(state);
            Assert.AreEqual(gain, terms.LoyaltyDelta); Assert.AreEqual(.5f, terms.UnrestDelta);
            Assert.AreEqual(-1, terms.MoraleDelta); Assert.AreEqual(.5f, terms.ArmyApprovalDelta);
            Success(CampaignCore.RecruitThroughDumas(state)); Assert.AreEqual(1600, state.Troops);
            Assert.AreEqual(100, Dumas(state).Loyalty); Assert.AreEqual(0, state.Morale);
            Assert.AreEqual(100, CampaignCore.Region(state, "ile").Unrest); Reload(state);
        }

        [TestCase("gold")]
        [TestCase("food")]
        [TestCase("supplies")]
        [TestCase("manpower")]
        [TestCase("capacity")]
        public void FailedExtraRecruitmentDoesNotSpendItsRightOrAnyResource(string shortage)
        {
            var state = Ready();
            switch (shortage)
            {
                case "gold": state.Gold = 119; break;
                case "food": state.Food = 19; break;
                case "supplies": state.MilitarySupplies = 14; break;
                case "manpower": state.Manpower = 199; break;
                case "capacity": state.Troops = 100000000 - 199; break;
            }
            Refused(state, () => CampaignCore.RecruitThroughDumas(state), shortage == "capacity" ? "error.capacity" : "error.recruit.cost");
            Assert.IsFalse(state.DumasExtraRecruitUsed); Assert.AreEqual(60, Dumas(state).Loyalty);
            var terms = CampaignCore.GetOfficerCommissionTerms(state);
            Assert.AreEqual(state.Troops, terms.TroopsAfterRecruit); Assert.AreEqual(state.Manpower, terms.ManpowerAfterRecruit); Reload(state);
        }

        [Test]
        public void ActualBattleLossesChangeRevocationPriceWithoutClearingTheRightOrOpenVictory()
        {
            var state = Ready(); Success(CampaignCore.RecruitThroughDumas(state));
            Success(CampaignCore.ResolveBattle(state, "champagne", "battle-0-2-ile-champagne", true, 196, 60));
            Assert.AreEqual(1404, state.Troops); Assert.AreEqual(117, CampaignCore.GetOfficerCommissionTerms(state).RevokeGoldCost);
            string victory = state.PendingVictoryId; int gold = state.Gold;
            Success(CampaignCore.RevokeOfficerCommission(state)); Assert.AreEqual(gold - 117, state.Gold);
            Assert.AreEqual(victory, state.PendingVictoryId); Assert.AreEqual(1404, state.Troops);
            Success(CampaignCore.GrantOfficerCommission(state)); Assert.AreEqual(victory, state.PendingVictoryId); Reload(state);
        }

        [Test]
        public void AZeroArmyAndZeroPowerCanReclaimTheRightForFreeAndUseTheExistingRecoveryActions()
        {
            var state = CampaignCore.Create(); Success(CampaignCore.GrantOfficerCommission(state));
            state.Power = 0; state.Gold = 0; Dumas(state).Relationship = 0;
            Success(CampaignCore.ResolveBattle(state, "champagne", "battle-0-2-ile-champagne", false, 1200, 0));
            Assert.AreEqual(0, state.Troops); Assert.AreEqual(0, state.Power);
            Assert.AreEqual(0, CampaignCore.GetOfficerCommissionTerms(state).RevokeGoldCost);
            Success(CampaignCore.RevokeOfficerCommission(state)); Assert.AreEqual(0, state.Gold);
            Refused(state, () => CampaignCore.GrantOfficerCommission(state), "error.army.empty");
            Success(CampaignCore.Act(state, "tax", "ile")); Success(CampaignCore.Act(state, "tax", "normandy"));
            Success(CampaignCore.Act(state, "recruit", "ile"));
            Assert.AreEqual(200, state.Troops); Assert.AreEqual(2200, state.Manpower); Assert.AreEqual(80, state.Gold);
            Assert.AreEqual(0, state.Power); Reload(state);
        }

        [Test]
        public void UnaffordableRevocationIsAtomicAndDoesNotNeedPersonalPowerOrGoodRelations()
        {
            var state = Ready(); state.Gold = 116; state.Power = 0; Dumas(state).Relationship = 0;
            Refused(state, () => CampaignCore.RevokeOfficerCommission(state), "error.commission.gold");
            Assert.AreEqual("117", CampaignCore.CanRevokeOfficerCommission(state).Args[0]);
            Success(CampaignCore.Act(state, "tax", "normandy"));
            Success(CampaignCore.RevokeOfficerCommission(state)); Assert.AreEqual(99, state.Gold);
            Assert.AreEqual(1400, state.Troops); Assert.AreEqual(0, state.Power); Reload(state);
        }

        [Test]
        public void PaidLoyaltyCanActuallyCrossTheExistingRecognitionThresholdWithoutChangingItsRules()
        {
            var state = Ready(); Dumas(state).Loyalty = 82;
            Success(CampaignCore.RecruitThroughDumas(state));
            Success(CampaignCore.ResolveBattle(state, "champagne", "battle-0-2-ile-champagne", true, 100, 60));
            Assert.AreEqual(83, Dumas(state).Loyalty); Assert.AreEqual(83, Dumas(state).Ambition);
            Assert.AreEqual(0, CampaignCore.GetVictoryDecisionTerms(state, "recognize").PowerCost);
            float power = state.Power; Success(CampaignCore.ResolveVictory(state, state.PendingVictoryId, "recognize"));
            Assert.AreEqual(power, state.Power); Reload(state);
        }

        [Test]
        public void ExistingPetitionMandateAndCalendarGuardsPrecedeAllNewCommandEffects()
        {
            var state = CampaignCore.Create("crown"); Success(CampaignCore.IssueMandate(state, "ile"));
            Success(CampaignCore.GrantRegionalAccord(state, "normandy"));
            Success(CampaignCore.GrantOfficerCommission(state));
            Success(CampaignCore.Act(state, "recruit", "ile")); Success(CampaignCore.RecruitThroughDumas(state));
            string obligation = CampaignCore.MandateId(state.Obligation);
            Advance(state); Advance(state);
            Refused(state, () => CampaignCore.GrantOfficerCommission(state), "error.mandate.petition");
            Refused(state, () => CampaignCore.RecruitThroughDumas(state), "error.mandate.petition");
            Refused(state, () => CampaignCore.RevokeOfficerCommission(state), "error.mandate.petition");
            Success(CampaignCore.ChoosePetition(state, "negotiate"));
            Refused(state, () => CampaignCore.GrantOfficerCommission(state), "error.mandate.due");
            Refused(state, () => CampaignCore.RecruitThroughDumas(state), "error.mandate.due");
            Refused(state, () => CampaignCore.RevokeOfficerCommission(state), "error.mandate.due");
            Success(CampaignCore.ResolveMandate(state, obligation, "fulfil"));
            Assert.AreEqual("normandy", state.AccordRegionId); Assert.AreEqual(4, state.AccordUntilWeek);
            Advance(state); Advance(state); Assert.IsFalse(CampaignCore.HasRegionalAccord(state)); Reload(state);
            var final = CampaignCore.Create(); Success(CampaignCore.GrantOfficerCommission(final));
            final.Week = 1000000; final.PetitionResolved = true;
            Refused(final, () => CampaignCore.GrantOfficerCommission(final), "error.week.limit");
            Refused(final, () => CampaignCore.RecruitThroughDumas(final), "error.week.limit");
            Refused(final, () => CampaignCore.RevokeOfficerCommission(final), "error.week.limit"); Reload(final);
        }

        [Test]
        public void ARejectedWeekCannotClearTheGlobalUsedFlag()
        {
            var state = Ready(); Advance(state); Advance(state);
            Success(CampaignCore.ChoosePetition(state, "negotiate"));
            Success(CampaignCore.Act(state, "recruit", "ile")); Success(CampaignCore.RecruitThroughDumas(state));
            state.Week = 1000000; CampaignCore.Validate(state);
            Refused(state, () => CampaignCore.NextWeek(state), "error.week.limit");
            Assert.IsTrue(state.DumasExtraRecruitUsed); Reload(state);
        }

        [TestCase("fresh")]
        [TestCase("open")]
        [TestCase("used")]
        [TestCase("closed_used")]
        [TestCase("budget_used")]
        public void V7RoundTripsBothRightsAndTheirIndependentUsageState(string phase)
        {
            var state = CampaignCore.Create();
            if (phase != "fresh") Success(CampaignCore.GrantOfficerCommission(state));
            if (phase == "used" || phase == "closed_used" || phase == "budget_used")
            { Success(CampaignCore.Act(state, "recruit", "ile")); Success(CampaignCore.RecruitThroughDumas(state)); }
            if (phase == "closed_used" || phase == "budget_used") Success(CampaignCore.RevokeOfficerCommission(state));
            if (phase == "budget_used") Success(CampaignCore.SetArmyEstablishment(state, "budget", 1400));
            var loaded = Reload(state);
            Assert.AreEqual(JsonUtility.ToJson(CampaignCore.GetOfficerCommissionTerms(state)), JsonUtility.ToJson(CampaignCore.GetOfficerCommissionTerms(loaded)));
        }

        [TestCase(7, "missing_right")]
        [TestCase(7, "null_right")]
        [TestCase(7, "invalid_right")]
        [TestCase(7, "missing_used")]
        [TestCase(7, "null_used")]
        [TestCase(7, "invalid_used")]
        [TestCase(8, "missing_right")]
        [TestCase(8, "null_right")]
        [TestCase(8, "invalid_right")]
        [TestCase(8, "missing_used")]
        [TestCase(8, "null_used")]
        [TestCase(8, "invalid_used")]
        public void V7AndCurrentRequireBothExplicitBooleanFields(int version, string corruption)
        {
            string json = CampaignArchive.Serialize(CampaignCore.Create(), false).Replace("\"Version\":" + CampaignArchive.CurrentVersion, "\"Version\":" + version);
            string field = corruption.EndsWith("right") ? "DumasOfficerCommission" : "DumasExtraRecruitUsed";
            string original = "\"" + field + "\":false";
            string replacement = corruption.StartsWith("missing") ? "\"Ignored" + field + "\":false" :
                "\"" + field + "\":" + (corruption.StartsWith("null") ? "null" : "\"not-a-boolean\"");
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize(json.Replace(original, replacement)));
        }

        [Test]
        public void V7KeepsACommissionAlongsideTheActualNpcNoticeVictoryAndOriginalPoliticalPromises()
        {
            var state = CampaignCore.Create("crown"); Success(CampaignCore.IssueMandate(state, "ile"));
            Success(CampaignCore.GrantRegionalAccord(state, "normandy"));
            Success(CampaignCore.GrantOfficerCommission(state));
            Success(CampaignCore.Act(state, "recruit", "ile")); Success(CampaignCore.RecruitThroughDumas(state));
            state.Food = 0; foreach (var region in state.Regions) region.Unrest = 100;
            Advance(state); Assert.AreEqual(1472, state.Troops); Assert.AreEqual(2, state.DumasForageDueWeek);
            Success(CampaignCore.ResolveBattle(state, "champagne", "battle-1-2-ile-champagne", true, 50, 60));
            var loaded = Reload(state);
            Assert.IsTrue(loaded.DumasOfficerCommission); Assert.IsFalse(loaded.DumasExtraRecruitUsed);
            Assert.AreEqual(1422, loaded.Troops); Assert.AreEqual(2, loaded.DumasForageDueWeek);
            Assert.AreEqual(5, loaded.DumasNextForageWeek); Assert.AreEqual("normandy", loaded.AccordRegionId);
            Assert.AreEqual(4, loaded.AccordUntilWeek); Assert.AreEqual("ile", loaded.Obligation.RegionId);
            Assert.AreEqual(2, loaded.Obligation.DueWeek); Assert.AreEqual("battle-1-2-ile-champagne", loaded.PendingVictoryId);
            Assert.AreEqual(119, CampaignCore.GetOfficerCommissionTerms(loaded).RevokeGoldCost);
            Success(CampaignCore.RevokeOfficerCommission(loaded));
            Assert.AreEqual(state.PendingVictoryId, loaded.PendingVictoryId);
            Assert.AreEqual(CampaignCore.MandateId(state.Obligation), CampaignCore.MandateId(loaded.Obligation));
            Assert.AreEqual(2, loaded.DumasForageDueWeek); Reload(loaded);
        }

        [TestCase("budget")]
        [TestCase("used_without_normal")]
        public void InvalidRightsCannotBeSavedOrUsedForCommands(string corruption)
        {
            var state = CampaignCore.Create();
            if (corruption == "budget")
            { Success(CampaignCore.SetArmyEstablishment(state, "budget", 1200)); state.DumasOfficerCommission = true; }
            else state.DumasExtraRecruitUsed = true;
            Assert.Throws<ArgumentException>(() => CampaignArchive.Serialize(state));
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize("{\"Version\":7,\"State\":" + Snapshot(state) + "}"));
            Assert.IsNull(CampaignCore.GetOfficerCommissionTerms(state));
            Refused(state, () => CampaignCore.GrantOfficerCommission(state), "error.commission.state");
            Refused(state, () => CampaignCore.RecruitThroughDumas(state), "error.commission.state");
            Refused(state, () => CampaignCore.RevokeOfficerCommission(state), "error.commission.state");
        }

        static string Older(string json, int version)
        {
            json = json.Replace("\"Version\":" + CampaignArchive.CurrentVersion, "\"Version\":" + version)
                .Replace("\"DumasOfficerCommission\":", "\"IgnoredCommission\":")
                .Replace("\"DumasExtraRecruitUsed\":", "\"IgnoredExtra\":");
            if (version < 6) json = json.Replace("\"ArmyPolicyId\":", "\"IgnoredArmy\":")
                .Replace("\"ArmyTargetTroops\":", "\"IgnoredTarget\":").Replace("\"ArmyReductionDueWeek\":", "\"IgnoredReduction\":");
            if (version < 5) json = json.Replace("\"DumasForageDueWeek\":", "\"IgnoredForage\":")
                .Replace("\"DumasNextForageWeek\":", "\"IgnoredNextForage\":");
            if (version < 4) json = json.Replace("\"PendingVictoryId\":", "\"IgnoredVictory\":");
            if (version < 3) json = json.Replace("\"AccordRegionId\":", "\"IgnoredAccord\":")
                .Replace("\"AccordUntilWeek\":", "\"IgnoredAccordUntil\":");
            if (version == 1) json = json.Replace("\"RoleId\":", "\"IgnoredRole\":")
                .Replace("\"NextMandateWeek\":", "\"IgnoredMandateNext\":").Replace("\"Mandates\":", "\"IgnoredMandates\":");
            return json;
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void OldSchemasAcquireNoRightsAndPreserveEveryFeatureTheirOwnVersionAlreadyHad(int version)
        {
            var state = CampaignCore.Create(version == 1 ? "legacy" : "crown");
            if (version >= 2) Success(CampaignCore.IssueMandate(state, "ile"));
            if (version >= 3) Success(CampaignCore.GrantRegionalAccord(state, "normandy"));
            if (version >= 6) Success(CampaignCore.SetArmyEstablishment(state, "budget", 900));
            if (version >= 5)
            {
                state.Food = 0; foreach (var region in state.Regions) region.Unrest = 100;
                Advance(state);
            }
            if (version >= 4) Success(CampaignCore.ResolveBattle(state, "champagne", "battle-" + state.Week + "-2-ile-champagne", true, 50, 60));
            string json = Older(CampaignArchive.Serialize(state, false), version);
            StringAssert.DoesNotContain("\"DumasOfficerCommission\":", json);
            var loaded = CampaignArchive.Deserialize(json); Assert.AreEqual(Snapshot(state), Snapshot(loaded));
            Assert.IsFalse(loaded.DumasOfficerCommission); Assert.IsFalse(loaded.DumasExtraRecruitUsed);
            if (version >= 6) { Assert.AreEqual(900, loaded.ArmyTargetTroops); Assert.AreEqual(2, loaded.ArmyReductionDueWeek); }
            if (version >= 5) Assert.AreEqual(2, loaded.DumasForageDueWeek);
            if (version >= 4) Assert.IsTrue(CampaignCore.HasPendingVictory(loaded));
            if (version >= 3) Assert.AreEqual("normandy", loaded.AccordRegionId);
            if (version >= 2) Assert.IsNotNull(loaded.Obligation); Reload(loaded);
        }

        [TestCase(5)]
        [TestCase(6)]
        public void V5AndV6PreserveBrokenAccordAndVetoedNpcCooldowns(int version)
        {
            var state = CampaignCore.Create("crown"); Success(CampaignCore.IssueMandate(state, "ile"));
            Success(CampaignCore.GrantRegionalAccord(state, "normandy")); Success(CampaignCore.Act(state, "tax", "normandy"));
            if (version == 6) Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
            state.Food = 0; foreach (var region in state.Regions) region.Unrest = 100;
            Advance(state); Success(CampaignCore.VetoDumasInitiative(state, 2));
            var loaded = CampaignArchive.Deserialize(Older(CampaignArchive.Serialize(state, false), version));
            Assert.AreEqual(Snapshot(state), Snapshot(loaded)); Assert.AreEqual(4, loaded.AccordUntilWeek);
            Assert.AreEqual("", loaded.AccordRegionId); Assert.AreEqual(5, loaded.DumasNextForageWeek);
            if (version == 6) Assert.AreEqual(2, loaded.ArmyReductionDueWeek); Reload(loaded);
        }

        [TestCase(1, false)]
        [TestCase(2, false)]
        [TestCase(3, false)]
        [TestCase(4, false)]
        [TestCase(5, false)]
        [TestCase(6, false)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(3, true)]
        [TestCase(4, true)]
        [TestCase(5, true)]
        [TestCase(6, true)]
        public void OlderVersionNumbersCannotEraseEitherAnOpenRightOrAClosedUsedRight(int version, bool revoked)
        {
            var state = Ready(); Success(CampaignCore.RecruitThroughDumas(state));
            if (revoked) Success(CampaignCore.RevokeOfficerCommission(state));
            string json = CampaignArchive.Serialize(state, false).Replace("\"Version\":" + CampaignArchive.CurrentVersion, "\"Version\":" + version);
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize(json));
        }
    }
}
#endif
