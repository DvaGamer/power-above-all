#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    public sealed class DumasInitiativeTests
    {
        static string Snapshot(CampaignState state) => JsonUtility.ToJson(state);
        static CharacterState Dumas(CampaignState state) => state.Characters.Find(item => item.Id == "dumas");
        static void Success(ActionResult result) { Assert.IsTrue(result.Ok, result.Key); }
        static CampaignState Copy(CampaignState state) => JsonUtility.FromJson<CampaignState>(Snapshot(state));
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
        static void SetHunger(CampaignState state)
        {
            state.Food = 0;
            foreach (var region in state.Regions) region.Unrest = 100;
        }
        // Sınır fixture'ı: uyarı gerçek bir açlık hesabından doğar, sonra sayısal kenarlar kurulur.
        static CampaignState ForageState(int troops = 1200)
        {
            var state = CampaignCore.Create(); SetHunger(state);
            Success(CampaignCore.NextWeek(state));
            Assert.AreEqual(2, state.DumasForageDueWeek); Assert.AreEqual(5, state.DumasNextForageWeek);
            foreach (var region in state.Regions) region.Unrest = 60;
            state.Troops = troops;
            CampaignCore.Validate(state); return state;
        }
        static void AssertSameExceptJournal(CampaignState expected, CampaignState actual)
        {
            var left = Copy(expected); var right = Copy(actual);
            left.Journal.Clear(); right.Journal.Clear();
            Assert.AreEqual(Snapshot(left), Snapshot(right));
        }
        static void AssertWeekMatches(CampaignState state, EconomyForecast forecast, int oldGold, int oldFood)
        {
            Assert.AreEqual(Math.Max(0, oldGold + forecast.NetGold), state.Gold);
            Assert.AreEqual(Math.Max(0, oldFood + forecast.NetFood), state.Food);
            var entry = state.Journal.Find(item => item.Key == "log.week" && item.Week == state.Week);
            Assert.AreEqual(forecast.TaxIncome.ToString(), entry.Args[1]);
            Assert.AreEqual(forecast.ArmyCost.ToString(), entry.Args[2]);
            Assert.AreEqual(forecast.NetFood.ToString(), entry.Args[3]);
        }

        [Test]
        public void OnlyActualFoodHungerAnnouncesAndTheOldFirstShortageIsUnchanged()
        {
            var sufficient = CampaignCore.Create(); sufficient.Food = 0;
            Assert.GreaterOrEqual(CampaignCore.Forecast(sufficient).NetFood, 0);
            Success(CampaignCore.NextWeek(sufficient)); Assert.IsFalse(CampaignCore.HasDumasInitiative(sufficient));
            var unpaid = CampaignCore.Create(); unpaid.Troops = 12000; unpaid.Gold = 0; unpaid.Food = 10000;
            Success(CampaignCore.NextWeek(unpaid)); Assert.IsFalse(CampaignCore.HasDumasInitiative(unpaid));
            var state = CampaignCore.Create(); SetHunger(state);
            Success(CampaignCore.NextWeek(state));
            Assert.AreEqual(1104, state.Troops); Assert.AreEqual(0, state.Food);
            Assert.AreEqual(75, state.Supply); Assert.AreEqual(63, state.Morale);
            Assert.AreEqual(50, state.Power); Assert.AreEqual(55, Dumas(state).Loyalty);
            Assert.AreEqual(80, Dumas(state).Ambition);
            Assert.AreEqual(2, state.DumasForageDueWeek); Assert.AreEqual(5, state.DumasNextForageWeek);
            Assert.AreEqual(1, state.Journal.FindAll(item => item.Key == "log.dumas.announced").Count);
            Reload(state);
        }

        [Test]
        public void ForecastAndActualWeekShareOneCompleteTransferAndOneLocalEffect()
        {
            var state = ForageState(); string before = Snapshot(state);
            var terms = CampaignCore.GetDumasInitiativeTerms(state); var forecast = CampaignCore.Forecast(state);
            Assert.AreEqual(before, Snapshot(state)); Assert.AreEqual("gather", terms.Disposition);
            Assert.AreEqual(17, terms.FoodGathered); Assert.AreEqual(17, terms.FoodShortfall);
            Assert.AreEqual(133, forecast.Production); Assert.AreEqual(17, forecast.ForageFood); Assert.AreEqual(0, forecast.NetFood);
            Assert.AreEqual(8, terms.UnrestDelta); Assert.AreEqual(-6, terms.EliteLoyaltyDelta); Assert.AreEqual(4, terms.PowerCost);
            int gold = state.Gold, food = state.Food, troops = state.Troops; float power = state.Power;
            terms.FoodGathered = 999; terms.UnrestDelta = 99;
            Success(CampaignCore.NextWeek(state));
            AssertWeekMatches(state, forecast, gold, food); Assert.AreEqual(troops, state.Troops);
            Assert.AreEqual(0, state.Food, "Toplama NetFood'a bir kez girer; stoka ikinci kez eklenmez.");
            Assert.AreEqual(67, CampaignCore.Region(state, "ile").Unrest, "60+8 toplama+2 şehir−3 garnizon.");
            Assert.AreEqual(54, CampaignCore.Region(state, "ile").EliteLoyalty);
            Assert.AreEqual(83, Dumas(state).Ambition); Assert.AreEqual(power - 4 + .5f, state.Power);
            Assert.AreEqual(85, state.Supply); Assert.AreEqual(66, state.Morale);
            Assert.AreEqual(0, state.DumasForageDueWeek); Assert.AreEqual(5, state.DumasNextForageWeek);
            Assert.AreEqual(1, state.Journal.FindAll(item => item.Key == "log.dumas.gathered").Count);
            Assert.AreEqual(2, state.Journal.Find(item => item.Key == "log.dumas.gathered").Week);
            Reload(state);
        }

        [TestCase(1890, 40, "gather")]
        [TestCase(1920, 41, "too_large")]
        public void FortyAndFortyOneAreDecidedAfterTheLocalProductionLoss(int troops, int needed, string disposition)
        {
            var state = ForageState(troops); var baseline = Copy(state); baseline.DumasForageDueWeek = 0;
            var raw = CampaignCore.Forecast(baseline); var terms = CampaignCore.GetDumasInitiativeTerms(state);
            Assert.AreEqual(needed - 1, -raw.NetFood, "Yerel zarar tam bir yuvarlanmış üretim birimi tüketir.");
            Assert.AreEqual(needed, terms.FoodShortfall); Assert.AreEqual(disposition, terms.Disposition);
            var forecast = CampaignCore.Forecast(state);
            if (needed == 40)
            {
                Assert.AreEqual(40, terms.FoodGathered); Assert.AreEqual(40, forecast.ForageFood); Assert.AreEqual(0, forecast.NetFood);
                Success(CampaignCore.NextWeek(state)); Assert.AreEqual(troops, state.Troops);
            }
            else
            {
                Assert.AreEqual(0, terms.FoodGathered); Assert.AreEqual(0, terms.PowerCost); Assert.AreEqual(0, terms.AmbitionDelta);
                Assert.AreEqual(0, terms.UnrestDelta); Assert.AreEqual(0, terms.EliteLoyaltyDelta);
                Assert.AreEqual(0, forecast.ForageFood); Assert.AreEqual(raw.NetFood, forecast.NetFood);
                Success(CampaignCore.NextWeek(state)); Success(CampaignCore.NextWeek(baseline));
                AssertSameExceptJournal(baseline, state);
                Assert.AreEqual(troops - (int)Math.Ceiling(troops * .08d), state.Troops);
                Assert.AreEqual(1, state.Journal.FindAll(item => item.Key == "log.dumas.too_large").Count);
            }
        }

        [TestCase("sufficient")]
        [TestCase("no_army")]
        public void CancelledInitiativesHaveNoLocalPoliticalOrEconomicEffects(string disposition)
        {
            var state = ForageState();
            if (disposition == "sufficient") state.Food = 20;
            else state.Troops = 0;
            var baseline = Copy(state); baseline.DumasForageDueWeek = 0;
            var terms = CampaignCore.GetDumasInitiativeTerms(state);
            Assert.AreEqual(disposition, terms.Disposition); Assert.AreEqual(0, terms.FoodGathered);
            Assert.AreEqual(0, terms.PowerCost); Assert.AreEqual(0, terms.UnrestDelta); Assert.AreEqual(0, terms.EliteLoyaltyDelta);
            Success(CampaignCore.NextWeek(state)); Success(CampaignCore.NextWeek(baseline));
            AssertSameExceptJournal(baseline, state); Assert.AreEqual(5, state.DumasNextForageWeek);
        }

        [TestCase(0f, 0f)]
        [TestCase(2.5f, 2.5f)]
        [TestCase(55f, 4f)]
        public void AutomaticPoliticalCostIsTheActualAvailablePowerWithoutAnAffordabilityGate(float power, float price)
        {
            var state = ForageState(); state.Power = power;
            Assert.AreEqual(price, CampaignCore.GetDumasInitiativeTerms(state).PowerCost);
            Success(CampaignCore.NextWeek(state)); Assert.AreEqual(power - price + .5f, state.Power);
            Assert.AreEqual(1200, state.Troops);
        }

        [Test]
        public void LocalAndCharacterCapsArePreviewedAndEarnedLoyaltyKeepsInitiativePoliticallyFree()
        {
            var state = ForageState(); var region = CampaignCore.Region(state, "ile");
            region.Unrest = 98; region.EliteLoyalty = 2;
            Dumas(state).Ambition = 99; Dumas(state).Loyalty = 100;
            var terms = CampaignCore.GetDumasInitiativeTerms(state);
            Assert.AreEqual(2, terms.UnrestDelta); Assert.AreEqual(-2, terms.EliteLoyaltyDelta);
            Assert.AreEqual(1, terms.AmbitionDelta); Assert.AreEqual(0, terms.PowerCost);
            float power = state.Power;
            Success(CampaignCore.NextWeek(state)); Assert.AreEqual(100, Dumas(state).Ambition);
            Assert.AreEqual(0, region.EliteLoyalty); Assert.AreEqual(power + .5f, state.Power);
        }

        [Test]
        public void VetoIsAtomicOnRefusalClampedOnSuccessAndCannotResetCooldown()
        {
            var state = ForageState(); Dumas(state).Relationship = 2.5f;
            Assert.AreEqual(-2.5f, CampaignCore.GetDumasInitiativeTerms(state).VetoRelationshipDelta);
            Refused(state, () => CampaignCore.VetoDumasInitiative(state, 3), "error.dumas.stale");
            Success(CampaignCore.VetoDumasInitiative(state, 2));
            Assert.AreEqual(0, Dumas(state).Relationship); Assert.AreEqual(0, state.DumasForageDueWeek);
            Assert.AreEqual(5, state.DumasNextForageWeek); Assert.AreEqual(0, CampaignCore.Forecast(state).ForageFood);
            Refused(state, () => CampaignCore.VetoDumasInitiative(state, 2), "error.dumas.none");
            for (int week = 2; week <= 4; week++)
            {
                SetHunger(state); Success(CampaignCore.NextWeek(state));
                Assert.AreEqual(0, state.DumasForageDueWeek); Assert.AreEqual(5, state.DumasNextForageWeek);
                if (state.PendingPetition) Success(CampaignCore.ChoosePetition(state, "negotiate"));
            }
            SetHunger(state); Success(CampaignCore.NextWeek(state));
            Assert.AreEqual(6, state.DumasForageDueWeek); Assert.AreEqual(9, state.DumasNextForageWeek);
        }

        [Test]
        public void TheCampFollowsARealMarchWithoutChangingTheAnnouncementDeadline()
        {
            var state = ForageState(); state.Food = 20;
            Success(CampaignCore.March(state, "normandy"));
            var terms = CampaignCore.GetDumasInitiativeTerms(state);
            Assert.AreEqual("normandy", terms.RegionId); Assert.AreEqual(2, terms.DueWeek);
            Assert.AreEqual(5, terms.NextForageWeek); Assert.AreEqual("gather", terms.Disposition);
            float parisElite = CampaignCore.Region(state, "ile").EliteLoyalty;
            Success(CampaignCore.NextWeek(state));
            Assert.AreEqual(parisElite, CampaignCore.Region(state, "ile").EliteLoyalty);
            Assert.AreEqual(54, CampaignCore.Region(state, "normandy").EliteLoyalty);
        }

        [Test]
        public void HypotheticalAccordCanRemoveTheNeedAndItsPriceMatchesTheActualSharedPlan()
        {
            var state = ForageState(750);
            Assert.AreEqual(2, CampaignCore.GetDumasInitiativeTerms(state).FoodGathered);
            string before = Snapshot(state);
            var terms = CampaignCore.GetRegionalAccordTerms(state, "orleans");
            Assert.AreEqual(before, Snapshot(state));
            Success(CampaignCore.GrantRegionalAccord(state, "orleans"));
            Assert.AreEqual("sufficient", CampaignCore.GetDumasInitiativeTerms(state).Disposition);
            Assert.AreEqual(0, CampaignCore.Forecast(state).ForageFood);
            Assert.AreEqual(terms.ProjectedTaxIncome, CampaignCore.Forecast(state).TaxIncome);
            var noHoliday = Copy(state); noHoliday.AccordRegionId = "";
            Assert.AreEqual(CampaignCore.Forecast(noHoliday).TaxIncome - CampaignCore.Forecast(state).TaxIncome, terms.TaxForgone);
            Assert.AreEqual(60, CampaignCore.Region(state, "ile").Unrest);
            int gold = state.Gold, food = state.Food; var forecast = CampaignCore.Forecast(state);
            Success(CampaignCore.NextWeek(state)); AssertWeekMatches(state, forecast, gold, food);
            Assert.AreEqual(60, CampaignCore.Region(state, "ile").EliteLoyalty);
        }

        [Test]
        public void PetitionAndMandateGuardsPreserveNpcAndVictoryUntilARealWeekAndFourTaxSettlements()
        {
            var state = CampaignCore.Create("crown");
            Success(CampaignCore.IssueMandate(state, "ile")); string mandate = CampaignCore.MandateId(state.Obligation);
            Success(CampaignCore.GrantRegionalAccord(state, "normandy")); Success(CampaignCore.NextWeek(state));
            SetHunger(state); Success(CampaignCore.NextWeek(state));
            Assert.AreEqual(3, state.DumasForageDueWeek);
            Success(CampaignCore.ResolveBattle(state, "champagne", "battle-2-2-ile-champagne", true, 10, 60));
            string victory = state.PendingVictoryId;
            Refused(state, () => CampaignCore.VetoDumasInitiative(state, 3), "error.mandate.petition");
            Refused(state, () => CampaignCore.NextWeek(state), "error.petition.pending");
            state = Reload(state); Success(CampaignCore.ChoosePetition(state, "negotiate"));
            Refused(state, () => CampaignCore.VetoDumasInitiative(state, 3), "error.mandate.due");
            Refused(state, () => CampaignCore.NextWeek(state), "error.mandate.due");
            Assert.AreEqual(victory, state.PendingVictoryId); Assert.AreEqual("ile", state.Obligation.RegionId);
            Assert.AreEqual(150, state.Obligation.GoldDue); Assert.AreEqual(2, state.Obligation.DueWeek);
            Success(CampaignCore.ResolveMandate(state, mandate, "fulfil"));
            for (int week = 3; week <= 4; week++)
            {
                var forecast = CampaignCore.Forecast(state); int gold = state.Gold, food = state.Food;
                Success(CampaignCore.NextWeek(state)); AssertWeekMatches(state, forecast, gold, food);
                Assert.AreEqual(week < 4, CampaignCore.HasRegionalAccord(state));
            }
            Assert.AreEqual("", state.PendingVictoryId); Assert.AreEqual(0, state.DumasForageDueWeek);
            Assert.AreEqual(6, state.DumasNextForageWeek); Assert.AreEqual(4, state.AccordUntilWeek);
            Assert.IsNull(state.Obligation); Reload(state);
        }

        [TestCase("fresh")]
        [TestCase("active")]
        [TestCase("veto")]
        [TestCase("completed")]
        public void V5RoundTripPreservesInitiativesAndCooldowns(string phase)
        {
            var state = phase == "fresh" ? CampaignCore.Create() : ForageState();
            if (phase == "veto") Success(CampaignCore.VetoDumasInitiative(state, 2));
            if (phase == "completed") Success(CampaignCore.NextWeek(state));
            var loaded = Reload(state);
            Assert.AreEqual(phase == "active", CampaignCore.HasDumasInitiative(loaded));
            Assert.AreEqual(phase == "fresh" ? 0 : 5, loaded.DumasNextForageWeek);
            Assert.AreEqual(JsonUtility.ToJson(CampaignCore.Forecast(state)), JsonUtility.ToJson(CampaignCore.Forecast(loaded)));
        }

        [TestCase(5, "missing_due")]
        [TestCase(5, "null_due")]
        [TestCase(5, "text_due")]
        [TestCase(5, "missing_next")]
        [TestCase(5, "null_next")]
        [TestCase(6, "missing_due")]
        [TestCase(6, "null_due")]
        [TestCase(6, "text_due")]
        [TestCase(6, "missing_next")]
        [TestCase(6, "null_next")]
        [TestCase(7, "missing_due")]
        [TestCase(7, "null_due")]
        [TestCase(7, "text_due")]
        [TestCase(7, "missing_next")]
        [TestCase(7, "null_next")]
        public void V5AndCurrentRequireBothTypedInitiativeFields(int version, string corruption)
        {
            string json = CampaignArchive.Serialize(CampaignCore.Create(), false).Replace("\"Version\":7", "\"Version\":" + version);
            switch (corruption)
            {
                case "missing_due": json = json.Replace("\"DumasForageDueWeek\":", "\"IgnoredDue\":"); break;
                case "null_due": json = json.Replace("\"DumasForageDueWeek\":0", "\"DumasForageDueWeek\":null"); break;
                case "text_due": json = json.Replace("\"DumasForageDueWeek\":0", "\"DumasForageDueWeek\":\"bad-date\""); break;
                case "missing_next": json = json.Replace("\"DumasNextForageWeek\":", "\"IgnoredNext\":"); break;
                case "null_next": json = json.Replace("\"DumasNextForageWeek\":0", "\"DumasNextForageWeek\":null"); break;
            }
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize(json));
        }

        [TestCase("due_old")]
        [TestCase("due_far")]
        [TestCase("next_negative")]
        [TestCase("next_early")]
        [TestCase("next_far")]
        [TestCase("next_mismatch")]
        public void CorruptTimersRejectArchiveAndVetoWithoutMutation(string corruption)
        {
            var state = ForageState();
            switch (corruption)
            {
                case "due_old": state.DumasForageDueWeek = 1; break;
                case "due_far": state.DumasForageDueWeek = 3; break;
                case "next_negative": state.DumasNextForageWeek = -1; break;
                case "next_early": state.DumasNextForageWeek = 4; break;
                case "next_far": state.DumasNextForageWeek = 6; break;
                case "next_mismatch": state.DumasNextForageWeek = 0; break;
            }
            Assert.Throws<ArgumentException>(() => CampaignArchive.Serialize(state));
            Refused(state, () => CampaignCore.VetoDumasInitiative(state, 2), "error.dumas.state");
            Assert.IsNull(CampaignCore.GetDumasInitiativeTerms(state));
        }

        static string Older(string json, int version)
        {
            json = json.Replace("\"Version\":7", "\"Version\":" + version)
                .Replace("\"DumasForageDueWeek\":", "\"IgnoredForageDue\":")
                .Replace("\"DumasNextForageWeek\":", "\"IgnoredForageNext\":");
            if (version < 4) json = json.Replace("\"PendingVictoryId\":", "\"IgnoredVictory\":");
            if (version < 3) json = json.Replace("\"AccordRegionId\":", "\"IgnoredRegion\":")
                .Replace("\"AccordUntilWeek\":", "\"IgnoredUntil\":");
            if (version == 1) json = json.Replace("\"RoleId\":", "\"IgnoredRole\":")
                .Replace("\"NextMandateWeek\":", "\"IgnoredMandateWeek\":").Replace("\"Mandates\":", "\"IgnoredMandates\":");
            return json;
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void ActualOlderArchivesMigrateEmptyNpcFieldsWithoutLosingTheirOwnFeatures(int version)
        {
            var state = CampaignCore.Create(version == 1 ? "legacy" : "crown");
            if (version >= 2) Success(CampaignCore.IssueMandate(state, "ile"));
            if (version >= 3) Success(CampaignCore.GrantRegionalAccord(state, "normandy"));
            if (version >= 4) Success(CampaignCore.ResolveBattle(state, "champagne", "battle-0-2-ile-champagne", true, 90, 60));
            string json = Older(CampaignArchive.Serialize(state, false), version);
            StringAssert.DoesNotContain("\"DumasForageDueWeek\":", json);
            var loaded = CampaignArchive.Deserialize(json);
            Assert.AreEqual(Snapshot(state), Snapshot(loaded));
            Assert.AreEqual(0, loaded.DumasForageDueWeek); Assert.AreEqual(0, loaded.DumasNextForageWeek); Reload(loaded);
        }

        [TestCase(3)]
        [TestCase(4)]
        public void V3AndV4BrokenAccordCooldownIsPreservedInV5(int version)
        {
            var state = CampaignCore.Create("crown"); Success(CampaignCore.IssueMandate(state, "ile"));
            Success(CampaignCore.GrantRegionalAccord(state, "normandy")); Success(CampaignCore.Act(state, "tax", "normandy"));
            var loaded = CampaignArchive.Deserialize(Older(CampaignArchive.Serialize(state, false), version));
            Assert.AreEqual(Snapshot(state), Snapshot(loaded)); Assert.AreEqual(4, loaded.AccordUntilWeek);
            Assert.AreEqual("", loaded.AccordRegionId); Assert.AreEqual(150, loaded.Obligation.GoldDue);
        }

        [TestCase(1, false)]
        [TestCase(1, true)]
        [TestCase(2, false)]
        [TestCase(2, true)]
        [TestCase(3, false)]
        [TestCase(3, true)]
        [TestCase(4, false)]
        [TestCase(4, true)]
        public void OldVersionNumbersCannotHideAnActiveOrVetoedInitiative(int version, bool vetoed)
        {
            var state = ForageState(); if (vetoed) Success(CampaignCore.VetoDumasInitiative(state, 2));
            string json = CampaignArchive.Serialize(state, false).Replace("\"Version\":7", "\"Version\":" + version);
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize(json));
        }

        [Test]
        public void LastCooldownBoundaryAllowsCompletionButNeverSchedulesBeyondTheCalendar()
        {
            var state = CampaignCore.Create(); state.Week = 999995; state.PetitionResolved = true; SetHunger(state);
            Success(CampaignCore.NextWeek(state)); Assert.AreEqual(999997, state.DumasForageDueWeek);
            Assert.AreEqual(1000000, state.DumasNextForageWeek);
            state.Food = 1000; Success(CampaignCore.NextWeek(state)); Assert.AreEqual(0, state.DumasForageDueWeek);
            while (state.Week < 1000000) Success(CampaignCore.NextWeek(state));
            Refused(state, () => CampaignCore.NextWeek(state), "error.week.limit"); Reload(state);
            state = CampaignCore.Create(); state.Week = 999996; state.PetitionResolved = true; SetHunger(state);
            Success(CampaignCore.NextWeek(state)); Assert.AreEqual(0, state.DumasForageDueWeek); Assert.AreEqual(0, state.DumasNextForageWeek);
        }

        [Test]
        public void ActualInitiativeReasonsAndReportsFormatInRussianAndTurkishWithoutSavingPreferences()
        {
            var messages = new List<ActionResult>();
            foreach (string disposition in new[] { "gather", "too_large", "sufficient", "no_army" })
            {
                var state = ForageState(disposition == "too_large" ? 1920 : 1200);
                if (disposition == "sufficient") state.Food = 20;
                if (disposition == "no_army") state.Troops = 0;
                var terms = CampaignCore.GetDumasInitiativeTerms(state);
                messages.Add(new ActionResult { Key = terms.ReasonKey, Args = terms.ReasonArgs });
                Success(CampaignCore.NextWeek(state));
                foreach (var entry in state.Journal)
                    if (entry.Key.StartsWith("log.dumas.", StringComparison.Ordinal))
                        messages.Add(new ActionResult { Key = entry.Key, Args = entry.Args });
            }
            var veto = ForageState();
            messages.Add(CampaignCore.CanVetoDumasInitiative(veto, 2));
            messages.Add(CampaignCore.VetoDumasInitiative(veto, 3));
            messages.Add(CampaignCore.VetoDumasInitiative(veto, 2));
            messages.Add(CampaignCore.VetoDumasInitiative(veto, 2));
            veto.DumasForageDueWeek = -1; messages.Add(CampaignCore.VetoDumasInitiative(veto, 2));
            var actualKeys = new HashSet<string>(); foreach (var message in messages) actualKeys.Add(message.Key);
            foreach (var asset in Resources.LoadAll<TextAsset>("Localization"))
            {
                var table = JsonUtility.FromJson<L.Table>(asset.text);
                foreach (var entry in table.entries)
                    if (entry.key.StartsWith("log.dumas.", StringComparison.Ordinal) || entry.key.StartsWith("error.dumas.", StringComparison.Ordinal) ||
                        entry.key.StartsWith("dumas.reason.", StringComparison.Ordinal))
                        Assert.IsTrue(actualKeys.Contains(entry.key), entry.key);
            }
            string profile = Environment.GetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE"), language = L.Language;
            bool hadPreference = PlayerPrefs.HasKey("language"); string preference = PlayerPrefs.GetString("language", "");
            try
            {
                Environment.SetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE", "dumas-localization-check"); L.Initialize();
                foreach (string selected in new[] { "ru", "tr" })
                {
                    L.SetLanguage(selected);
                    foreach (var message in messages)
                    {
                        string rendered = L.Text(message.Key, message.Args);
                        Assert.AreNotEqual(message.Key, rendered); StringAssert.DoesNotContain("{", rendered);
                        StringAssert.DoesNotContain("region.", rendered);
                    }
                }
                Assert.AreEqual(hadPreference, PlayerPrefs.HasKey("language")); Assert.AreEqual(preference, PlayerPrefs.GetString("language", ""));
            }
            finally { L.SetLanguage(language); Environment.SetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE", profile); }
        }
    }
}
#endif
