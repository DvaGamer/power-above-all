#if UNITY_EDITOR
using System;
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    public sealed class RegionalReformTests
    {
        const int LastWeek = 1000000;
        static string Snapshot(CampaignState state) => JsonUtility.ToJson(state);
        static CharacterState Person(CampaignState state, string id) => state.Characters.Find(person => person.Id == id);
        static void Success(ActionResult result) { Assert.IsTrue(result.Ok, result.Key); }
        static CampaignState Copy(CampaignState state) => JsonUtility.FromJson<CampaignState>(Snapshot(state));
        static CampaignState Reload(CampaignState state)
        {
            string before = Snapshot(state), archive = CampaignArchive.Serialize(state, false);
            StringAssert.Contains("\"Version\":8", archive);
            var loaded = CampaignArchive.Deserialize(archive); Assert.AreEqual(before, Snapshot(loaded)); return loaded;
        }
        static void Refused(CampaignState state, Func<ActionResult> action, string key)
        {
            string before = Snapshot(state); var result = action();
            Assert.IsFalse(result.Ok); Assert.AreEqual(key, result.Key); Assert.AreEqual(before, Snapshot(state));
        }
        static void Advance(CampaignState state)
        {
            var forecast = CampaignCore.Forecast(state); int gold = state.Gold, food = state.Food;
            Success(CampaignCore.NextWeek(state));
            Assert.AreEqual(Math.Max(0, gold + forecast.NetGold), state.Gold);
            Assert.AreEqual(Math.Max(0, food + forecast.NetFood), state.Food);
            if (state.PendingPetition) Success(CampaignCore.ChoosePetition(state, "negotiate"));
            CampaignCore.Validate(state);
        }
        static CampaignState Active(string mode = "provisioning", string role = "legacy")
        {
            var state = CampaignCore.Create(role);
            Success(CampaignCore.BeginRegionalReform(state, "normandy", mode));
            for (int week = 0; week < 4; week++) Advance(state);
            Assert.AreEqual("active", CampaignCore.GetRegionalReformTerms(state).StatusId);
            return state;
        }
        static CampaignState WithoutReform(CampaignState state)
        {
            // Karşılaştırma fixture'ı: aynı gerçek haftanın yalnız reform bileşeni çıkarılır.
            var copy = Copy(state); copy.ReformRegionId = copy.ReformModeId = ""; copy.ReformStepsRemaining = 0;
            CampaignCore.Validate(copy); return copy;
        }

        [TestCase("provisioning", "morel", -8, 5)]
        [TestCase("commerce", "valcourt", 8, -5)]
        public void PaidPreparationKeepsFourOldBudgetsAndOnlyTheFifthUsesTheReformedEconomy(string mode, string sponsor, int taxDelta, int foodDelta)
        {
            var state = CampaignCore.Create(); var proposed = CampaignCore.GetRegionalReformTerms(state, "normandy", mode);
            Assert.AreEqual("proposed", proposed.StatusId); Assert.AreEqual(sponsor, proposed.SponsorId);
            Assert.AreEqual(32, proposed.BaseTax); Assert.AreEqual(20, proposed.BaseFood);
            Assert.AreEqual(taxDelta, proposed.NominalTaxDelta); Assert.AreEqual(foodDelta, proposed.NominalFoodDelta);
            Assert.AreEqual(4, proposed.EarliestActivationWeek); Assert.AreEqual(5, proposed.EarliestFirstReformedBudgetWeek);
            Success(CampaignCore.BeginRegionalReform(state, "normandy", mode));
            Assert.AreEqual(720, state.Gold); Assert.AreEqual(51, state.Power);
            Assert.AreEqual(360, state.Food); Assert.AreEqual(1200, state.Troops); Assert.AreEqual(2400, state.Manpower);
            var comparison = WithoutReform(state);
            for (int week = 1; week <= 4; week++)
            {
                Assert.AreEqual(SnapshotForecast(CampaignCore.Forecast(comparison)), SnapshotForecast(CampaignCore.Forecast(state)));
                Advance(state); Advance(comparison);
                Assert.AreEqual(comparison.Gold, state.Gold); Assert.AreEqual(comparison.Food, state.Food);
                Assert.AreEqual(4 - week, state.ReformStepsRemaining);
                Assert.AreEqual(week == 4 ? 54 : 50, Person(state, sponsor).Relationship);
                state = Reload(state);
            }
            var active = CampaignCore.GetRegionalReformTerms(state); var plain = CampaignCore.Forecast(comparison);
            Assert.AreEqual("active", active.StatusId); Assert.IsTrue(CampaignCore.HasRegionalReform(state));
            Assert.AreEqual(-1, active.EarliestActivationWeek); Assert.AreEqual(5, active.NextBudgetWeek);
            Assert.AreEqual(active.WithReformTaxIncome, active.CurrentTaxIncome);
            Assert.AreEqual(active.WithReformProduction, active.CurrentProduction);
            Assert.AreEqual(plain.TaxIncome, active.WithoutReformTaxIncome);
            Assert.AreEqual(plain.Production, active.WithoutReformProduction);
            Assert.AreNotEqual(0, active.TaxIncomeDelta); Assert.AreNotEqual(0, active.ProductionDelta);
            int taxGap = active.TaxIncomeDelta, foodGap = active.NetFoodDelta;
            Advance(state); Advance(comparison);
            Assert.AreEqual(taxGap, state.Gold - comparison.Gold);
            Assert.AreEqual(foodGap, state.Food - comparison.Food);
            Assert.AreEqual(54, Person(state, sponsor).Relationship);
            Assert.AreEqual(1, state.Journal.FindAll(entry => entry.Key == "log.reform.completed").Count);
            Assert.AreEqual(32, Array.Find(CampaignCore.Regions, region => region.Id == "normandy").BaseTax);
            Assert.AreEqual(20, Array.Find(CampaignCore.Regions, region => region.Id == "normandy").BaseFood);
        }

        static string SnapshotForecast(EconomyForecast forecast) => string.Join(",", new[] {
            forecast.TaxIncome, forecast.ArmyCost, forecast.Production, forecast.CivilianConsumption,
            forecast.ArmyConsumption, forecast.SubsidyConsumption, forecast.NetGold, forecast.NetFood, forecast.ForageFood });

        [TestCase("brittany", 6, 5)]
        [TestCase("provence", 8, 3)]
        public void OriginalQuarterSharesAreRoundedIndependentlyAwayFromZero(string regionId, int taxShare, int foodShare)
        {
            var state = CampaignCore.Create(); var food = CampaignCore.GetRegionalReformTerms(state, regionId, "provisioning");
            var tax = CampaignCore.GetRegionalReformTerms(state, regionId, "commerce");
            Assert.AreEqual(-taxShare, food.NominalTaxDelta); Assert.AreEqual(foodShare, food.NominalFoodDelta);
            Assert.AreEqual(taxShare, tax.NominalTaxDelta); Assert.AreEqual(-foodShare, tax.NominalFoodDelta);
        }

        [TestCase(64f, 55f, 35f, true, 4)]
        [TestCase(65f, 55f, 60f, false, 3)]
        [TestCase(10f, 54.9f, 35f, false, 4)]
        [TestCase(10f, 55f, 35f, true, 3)]
        public void EligibilityIsDecidedAfterTheActualRegionalEffectsInsteadOfTodaysPreview(float unrest, float control, float approval, bool readyNow, int remaining)
        {
            var state = CampaignCore.Create(); var region = CampaignCore.Region(state, "normandy");
            region.Unrest = unrest; region.Control = control; state.Factions.Find(f => f.Id == "urban").Approval = approval;
            var proposed = CampaignCore.GetRegionalReformTerms(state, "normandy", "provisioning");
            Assert.AreEqual("proposed", proposed.StatusId); Assert.AreEqual(readyNow, proposed.RegionReadyNow);
            Success(CampaignCore.BeginRegionalReform(state, "normandy", "provisioning"));
            Advance(state); Assert.AreEqual(remaining, state.ReformStepsRemaining);
            Assert.AreEqual(remaining == 3 ? 1 : 0, state.Journal.FindAll(entry => entry.Key == "log.reform.progress").Count);
        }

        [Test]
        public void UnrestCanPausePreparationForSeveralSuccessfulWeeksThenPublicBreadAllowsProgress()
        {
            var state = CampaignCore.Create(); Success(CampaignCore.BeginRegionalReform(state, "champagne", "provisioning"));
            Assert.AreEqual("blocked", CampaignCore.GetRegionalReformTerms(state).StatusId);
            Advance(state); Advance(state); Assert.AreEqual(4, state.ReformStepsRemaining);
            Assert.AreEqual(0, state.Journal.FindAll(entry => entry.Key == "log.reform.progress").Count);
            Success(CampaignCore.Act(state, "bread", "champagne")); Advance(state);
            Assert.AreEqual(3, state.ReformStepsRemaining);
            Assert.AreEqual("champagne", state.ReformRegionId); Reload(state);
        }

        [Test]
        public void PetitionAndDuePromiseRefuseAtomicallyButTheWeekThatCreatesThePetitionKeepsItsStep()
        {
            var state = CampaignCore.Create("crown"); Success(CampaignCore.IssueMandate(state, "ile"));
            Success(CampaignCore.BeginRegionalReform(state, "normandy", "provisioning"));
            Success(CampaignCore.NextWeek(state)); Success(CampaignCore.NextWeek(state));
            Assert.AreEqual(2, state.ReformStepsRemaining); Assert.IsTrue(state.PendingPetition);
            Refused(state, () => CampaignCore.NextWeek(state), "error.petition.pending");
            Refused(state, () => CampaignCore.BeginRegionalReform(state, "ile", "commerce"), "error.mandate.petition");
            Refused(state, () => CampaignCore.EndRegionalReform(state), "error.mandate.petition");
            state = Reload(state); Success(CampaignCore.ChoosePetition(state, "negotiate"));
            Refused(state, () => CampaignCore.NextWeek(state), "error.mandate.due");
            Refused(state, () => CampaignCore.EndRegionalReform(state), "error.mandate.due");
            Refused(state, () => CampaignCore.BeginRegionalReform(state, "ile", "commerce"), "error.mandate.due");
            Success(CampaignCore.ResolveMandate(state, CampaignCore.MandateId(state.Obligation), "fulfil"));
            Advance(state); Assert.AreEqual(1, state.ReformStepsRemaining); Advance(state);
            Assert.AreEqual("active", CampaignCore.GetRegionalReformTerms(state).StatusId);
        }

        [Test]
        public void ReadOnlyQueriesDoNotAdvanceTimeOrMoveTheOriginalProjectAndTheirDtosAreDetached()
        {
            var state = CampaignCore.Create(); string closed = Snapshot(state);
            var terms = CampaignCore.GetRegionalReformTerms(state);
            Assert.AreEqual("closed", terms.StatusId); Assert.AreEqual("", terms.RegionId); Assert.AreEqual("", terms.SponsorId);
            Assert.AreEqual(terms.CurrentTaxIncome, terms.WithReformTaxIncome); Assert.AreEqual(0, terms.NetFoodDelta);
            Assert.IsNull(CampaignCore.GetRegionalReformTerms(null));
            Assert.IsNull(CampaignCore.GetRegionalReformTerms(state, "missing", "commerce"));
            Assert.IsNull(CampaignCore.GetRegionalReformTerms(state, "ile", "unknown")); Assert.AreEqual(closed, Snapshot(state));
            Success(CampaignCore.BeginRegionalReform(state, "champagne", "provisioning"));
            Success(CampaignCore.March(state, "normandy")); state.SelectedRegionId = "provence";
            string before = Snapshot(state);
            for (int i = 0; i < 10; i++)
            {
                var blocked = CampaignCore.GetRegionalReformTerms(state);
                Assert.AreEqual("champagne", blocked.RegionId); Assert.AreEqual("morel", blocked.SponsorId);
                Assert.AreEqual("reform.wait.unrest", blocked.WaitReasonKey); Assert.AreEqual(4, blocked.WaitReasonArgs.Length);
                blocked.WaitReasonArgs[0] = "changed"; blocked.StepsRemaining = 0;
                Assert.IsNull(CampaignCore.GetRegionalReformTerms(state, "provence", "commerce"));
                CampaignCore.CanEndRegionalReform(state); CampaignCore.Forecast(state);
                Refused(state, () => CampaignCore.BeginRegionalReform(state, "provence", "commerce"), "error.reform.open");
            }
            Assert.AreEqual(before, Snapshot(state)); Assert.AreEqual(0, state.Week); Assert.AreEqual(4, state.ReformStepsRemaining);
        }

        [TestCase("gold", "error.reform.gold")]
        [TestCase("power", "error.reform.power")]
        [TestCase("region", "error.region")]
        [TestCase("mode", "error.reform.mode")]
        [TestCase("state", "error.reform.state")]
        public void RejectedStartsCannotChargePartialPricesOrChangeTheJournal(string failure, string reason)
        {
            var state = CampaignCore.Create(); string region = "normandy", mode = "provisioning";
            if (failure == "gold") { state.Gold = 119; state.Power = 0; }
            if (failure == "power") state.Power = 3.99f;
            if (failure == "region") { region = "missing"; mode = "unknown"; }
            if (failure == "mode") mode = "unknown";
            if (failure == "state") state.ReformStepsRemaining = 1;
            Refused(state, () => CampaignCore.CanBeginRegionalReform(state, region, mode), reason);
            Refused(state, () => CampaignCore.BeginRegionalReform(state, region, mode), reason);
            if (failure == "gold" || failure == "power") Assert.IsNotNull(CampaignCore.GetRegionalReformTerms(state, region, mode));
            Assert.IsFalse(CampaignCore.HasRegionalReform(state));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void EndingNeedsNoStocksOrPowerAndDoesNotUndoPeopleOrRefundThePaidPreparation(bool active)
        {
            var state = active ? Active() : CampaignCore.Create();
            if (!active) Success(CampaignCore.BeginRegionalReform(state, "normandy", "provisioning"));
            state.Gold = state.Food = state.MilitarySupplies = 0; state.Power = 0; Person(state, "morel").Relationship = 2.5f;
            int troops = state.Troops, manpower = state.Manpower, week = state.Week;
            Assert.AreEqual(-2.5f, CampaignCore.GetRegionalReformTerms(state).EndRelationshipDelta);
            Success(CampaignCore.EndRegionalReform(state));
            Assert.AreEqual(0, state.Gold); Assert.AreEqual(0, state.Food); Assert.AreEqual(0, state.MilitarySupplies); Assert.AreEqual(0, state.Power);
            Assert.AreEqual(troops, state.Troops); Assert.AreEqual(manpower, state.Manpower); Assert.AreEqual(week, state.Week);
            Assert.AreEqual(0, Person(state, "morel").Relationship); Assert.IsFalse(CampaignCore.HasRegionalReform(state));
            Assert.AreEqual("", state.ReformRegionId); Assert.AreEqual("", state.ReformModeId); Assert.AreEqual(0, state.ReformStepsRemaining);
            Refused(state, () => CampaignCore.EndRegionalReform(state), "error.reform.none"); Reload(state);
        }

        [Test]
        public void ExactStartupFundsAreFullyChargedAndTheNowEmptyTreasuryDoesNotPreventCancellation()
        {
            var state = CampaignCore.Create(); state.Gold = 120; state.Power = 4;
            Success(CampaignCore.BeginRegionalReform(state, "normandy", "provisioning"));
            Assert.AreEqual(0, state.Gold); Assert.AreEqual(0, state.Power); Assert.AreEqual(4, state.ReformStepsRemaining);
            Success(CampaignCore.EndRegionalReform(state));
            Assert.AreEqual(0, state.Gold); Assert.AreEqual(0, state.Power);
            Refused(state, () => CampaignCore.BeginRegionalReform(state, "normandy", "commerce"), "error.reform.gold");
            Reload(state);
        }

        [Test]
        public void EndingAnActiveReformPreservesAlreadyEarnedStocksAndOnlyChangesTheNextBudget()
        {
            var state = Active(); Advance(state);
            var plain = WithoutReform(state); var next = CampaignCore.Forecast(plain);
            int gold = state.Gold, food = state.Food, supplies = state.MilitarySupplies;
            int troops = state.Troops, manpower = state.Manpower; float power = state.Power;
            Success(CampaignCore.EndRegionalReform(state));
            Assert.AreEqual(gold, state.Gold); Assert.AreEqual(food, state.Food); Assert.AreEqual(supplies, state.MilitarySupplies);
            Assert.AreEqual(troops, state.Troops); Assert.AreEqual(manpower, state.Manpower); Assert.AreEqual(power, state.Power);
            Assert.AreEqual(46, Person(state, "morel").Relationship);
            Assert.AreEqual(SnapshotForecast(next), SnapshotForecast(CampaignCore.Forecast(state)));
            Advance(state); Assert.AreEqual(food + next.NetFood, state.Food); Assert.AreEqual(gold + next.NetGold, state.Gold);
        }

        [Test]
        public void SwitchingNeedsExplicitCancellationAndAFreshFullPaymentWithANewFourStepProject()
        {
            var state = CampaignCore.Create(); Success(CampaignCore.BeginRegionalReform(state, "normandy", "provisioning")); Advance(state);
            Refused(state, () => CampaignCore.BeginRegionalReform(state, "ile", "commerce"), "error.reform.open");
            int gold = state.Gold; float power = state.Power;
            Success(CampaignCore.EndRegionalReform(state)); Assert.AreEqual(gold, state.Gold); Assert.AreEqual(power, state.Power);
            Assert.AreEqual(42, Person(state, "morel").Relationship);
            Success(CampaignCore.BeginRegionalReform(state, "ile", "commerce"));
            Assert.AreEqual(gold - 120, state.Gold); Assert.AreEqual(power - 4, state.Power);
            Assert.AreEqual("ile", state.ReformRegionId); Assert.AreEqual("commerce", state.ReformModeId); Assert.AreEqual(4, state.ReformStepsRemaining);
            Assert.AreEqual(50, Person(state, "valcourt").Relationship); Reload(state);
        }

        [TestCase(98.5f, 1.5f)]
        [TestCase(100f, 0f)]
        public void CompletionRewardsOnlyTheRemainingRelationshipCapacity(float relationship, float delta)
        {
            var state = CampaignCore.Create(); Success(CampaignCore.BeginRegionalReform(state, "normandy", "provisioning"));
            for (int i = 0; i < 3; i++) Advance(state);
            Person(state, "morel").Relationship = relationship;
            Assert.AreEqual(delta, CampaignCore.GetRegionalReformTerms(state).CompletionRelationshipDelta);
            Advance(state); Assert.AreEqual(100, Person(state, "morel").Relationship);
            var completed = state.Journal.Find(entry => entry.Key == "log.reform.completed");
            Assert.AreEqual(delta.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture), completed.Args[3]);
        }

        [Test]
        public void AnAccordCompletingFirstCanUseAllRelationshipCapacityBeforeTheReformReward()
        {
            var state = CampaignCore.Create(); Success(CampaignCore.GrantRegionalAccord(state, "normandy"));
            Success(CampaignCore.BeginRegionalReform(state, "normandy", "provisioning")); Person(state, "morel").Relationship = 96;
            for (int i = 0; i < 4; i++) Advance(state);
            Assert.AreEqual(100, Person(state, "morel").Relationship);
            Assert.AreEqual("0", state.Journal.Find(entry => entry.Key == "log.reform.completed").Args[3]);
            Assert.AreEqual(1, state.Journal.FindAll(entry => entry.Key == "log.accord.completed").Count);
        }

        [Test]
        public void CalendarReservesTheFirstReformedBudgetButNeverTrapsAnExistingProject()
        {
            var state = CampaignCore.Create(); state.Week = LastWeek - 5; state.PetitionResolved = true;
            Success(CampaignCore.BeginRegionalReform(state, "normandy", "commerce"));
            Assert.AreEqual(LastWeek, CampaignCore.GetRegionalReformTerms(state).EarliestFirstReformedBudgetWeek);
            for (int i = 0; i < 5; i++) Advance(state);
            Assert.AreEqual(LastWeek, state.Week); Assert.AreEqual(-1, CampaignCore.GetRegionalReformTerms(state).NextBudgetWeek);
            Success(CampaignCore.EndRegionalReform(state));
            Refused(state, () => CampaignCore.BeginRegionalReform(state, "normandy", "commerce"), "error.reform.calendar");
            var late = CampaignCore.Create(); late.Week = LastWeek - 4; late.PetitionResolved = true;
            Refused(late, () => CampaignCore.BeginRegionalReform(late, "normandy", "commerce"), "error.reform.calendar");
            var paused = CampaignCore.Create(); Success(CampaignCore.BeginRegionalReform(paused, "champagne", "provisioning"));
            paused.Week = LastWeek; paused.PetitionResolved = true; paused = Reload(paused);
            Assert.AreEqual(-1, CampaignCore.GetRegionalReformTerms(paused).EarliestActivationWeek);
            Success(CampaignCore.EndRegionalReform(paused)); Assert.IsFalse(CampaignCore.HasRegionalReform(paused));
        }

        [TestCase("normandy", "provisioning")]
        [TestCase("ile", "provisioning")]
        [TestCase("normandy", "commerce")]
        [TestCase("ile", "commerce")]
        public void ActualHolidayAndItsTaxForgoneKeepTheReformInTheSameAndOtherRegion(string holidayRegion, string mode)
        {
            var state = Active(mode); Success(CampaignCore.GrantRegionalAccord(state, holidayRegion));
            string before = Snapshot(state); var terms = CampaignCore.GetRegionalReformTerms(state);
            var holiday = CampaignCore.GetActiveRegionalAccordTerms(state); var without = WithoutReform(state);
            var noHoliday = Copy(state); noHoliday.AccordRegionId = "";
            var forecast = CampaignCore.Forecast(state); var plain = CampaignCore.Forecast(without);
            Assert.AreEqual(forecast.TaxIncome, terms.CurrentTaxIncome);
            Assert.AreEqual(forecast.Production, terms.WithReformProduction);
            Assert.AreEqual(plain.TaxIncome, terms.WithoutReformTaxIncome);
            Assert.AreEqual(plain.Production, terms.WithoutReformProduction);
            Assert.AreEqual(forecast.TaxIncome - plain.TaxIncome, terms.TaxIncomeDelta);
            Assert.AreEqual(CampaignCore.Forecast(noHoliday).TaxIncome - forecast.TaxIncome, holiday.TaxForgone);
            if (holidayRegion == "normandy") Assert.AreEqual(0, terms.TaxIncomeDelta, "Muaf bölgenin gelecekteki vergisi bugünkü karşılaştırmaya eklenmez.");
            else Assert.AreNotEqual(0, terms.TaxIncomeDelta);
            Assert.AreNotEqual(0, terms.ProductionDelta); Assert.AreEqual(before, Snapshot(state));
            Advance(state); Assert.AreEqual(0, state.ReformStepsRemaining); Assert.AreEqual(8, state.AccordUntilWeek); Reload(state);
        }

        [Test]
        public void DumasForageUsesBothFullReformScenariosAndProductionIsNotCountedAgainAsExtraStock()
        {
            var state = Active();
            // Birim sınır fixture'ı: duyuru gerçek açlık hesabından gelir; sonra iki lens aynı koşulu okur.
            state.Food = 0; foreach (var region in state.Regions) region.Unrest = 100;
            Advance(state); Assert.IsTrue(CampaignCore.HasDumasInitiative(state));
            foreach (var region in state.Regions) region.Unrest = 60;
            state.Troops = 1200; CampaignCore.Validate(state);
            var plain = WithoutReform(state); string before = Snapshot(state);
            var terms = CampaignCore.GetRegionalReformTerms(state); var forecast = CampaignCore.Forecast(state);
            var plainForecast = CampaignCore.Forecast(plain); var initiative = CampaignCore.GetDumasInitiativeTerms(state);
            Assert.AreEqual("gather", initiative.Disposition);
            Assert.Greater(terms.WithoutReformForageFood, terms.WithReformForageFood);
            Assert.Greater(terms.WithReformForageFood, 0);
            Assert.AreEqual(plainForecast.ForageFood, terms.WithoutReformForageFood);
            Assert.AreEqual(forecast.ForageFood, terms.WithReformForageFood);
            Assert.AreEqual(plainForecast.Production, terms.WithoutReformProduction);
            Assert.AreEqual(forecast.Production, terms.WithReformProduction);
            Assert.Greater(terms.ProductionDelta, 0); Assert.AreEqual(0, terms.NetFoodDelta);
            Assert.AreEqual(0, terms.CurrentNetFood); Assert.AreEqual(0, terms.WithoutReformNetFood);
            Assert.AreEqual(before, Snapshot(state));
            float elite = CampaignCore.Region(state, "ile").EliteLoyalty;
            int troops = state.Troops; float ambition = Person(state, "dumas").Ambition, power = state.Power;
            Advance(state);
            Assert.AreEqual(0, state.Food); Assert.AreEqual(troops, state.Troops);
            Assert.AreEqual(67, CampaignCore.Region(state, "ile").Unrest);
            Assert.AreEqual(elite - 6, CampaignCore.Region(state, "ile").EliteLoyalty);
            Assert.AreEqual(ambition + 3, Person(state, "dumas").Ambition); Assert.AreEqual(power - 4 + .5f, state.Power);
            Assert.AreEqual(0, state.DumasForageDueWeek); Assert.AreEqual(1, state.Journal.FindAll(e => e.Key == "log.dumas.gathered").Count);
            Reload(state);
        }

        [Test]
        public void CompletingPreparationAndThenEndingItDoNotResizeRegionalResistance()
        {
            var state = CampaignCore.Create(); var region = CampaignCore.Region(state, "champagne");
            Success(CampaignCore.BeginRegionalReform(state, "champagne", "commerce"));
            Success(CampaignCore.Act(state, "bread", "champagne"));
            for (int i = 0; i < 4; i++) Advance(state);
            Assert.AreEqual(0, state.ReformStepsRemaining);
            region.Unrest = 70; region.Control = 60;
            var plain = WithoutReform(state);
            int expected = CampaignCore.GetRegionalResistance(plain, "champagne").EnemyTroops;
            Assert.Greater(expected, 0);
            Assert.AreEqual(expected, CampaignCore.GetRegionalResistance(state, "champagne").EnemyTroops);
            Success(CampaignCore.EndRegionalReform(state));
            Assert.AreEqual(expected, CampaignCore.GetRegionalResistance(state, "champagne").EnemyTroops);
        }

        [Test]
        public void V8PreservesAnActiveReformAlongsideARealNoticePromiseHolidayCommissionAndBattleReceipt()
        {
            var state = Active("provisioning", "crown");
            Success(CampaignCore.IssueMandate(state, "ile")); Success(CampaignCore.GrantRegionalAccord(state, "normandy"));
            Success(CampaignCore.GrantOfficerCommission(state)); Success(CampaignCore.Act(state, "recruit", "ile"));
            Success(CampaignCore.RecruitThroughDumas(state)); state.Food = 0;
            foreach (var region in state.Regions) region.Unrest = 100;
            Advance(state); Assert.IsTrue(CampaignCore.HasDumasInitiative(state));
            // Bu Core taşıma testi gerçek taktik galibiyet kanıtı değildir.
            Success(CampaignCore.ResolveBattle(state, "champagne", "battle-5-2-ile-champagne", true, 50, 60));
            var loaded = Reload(state);
            Assert.AreEqual("normandy", loaded.ReformRegionId); Assert.AreEqual("provisioning", loaded.ReformModeId);
            Assert.AreEqual(0, loaded.ReformStepsRemaining); Assert.IsTrue(loaded.DumasOfficerCommission);
            Assert.IsFalse(loaded.DumasExtraRecruitUsed); Assert.AreEqual(6, loaded.DumasForageDueWeek);
            Assert.AreEqual(9, loaded.DumasNextForageWeek); Assert.AreEqual("normandy", loaded.AccordRegionId);
            Assert.AreEqual(8, loaded.AccordUntilWeek); Assert.AreEqual(6, loaded.Obligation.DueWeek);
            Assert.AreEqual("battle-5-2-ile-champagne", loaded.PendingVictoryId);
            Assert.AreEqual(SnapshotForecast(CampaignCore.Forecast(state)), SnapshotForecast(CampaignCore.Forecast(loaded)));
        }

        [Test]
        public void TwoWeekArmyReductionDoesNotResetOrFinishTheIndependentFourWeekPreparation()
        {
            var state = CampaignCore.Create("army"); Success(CampaignCore.IssueMandate(state, "ile"));
            Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
            Success(CampaignCore.BeginRegionalReform(state, "normandy", "provisioning"));
            Advance(state); Advance(state);
            Assert.AreEqual(1000, state.Troops); Assert.AreEqual(2600, state.Manpower); Assert.AreEqual(0, state.ArmyReductionDueWeek);
            Assert.AreEqual(2, state.ReformStepsRemaining); Assert.AreEqual(80, state.Obligation.GoldDue);
            state = Reload(state); Success(CampaignCore.ResolveMandate(state, CampaignCore.MandateId(state.Obligation), "fulfil"));
            Advance(state); Advance(state); Assert.AreEqual(0, state.ReformStepsRemaining);
            Assert.AreEqual("budget", state.ArmyPolicyId); Assert.AreEqual(1000, state.ArmyTargetTroops); Reload(state);
        }

        static string Older(string json, int version)
        {
            json = json.Replace("\"Version\":8", "\"Version\":" + version)
                .Replace("\"ReformRegionId\":", "\"IgnoredReformRegion\":")
                .Replace("\"ReformModeId\":", "\"IgnoredReformMode\":")
                .Replace("\"ReformStepsRemaining\":", "\"IgnoredReformSteps\":");
            if (version < 7) json = json.Replace("\"DumasOfficerCommission\":", "\"IgnoredCommission\":")
                .Replace("\"DumasExtraRecruitUsed\":", "\"IgnoredExtraRecruit\":");
            if (version < 6) json = json.Replace("\"ArmyPolicyId\":", "\"IgnoredArmyPolicy\":")
                .Replace("\"ArmyTargetTroops\":", "\"IgnoredArmyTarget\":").Replace("\"ArmyReductionDueWeek\":", "\"IgnoredArmyDue\":");
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
        [TestCase(6)]
        [TestCase(7)]
        public void EveryOlderSchemaMigratesToClosedWithoutDroppingAnyFeatureItAlreadySupports(int version)
        {
            var state = CampaignCore.Create(version == 1 ? "legacy" : "crown");
            if (version >= 2) Success(CampaignCore.IssueMandate(state, "ile"));
            if (version >= 3) Success(CampaignCore.GrantRegionalAccord(state, "normandy"));
            if (version == 6) Success(CampaignCore.SetArmyEstablishment(state, "budget", 1000));
            if (version >= 7) Success(CampaignCore.GrantOfficerCommission(state));
            if (version >= 5)
            {
                state.Food = 0; foreach (var region in state.Regions) region.Unrest = 100;
                Advance(state); Assert.AreEqual(2, state.DumasForageDueWeek);
            }
            if (version >= 4) Success(CampaignCore.ResolveBattle(state, "champagne", "battle-" + state.Week + "-2-ile-champagne", true, 50, 60));
            var loaded = CampaignArchive.Deserialize(Older(CampaignArchive.Serialize(state, false), version));
            Assert.AreEqual(Snapshot(state), Snapshot(loaded)); Assert.IsFalse(CampaignCore.HasRegionalReform(loaded));
            Assert.AreEqual("", loaded.ReformRegionId); Assert.AreEqual("", loaded.ReformModeId); Assert.AreEqual(0, loaded.ReformStepsRemaining);
            if (version >= 7) Assert.IsTrue(loaded.DumasOfficerCommission);
            if (version == 6) { Assert.AreEqual(1000, loaded.ArmyTargetTroops); Assert.AreEqual(2, loaded.ArmyReductionDueWeek); }
            if (version >= 5) Assert.AreEqual(2, loaded.DumasForageDueWeek);
            if (version >= 4) Assert.IsTrue(CampaignCore.HasPendingVictory(loaded));
            if (version >= 3) Assert.AreEqual("normandy", loaded.AccordRegionId);
            if (version >= 2) Assert.IsNotNull(loaded.Obligation);
            Reload(loaded);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        public void OlderVersionNumbersCannotSilentlyEraseAPaidProject(int version)
        {
            var state = CampaignCore.Create(); Success(CampaignCore.BeginRegionalReform(state, "normandy", "provisioning"));
            string json = CampaignArchive.Serialize(state, false).Replace("\"Version\":8", "\"Version\":" + version);
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize(json));
        }

        [TestCase("missing_region")]
        [TestCase("null_region")]
        [TestCase("missing_mode")]
        [TestCase("null_mode")]
        [TestCase("missing_steps")]
        [TestCase("null_steps")]
        [TestCase("text_steps")]
        public void V8RequiresAllThreeTypedFieldsEvenWhenThereIsNoProject(string corruption)
        {
            string json = CampaignArchive.Serialize(CampaignCore.Create(), false);
            string field = corruption.EndsWith("region") ? "ReformRegionId" : corruption.EndsWith("mode") ? "ReformModeId" : "ReformStepsRemaining";
            string value = field == "ReformStepsRemaining" ? "0" : "\"\"";
            string original = "\"" + field + "\":" + value;
            string replacement = corruption.StartsWith("missing") ? "\"Ignored" + field + "\":" + value :
                "\"" + field + "\":" + (corruption.StartsWith("null") ? "null" : "\"not-an-integer\"");
            StringAssert.Contains(original, json);
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize(json.Replace(original, replacement)));
        }

        [TestCase("unknown_region")]
        [TestCase("unknown_mode")]
        [TestCase("orphan_mode")]
        [TestCase("orphan_steps")]
        [TestCase("negative_steps")]
        [TestCase("five_steps")]
        [TestCase("early_active")]
        [TestCase("early_progress")]
        public void ImpossibleProjectCombinationsCannotBeSavedLoadedOrUsedForCommands(string corruption)
        {
            var state = CampaignCore.Create();
            state.ReformRegionId = "normandy"; state.ReformModeId = "provisioning"; state.ReformStepsRemaining = 4;
            switch (corruption)
            {
                case "unknown_region": state.ReformRegionId = "missing"; break;
                case "unknown_mode": state.ReformModeId = "unknown"; break;
                case "orphan_mode": state.ReformRegionId = ""; state.ReformStepsRemaining = 0; break;
                case "orphan_steps": state.ReformRegionId = state.ReformModeId = ""; break;
                case "negative_steps": state.ReformStepsRemaining = -1; break;
                case "five_steps": state.ReformStepsRemaining = 5; break;
                case "early_active": state.ReformStepsRemaining = 0; break;
                case "early_progress": state.ReformStepsRemaining = 3; break;
            }
            Assert.Throws<ArgumentException>(() => CampaignArchive.Serialize(state));
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize("{\"Version\":8,\"State\":" + Snapshot(state) + "}"));
            Assert.IsNull(CampaignCore.GetRegionalReformTerms(state));
            Refused(state, () => CampaignCore.BeginRegionalReform(state, "normandy", "provisioning"), "error.reform.state");
            Refused(state, () => CampaignCore.EndRegionalReform(state), "error.reform.state");
        }
    }
}
#endif
