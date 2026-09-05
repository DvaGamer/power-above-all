using System;
using System.Globalization;
using NUnit.Framework;

namespace PowerAboveAll.Tests
{
    public sealed class LocalizationPresentationTests
    {
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
