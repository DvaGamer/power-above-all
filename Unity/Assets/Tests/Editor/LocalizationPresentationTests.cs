using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    public sealed class LocalizationPresentationTests
    {
        [TestCase("ru")]
        [TestCase("tr")]
        public void EveryLocalizationSourceImportsAndResolvesInTheRuntimeTable(string language)
        {
            string oldLanguage = L.Language;
            string oldProfile = Environment.GetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE");
            try
            {
                Environment.SetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE", "localization-import-check");
                L.Initialize(); L.SetLanguage(language);
                string folder = Path.Combine(Application.dataPath, "Resources/Localization");
                var files = Directory.GetFiles(folder, "*.json");
                Assert.Greater(files.Length, 0);
                foreach (string file in files)
                {
                    string resource = "Localization/" + Path.GetFileNameWithoutExtension(file);
                    var asset = Resources.Load<TextAsset>(resource);
                    Assert.IsNotNull(asset, "Source exists but Unity did not import its resource: " + resource);
                    var table = JsonUtility.FromJson<L.Table>(File.ReadAllText(file));
                    Assert.IsNotNull(table.entries, file);
                    foreach (var entry in table.entries)
                        Assert.AreEqual(language == "tr" ? entry.tr : entry.ru, L.Text(entry.key), resource + ": " + entry.key);
                }
            }
            finally
            {
                L.SetLanguage(oldLanguage);
                Environment.SetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE", oldProfile);
            }
        }

        [TestCase("ru")]
        [TestCase("tr")]
        public void PoliticalChangesKeepTheirSignsWhileRawJournalAmountsUseLocale(string language)
        {
            string oldLanguage = L.Language;
            string oldProfile = Environment.GetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE");
            try
            {
                Environment.SetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE", "localization-presentation-check");
                L.Initialize(); L.SetLanguage(language);
                StringAssert.Contains("+120", L.Text("ui.mandate.effect.delta", "delta", "+120"));
                StringAssert.Contains("−120", L.Text("ui.mandate.effect.delta", "delta", "−120"));
                string localized = 1200.ToString("N0", CultureInfo.GetCultureInfo(language == "tr" ? "tr-TR" : "ru-RU"));
                StringAssert.Contains(localized, L.Text("ui.mandate.effect.delta", "amount", "1200"));
            }
            finally
            {
                L.SetLanguage(oldLanguage);
                Environment.SetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE", oldProfile);
            }
        }
    }
}
