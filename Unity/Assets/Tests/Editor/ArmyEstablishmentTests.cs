#if UNITY_EDITOR
using System;
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    public sealed class ArmyEstablishmentTests
    {
        const int MaximumWeek = 1000000, MaximumStock = 100000000;
        static string Snapshot(CampaignState state) => JsonUtility.ToJson(state);
        static CharacterState Dumas(CampaignState state) => state.Characters.Find(person => person.Id == "dumas");
        static void Success(ActionResult result) { Assert.IsTrue(result.Ok, result.Key); }
        static CampaignState Reload(CampaignState state)
        {
            string before = Snapshot(state), json = CampaignArchive.Serialize(state, false);
            StringAssert.Contains("\"Version\":7", json);
            var loaded = CampaignArchive.Deserialize(json);
            Assert.AreEqual(before, Snapshot(loaded)); return loaded;
        }
        static void Refused(CampaignState state, Func<ActionResult> action, string reason)
        {
            string before = Snapshot(state); var result = action();
            Assert.IsFalse(result.Ok); Assert.AreEqual(reason, result.Key);
            Assert.AreEqual(before, Snapshot(state));
        }
        static void Advance(CampaignState state)
        {
            var forecast = CampaignCore.Forecast(state); int gold = state.Gold, food = state.Food;
            Success(CampaignCore.NextWeek(state));
            Assert.AreEqual(Math.Max(0, gold + forecast.NetGold), state.Gold);
            Assert.AreEqual(Math.Max(0, food + forecast.NetFood), state.Food);
            var log = state.Journal.Find(entry => entry.Key == "log.week" && entry.Week == state.Week);
            Assert.AreEqual(forecast.ArmyCost.ToString(), log.Args[2]);
            Assert.AreEqual(forecast.NetFood.ToString(), log.Args[3]);
            CampaignCore.Validate(state);
        }
        static CampaignState Late(int week)
        {
            var state = CampaignCore.Create(); state.Week = week; state.PetitionResolved = true;
            CampaignCore.Validate(state); return state;
        }

        [Test]
        public void CampaignDefaultHasAReadOnlyDocumentAndNeverSchedulesReductions()
        {
            var state = CampaignCore.Create(); string before = Snapshot(state);
            var terms = CampaignCore.GetArmyEstablishmentTerms(state);
            Assert.IsFalse(CampaignCore.HasArmyEstablishment(state));
            Assert.AreEqual("campaign", terms.Disposition); Assert.AreEqual(1200, terms.CurrentTroops);
            Assert.AreEqual(0, terms.NextBatchTroops); Assert.AreEqual(136, terms.ArmyCostAfterBatch);
            Assert.AreEqual(before, Snapshot(state));
            Advance(state); Advance(state);
            Assert.AreEqual(1200, state.Troops); Assert.AreEqual(2400, state.Manpower);
            Assert.AreEqual(50, Dumas(state).Relationship); Reload(state);
        }

        [Test]
        public void TwoOldBudgetsPrecedeTheFirstTransferEvenWhenThePetitionArrivesAtItsDeadline()
        {
            var state = CampaignCore.Create(); string before = Snapshot(state);
            var terms = CampaignCore.GetArmyEstablishmentTerms(state, "budget", 1000);
            Assert.AreEqual(before, Snapshot(state));
            Assert.AreEqual(2, terms.DueWeek); Assert.AreEqual(2, terms.WeeksRemaining);
            Assert.AreEqual(3, terms.FirstReducedBudgetWeek); Assert.AreEqual(200, terms.NextBatchTroops);
            Assert.AreEqual(1000, terms.TroopsAfterBatch); Assert.AreEqual(2600, terms.ManpowerAfterBatch);
            Assert.AreEqual(136, terms.CurrentArmyCost); Assert.AreEqual(120, terms.ArmyCostAfterBatch);
            Assert.AreEqual(40, terms.CurrentArmyConsumption); Assert.AreEqual(34, terms.ArmyConsumptionAfterBatch);
            Assert.AreEqual(-4, terms.DumasRelationshipDelta);
            terms.NextBatchTroops = 999; terms.DueWeek = 1;
            Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
            Advance(state); Assert.AreEqual(1200, state.Troops); Assert.AreEqual(2400, state.Manpower);
            Assert.AreEqual(136, CampaignCore.Forecast(state).ArmyCost);
            Advance(state); Assert.IsTrue(state.PendingPetition);
            Assert.AreEqual(1000, state.Troops); Assert.AreEqual(2600, state.Manpower);
            Assert.AreEqual(46, Dumas(state).Relationship); Assert.AreEqual(0, state.ArmyReductionDueWeek);
            Assert.AreEqual("136", state.Journal.Find(entry => entry.Key == "log.week").Args[2]);
            var reduced = state.Journal.Find(entry => entry.Key == "log.establishment.reduced");
            Assert.AreEqual(2, reduced.Week); CollectionAssert.AreEqual(new[] { "200", "1000", "4", "1000" }, reduced.Args);
            Assert.AreEqual(120, CampaignCore.Forecast(state).ArmyCost);
            Assert.AreEqual(34, CampaignCore.Forecast(state).ArmyConsumption);
            Assert.AreEqual("at_target", CampaignCore.GetArmyEstablishmentTerms(state).Disposition);
            Refused(state, () => CampaignCore.NextWeek(state), "error.petition.pending");
            Success(CampaignCore.ChoosePetition(state, "negotiate")); Advance(state);
            Assert.AreEqual(1, state.Journal.FindAll(entry => entry.Key == "log.establishment.reduced").Count);
            Reload(state);
        }

        [Test]
        public void ATargetAboveTheArmyDoesNotHireAndRecruitmentRestartsOrPreservesTheExistingClock()
        {
            var state = CampaignCore.Create();
            Success(CampaignCore.SetArmyEstablishment(state, "budget", 1400));
            Assert.AreEqual(1200, state.Troops); Assert.AreEqual(0, state.ArmyReductionDueWeek);
            Success(CampaignCore.Act(state, "recruit", "ile"));
            Assert.AreEqual(1400, state.Troops); Assert.AreEqual(0, state.ArmyReductionDueWeek);
            Advance(state);
            int gold = state.Gold, food = state.Food, materials = state.MilitarySupplies;
            Success(CampaignCore.Act(state, "recruit", "ile"));
            Assert.AreEqual(gold - 120, state.Gold); Assert.AreEqual(food - 20, state.Food);
            Assert.AreEqual(materials - 15, state.MilitarySupplies); Assert.AreEqual(2000, state.Manpower);
            Assert.AreEqual(3, state.ArmyReductionDueWeek);
            Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
            Assert.AreEqual(3, state.ArmyReductionDueWeek); Reload(state);

            var earlier = CampaignCore.Create(); Success(CampaignCore.SetArmyEstablishment(earlier, "budget", 1000));
            Advance(earlier); Success(CampaignCore.Act(earlier, "recruit", "ile"));
            Assert.AreEqual(2, earlier.ArmyReductionDueWeek, "Asker alımı mevcut tarihi ertelemez.");
            Success(CampaignCore.SetArmyEstablishment(earlier, "budget", 900));
            Assert.AreEqual(2, earlier.ArmyReductionDueWeek);
            Success(CampaignCore.SetArmyEstablishment(earlier, "budget", 1500));
            Assert.AreEqual(0, earlier.ArmyReductionDueWeek);
            Success(CampaignCore.SetArmyEstablishment(earlier, "budget", 1300));
            Assert.AreEqual(3, earlier.ArmyReductionDueWeek, "Gerçek iptalden sonra yeni iki hafta gerekir.");
        }

        [Test]
        public void CancellationNeverReturnsDepartedSoldiersAndRehiringStillCostsNormalResources()
        {
            var state = CampaignCore.Create(); Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
            Advance(state); Advance(state); Success(CampaignCore.ChoosePetition(state, "negotiate"));
            int gold = state.Gold, food = state.Food, supplies = state.MilitarySupplies;
            Success(CampaignCore.SetArmyEstablishment(state, "campaign", 0));
            Assert.AreEqual(1000, state.Troops); Assert.AreEqual(2600, state.Manpower);
            Assert.AreEqual(gold, state.Gold); Assert.AreEqual(food, state.Food);
            Success(CampaignCore.Act(state, "recruit", "ile"));
            Assert.AreEqual(1200, state.Troops); Assert.AreEqual(2400, state.Manpower);
            Assert.AreEqual(gold - 120, state.Gold); Assert.AreEqual(food - 20, state.Food);
            Assert.AreEqual(supplies - 15, state.MilitarySupplies); Assert.AreEqual(0, state.ArmyReductionDueWeek);
            Refused(state, () => CampaignCore.SetArmyEstablishment(state, "campaign", 0), "error.establishment.unchanged");
            Advance(state); Assert.AreEqual(1200, state.Troops); Reload(state);
        }

        [Test]
        public void DueWeekHungerLosesTheOldEightPercentBeforeOnlyTheLivingExcessLeaves()
        {
            var state = CampaignCore.Create(); Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
            Advance(state); state.Food = 0;
            foreach (var region in state.Regions) region.Unrest = 100;
            Assert.Less(CampaignCore.Forecast(state).NetFood, 0);
            Advance(state);
            Assert.AreEqual(1000, state.Troops); Assert.AreEqual(2504, state.Manpower);
            Assert.AreEqual("96", state.Journal.Find(entry => entry.Key == "log.shortage").Args[0]);
            Assert.AreEqual("104", state.Journal.Find(entry => entry.Key == "log.establishment.reduced").Args[0]);
            Assert.AreEqual(46, Dumas(state).Relationship); Assert.AreEqual(3, state.DumasForageDueWeek);
            Assert.AreEqual(0, state.ArmyReductionDueWeek); Reload(state);
        }

        [TestCase(true, 201, 999)]
        [TestCase(false, 1200, 0)]
        public void BattleLossesBeforeTheDeadlineCancelAGroupWithoutInventingReserveOrAnotherDumasPenalty(bool won, int casualties, int survivors)
        {
            var state = CampaignCore.Create(); Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
            Assert.AreEqual(1200, state.Troops, "Bekleyen birlikler savaşta hâlâ canlı ve kullanılabilir.");
            Success(CampaignCore.ResolveBattle(state, "champagne", "battle-0-2-ile-champagne", won, casualties, 60));
            Assert.AreEqual(survivors, state.Troops); Assert.AreEqual(0, state.ArmyReductionDueWeek);
            float relation = Dumas(state).Relationship;
            Advance(state); Advance(state);
            Assert.AreEqual(survivors, state.Troops); Assert.AreEqual(2400, state.Manpower);
            Assert.AreEqual(relation, Dumas(state).Relationship);
            Assert.IsNull(state.Journal.Find(entry => entry.Key == "log.establishment.reduced")); Reload(state);
        }

        [Test]
        public void HungryPeacefulMarchAlsoCancelsExcessThatNoLongerExists()
        {
            var state = CampaignCore.Create(); Success(CampaignCore.SetArmyEstablishment(state, "budget", 1190));
            state.Food = 0; Success(CampaignCore.March(state, "normandy"));
            Assert.AreEqual(1176, state.Troops); Assert.AreEqual(0, state.ArmyReductionDueWeek);
            Assert.AreEqual(2400, state.Manpower); Assert.AreEqual(50, Dumas(state).Relationship); Reload(state);
        }

        [Test]
        public void CapacityReservesTheWholeExcessAndRecruitmentPreservesThatBound()
        {
            var state = CampaignCore.Create(); state.Troops = 1600; state.Manpower = MaximumStock - 200;
            Assert.IsNull(CampaignCore.GetArmyEstablishmentTerms(state, "budget", 1000));
            Refused(state, () => CampaignCore.SetArmyEstablishment(state, "budget", 1000), "error.establishment.capacity");
            state.Manpower = MaximumStock - 600;
            Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
            Success(CampaignCore.Act(state, "recruit", "ile"));
            Assert.AreEqual(MaximumStock, (long)state.Manpower + state.Troops - state.ArmyTargetTroops);
            Advance(state); Advance(state);
            Assert.AreEqual(MaximumStock - 600, state.Manpower);
            Assert.AreEqual(1600, state.Troops); Assert.AreEqual(4, state.ArmyReductionDueWeek); Reload(state);
        }

        [TestCase(0f, 0f)]
        [TestCase(2.5f, -2.5f)]
        [TestCase(100f, -4f)]
        public void TheVisibleDumasPenaltyIsTheActualClampedChange(float before, float delta)
        {
            var state = CampaignCore.Create(); Dumas(state).Relationship = before;
            Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
            Assert.AreEqual(delta, CampaignCore.GetArmyEstablishmentTerms(state).DumasRelationshipDelta);
            Advance(state); Advance(state);
            Assert.AreEqual(before + delta, Dumas(state).Relationship);
            Assert.AreEqual((-delta).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture),
                state.Journal.Find(entry => entry.Key == "log.establishment.reduced").Args[2]);
        }

        [Test]
        public void ZeroTargetRemovesTheGarrisonOnlyAfterItsLastPaidRegionalWeekAndAllowsRecovery()
        {
            var state = CampaignCore.Create(); state.Troops = 200;
            Success(CampaignCore.SetArmyEstablishment(state, "budget", 0));
            Assert.IsTrue(CampaignCore.GetArmyEstablishmentTerms(state).WillRemoveGarrison);
            Advance(state); float control = CampaignCore.Region(state, "ile").Control;
            Advance(state);
            Assert.AreEqual(0, state.Troops); Assert.AreEqual(2600, state.Manpower);
            Assert.AreEqual(control + 2, CampaignCore.Region(state, "ile").Control);
            Success(CampaignCore.ChoosePetition(state, "negotiate"));
            control = CampaignCore.Region(state, "ile").Control; Advance(state);
            Assert.AreEqual(control, CampaignCore.Region(state, "ile").Control);
            Assert.AreEqual(0, CampaignCore.Forecast(state).ArmyConsumption);
            Success(CampaignCore.Act(state, "recruit", "ile"));
            Assert.AreEqual(200, state.Troops); Assert.AreEqual(2400, state.Manpower);
            Assert.AreEqual(5, state.ArmyReductionDueWeek); Reload(state);
        }

        [Test]
        public void MultipleGroupsKeepTheirTwoWeekCadenceAndTheLastGroupContainsOnlyTheRemainder()
        {
            var state = CampaignCore.Create(); Success(CampaignCore.SetArmyEstablishment(state, "budget", 750));
            for (int week = 1; week <= 6; week++)
            {
                if (state.PendingPetition) Success(CampaignCore.ChoosePetition(state, "negotiate"));
                Advance(state);
                Assert.AreEqual(week < 2 ? 1200 : week < 4 ? 1000 : week < 6 ? 800 : 750, state.Troops);
                Assert.AreEqual(3600, state.Troops + state.Manpower);
            }
            var last = state.Journal.Find(entry => entry.Key == "log.establishment.reduced");
            Assert.AreEqual("50", last.Args[0]); Assert.AreEqual(38, Dumas(state).Relationship);
            Assert.AreEqual(0, state.ArmyReductionDueWeek); Reload(state);
        }

        [TestCase("unknown", 0, "error.establishment.policy")]
        [TestCase(null, 0, "error.establishment.policy")]
        [TestCase("campaign", 1000, "error.establishment.target")]
        [TestCase("budget", -1, "error.establishment.target")]
        [TestCase("budget", MaximumStock + 1, "error.establishment.target")]
        [TestCase("campaign", 0, "error.establishment.unchanged")]
        public void InvalidOrIdenticalCommandsRefuseWithoutAnyJournalOrStateChange(string policy, int target, string reason)
        {
            var state = CampaignCore.Create();
            Refused(state, () => CampaignCore.SetArmyEstablishment(state, policy, target), reason);
        }

        [Test]
        public void PetitionAndMandateGuardsBlockPolicyChangesButDoNotSwallowAnAlreadyEarnedGroup()
        {
            var state = CampaignCore.Create("army");
            Success(CampaignCore.IssueMandate(state, "ile"));
            Success(CampaignCore.GrantRegionalAccord(state, "champagne"));
            Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
            string mandateId = CampaignCore.MandateId(state.Obligation);
            Advance(state); Advance(state);
            Assert.AreEqual(1000, state.Troops); Assert.AreEqual(2600, state.Manpower);
            Assert.IsTrue(CampaignCore.MandateDue(state)); Assert.IsTrue(CampaignCore.HasRegionalAccord(state));
            Refused(state, () => CampaignCore.SetArmyEstablishment(state, "campaign", 0), "error.mandate.petition");
            Success(CampaignCore.ChoosePetition(state, "negotiate"));
            Refused(state, () => CampaignCore.SetArmyEstablishment(state, "campaign", 0), "error.mandate.due");
            Refused(state, () => CampaignCore.NextWeek(state), "error.mandate.due");
            Success(CampaignCore.ResolveMandate(state, mandateId, "fulfil"));
            Advance(state); Advance(state);
            Assert.IsNull(state.Obligation); Assert.IsFalse(CampaignCore.HasRegionalAccord(state));
            Assert.AreEqual(4, state.AccordUntilWeek); Assert.AreEqual(1000, state.Troops); Reload(state);
        }

        [Test]
        public void DumasDueForecastUsesTheOldArmyAndOnlyThenDoesDemobilizationTransferItsSurvivors()
        {
            var state = CampaignCore.Create(); state.Food = 0;
            foreach (var region in state.Regions) region.Unrest = 100;
            Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
            Advance(state); Assert.AreEqual(1104, state.Troops); Assert.AreEqual(2, state.DumasForageDueWeek);
            foreach (var region in state.Regions) region.Unrest = 60;
            var before = CampaignCore.Forecast(state); var terms = CampaignCore.GetDumasInitiativeTerms(state);
            Assert.AreEqual(37, before.ArmyConsumption); Assert.AreEqual(14, terms.FoodGathered);
            float relation = Dumas(state).Relationship; Advance(state);
            Assert.AreEqual(1000, state.Troops); Assert.AreEqual(2504, state.Manpower);
            Assert.AreEqual(relation - 4, Dumas(state).Relationship); Assert.AreEqual(0, state.DumasForageDueWeek);
            Assert.AreEqual("14", state.Journal.Find(entry => entry.Key == "log.dumas.gathered").Args[1]);
            Assert.AreEqual("104", state.Journal.Find(entry => entry.Key == "log.establishment.reduced").Args[0]);
            Assert.AreEqual(34, CampaignCore.Forecast(state).ArmyConsumption); Reload(state);
        }

        [Test]
        public void FinalCalendarPreservesAnExistingDueButCannotScheduleANewTwoWeekPromise()
        {
            var state = Late(MaximumWeek - 2);
            Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000)); Advance(state);
            Success(CampaignCore.SetArmyEstablishment(state, "budget", 900));
            Assert.AreEqual(MaximumWeek, state.ArmyReductionDueWeek);
            Assert.AreEqual(0, CampaignCore.GetArmyEstablishmentTerms(state).FirstReducedBudgetWeek);
            Advance(state); Assert.AreEqual(1000, state.Troops); Assert.AreEqual(0, state.ArmyReductionDueWeek);
            Assert.AreEqual("calendar", CampaignCore.GetArmyEstablishmentTerms(state).Disposition);
            Refused(state, () => CampaignCore.NextWeek(state), "error.week.limit");
            Refused(state, () => CampaignCore.SetArmyEstablishment(state, "campaign", 0), "error.week.limit"); Reload(state);

            var late = Late(MaximumWeek - 1);
            Refused(late, () => CampaignCore.SetArmyEstablishment(late, "budget", 1000), "error.establishment.calendar");
            Success(CampaignCore.SetArmyEstablishment(late, "budget", 1200));
            Success(CampaignCore.Act(late, "recruit", "ile"));
            Assert.AreEqual(1400, late.Troops); Assert.AreEqual(0, late.ArmyReductionDueWeek);
            Assert.AreEqual("calendar", CampaignCore.GetArmyEstablishmentTerms(late).Disposition); Reload(late);
        }

        [TestCase("fresh")]
        [TestCase("scheduled")]
        [TestCase("at_target")]
        [TestCase("cancelled")]
        [TestCase("calendar")]
        public void V6RoundTripsEveryCanonicalPolicyState(string phase)
        {
            var state = phase == "calendar" ? Late(MaximumWeek - 1) : CampaignCore.Create();
            if (phase == "scheduled") Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
            if (phase == "at_target" || phase == "calendar") Success(CampaignCore.SetArmyEstablishment(state, "budget", 1200));
            if (phase == "calendar") Success(CampaignCore.Act(state, "recruit", "ile"));
            if (phase == "cancelled")
            {
                Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
                Success(CampaignCore.SetArmyEstablishment(state, "campaign", 0));
            }
            var loaded = Reload(state);
            Assert.AreEqual(JsonUtility.ToJson(CampaignCore.GetArmyEstablishmentTerms(state)),
                JsonUtility.ToJson(CampaignCore.GetArmyEstablishmentTerms(loaded)));
        }

        [Test]
        public void ALastPenultimateWeekGroupMayLeaveACanonicalUnscheduledRemainder()
        {
            var state = Late(MaximumWeek - 3);
            Success(CampaignCore.SetArmyEstablishment(state, "budget", 0));
            Assert.AreEqual(MaximumWeek - 1, state.ArmyReductionDueWeek);
            Advance(state); Advance(state);
            Assert.AreEqual(1000, state.Troops); Assert.AreEqual(0, state.ArmyReductionDueWeek);
            var terms = CampaignCore.GetArmyEstablishmentTerms(state);
            Assert.AreEqual("calendar", terms.Disposition); Assert.AreEqual(0, terms.NextBatchTroops);
            Assert.AreEqual(1000, terms.TroopsAfterBatch); Reload(state);
            Advance(state); Assert.AreEqual(1000, state.Troops); Reload(state);
        }

        [Test]
        public void V6RetainsConcurrentArmyDateNpcNoticeVictoryMandateAndOriginalHoliday()
        {
            var state = CampaignCore.Create("crown");
            Success(CampaignCore.IssueMandate(state, "ile"));
            Success(CampaignCore.GrantRegionalAccord(state, "normandy"));
            Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
            state.Food = 0; foreach (var region in state.Regions) region.Unrest = 100;
            Advance(state);
            Success(CampaignCore.ResolveBattle(state, "champagne", "battle-1-2-ile-champagne", true, 50, 60));
            var loaded = Reload(state);
            Assert.AreEqual(1054, loaded.Troops); Assert.AreEqual(2, loaded.ArmyReductionDueWeek);
            Assert.AreEqual(2, loaded.DumasForageDueWeek); Assert.AreEqual(5, loaded.DumasNextForageWeek);
            Assert.AreEqual("ile", loaded.Obligation.RegionId); Assert.AreEqual(2, loaded.Obligation.DueWeek);
            Assert.AreEqual("normandy", loaded.AccordRegionId); Assert.AreEqual(4, loaded.AccordUntilWeek);
            Assert.AreEqual("battle-1-2-ile-champagne", loaded.PendingVictoryId);
            Assert.AreEqual(Snapshot(state), Snapshot(loaded));
        }

        [TestCase(6, "missing_policy")]
        [TestCase(6, "null_policy")]
        [TestCase(6, "missing_target")]
        [TestCase(6, "null_target")]
        [TestCase(6, "missing_due")]
        [TestCase(6, "null_due")]
        [TestCase(6, "text_due")]
        [TestCase(7, "missing_policy")]
        [TestCase(7, "null_policy")]
        [TestCase(7, "missing_target")]
        [TestCase(7, "null_target")]
        [TestCase(7, "missing_due")]
        [TestCase(7, "null_due")]
        [TestCase(7, "text_due")]
        public void V6AndCurrentRequireAllThreeExplicitTypedArmyFields(int version, string corruption)
        {
            string json = CampaignArchive.Serialize(CampaignCore.Create(), false).Replace("\"Version\":7", "\"Version\":" + version);
            switch (corruption)
            {
                case "missing_policy": json = json.Replace("\"ArmyPolicyId\":", "\"IgnoredPolicy\":"); break;
                case "null_policy": json = json.Replace("\"ArmyPolicyId\":\"campaign\"", "\"ArmyPolicyId\":null"); break;
                case "missing_target": json = json.Replace("\"ArmyTargetTroops\":", "\"IgnoredTarget\":"); break;
                case "null_target": json = json.Replace("\"ArmyTargetTroops\":0", "\"ArmyTargetTroops\":null"); break;
                case "missing_due": json = json.Replace("\"ArmyReductionDueWeek\":", "\"IgnoredDue\":"); break;
                case "null_due": json = json.Replace("\"ArmyReductionDueWeek\":0", "\"ArmyReductionDueWeek\":null"); break;
                case "text_due": json = json.Replace("\"ArmyReductionDueWeek\":0", "\"ArmyReductionDueWeek\":\"not-a-week\""); break;
            }
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize(json));
        }

        [TestCase("policy")]
        [TestCase("campaign_target")]
        [TestCase("campaign_due")]
        [TestCase("missing_schedule")]
        [TestCase("past_due")]
        [TestCase("far_due")]
        [TestCase("no_excess")]
        [TestCase("capacity")]
        [TestCase("negative_target")]
        public void InvalidPolicyStateCannotBeSavedOrUsedForCommands(string corruption)
        {
            var state = CampaignCore.Create(); Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
            switch (corruption)
            {
                case "policy": state.ArmyPolicyId = "unknown"; break;
                case "campaign_target": state.ArmyPolicyId = "campaign"; state.ArmyReductionDueWeek = 0; break;
                case "campaign_due": state.ArmyPolicyId = "campaign"; state.ArmyTargetTroops = 0; break;
                case "missing_schedule": state.ArmyReductionDueWeek = 0; break;
                case "past_due": state.Week = 2; state.PendingPetition = true; break;
                case "far_due": state.ArmyReductionDueWeek = 3; break;
                case "no_excess": state.ArmyTargetTroops = 1200; break;
                case "capacity": state.Manpower = MaximumStock; break;
                case "negative_target": state.ArmyTargetTroops = -1; break;
            }
            Assert.Throws<ArgumentException>(() => CampaignArchive.Serialize(state));
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize("{\"Version\":6,\"State\":" + Snapshot(state) + "}"));
            Assert.IsNull(CampaignCore.GetArmyEstablishmentTerms(state));
            Refused(state, () => CampaignCore.SetArmyEstablishment(state, "campaign", 0), "error.establishment.state");
        }

        static string Older(string json, int version)
        {
            json = json.Replace("\"Version\":7", "\"Version\":" + version)
                .Replace("\"ArmyPolicyId\":", "\"IgnoredArmyPolicy\":")
                .Replace("\"ArmyTargetTroops\":", "\"IgnoredArmyTarget\":")
                .Replace("\"ArmyReductionDueWeek\":", "\"IgnoredArmyDue\":");
            if (version < 5) json = json.Replace("\"DumasForageDueWeek\":", "\"IgnoredForageDue\":")
                .Replace("\"DumasNextForageWeek\":", "\"IgnoredForageNext\":");
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
        public void GenuineOlderSchemasMigrateToCampaignWhileTheirIndependentFeaturesSurvive(int version)
        {
            var state = CampaignCore.Create(version == 1 ? "legacy" : "crown");
            if (version >= 2) Success(CampaignCore.IssueMandate(state, "ile"));
            if (version >= 3) Success(CampaignCore.GrantRegionalAccord(state, "normandy"));
            if (version >= 5)
            {
                state.Food = 0; foreach (var region in state.Regions) region.Unrest = 100;
                Advance(state);
            }
            if (version >= 4) Success(CampaignCore.ResolveBattle(state, "champagne",
                "battle-" + state.Week + "-2-ile-champagne", true, 90, 60));
            string json = Older(CampaignArchive.Serialize(state, false), version);
            StringAssert.DoesNotContain("\"ArmyPolicyId\":", json);
            var loaded = CampaignArchive.Deserialize(json);
            Assert.AreEqual(Snapshot(state), Snapshot(loaded));
            Assert.AreEqual("campaign", loaded.ArmyPolicyId); Assert.AreEqual(0, loaded.ArmyReductionDueWeek);
            if (version >= 5) Assert.AreEqual(2, loaded.DumasForageDueWeek);
            if (version >= 4) Assert.IsTrue(CampaignCore.HasPendingVictory(loaded));
            if (version >= 3) Assert.IsTrue(CampaignCore.HasRegionalAccord(loaded));
            if (version >= 2) Assert.IsNotNull(loaded.Obligation); Reload(loaded);
        }

        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void OlderArchivesPreserveBrokenAccordCooldownIndependentlyOfArmyMigration(int version)
        {
            var state = CampaignCore.Create("crown"); Success(CampaignCore.IssueMandate(state, "ile"));
            Success(CampaignCore.GrantRegionalAccord(state, "normandy")); Success(CampaignCore.Act(state, "tax", "normandy"));
            if (version == 5)
            {
                state.Food = 0; foreach (var region in state.Regions) region.Unrest = 100;
                Advance(state); Success(CampaignCore.VetoDumasInitiative(state, 2));
            }
            var loaded = CampaignArchive.Deserialize(Older(CampaignArchive.Serialize(state, false), version));
            Assert.AreEqual(Snapshot(state), Snapshot(loaded)); Assert.AreEqual("", loaded.AccordRegionId);
            Assert.AreEqual(4, loaded.AccordUntilWeek);
            if (version == 5) Assert.AreEqual(5, loaded.DumasNextForageWeek);
            Reload(loaded);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void AnOldVersionNumberCannotSilentlyEraseANewBudgetPolicy(int version)
        {
            var state = CampaignCore.Create(); Success(CampaignCore.SetArmyEstablishment(state, "budget", 1200));
            string json = CampaignArchive.Serialize(state, false).Replace("\"Version\":7", "\"Version\":" + version);
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize(json));
        }

        [Test]
        public void LegacyMigrationAcceptsAnAbsentPolicyButNeverLaundersAnExplicitEmptyPolicy()
        {
            string json = CampaignArchive.Serialize(CampaignCore.Create(), false).Replace("\"Version\":7", "\"Version\":5");
            string absent = json.Replace("\"ArmyPolicyId\":", "\"IgnoredPolicy\":");
            Assert.AreEqual("campaign", CampaignArchive.Deserialize(absent).ArmyPolicyId);
            string empty = json.Replace("\"ArmyPolicyId\":\"campaign\"", "\"ArmyPolicyId\":\"\"");
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize(empty));
        }
    }
}
#endif
