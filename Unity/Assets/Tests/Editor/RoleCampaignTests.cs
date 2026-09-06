#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    // Kurgusal rol sözleşmelerini gerçek sefer kararları ve arşiv sınırında doğrular.
    public sealed class RoleCampaignTests
    {
        static string Snapshot(CampaignState state) => JsonUtility.ToJson(state);
        static FactionState Faction(CampaignState state, string id) => state.Factions.Find(item => item.Id == id);
        static CharacterState Person(CampaignState state, string id) => state.Characters.Find(item => item.Id == id);
        static string Id(CampaignState state) => CampaignCore.MandateId(state.Obligation);

        static void Succeeds(ActionResult result)
        { Assert.IsTrue(result.Ok, result.Key); }

        static void Refuses(CampaignState state, Func<ActionResult> action, string key)
        {
            string before = Snapshot(state);
            ActionResult result = action();
            Assert.IsFalse(result.Ok, key);
            Assert.AreEqual(key, result.Key);
            Assert.AreEqual(before, Snapshot(state), "Ret; kaynakları, bölgeyi, yükümlülüğü veya günlüğü değiştirmemeli.");
        }

        static CampaignState Reload(CampaignState state)
        {
            string before = Snapshot(state);
            string archive = CampaignArchive.Serialize(state, false);
            StringAssert.Contains("\"Version\":8", archive);
            CampaignState loaded = CampaignArchive.Deserialize(archive);
            Assert.AreEqual(before, Snapshot(loaded), "Kayıt bütün sefer durumunu korumalı.");
            return loaded;
        }

        static void Advance(CampaignState state, int week)
        { Succeeds(CampaignCore.NextWeek(state)); Assert.AreEqual(week, state.Week); }

        [Test]
        public void CrownAdvanceFundsRecruitmentAndEarlyRepaymentPreservesFourWeekCooldown()
        {
            var state = Reload(CampaignCore.Create("crown"));
            string before = Snapshot(state);
            MandateTerms preview = CampaignCore.GetMandateTerms(state, "normandy");
            Succeeds(CampaignCore.CanIssueMandate(state, "normandy"));
            Assert.AreEqual(before, Snapshot(state), "Belgeyi okumak veya düğmeyi kontrol etmek avansı vermemeli.");
            Assert.AreEqual(120, preview.Immediate.Gold);
            Assert.AreEqual(-150, preview.Fulfil.Gold);
            Assert.AreEqual(2, preview.DueWeek);
            Succeeds(CampaignCore.IssueMandate(state, "normandy"));
            Assert.AreEqual(960, state.Gold);
            Assert.AreEqual("ile", state.Obligation.RegionId, "Kraliyet avansı seçili eyaletten bağımsız saraya aittir.");
            Assert.AreEqual(62, Faction(state, "crown").Approval);
            string firstId = Id(state);
            Refuses(state, () => CampaignCore.IssueMandate(state, "ile"), "error.mandate.open");

            Succeeds(CampaignCore.Act(state, "recruit", "ile"));
            Assert.AreEqual(1400, state.Troops);
            state = Reload(state);
            Succeeds(CampaignCore.ResolveMandate(state, firstId, "fulfil"));
            Assert.AreEqual(0, state.Week, "Erken ödeme için haftayı tüketmek gerekmez.");
            Assert.AreEqual(690, state.Gold);
            Assert.AreEqual(67, Faction(state, "crown").Approval);
            Assert.AreEqual(54, Person(state, "valcourt").Relationship);
            Assert.IsNull(state.Obligation);
            Assert.AreEqual(4, state.NextMandateWeek);
            state = Reload(state);
            Refuses(state, () => CampaignCore.ResolveMandate(state, firstId, "fulfil"), "error.mandate.none");
            Refuses(state, () => CampaignCore.IssueMandate(state, "ile"), "error.mandate.cooldown");
            Advance(state, 1);
            Refuses(state, () => CampaignCore.IssueMandate(state, "ile"), "error.mandate.cooldown");
            Advance(state, 2);
            Succeeds(CampaignCore.ChoosePetition(state, "negotiate"));
            Advance(state, 3);
            Refuses(state, () => CampaignCore.IssueMandate(state, "ile"), "error.mandate.cooldown");
            Advance(state, 4);
            Succeeds(CampaignCore.IssueMandate(state, "ile"));
            Assert.AreNotEqual(firstId, Id(state));
            state = Reload(state);
            Refuses(state, () => CampaignCore.ResolveMandate(state, firstId, "break"), "error.mandate.stale");
            Succeeds(CampaignCore.ResolveMandate(state, Id(state), "fulfil"));
        }

        [Test]
        public void AssemblyPromiseOpensChampagneWithoutBattleAndBreakingItRestoresResistanceAfterTravel()
        {
            var state = CampaignCore.Create("assembly");
            Assert.IsTrue(CampaignCore.CanMarch(state, "champagne").RequiresBattle);
            Succeeds(CampaignCore.IssueMandate(state, "champagne"));
            Assert.AreEqual(51, CampaignCore.Region(state, "champagne").Unrest);
            Assert.AreEqual(66.5f, CampaignCore.Region(state, "champagne").Control);
            Assert.AreEqual(360, state.Food, "Söz, tahılı teslim etmeden yolu açar.");
            Assert.IsFalse(CampaignCore.CanMarch(state, "champagne").RequiresBattle);
            state = Reload(state);
            Succeeds(CampaignCore.March(state, "champagne"));
            Assert.AreEqual("champagne", state.ArmyRegionId);
            Assert.AreEqual(0, state.ResolvedBattles.Count);
            Advance(state, 1);
            Succeeds(CampaignCore.March(state, "ile"));
            state.SelectedRegionId = "normandy";
            state = Reload(state);
            Assert.AreEqual("champagne", state.Obligation.RegionId);
            float unrest = CampaignCore.Region(state, "champagne").Unrest;
            string paris = JsonUtility.ToJson(CampaignCore.Region(state, "ile"));
            int grain = state.Food;
            float power = state.Power;
            Succeeds(CampaignCore.ResolveMandate(state, Id(state), "break"));
            Assert.AreEqual(unrest + 18, CampaignCore.Region(state, "champagne").Unrest);
            Assert.AreEqual(paris, JsonUtility.ToJson(CampaignCore.Region(state, "ile")), "Yeni ordu konumu eski sözün hedefi olmamalı.");
            Assert.AreEqual(grain, state.Food);
            Assert.AreEqual(power - 4, state.Power);
            Assert.AreEqual(32, Faction(state, "assembly").Approval);
            Assert.AreEqual(40, Person(state, "morel").Relationship);
            Advance(state, 2);
            Succeeds(CampaignCore.ChoosePetition(state, "negotiate"));
            Assert.IsTrue(CampaignCore.CanMarch(state, "champagne").RequiresBattle);
            Refuses(state, () => CampaignCore.March(state, "champagne"), "march.battle");
            state = Reload(state);
            Assert.AreEqual(0, state.ResolvedBattles.Count, "Diplomatik açılan yol sahte savaş sonucu yaratmamalı.");
        }

        [Test]
        public void ArmyLevyRepairsItsOriginalRegionWithoutUndoingLaterBreadReliefAfterArmyMoves()
        {
            var state = CampaignCore.Create("army");
            Refuses(state, () => CampaignCore.IssueMandate(state, "normandy"), "error.mandate.army.location");
            Succeeds(CampaignCore.IssueMandate(state, "ile"));
            Assert.AreEqual(400, state.Food);
            Assert.AreEqual(135, state.MilitarySupplies);
            Assert.AreEqual(56, CampaignCore.Region(state, "ile").Unrest);
            Assert.AreEqual(54, CampaignCore.Region(state, "ile").EliteLoyalty);
            Succeeds(CampaignCore.March(state, "normandy"));
            state.SelectedRegionId = "provence";
            Succeeds(CampaignCore.Act(state, "bread", "ile"));
            Assert.AreEqual(41, CampaignCore.Region(state, "ile").Unrest);
            state = Reload(state);
            Assert.AreEqual("ile", state.Obligation.RegionId);
            string normandy = JsonUtility.ToJson(CampaignCore.Region(state, "normandy"));
            string provence = JsonUtility.ToJson(CampaignCore.Region(state, "provence"));
            int gold = state.Gold, food = state.Food, materials = state.MilitarySupplies;
            Succeeds(CampaignCore.ResolveMandate(state, Id(state), "fulfil"));
            Assert.AreEqual(gold - 80, state.Gold);
            Assert.AreEqual(food, state.Food);
            Assert.AreEqual(materials, state.MilitarySupplies);
            Assert.AreEqual(36, CampaignCore.Region(state, "ile").Unrest, "Tazminat önceki ekmek yardımını geri almadan eklenmeli.");
            Assert.AreEqual(58, CampaignCore.Region(state, "ile").EliteLoyalty);
            Assert.IsTrue(CampaignCore.Region(state, "ile").BreadUsed);
            Assert.AreEqual(54, Person(state, "dumas").Relationship);
            Assert.AreEqual(normandy, JsonUtility.ToJson(CampaignCore.Region(state, "normandy")));
            Assert.AreEqual(provence, JsonUtility.ToJson(CampaignCore.Region(state, "provence")));
            Assert.AreEqual("normandy", state.ArmyRegionId);
            state = Reload(state);
            Refuses(state, () => CampaignCore.IssueMandate(state, "normandy"), "error.mandate.cooldown");
        }

        [Test]
        public void PetitionHasPriorityAtDeadlineAndNeitherBlockedActionRunsAnotherEconomicWeek()
        {
            var state = CampaignCore.Create("crown");
            Succeeds(CampaignCore.IssueMandate(state, "ile"));
            Advance(state, 1);
            Assert.IsFalse(CampaignCore.MandateDue(state));
            Advance(state, 2);
            Assert.IsTrue(state.PendingPetition);
            Assert.IsTrue(CampaignCore.MandateDue(state));
            state = Reload(state);
            for (int click = 0; click < 3; click++)
            {
                Refuses(state, () => CampaignCore.NextWeek(state), "error.petition.pending");
                Refuses(state, () => CampaignCore.ResolveMandate(state, Id(state), "fulfil"), "error.mandate.petition");
                Refuses(state, () => CampaignCore.ResolveMandate(state, Id(state), "break"), "error.mandate.petition");
            }
            Succeeds(CampaignCore.ChoosePetition(state, "negotiate"));
            for (int click = 0; click < 3; click++)
                Refuses(state, () => CampaignCore.NextWeek(state), "error.mandate.due");
            int dueGold = state.Gold;
            Succeeds(CampaignCore.ResolveMandate(state, Id(state), "fulfil"));
            Assert.AreEqual(dueGold - 150, state.Gold, "Hafta işlemi söz borcunu otomatik ödememeli.");
            state = Reload(state);
            EconomyForecast forecast = CampaignCore.Forecast(state);
            int gold = state.Gold, food = state.Food;
            Advance(state, 3);
            Assert.AreEqual(gold + forecast.NetGold, state.Gold);
            Assert.AreEqual(food + forecast.NetFood, state.Food);
        }

        [TestCase("crown", 149, 360, "error.mandate.gold")]
        [TestCase("assembly", 840, 39, "error.mandate.food")]
        [TestCase("army", 79, 400, "error.mandate.gold")]
        public void UnaffordablePromiseCannotPartiallyPayButAnExplicitBreakRemainsPossible(string role, int gold, int food, string error)
        {
            var state = CampaignCore.Create(role);
            Succeeds(CampaignCore.IssueMandate(state, "ile"));
            // Geçerli yoksulluk başlangıç koşulu; reddin kaynak sınırında atomikliği sınanır.
            state.Gold = gold; state.Food = food;
            state = Reload(state);
            Refuses(state, () => CampaignCore.CanResolveMandate(state, Id(state), "fulfil"), error);
            Refuses(state, () => CampaignCore.ResolveMandate(state, Id(state), "fulfil"), error);
            float power = state.Power;
            Succeeds(CampaignCore.ResolveMandate(state, Id(state), "break"));
            Assert.AreEqual(gold, state.Gold);
            Assert.AreEqual(food, state.Food);
            Assert.Less(state.Power, power);
            Assert.IsNull(state.Obligation);
            Reload(state);
        }

        [Test]
        public void PreconditionsAndInvalidResolutionCannotIssueFreeResourcesOrEraseAnOpenPromise()
        {
            var legacy = CampaignCore.Create();
            Refuses(legacy, () => CampaignCore.IssueMandate(legacy, "ile"), "error.role.legacy");
            var state = CampaignCore.Create("army");
            Refuses(state, () => CampaignCore.IssueMandate(state, "missing"), "error.mandate.region");
            state.Power = 9;
            Refuses(state, () => CampaignCore.IssueMandate(state, "ile"), "error.mandate.power");
            state.Power = 10; state.Troops = 0;
            Refuses(state, () => CampaignCore.IssueMandate(state, "ile"), "error.mandate.army.empty");
            // Normal asker toplama rol kilidiyle engellenmez; yok olan ordu geri kurulabilir.
            Succeeds(CampaignCore.Act(state, "recruit", "ile"));
            Succeeds(CampaignCore.IssueMandate(state, "ile"));
            Refuses(state, () => CampaignCore.ResolveMandate(state, Id(state), "forgive"), "error.mandate.choice");
            Refuses(state, () => CampaignCore.ResolveMandate(state, null, "fulfil"), "error.mandate.stale");
            Refuses(state, () => CampaignCore.ResolveMandate(state, Id(state) + "-old", "break"), "error.mandate.stale");
            var full = CampaignCore.Create("crown"); full.Gold = 100000000;
            Refuses(full, () => CampaignCore.IssueMandate(full, "ile"), "error.mandate.capacity");
        }

        [TestCase("crown")]
        [TestCase("assembly")]
        [TestCase("army")]
        public void ArchiveRoundTripsFreshOutstandingAndResolvedRoles(string role)
        {
            var state = Reload(CampaignCore.Create(role));
            Assert.IsNull(state.Obligation);
            Succeeds(CampaignCore.IssueMandate(state, "ile"));
            state = Reload(state);
            string id = Id(state);
            Assert.IsNotNull(state.Obligation);
            Assert.AreEqual(role, state.RoleId);
            Assert.AreEqual(2, state.Obligation.DueWeek);
            Succeeds(CampaignCore.ResolveMandate(state, id, "fulfil"));
            state = Reload(state);
            Assert.IsNull(state.Obligation);
            Assert.AreEqual(4, state.NextMandateWeek);
            Refuses(state, () => CampaignCore.ResolveMandate(state, id, "fulfil"), "error.mandate.none");
        }

        // Gerçek v1 şeması: yeni rol alanları JSON'da fiziksel olarak bulunmaz.
        [Serializable] sealed class LegacyState
        {
            public int Week, Gold, Food, MilitarySupplies, Manpower, Troops, Moves;
            public string ArmyRegionId, SelectedRegionId;
            public float Morale, Supply, Fatigue, Power;
            public List<RegionState> Regions;
            public List<FactionState> Factions;
            public List<CharacterState> Characters;
            public List<LogEntry> Journal;
            public List<string> ResolvedBattles;
            public bool SubsidyParis, PendingPetition, PetitionResolved;
            public LegacyState(CampaignState state)
            {
                Week = state.Week; Gold = state.Gold; Food = state.Food; MilitarySupplies = state.MilitarySupplies;
                Manpower = state.Manpower; Troops = state.Troops; Moves = state.Moves;
                ArmyRegionId = state.ArmyRegionId; SelectedRegionId = state.SelectedRegionId;
                Morale = state.Morale; Supply = state.Supply; Fatigue = state.Fatigue; Power = state.Power;
                Regions = state.Regions; Factions = state.Factions; Characters = state.Characters;
                Journal = state.Journal; ResolvedBattles = state.ResolvedBattles;
                SubsidyParis = state.SubsidyParis; PendingPetition = state.PendingPetition; PetitionResolved = state.PetitionResolved;
            }
        }

        [Test]
        public void ActualV1WithoutRoleFieldsMigratesToLegacyAndRetainsBattleHistoryAndPendingPetition()
        {
            var original = CampaignCore.Create();
            Succeeds(CampaignCore.ResolveBattle(original, "champagne", "battle-0-2-ile-champagne", true, 90, 62));
            Advance(original, 1); Advance(original, 2);
            string v1Body = JsonUtility.ToJson(new LegacyState(original));
            StringAssert.DoesNotContain("RoleId", v1Body);
            StringAssert.DoesNotContain("Obligation", v1Body);
            var loaded = CampaignArchive.Deserialize("{\"Version\":1,\"State\":" + v1Body + "}");
            Assert.AreEqual(Snapshot(original), Snapshot(loaded));
            Assert.AreEqual("legacy", loaded.RoleId);
            Assert.IsNull(loaded.Obligation);
            Assert.IsTrue(loaded.PendingPetition);
            CollectionAssert.AreEqual(original.ResolvedBattles, loaded.ResolvedBattles);
            loaded = Reload(loaded);
            Refuses(loaded, () => CampaignCore.NextWeek(loaded), "error.petition.pending");
            Succeeds(CampaignCore.ChoosePetition(loaded, "negotiate"));
            Refuses(loaded, () => CampaignCore.IssueMandate(loaded, "champagne"), "error.role.legacy");
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize("{\"Version\":2,\"State\":" + v1Body + "}"), "Bozuk yeni kayıt eski kayıt gibi yorumlanmamalı.");
        }

        [TestCase("role")]
        [TestCase("kind")]
        [TestCase("region")]
        [TestCase("gold")]
        [TestCase("food")]
        [TestCase("due")]
        [TestCase("issued")]
        [TestCase("cooldown")]
        [TestCase("overdue")]
        [TestCase("empty")]
        [TestCase("multiple")]
        [TestCase("nullEntry")]
        [TestCase("nullList")]
        public void CorruptOutstandingTermsAreRejectedByArchiveAndActionsWithoutMutation(string corruption)
        {
            var state = CampaignCore.Create("crown");
            Succeeds(CampaignCore.IssueMandate(state, "ile"));
            switch (corruption)
            {
                case "role": state.RoleId = "assembly"; break;
                case "kind": state.Obligation.Kind = "field_levy"; break;
                case "region": state.Obligation.RegionId = "normandy"; break;
                case "gold": state.Obligation.GoldDue = 1; break;
                case "food": state.Obligation.FoodDue = 1; break;
                case "due": state.Obligation.DueWeek = 3; break;
                case "issued": state.Obligation.IssuedWeek = -1; break;
                case "cooldown": state.NextMandateWeek = 0; break;
                case "overdue": state.Week = 3; state.PetitionResolved = true; break;
                case "empty": state.Obligation = new MandateObligation(); break;
                case "multiple": state.Mandates.Add(state.Obligation); break;
                case "nullEntry": state.Mandates[0] = null; break;
                case "nullList": state.Mandates = null; break;
            }
            string before = Snapshot(state);
            Assert.Throws<ArgumentException>(() => CampaignArchive.Serialize(state));
            // Null listeyi JsonUtility'nin nasıl yazdığına bağlı sahte fixture kurmayız;
            // gerçek JSON null ve eksik alan ayrı sınır testinde açıkça üretilir.
            if (corruption != "nullList")
                Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize("{\"Version\":2,\"State\":" + before + "}"));
            Refuses(state, () => CampaignCore.IssueMandate(state, "ile"), "error.role.invalid");
            Refuses(state, () => CampaignCore.ResolveMandate(state, Id(state), "break"), "error.role.invalid");
        }

        [TestCase(2, "missing")]
        [TestCase(2, "null")]
        [TestCase(3, "missing")]
        [TestCase(3, "null")]
        [TestCase(4, "missing")]
        [TestCase(4, "null")]
        [TestCase(5, "missing")]
        [TestCase(5, "null")]
        [TestCase(6, "missing")]
        [TestCase(6, "null")]
        [TestCase(7, "missing")]
        [TestCase(7, "null")]
        [TestCase(8, "missing")]
        [TestCase(8, "null")]
        public void CurrentAndV2MissingOrNullMandateCollectionCannotSilentlyLoadAsAnEmptyPromiseList(int version, string representation)
        {
            string valid = CampaignArchive.Serialize(CampaignCore.Create("crown"), false);
            valid = valid.Replace("\"Version\":8", "\"Version\":" + version);
            StringAssert.Contains("\"Mandates\":[]", valid);
            // Alanı yeniden adlandırmak mevcut RoleId dahil diğer bütün alanları korur.
            string invalid = valid.Replace("\"Mandates\":[]", representation == "missing" ? "\"IgnoredOldMandates\":[]" : "\"Mandates\":null");
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize(invalid));
        }

        [Test]
        public void UnknownRolesAndDisguisedLegacyMandatesAreNotSilentlyAccepted()
        {
            Assert.Throws<ArgumentException>(() => CampaignCore.Create("emperor"));
            Assert.Throws<ArgumentException>(() => CampaignCore.Create((string)null));
            var state = CampaignCore.Create("crown");
            Succeeds(CampaignCore.IssueMandate(state, "ile"));
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize("{\"Version\":1,\"State\":" + Snapshot(state) + "}"));
            var unknown = CampaignCore.Create(); unknown.RoleId = "emperor";
            Assert.Throws<ArgumentException>(() => CampaignArchive.Deserialize("{\"Version\":2,\"State\":" + Snapshot(unknown) + "}"));
        }

        [Test]
        public void ActualRoleLogsAndRefusalsFormatInRussianAndTurkishWithoutChangingUserPreference()
        {
            var messages = new List<ActionResult>();
            foreach (string role in new[] { "crown", "assembly", "army" })
                foreach (string choice in new[] { "fulfil", "break" })
                {
                    var state = CampaignCore.Create(role);
                    messages.Add(new ActionResult { Key = state.Journal[0].Key, Args = state.Journal[0].Args });
                    messages.Add(CampaignCore.CanIssueMandate(state, "ile"));
                    messages.Add(CampaignCore.IssueMandate(state, "ile"));
                    messages.Add(CampaignCore.IssueMandate(state, "ile"));
                    messages.Add(CampaignCore.ResolveMandate(state, "old", choice));
                    messages.Add(CampaignCore.ResolveMandate(state, Id(state), "unknown"));
                    messages.Add(CampaignCore.ResolveMandate(state, Id(state), choice));
                    messages.Add(CampaignCore.ResolveMandate(state, "old", choice));
                    messages.Add(CampaignCore.IssueMandate(state, "ile"));
                }
            var crown = CampaignCore.Create("crown");
            messages.Add(CampaignCore.IssueMandate(crown, "missing"));
            crown.Power = 9; messages.Add(CampaignCore.IssueMandate(crown, "ile"));
            crown.Power = 55; crown.Gold = 100000000; messages.Add(CampaignCore.IssueMandate(crown, "ile"));
            crown.Gold = 840; Succeeds(CampaignCore.IssueMandate(crown, "ile"));
            crown.Gold = 0; messages.Add(CampaignCore.ResolveMandate(crown, Id(crown), "fulfil"));
            Advance(crown, 1); Advance(crown, 2);
            messages.Add(CampaignCore.ResolveMandate(crown, Id(crown), "break"));
            Succeeds(CampaignCore.ChoosePetition(crown, "negotiate"));
            messages.Add(CampaignCore.NextWeek(crown));
            var assembly = CampaignCore.Create("assembly"); Succeeds(CampaignCore.IssueMandate(assembly, "ile"));
            assembly.Food = 0; messages.Add(CampaignCore.ResolveMandate(assembly, Id(assembly), "fulfil"));
            var army = CampaignCore.Create("army"); messages.Add(CampaignCore.IssueMandate(army, "normandy"));
            army.Troops = 0; messages.Add(CampaignCore.IssueMandate(army, "ile"));
            var legacy = CampaignCore.Create(); messages.Add(CampaignCore.IssueMandate(legacy, "ile"));
            legacy.RoleId = "invalid"; messages.Add(CampaignCore.IssueMandate(legacy, "ile"));
            var lastWeek = CampaignCore.Create("crown"); lastWeek.Week = 1000000; lastWeek.PetitionResolved = true;
            messages.Add(CampaignCore.IssueMandate(lastWeek, "ile"));

            var actualKeys = new HashSet<string>();
            foreach (var message in messages) actualKeys.Add(message.Key);
            var table = JsonUtility.FromJson<L.Table>(Resources.Load<TextAsset>("Localization/roles-core").text);
            foreach (var entry in table.entries)
                Assert.IsTrue(actualKeys.Contains(entry.key), "Çekirdek çevirisi gerçek çağrı bağlamında örneklenmeli: " + entry.key);

            string profile = Environment.GetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE");
            string language = L.Language;
            bool hadPreference = PlayerPrefs.HasKey("language");
            string preference = PlayerPrefs.GetString("language", "");
            try
            {
                Environment.SetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE", "role-localization-check");
                L.Initialize();
                foreach (string selectedLanguage in new[] { "ru", "tr" })
                {
                    L.SetLanguage(selectedLanguage);
                    foreach (var message in messages)
                    {
                        string rendered = L.Text(message.Key, message.Args);
                        Assert.AreNotEqual(message.Key, rendered, selectedLanguage + ": " + message.Key);
                        Assert.IsNotEmpty(rendered);
                        StringAssert.DoesNotContain("{", rendered);
                        StringAssert.DoesNotContain("region.", rendered);
                    }
                }
                Assert.AreEqual(hadPreference, PlayerPrefs.HasKey("language"));
                Assert.AreEqual(preference, PlayerPrefs.GetString("language", ""));
            }
            finally
            {
                L.SetLanguage(language);
                Environment.SetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE", profile);
            }
        }
    }
}
#endif
