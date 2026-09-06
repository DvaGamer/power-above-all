#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    public sealed class RegionalAccordTests
    {
        static string Snapshot(CampaignState state) => JsonUtility.ToJson(state);
        static FactionState Assembly(CampaignState state) => state.Factions.Find(item => item.Id == "assembly");
        static CharacterState Morel(CampaignState state) => state.Characters.Find(item => item.Id == "morel");
        static void Success(ActionResult result) { Assert.IsTrue(result.Ok, result.Key); }
        static CampaignState Reload(CampaignState state)
        {
            string before = Snapshot(state);
            string json = CampaignArchive.Serialize(state, false);
            StringAssert.Contains("\"Version\":4", json);
            var loaded = CampaignArchive.Deserialize(json);
            Assert.AreEqual(before, Snapshot(loaded));
            return loaded;
        }
        static void Refused(CampaignState state, Func<ActionResult> action, string reason)
        {
            string before = Snapshot(state); var result = action();
            Assert.IsFalse(result.Ok); Assert.AreEqual(reason, result.Key);
            Assert.AreEqual(before, Snapshot(state), "Ret, günlük ve anlaşma tarihi dahil atomik olmalı.");
        }
        static void Advance(CampaignState state)
        {
            Success(CampaignCore.NextWeek(state));
            if (state.PendingPetition) Success(CampaignCore.ChoosePetition(state, "relief"));
        }

        [TestCase("legacy")]
        [TestCase("crown")]
        [TestCase("assembly")]
        [TestCase("army")]
        public void VoluntaryHolidayOpensChampagneWithoutGivingFreeMarchOrChangingRole(string role)
        {
            var state = CampaignCore.Create(role);
            Assert.IsTrue(CampaignCore.CanMarch(state, "champagne").RequiresBattle);
            string before = Snapshot(state);
            var terms = CampaignCore.GetRegionalAccordTerms(state, "champagne");
            Success(CampaignCore.CanGrantRegionalAccord(state, "champagne"));
            Assert.AreEqual(before, Snapshot(state));
            Assert.AreEqual(4, terms.UntilWeek); Assert.AreEqual(4, terms.RemainingWeeks); Assert.IsFalse(terms.IsActive);
            Assert.AreEqual(-10, terms.Immediate.Unrest); Assert.AreEqual(3, terms.Immediate.Control);
            Assert.AreEqual(5, terms.Fulfil.Approval); Assert.AreEqual(4, terms.Fulfil.Relationship);
            Success(CampaignCore.GrantRegionalAccord(state, "champagne"));
            Assert.AreEqual(59, CampaignCore.Region(state, "champagne").Unrest);
            Assert.AreEqual(63.5f, CampaignCore.Region(state, "champagne").Control);
            Assert.AreEqual(terms.ProjectedTaxIncome, CampaignCore.Forecast(state).TaxIncome);
            Assert.IsFalse(CampaignCore.CanMarch(state, "champagne").RequiresBattle);
            var march = CampaignCore.PreviewMarch(state, "champagne");
            Assert.IsTrue(march.Difficult); Assert.AreEqual(18, march.FoodCost); Assert.AreEqual(0, march.MovesAfter);
            Assert.AreEqual(840, state.Gold); Assert.AreEqual(360, state.Food); Assert.AreEqual(55, state.Power);
            Assert.AreEqual(45, Assembly(state).Approval); Assert.AreEqual(50, Morel(state).Relationship);
            Assert.AreEqual(role, state.RoleId); Assert.IsNull(state.Obligation);
            Success(CampaignCore.March(state, "champagne"));
            Assert.AreEqual(1200, state.Troops); Assert.AreEqual(342, state.Food); Assert.AreEqual("champagne", state.ArmyRegionId);
            Reload(state);
        }

        [Test]
        public void TaxPreviewUsesTheSameWholeForecastAndUpdatesWithLocalDecisions()
        {
            var state = CampaignCore.Create();
            var terms = CampaignCore.GetRegionalAccordTerms(state, "champagne");
            var counterpart = Reload(state);
            var region = CampaignCore.Region(counterpart, "champagne");
            region.Unrest -= 10; region.Control += 3;
            int fullTaxOnCalmerRegion = CampaignCore.Forecast(counterpart).TaxIncome;
            Assert.AreEqual(CampaignCore.Forecast(state).TaxIncome, terms.CurrentTaxIncome);
            Assert.AreEqual(fullTaxOnCalmerRegion - terms.ProjectedTaxIncome, terms.TaxForgone);
            Assert.Greater(terms.TaxForgone, 0);
            Success(CampaignCore.GrantRegionalAccord(state, "champagne"));
            Assert.AreEqual(terms.ProjectedTaxIncome, CampaignCore.Forecast(state).TaxIncome);
            state.SelectedRegionId = "brittany";
            var active = CampaignCore.GetActiveRegionalAccordTerms(state);
            Assert.IsTrue(active.IsActive); Assert.AreEqual("champagne", active.RegionId);
            Assert.AreEqual(active.CurrentTaxIncome, active.ProjectedTaxIncome);
            Assert.AreEqual(terms.TaxForgone, active.TaxForgone);
            Assert.IsNull(CampaignCore.GetRegionalAccordTerms(state, "brittany"));
            Success(CampaignCore.Act(state, "bread", "champagne"));
            var changed = CampaignCore.GetActiveRegionalAccordTerms(state);
            Assert.Greater(changed.TaxForgone, active.TaxForgone, "Kaybedilen katkı ilk günün sabit fiyatı değildir.");
            var withoutHoliday = Reload(state); withoutHoliday.AccordRegionId = "";
            Assert.AreEqual(CampaignCore.Forecast(withoutHoliday).TaxIncome - CampaignCore.Forecast(state).TaxIncome, changed.TaxForgone);
        }

        [Test]
        public void ExactlyFourActualTaxCalculationsPassBeforeOneRewardAndNoArrears()
        {
            var state = CampaignCore.Create();
            Success(CampaignCore.GrantRegionalAccord(state, "champagne"));
            for (int week = 1; week <= 4; week++)
            {
                int gold = state.Gold; var forecast = CampaignCore.Forecast(state);
                float approval = Assembly(state).Approval;
                Assert.AreEqual(5 - week, CampaignCore.GetActiveRegionalAccordTerms(state).RemainingWeeks);
                Success(CampaignCore.NextWeek(state));
                Assert.AreEqual(gold + forecast.NetGold, state.Gold, "Son hesap da tatili uygular; geriye dönük borç yok.");
                Assert.AreEqual(week < 4, CampaignCore.HasRegionalAccord(state));
                Assert.AreEqual(week == 4 ? 54 : 50, Morel(state).Relationship);
                Assert.AreEqual(approval + (week == 4 ? 5 : 0), Assembly(state).Approval);
                if (state.PendingPetition) Success(CampaignCore.ChoosePetition(state, "relief"));
                state = Reload(state);
            }
            Assert.AreEqual("", state.AccordRegionId); Assert.AreEqual(4, state.AccordUntilWeek);
            Assert.IsNull(CampaignCore.GetActiveRegionalAccordTerms(state));
            int beforeGold = state.Gold; var normalForecast = CampaignCore.Forecast(state);
            Advance(state);
            Assert.AreEqual(beforeGold + normalForecast.NetGold, state.Gold);
            Assert.AreEqual(54, Morel(state).Relationship);
            Assert.AreEqual(1, state.Journal.FindAll(entry => entry.Key == "log.accord.completed").Count);
            Success(CampaignCore.CanGrantRegionalAccord(state, "normandy"));
        }

        [Test]
        public void EarlyTaxAppliesBothPoliticalCostsAndRetainsOriginalCooldown()
        {
            var state = CampaignCore.Create();
            Success(CampaignCore.GrantRegionalAccord(state, "champagne"));
            Assert.IsTrue(CampaignCore.TaxBreaksRegionalAccord(state, "champagne"));
            Assert.IsFalse(CampaignCore.TaxBreaksRegionalAccord(state, "normandy"));
            Success(CampaignCore.Act(state, "tax", "champagne"));
            Assert.AreEqual(940, state.Gold); Assert.AreEqual(51, state.Power);
            var region = CampaignCore.Region(state, "champagne");
            Assert.AreEqual(81, region.Unrest); Assert.AreEqual(60.5f, region.Control); Assert.AreEqual(56, region.EliteLoyalty);
            Assert.AreEqual(35, Assembly(state).Approval); Assert.AreEqual(40, Morel(state).Relationship);
            Assert.IsFalse(CampaignCore.HasRegionalAccord(state)); Assert.AreEqual(4, state.AccordUntilWeek);
            Assert.IsTrue(CampaignCore.CanMarch(state, "champagne").RequiresBattle);
            Refused(state, () => CampaignCore.Act(state, "tax", "champagne"), "error.used");
            Refused(state, () => CampaignCore.GrantRegionalAccord(state, "normandy"), "error.accord.cooldown");
            state = Reload(state);
            for (int week = 0; week < 4; week++) Advance(state);
            Assert.AreEqual(40, Morel(state).Relationship, "Bozulan anlaşma süresi dolunca ödül vermez.");
            Assert.AreEqual(0, state.Journal.FindAll(entry => entry.Key == "log.accord.completed").Count);
            Success(CampaignCore.GrantRegionalAccord(state, "normandy"));
        }

        [TestCase("used")]
        [TestCase("capacity")]
        public void RefusedExtraordinaryTaxCannotBreakAnExistingHoliday(string guard)
        {
            var state = CampaignCore.Create();
            if (guard == "used") Success(CampaignCore.Act(state, "tax", "champagne"));
            else state.Gold = 100000000;
            Success(CampaignCore.GrantRegionalAccord(state, "champagne"));
            Refused(state, () => CampaignCore.Act(state, "tax", "champagne"), guard == "used" ? "error.used" : "error.capacity");
            Assert.IsTrue(CampaignCore.HasRegionalAccord(state));
            Reload(state);
        }

        [Test]
        public void OtherRegionsAndRecruitmentDoNotCancelTheOriginalRegionalHoliday()
        {
            var state = CampaignCore.Create("army");
            Success(CampaignCore.GrantRegionalAccord(state, "champagne"));
            Success(CampaignCore.Act(state, "tax", "normandy"));
            Success(CampaignCore.Act(state, "recruit", "ile"));
            Assert.AreEqual("champagne", CampaignCore.GetActiveRegionalAccordTerms(state).RegionId);
            Assert.AreEqual(50, Morel(state).Relationship);
            Refused(state, () => CampaignCore.GrantRegionalAccord(state, "normandy"), "error.accord.active");
        }

        [Test]
        public void RolePromiseKeepsItsOwnRegionPriceAndPriorityWhileHolidayCountsOnlyAdvancedWeeks()
        {
            var state = CampaignCore.Create("crown");
            Success(CampaignCore.IssueMandate(state, "ile"));
            string promise = JsonUtility.ToJson(state.Obligation);
            Success(CampaignCore.GrantRegionalAccord(state, "champagne"));
            Assert.AreEqual(promise, JsonUtility.ToJson(state.Obligation));
            state = Reload(state);
            Success(CampaignCore.NextWeek(state)); Success(CampaignCore.NextWeek(state));
            Refused(state, () => CampaignCore.NextWeek(state), "error.petition.pending");
            Refused(state, () => CampaignCore.GrantRegionalAccord(state, "normandy"), "error.mandate.petition");
            Success(CampaignCore.ChoosePetition(state, "relief"));
            Refused(state, () => CampaignCore.NextWeek(state), "error.mandate.due");
            Refused(state, () => CampaignCore.GrantRegionalAccord(state, "normandy"), "error.mandate.due");
            Assert.AreEqual(2, CampaignCore.GetActiveRegionalAccordTerms(state).RemainingWeeks);
            Assert.AreEqual(promise, JsonUtility.ToJson(state.Obligation));
            int gold = state.Gold;
            Success(CampaignCore.ResolveMandate(state, CampaignCore.MandateId(state.Obligation), "fulfil"));
            Assert.AreEqual(gold - 150, state.Gold);
            Advance(state); Advance(state);
            Assert.IsFalse(CampaignCore.HasRegionalAccord(state));
        }

        [Test]
        public void NoTroopsFoodGoldOrPowerDoesNotCreateANewCouncilProgressionLock()
        {
            var state = CampaignCore.Create();
            state.Gold = state.Food = state.Troops = state.MilitarySupplies = state.Manpower = 0;
            state.Power = 0;
            Success(CampaignCore.GrantRegionalAccord(state, "champagne"));
            Assert.AreEqual(0, state.Power); Assert.AreEqual(0, state.Gold); Assert.AreEqual(0, state.Troops);
            Success(CampaignCore.NextWeek(state)); CampaignCore.Validate(state);
            Reload(state);
        }

        [Test]
        public void LocalAndPoliticalEffectsRemainBoundedInsteadOfRestoringAnOldRegionSnapshot()
        {
            var state = CampaignCore.Create(); var region = CampaignCore.Region(state, "champagne");
            region.Unrest = 3; region.Control = 99; Morel(state).Relationship = 2; Assembly(state).Approval = 2; state.Power = 2;
            Success(CampaignCore.GrantRegionalAccord(state, "champagne"));
            Assert.AreEqual(0, region.Unrest); Assert.AreEqual(100, region.Control);
            Success(CampaignCore.Act(state, "tax", "champagne"));
            Assert.AreEqual(22, region.Unrest); Assert.AreEqual(97, region.Control);
            Assert.AreEqual(0, Morel(state).Relationship); Assert.AreEqual(0, Assembly(state).Approval); Assert.AreEqual(0, state.Power);
            CampaignCore.Validate(state);
        }

        [TestCase("fresh")]
        [TestCase("active")]
        [TestCase("cooldown")]
        [TestCase("complete")]
        public void CurrentRoundTripPreservesActiveAndEmptyFutureCooldownStates(string stage)
        {
            var state = CampaignCore.Create("assembly");
            if (stage != "fresh") Success(CampaignCore.GrantRegionalAccord(state, "champagne"));
            if (stage == "cooldown") Success(CampaignCore.Act(state, "tax", "champagne"));
            if (stage == "complete") for (int i = 0; i < 4; i++) Advance(state);
            var loaded = Reload(state);
            Assert.AreEqual(CampaignCore.HasRegionalAccord(state), CampaignCore.HasRegionalAccord(loaded));
            if (stage == "cooldown")
            {
                Assert.AreEqual("", loaded.AccordRegionId); Assert.Greater(loaded.AccordUntilWeek, loaded.Week);
                Refused(loaded, () => CampaignCore.GrantRegionalAccord(loaded, "normandy"), "error.accord.cooldown");
            }
        }

        [TestCase(3, "missing_region")]
        [TestCase(3, "null_region")]
        [TestCase(3, "missing_until")]
        [TestCase(3, "null_until")]
        [TestCase(3, "text_until")]
        [TestCase(4, "missing_region")]
        [TestCase(4, "null_region")]
        [TestCase(4, "missing_until")]
        [TestCase(4, "null_until")]
        [TestCase(4, "text_until")]
        public void V3AndCurrentRequireBothExplicitTypedAccordFields(int version, string corruption)
        {
            string json = CampaignArchive.Serialize(CampaignCore.Create(), false);
            json = json.Replace("\"Version\":4", "\"Version\":" + version);
            switch (corruption)
            {
                case "missing_region": json = json.Replace("\"AccordRegionId\":", "\"IgnoredRegion\":"); break;
                case "null_region": json = json.Replace("\"AccordRegionId\":\"\"", "\"AccordRegionId\":null"); break;
                case "missing_until": json = json.Replace("\"AccordUntilWeek\":", "\"IgnoredUntil\":"); break;
                case "null_until": json = json.Replace("\"AccordUntilWeek\":0", "\"AccordUntilWeek\":null"); break;
                case "text_until": json = json.Replace("\"AccordUntilWeek\":0", "\"AccordUntilWeek\":\"not-a-week\""); break;
            }
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize(json));
        }

        [TestCase("region")]
        [TestCase("negative")]
        [TestCase("short")]
        [TestCase("distant")]
        [TestCase("elapsed")]
        [TestCase("null")]
        public void InvalidAccordStateIsRejectedByArchiveAndGrantWithoutMutation(string corruption)
        {
            var state = CampaignCore.Create();
            Success(CampaignCore.GrantRegionalAccord(state, "champagne"));
            switch (corruption)
            {
                case "region": state.AccordRegionId = "unknown"; break;
                case "negative": state.AccordUntilWeek = -1; break;
                case "short": state.AccordUntilWeek = 3; break;
                case "distant": state.AccordUntilWeek = 5; break;
                case "elapsed": state.Week = 4; state.PetitionResolved = true; break;
                case "null": state.AccordRegionId = null; break;
            }
            Assert.Throws<ArgumentException>(() => CampaignArchive.Serialize(state));
            Refused(state, () => CampaignCore.GrantRegionalAccord(state, "normandy"), "error.accord.state");
            Assert.IsNull(CampaignCore.GetRegionalAccordTerms(state, "normandy"));
        }

        static string WithoutAccordFields(string json)
        { return json.Replace("\"AccordRegionId\":", "\"IgnoredRegion\":").Replace("\"AccordUntilWeek\":", "\"IgnoredUntil\":"); }

        [Test]
        public void ActualV1AndV2WithoutNewFieldsMigrateExplicitlyWithoutLosingExistingPromises()
        {
            var legacy = CampaignCore.Create();
            string v1 = WithoutAccordFields(CampaignArchive.Serialize(legacy, false))
                .Replace("\"Version\":4", "\"Version\":1").Replace("\"RoleId\":", "\"IgnoredRole\":")
                .Replace("\"PendingVictoryId\":", "\"IgnoredVictory\":")
                .Replace("\"NextMandateWeek\":", "\"IgnoredNext\":").Replace("\"Mandates\":", "\"IgnoredMandates\":");
            Assert.AreEqual(Snapshot(legacy), Snapshot(CampaignArchive.Deserialize(v1)));
            var currentRole = CampaignCore.Create("crown");
            Success(CampaignCore.IssueMandate(currentRole, "ile"));
            string v2 = WithoutAccordFields(CampaignArchive.Serialize(currentRole, false)).Replace("\"Version\":4", "\"Version\":2")
                .Replace("\"PendingVictoryId\":", "\"IgnoredVictory\":");
            StringAssert.DoesNotContain("\"AccordRegionId\":", v2);
            StringAssert.DoesNotContain("\"AccordUntilWeek\":", v2);
            var loaded = CampaignArchive.Deserialize(v2);
            Assert.AreEqual(Snapshot(currentRole), Snapshot(loaded));
            Assert.AreEqual(150, loaded.Obligation.GoldDue); Assert.AreEqual("crown", loaded.RoleId);
            Assert.AreEqual("", loaded.AccordRegionId); Assert.AreEqual(0, loaded.AccordUntilWeek);
            Reload(loaded);
        }

        [TestCase(1, false)]
        [TestCase(1, true)]
        [TestCase(2, false)]
        [TestCase(2, true)]
        public void OlderVersionCannotSilentlyEraseActiveHolidayOrFutureCooldown(int version, bool broken)
        {
            var state = CampaignCore.Create();
            Success(CampaignCore.GrantRegionalAccord(state, "champagne"));
            if (broken) Success(CampaignCore.Act(state, "tax", "champagne"));
            string downgraded = CampaignArchive.Serialize(state, false).Replace("\"Version\":4", "\"Version\":" + version);
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize(downgraded));
        }

        [Test]
        public void CalendarLimitRejectsOnlyNewTooLateAgreements()
        {
            var state = CampaignCore.Create(); state.Week = 999997; state.PetitionResolved = true;
            Refused(state, () => CampaignCore.GrantRegionalAccord(state, "champagne"), "error.accord.calendar");
            Assert.IsNull(CampaignCore.GetRegionalAccordTerms(state, "champagne"));
            state.Week = 999996; Success(CampaignCore.GrantRegionalAccord(state, "champagne"));
            for (int i = 0; i < 4; i++) Success(CampaignCore.NextWeek(state));
            Assert.AreEqual(1000000, state.Week); Assert.IsFalse(CampaignCore.HasRegionalAccord(state)); Reload(state);
        }
    }
}
#endif
