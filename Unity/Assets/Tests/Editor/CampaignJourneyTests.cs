#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    // Gerçek karar zincirini sürdürür; savaş raporları burada kontrollü çekirdek girdileridir.
    public sealed class CampaignJourneyTests
    {
        private const string FirstBattle = "battle-0-2-ile-champagne";
        private const string SecondBattle = "battle-1-2-champagne-lorraine";

        private static string Snapshot(CampaignState state) { return JsonUtility.ToJson(state); }

        private static CampaignState Reload(CampaignState state)
        {
            string saved = Snapshot(state);
            var loaded = JsonUtility.FromJson<CampaignState>(saved);
            CampaignCore.Validate(loaded);
            Assert.AreEqual(saved, Snapshot(loaded), "Kayıt bütün sefer durumunu korumalı.");
            return loaded;
        }

        private static CampaignState Advance(CampaignState state, int expectedWeek)
        {
            Assert.IsTrue(CampaignCore.NextWeek(state).Ok, "Hafta ilerleyebilmeli.");
            Assert.AreEqual(expectedWeek, state.Week);
            return Reload(state);
        }

        private static void RejectReplay(CampaignState state, string target, string id, bool won, int casualties, float morale)
        {
            string before = Snapshot(state);
            ActionResult replay = CampaignCore.ResolveBattle(state, target, id, won, casualties, morale);
            Assert.IsFalse(replay.Ok);
            Assert.AreEqual("error.battle.duplicate", replay.Key);
            Assert.AreEqual(before, Snapshot(state), "Eski rapor kaynakları, siyaseti, hareketi ve günlüğü değiştirmemeli.");
        }

        private static CampaignState SixWeekJourney()
        {
            var state = CampaignCore.Create();
            // İki haftalık vergi baskısı Lorraine'de ikinci, ayrı bir cephe doğurur.
            Assert.IsTrue(CampaignCore.Act(state, "tax", "lorraine").Ok);
            Assert.IsTrue(CampaignCore.ResolveBattle(state, "champagne", FirstBattle, true, 90, 62).Ok);
            Assert.AreEqual("champagne", state.ArmyRegionId);
            Assert.AreEqual(1110, state.Troops);
            state = Reload(state);
            RejectReplay(state, "champagne", FirstBattle, true, 90, 62);

            state = Advance(state, 1);
            Assert.IsTrue(CampaignCore.Act(state, "tax", "lorraine").Ok);
            Assert.IsTrue(CampaignCore.CanMarch(state, "lorraine").RequiresBattle);
            Assert.IsTrue(CampaignCore.ResolveBattle(state, "lorraine", SecondBattle, false, 110, 45).Ok);
            Assert.AreEqual("champagne", state.ArmyRegionId, "Yenilgi ordunun kökenini korumalı.");
            Assert.AreEqual(1000, state.Troops);
            state = Reload(state);
            CollectionAssert.AreEqual(new[] { FirstBattle, SecondBattle }, state.ResolvedBattles);

            state = Advance(state, 2);
            Assert.IsTrue(state.PendingPetition);
            string waiting = Snapshot(state);
            for (int attempt = 0; attempt < 3; attempt++)
            {
                Assert.IsFalse(CampaignCore.NextWeek(state).Ok);
                Assert.AreEqual(waiting, Snapshot(state), "Bekleyen dilekçe tekrar tıklamalarda ekonomik dönemi işletmemeli.");
            }
            state = Reload(state);
            Assert.IsTrue(CampaignCore.ChoosePetition(state, "negotiate").Ok);
            state = Advance(state, 3);
            Assert.IsTrue(CampaignCore.March(state, "burgundy").Ok);
            Assert.IsTrue(CampaignCore.Act(state, "recruit", "burgundy").Ok);
            Assert.AreEqual(1200, state.Troops, "İki savaşın ardından yeni askerler orduya katılmalı.");

            state = Advance(state, 4);
            Assert.IsTrue(CampaignCore.March(state, "orleans").Ok);
            Assert.IsTrue(CampaignCore.Act(state, "subsidy", "ile").Ok);
            state = Advance(state, 5);
            Assert.IsTrue(CampaignCore.Act(state, "bread", "orleans").Ok);
            state = Advance(state, 6);
            Assert.AreEqual("orleans", state.ArmyRegionId);
            Assert.IsTrue(state.PetitionResolved);
            return state;
        }

        [Test]
        public void SixWeeksKeepTwoDifferentBattleResultsAcrossLoadsAndRejectBothReplaysAtomically()
        {
            var state = SixWeekJourney();
            CollectionAssert.AreEqual(new[] { FirstBattle, SecondBattle }, state.ResolvedBattles);
            Assert.AreEqual(6, state.Week);
            Assert.IsFalse(state.PendingPetition);
            // Sonraki kayıt yüklemesinde rapor içeriğini değiştirmek de kimliğin tekrarını geçerli kılmaz.
            state = Reload(state);
            RejectReplay(state, "champagne", FirstBattle, false, 50, 20);
            RejectReplay(state, "lorraine", SecondBattle, true, 50, 90);
            CampaignCore.Validate(state);
        }

        [Test]
        public void VeteranHungryMarchMatchesPreviewAndResuppliedCampaignRecoversWithoutLosingBattleHistory()
        {
            var state = SixWeekJourney();
            Assert.IsTrue(CampaignCore.Act(state, "subsidy", "ile").Ok);
            Assert.IsFalse(state.SubsidyParis);
            // İki cepheden dönmüş seferin tükenen deposu; harita veya hareket kuralı değiştirilmez.
            state.Food = 0;
            state.MilitarySupplies = 2;
            state = Reload(state);
            string before = Snapshot(state);
            int troops = state.Troops;
            var preview = CampaignCore.PreviewMarch(state, "normandy");
            Assert.IsNotNull(preview);
            Assert.IsTrue(preview.Hungry);
            Assert.IsFalse(CampaignCore.CanMarch(state, "normandy").RequiresBattle);
            Assert.AreEqual(before, Snapshot(state), "Hazırlık önizlemesi kaynak tüketmemeli.");

            Assert.IsTrue(CampaignCore.March(state, "normandy").Ok);
            Assert.AreEqual("normandy", state.ArmyRegionId);
            Assert.AreEqual(preview.FoodAfter, state.Food);
            Assert.AreEqual(preview.MilitarySuppliesAfter, state.MilitarySupplies);
            Assert.AreEqual(preview.MovesAfter, state.Moves);
            Assert.AreEqual(preview.Supply, state.Supply);
            Assert.AreEqual(preview.Fatigue, state.Fatigue);
            Assert.AreEqual(preview.Morale, state.Morale);
            Assert.Less(state.Troops, troops, "Aç yürüyüşteki kayıp gerçek orduya uygulanmalı.");
            Assert.IsTrue(state.Journal.Exists(entry => entry.Key == "log.march.attrition"));
            state = Reload(state);

            // Subvansiyonu kapatmak ülkenin net tahıl açığını tek başına gidermez.
            // Toparlanma kısmının açık başlangıç koşulu: depoda yeniden yiyecek vardır.
            state.Food = 60;
            state = Reload(state);
            var recoveryForecast = CampaignCore.Forecast(state);
            Assert.Greater(state.Food + recoveryForecast.NetFood, 0, "Toparlanma koşulu haftanın gıda ihtiyacını karşılamalı.");
            Assert.GreaterOrEqual(state.Gold + recoveryForecast.NetGold, 0, "Toparlanma koşulu ordunun ücretini ödeyebilmeli.");
            float morale = state.Morale, supply = state.Supply;
            state = Advance(state, 7);
            Assert.Greater(state.Food, 0);
            Assert.Greater(state.MilitarySupplies, 0);
            Assert.Greater(state.Morale, morale);
            Assert.Greater(state.Supply, supply);
            CollectionAssert.AreEqual(new[] { FirstBattle, SecondBattle }, state.ResolvedBattles);
            RejectReplay(state, "champagne", FirstBattle, true, 90, 62);
            RejectReplay(state, "lorraine", SecondBattle, false, 110, 45);
        }
    }
}
#endif
