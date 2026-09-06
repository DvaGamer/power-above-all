#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    public sealed class CampaignVictoryDecisionTests
    {
        static string Snapshot(CampaignState state) => JsonUtility.ToJson(state);
        static CharacterState Dumas(CampaignState state) => state.Characters.Find(item => item.Id == "dumas");
        static void Success(ActionResult result) { Assert.IsTrue(result.Ok, result.Key); }
        static string BattleId(CampaignState state, string target)
        { return "battle-" + state.Week + "-" + state.Moves + "-" + state.ArmyRegionId + "-" + target; }
        static CampaignState Winner(CampaignState state = null, string target = "champagne")
        {
            state = state ?? CampaignCore.Create();
            Success(CampaignCore.ResolveBattle(state, target, BattleId(state, target), true, 196, 68));
            Assert.IsTrue(CampaignCore.HasPendingVictory(state));
            CampaignCore.Validate(state);
            return state;
        }
        static void Refused(CampaignState state, Func<ActionResult> action, string reason)
        {
            string before = Snapshot(state); var result = action();
            Assert.IsFalse(result.Ok); Assert.AreEqual(reason, result.Key);
            Assert.AreEqual(before, Snapshot(state), "Reddedilen seçim, günlük dahil bütün durumu korumalı.");
        }
        static CampaignState Reload(CampaignState state)
        {
            string before = Snapshot(state), json = CampaignArchive.Serialize(state, false);
            StringAssert.Contains("\"Version\":4", json);
            var loaded = CampaignArchive.Deserialize(json);
            Assert.AreEqual(before, Snapshot(loaded));
            return loaded;
        }
        static void Advance(CampaignState state)
        {
            Success(CampaignCore.NextWeek(state));
            if (state.PendingPetition) Success(CampaignCore.ChoosePetition(state, "negotiate"));
        }
        static string AsOlder(string json, int version)
        {
            json = json.Replace("\"Version\":4", "\"Version\":" + version)
                .Replace("\"PendingVictoryId\":", "\"IgnoredVictory\":");
            if (version < 3) json = json.Replace("\"AccordRegionId\":", "\"IgnoredRegion\":")
                .Replace("\"AccordUntilWeek\":", "\"IgnoredUntil\":");
            if (version == 1) json = json.Replace("\"RoleId\":", "\"IgnoredRole\":")
                .Replace("\"NextMandateWeek\":", "\"IgnoredNext\":").Replace("\"Mandates\":", "\"IgnoredMandates\":");
            return json;
        }

        [Test]
        public void ExistingVictoryResultIsUnchangedAndOnlyAnAppliedWinOffersAChoice()
        {
            var state = CampaignCore.Create();
            Assert.IsFalse(CampaignCore.HasPendingVictory(state));
            Assert.IsNull(CampaignCore.GetVictoryDecisionTerms(state, "bonus"));
            string id = BattleId(state, "champagne");
            Refused(state, () => CampaignCore.ResolveBattle(state, "champagne", "stale", true, 196, 68), "error.battle.stale");
            Winner(state);
            Assert.AreEqual(id, state.PendingVictoryId); Assert.AreEqual(1004, state.Troops);
            Assert.AreEqual(59, state.Power); Assert.AreEqual(35, state.Fatigue); Assert.AreEqual(71, state.Morale);
            Assert.AreEqual(342, state.Food); Assert.AreEqual(840, state.Gold);
            Assert.AreEqual(83, Dumas(state).Ambition); Assert.AreEqual(52, Dumas(state).Relationship);
            Assert.AreEqual(60, Dumas(state).Loyalty); Assert.AreEqual(78, Dumas(state).Competence);
            Assert.AreEqual(49, CampaignCore.Region(state, "champagne").Unrest);
            Assert.AreEqual(70.5f, CampaignCore.Region(state, "champagne").Control);
            Assert.AreEqual(64, state.Factions.Find(item => item.Id == "army").Approval);
            Assert.AreEqual(32, state.Factions.Find(item => item.Id == "urban").Approval);
            Refused(state, () => CampaignCore.ResolveBattle(state, "champagne", id, true, 196, 68), "error.battle.duplicate");
            Assert.AreEqual(1, state.ResolvedBattles.Count);
            Reload(state);
        }

        [Test]
        public void RecognitionUsesPreviewAndSpendsPersonalPowerOnlyOnce()
        {
            var state = Winner(); string id = state.PendingVictoryId, before = Snapshot(state);
            var terms = CampaignCore.GetVictoryDecisionTerms(state, "recognize");
            Success(CampaignCore.CanResolveVictory(state, id, "recognize"));
            Assert.AreEqual(before, Snapshot(state));
            Assert.AreEqual(id, terms.BattleId); Assert.AreEqual("champagne", terms.RegionId); Assert.AreEqual("recognize", terms.ChoiceId);
            Assert.AreEqual(0, terms.GoldCost); Assert.AreEqual(4, terms.PowerCost);
            Assert.AreEqual(-12, terms.FatigueDelta); Assert.AreEqual(4, terms.RelationshipDelta); Assert.AreEqual(3, terms.AmbitionDelta);
            Success(CampaignCore.ResolveVictory(state, id, "recognize"));
            Assert.AreEqual(55, state.Power); Assert.AreEqual(23, state.Fatigue); Assert.AreEqual(840, state.Gold);
            Assert.AreEqual(56, Dumas(state).Relationship); Assert.AreEqual(86, Dumas(state).Ambition);
            Assert.AreEqual(60, Dumas(state).Loyalty); Assert.AreEqual(78, Dumas(state).Competence);
            Assert.AreEqual("", state.PendingVictoryId);
            Refused(state, () => CampaignCore.ResolveVictory(state, id, "recognize"), "error.victory.none");
            Reload(state);
        }

        [TestCase(82.5f, 4f)]
        [TestCase(83f, 0f)]
        [TestCase(83.5f, 0f)]
        public void RecognitionPriceUsesPreChoiceLoyaltyAndAmbitionIncludingEquality(float loyalty, float price)
        {
            var state = Winner(); Dumas(state).Loyalty = loyalty;
            float before = state.Power;
            Assert.AreEqual(price, CampaignCore.GetVictoryDecisionTerms(state, "recognize").PowerCost);
            Success(CampaignCore.ResolveVictory(state, state.PendingVictoryId, "recognize"));
            Assert.AreEqual(before - price, state.Power); Assert.AreEqual(86, Dumas(state).Ambition);
        }

        [Test]
        public void RecognitionPreviewShowsActualClampedChangesAndCannotBeTamperedWith()
        {
            var state = Winner(); state.Fatigue = 5; state.Power = 0;
            Dumas(state).Relationship = 99; Dumas(state).Ambition = 99; Dumas(state).Loyalty = 100;
            var terms = CampaignCore.GetVictoryDecisionTerms(state, "recognize");
            Assert.AreEqual(-5, terms.FatigueDelta); Assert.AreEqual(1, terms.RelationshipDelta); Assert.AreEqual(1, terms.AmbitionDelta);
            terms.FatigueDelta = -100; terms.RelationshipDelta = 90; terms.PowerCost = 90;
            Success(CampaignCore.ResolveVictory(state, state.PendingVictoryId, "recognize"));
            Assert.AreEqual(0, state.Fatigue); Assert.AreEqual(0, state.Power);
            Assert.AreEqual(100, Dumas(state).Relationship); Assert.AreEqual(100, Dumas(state).Ambition);
            CampaignCore.Validate(state);
        }

        [Test]
        public void RecognitionAtExactlyFourPowerPaysTheFullPriceAndAllowsOrdinaryRecovery()
        {
            var state = Winner(); state.Power = 4;
            Success(CampaignCore.ResolveVictory(state, state.PendingVictoryId, "recognize"));
            Assert.AreEqual(0, state.Power);
            Success(CampaignCore.NextWeek(state));
            Assert.AreEqual(.5f, state.Power);
        }

        [Test]
        public void BonusPricesSurvivorsAndRecruitmentChangesTheLiveQuoteWithoutMovingTheOriginalRegion()
        {
            var state = Winner(); state.SelectedRegionId = "brittany";
            var terms = CampaignCore.GetVictoryDecisionTerms(state, "bonus");
            Assert.AreEqual(84, terms.GoldCost); Assert.AreEqual(0, terms.PowerCost);
            Assert.AreEqual(5, terms.LoyaltyDelta); Assert.AreEqual(3, terms.ControlDelta);
            Assert.AreEqual("champagne", terms.RegionId);
            Success(CampaignCore.Act(state, "recruit", "champagne"));
            Assert.AreEqual(1204, state.Troops);
            terms = CampaignCore.GetVictoryDecisionTerms(state, "bonus");
            Assert.AreEqual(101, terms.GoldCost, "Bir haftalık asker maaşı; teçhizat gideri dahil değil.");
            int gold = state.Gold; float power = state.Power, fatigue = state.Fatigue;
            float otherControl = CampaignCore.Region(state, "brittany").Control;
            Success(CampaignCore.ResolveVictory(state, state.PendingVictoryId, "bonus"));
            Assert.AreEqual(gold - 101, state.Gold); Assert.AreEqual(power, state.Power); Assert.AreEqual(fatigue, state.Fatigue);
            Assert.AreEqual(65, Dumas(state).Loyalty); Assert.AreEqual(83, Dumas(state).Ambition);
            Assert.AreEqual(73.5f, CampaignCore.Region(state, "champagne").Control);
            Assert.AreEqual(otherControl, CampaignCore.Region(state, "brittany").Control);
            Reload(state);
        }

        [Test]
        public void BonusCapsAreVisibleAndDeclineKeepsAllExistingResults()
        {
            var state = Winner(); Dumas(state).Loyalty = 98; CampaignCore.Region(state, "champagne").Control = 99.5f;
            var terms = CampaignCore.GetVictoryDecisionTerms(state, "bonus");
            Assert.AreEqual(2, terms.LoyaltyDelta); Assert.AreEqual(.5f, terms.ControlDelta);
            Success(CampaignCore.ResolveVictory(state, state.PendingVictoryId, "bonus"));
            Assert.AreEqual(100, Dumas(state).Loyalty); Assert.AreEqual(100, CampaignCore.Region(state, "champagne").Control);
            state = Winner(); string id = state.PendingVictoryId;
            var expected = JsonUtility.FromJson<CampaignState>(Snapshot(state)); expected.PendingVictoryId = "";
            Success(CampaignCore.ResolveVictory(state, id, "decline"));
            Assert.AreEqual("log.victory.decline", state.Journal[0].Key);
            state.Journal.RemoveAt(0);
            Assert.AreEqual(Snapshot(expected), Snapshot(state), "Ret, eski zaferden başka bir etkiyi değiştirmez.");
        }

        [Test]
        public void EmptyBonusIsAtomicAndEitherRemainingPositiveBenefitKeepsTheBonusAvailable()
        {
            var state = Winner(); Dumas(state).Loyalty = 100; CampaignCore.Region(state, "champagne").Control = 100;
            string id = state.PendingVictoryId;
            Refused(state, () => CampaignCore.CanResolveVictory(state, id, "bonus"), "error.victory.bonus_complete");
            Refused(state, () => CampaignCore.ResolveVictory(state, id, "bonus"), "error.victory.bonus_complete");
            foreach (bool remainingLoyalty in new[] { true, false })
            {
                var oneBenefit = JsonUtility.FromJson<CampaignState>(Snapshot(state));
                if (remainingLoyalty) Dumas(oneBenefit).Loyalty = 99;
                else CampaignCore.Region(oneBenefit, "champagne").Control = 99;
                var terms = CampaignCore.GetVictoryDecisionTerms(oneBenefit, "bonus");
                Assert.AreEqual(remainingLoyalty ? 1 : 0, terms.LoyaltyDelta);
                Assert.AreEqual(remainingLoyalty ? 0 : 1, terms.ControlDelta);
                Success(CampaignCore.CanResolveVictory(oneBenefit, id, "bonus"));
                Success(CampaignCore.ResolveVictory(oneBenefit, id, "bonus"));
                Assert.AreEqual(756, oneBenefit.Gold);
                Assert.AreEqual(100, Dumas(oneBenefit).Loyalty);
                Assert.AreEqual(100, CampaignCore.Region(oneBenefit, "champagne").Control);
            }
            Success(CampaignCore.ResolveVictory(state, id, "decline"));
            Assert.AreEqual(840, state.Gold);
        }

        [TestCase("stale", "error.victory.stale")]
        [TestCase("choice", "error.victory.choice")]
        [TestCase("power", "error.victory.power")]
        [TestCase("gold", "error.victory.gold")]
        public void RefusedChoicesAreAtomicAndLeaveTheOtherAlternativesAvailable(string scenario, string reason)
        {
            var state = Winner(); string id = state.PendingVictoryId, choice = "recognize";
            if (scenario == "stale") id = "battle-0-1-ile-champagne";
            if (scenario == "choice") choice = "impossible";
            if (scenario == "power") state.Power = 3.99f;
            if (scenario == "gold") { state.Gold = 83; choice = "bonus"; }
            Refused(state, () => CampaignCore.CanResolveVictory(state, id, choice), reason);
            Refused(state, () => CampaignCore.ResolveVictory(state, id, choice), reason);
            Assert.IsNotNull(CampaignCore.GetVictoryDecisionTerms(state, "decline"));
            Success(CampaignCore.ResolveVictory(state, state.PendingVictoryId, "decline"));
        }

        [Test]
        public void AnOfferSurvivesRejectedTravelAndExpiresOnlyWhenTheWeekActuallyAdvances()
        {
            var state = Winner();
            Refused(state, () => CampaignCore.March(state, "lorraine"), "error.moves");
            Refused(state, () => CampaignCore.March(state, "unknown"), "error.region");
            string old = state.PendingVictoryId;
            Success(CampaignCore.NextWeek(state));
            Assert.IsFalse(CampaignCore.HasPendingVictory(state));
            Refused(state, () => CampaignCore.ResolveVictory(state, old, "bonus"), "error.victory.none");
            Success(CampaignCore.March(state, "lorraine"));
            Assert.AreEqual("", state.PendingVictoryId); CampaignCore.Validate(state);
        }

        [Test]
        public void VictoryAfterARealOneMoveApproachHasAValidSavableIdentity()
        {
            var state = CampaignCore.Create();
            Success(CampaignCore.March(state, "picardy")); Assert.AreEqual(1, state.Moves);
            Winner(state);
            Assert.AreEqual("battle-0-1-picardy-champagne", state.PendingVictoryId);
            Assert.AreEqual(0, state.Moves);
            Reload(state);
        }

        [Test]
        public void CalendarRefusalPreservesAnOptionalDecisionWithoutAddingAWeekGate()
        {
            var state = CampaignCore.Create(); state.Week = 1000000; state.PetitionResolved = true;
            Winner(state);
            Refused(state, () => CampaignCore.NextWeek(state), "error.week.limit");
            Success(CampaignCore.ResolveVictory(state, state.PendingVictoryId, "decline"));
            Reload(state);
        }

        [Test]
        public void ANewActualVictoryHasANewIdentityAndDefeatNeverLeavesAnOffer()
        {
            var state = Winner(); string old = state.PendingVictoryId;
            Success(CampaignCore.NextWeek(state)); CampaignCore.Region(state, "lorraine").Unrest = 70;
            Winner(state, "lorraine");
            Assert.AreNotEqual(old, state.PendingVictoryId);
            Refused(state, () => CampaignCore.ResolveVictory(state, old, "bonus"), "error.victory.stale");
            Advance(state); CampaignCore.Region(state, "burgundy").Unrest = 70;
            Success(CampaignCore.ResolveBattle(state, "burgundy", BattleId(state, "burgundy"), false, state.Troops, 0));
            Assert.AreEqual(0, state.Troops); Assert.AreEqual("", state.PendingVictoryId);
            CampaignCore.Validate(state);
            Success(CampaignCore.Act(state, "recruit", "lorraine"));
            Assert.AreEqual(200, state.Troops, "Yenilgi yeni bir oyun sonu veya zafer seçim kilidi üretmez.");
        }

        [Test]
        public void PetitionAndDuePromiseKeepPriorityWithoutChangingFourHolidaySettlements()
        {
            var state = CampaignCore.Create("crown");
            Success(CampaignCore.IssueMandate(state, "ile")); string mandate = CampaignCore.MandateId(state.Obligation);
            Success(CampaignCore.GrantRegionalAccord(state, "normandy"));
            Success(CampaignCore.NextWeek(state)); Success(CampaignCore.NextWeek(state));
            Winner(state); string victory = state.PendingVictoryId;
            Refused(state, () => CampaignCore.ResolveVictory(state, victory, "decline"), "error.mandate.petition");
            Refused(state, () => CampaignCore.NextWeek(state), "error.petition.pending");
            state = Reload(state);
            Success(CampaignCore.ChoosePetition(state, "negotiate"));
            Refused(state, () => CampaignCore.ResolveVictory(state, victory, "bonus"), "error.mandate.due");
            Refused(state, () => CampaignCore.NextWeek(state), "error.mandate.due");
            Assert.AreEqual("ile", state.Obligation.RegionId); Assert.AreEqual(150, state.Obligation.GoldDue);
            Assert.AreEqual(2, state.Obligation.DueWeek); Assert.AreEqual("normandy", state.AccordRegionId);
            Assert.AreEqual(4, state.AccordUntilWeek);
            int gold = state.Gold; Success(CampaignCore.ResolveMandate(state, mandate, "fulfil"));
            Assert.AreEqual(gold - 150, state.Gold);
            Success(CampaignCore.ResolveVictory(state, victory, "bonus"));
            Assert.AreEqual(gold - 150 - 84, state.Gold);
            Success(CampaignCore.NextWeek(state)); Assert.IsTrue(CampaignCore.HasRegionalAccord(state));
            int before = state.Gold, net = CampaignCore.Forecast(state).NetGold;
            Success(CampaignCore.NextWeek(state)); Assert.AreEqual(before + net, state.Gold);
            Assert.IsFalse(CampaignCore.HasRegionalAccord(state)); Assert.AreEqual(4, state.AccordUntilWeek);
            Assert.IsNull(state.Obligation); Reload(state);
        }

        [Test]
        public void ShortageLowersLoyaltyAndChangesTheVisiblePriceOfTheNextVictory()
        {
            var state = Winner(); Dumas(state).Loyalty = 84;
            Success(CampaignCore.ResolveVictory(state, state.PendingVictoryId, "bonus"));
            Assert.AreEqual(89, Dumas(state).Loyalty);
            state.Food = 0;
            foreach (var region in state.Regions) region.Unrest = 100;
            Assert.Less(CampaignCore.Forecast(state).NetFood, 0, "Kıtlık fixture'ı gerçekten üretimden fazla tüketmeli.");
            Success(CampaignCore.NextWeek(state));
            Assert.AreEqual(84, Dumas(state).Loyalty);
            CampaignCore.Region(state, "lorraine").Unrest = 70;
            Winner(state, "lorraine");
            Assert.AreEqual(86, Dumas(state).Ambition);
            Assert.AreEqual(4, CampaignCore.GetVictoryDecisionTerms(state, "recognize").PowerCost);
        }

        [TestCase("fresh")]
        [TestCase("open")]
        [TestCase("recognize")]
        [TestCase("bonus")]
        [TestCase("decline")]
        [TestCase("week")]
        public void V4RoundTripPreservesOpenAndClosedDecisionsWithoutAddingOldRewards(string phase)
        {
            var state = phase == "fresh" ? CampaignCore.Create() : Winner();
            if (phase == "week") Success(CampaignCore.NextWeek(state));
            else if (phase != "fresh" && phase != "open") Success(CampaignCore.ResolveVictory(state, state.PendingVictoryId, phase));
            var loaded = Reload(state);
            Assert.AreEqual(phase == "open", CampaignCore.HasPendingVictory(loaded));
            if (phase == "open")
            {
                Success(CampaignCore.ResolveVictory(loaded, loaded.PendingVictoryId, "bonus"));
                Assert.AreEqual(756, loaded.Gold); Reload(loaded);
            }
        }

        [TestCase("missing")]
        [TestCase("null")]
        public void V4RequiresAnExplicitNonNullVictoryField(string representation)
        {
            string json = CampaignArchive.Serialize(CampaignCore.Create(), false);
            StringAssert.Contains("\"PendingVictoryId\":\"\"", json);
            json = json.Replace("\"PendingVictoryId\":\"\"", representation == "missing" ? "\"IgnoredVictory\":\"\"" : "\"PendingVictoryId\":null");
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize(json));
        }

        [TestCase("null")]
        [TestCase("unknown")]
        [TestCase("week")]
        [TestCase("target")]
        [TestCase("troops")]
        [TestCase("moves")]
        [TestCase("last")]
        public void CorruptPendingVictoryIsRejectedByArchiveAndChoicesWithoutMutation(string corruption)
        {
            var state = Winner();
            switch (corruption)
            {
                case "null": state.PendingVictoryId = null; break;
                case "unknown": state.PendingVictoryId = "not-a-battle"; break;
                case "week": state.Week = 1; break;
                case "target": state.ArmyRegionId = "ile"; break;
                case "troops": state.Troops = 0; break;
                case "moves": state.Moves = 1; break;
                case "last": state.ResolvedBattles.Add("battle-0-1-champagne-lorraine"); break;
            }
            Assert.Throws<ArgumentException>(() => CampaignArchive.Serialize(state));
            Refused(state, () => CampaignCore.ResolveVictory(state, state.PendingVictoryId, "decline"), "error.victory.state");
            Assert.IsFalse(CampaignCore.HasPendingVictory(state)); Assert.IsNull(CampaignCore.GetVictoryDecisionTerms(state, "bonus"));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void OlderArchivesDoNotInventChoicesForHistoricWinsOrAcceptHiddenActiveChoices(int version)
        {
            var state = Winner(); string json = CampaignArchive.Serialize(state, false);
            string disguised = json.Replace("\"Version\":4", "\"Version\":" + version);
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize(disguised));
            var loaded = CampaignArchive.Deserialize(AsOlder(json, version));
            state.PendingVictoryId = "";
            Assert.AreEqual(Snapshot(state), Snapshot(loaded));
            Assert.IsFalse(CampaignCore.HasPendingVictory(loaded));
            Reload(loaded);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ActualV3MigrationPreservesActiveHolidayOrBrokenFutureCooldownAndOpenRolePromise(bool broken)
        {
            var state = CampaignCore.Create("crown");
            Success(CampaignCore.IssueMandate(state, "ile"));
            Success(CampaignCore.GrantRegionalAccord(state, "normandy"));
            if (broken) Success(CampaignCore.Act(state, "tax", "normandy"));
            string v3 = AsOlder(CampaignArchive.Serialize(state, false), 3);
            StringAssert.Contains("\"Version\":3", v3); StringAssert.DoesNotContain("\"PendingVictoryId\":", v3);
            var loaded = CampaignArchive.Deserialize(v3);
            Assert.AreEqual(Snapshot(state), Snapshot(loaded));
            Assert.AreEqual(broken ? "" : "normandy", loaded.AccordRegionId); Assert.AreEqual(4, loaded.AccordUntilWeek);
            Assert.AreEqual(150, loaded.Obligation.GoldDue); Assert.AreEqual("ile", loaded.Obligation.RegionId);
            Assert.AreEqual("", loaded.PendingVictoryId); Reload(loaded);
        }

        [Test]
        public void ActualVictoryMessagesFormatInBothLanguagesWithoutWritingPreferences()
        {
            var messages = new List<ActionResult>();
            foreach (string choice in new[] { "recognize", "bonus", "decline" })
            {
                var state = Winner(); messages.Add(CampaignCore.CanResolveVictory(state, state.PendingVictoryId, choice));
                messages.Add(CampaignCore.ResolveVictory(state, "old", choice));
                messages.Add(CampaignCore.ResolveVictory(state, state.PendingVictoryId, "unknown"));
                messages.Add(CampaignCore.ResolveVictory(state, state.PendingVictoryId, choice));
                messages.Add(CampaignCore.ResolveVictory(state, "old", choice));
            }
            var invalid = Winner(); invalid.Power = 0;
            messages.Add(CampaignCore.ResolveVictory(invalid, invalid.PendingVictoryId, "recognize"));
            invalid.Gold = 0; messages.Add(CampaignCore.ResolveVictory(invalid, invalid.PendingVictoryId, "bonus"));
            Dumas(invalid).Loyalty = 100; CampaignCore.Region(invalid, "champagne").Control = 100;
            messages.Add(CampaignCore.ResolveVictory(invalid, invalid.PendingVictoryId, "bonus"));
            invalid.PendingVictoryId = null; messages.Add(CampaignCore.ResolveVictory(invalid, "old", "decline"));
            var keys = new HashSet<string>(); foreach (var message in messages) keys.Add(message.Key);
            var table = JsonUtility.FromJson<L.Table>(Resources.Load<TextAsset>("Localization/victory-core").text);
            foreach (var entry in table.entries) Assert.IsTrue(keys.Contains(entry.key), entry.key);
            string profile = Environment.GetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE"), language = L.Language;
            bool hadPreference = PlayerPrefs.HasKey("language"); string preference = PlayerPrefs.GetString("language", "");
            try
            {
                Environment.SetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE", "victory-localization-check"); L.Initialize();
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
