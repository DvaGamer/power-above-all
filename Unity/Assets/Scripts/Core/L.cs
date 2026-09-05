using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace PowerAboveAll
{
    public static class L
    {
        [Serializable] public class Entry { public string key, ru, tr; }
        [Serializable] public class Table { public Entry[] entries; }
        private static readonly Dictionary<string, Entry> entries = new Dictionary<string, Entry>();
        public static string Language { get; private set; } = "ru";
        public static bool IsReviewSession => Array.IndexOf(Environment.GetCommandLineArgs(), "-shots") >= 0 ||
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE"));
        public static void Initialize()
        {
            entries.Clear();
            foreach (var asset in Resources.LoadAll<TextAsset>("Localization"))
            {
                var table = JsonUtility.FromJson<Table>(asset.text);
                if (table == null || table.entries == null) throw new InvalidOperationException("Invalid localization: " + asset.name);
                foreach (var entry in table.entries)
                {
                    if (string.IsNullOrEmpty(entry.key) || string.IsNullOrEmpty(entry.ru) || string.IsNullOrEmpty(entry.tr) || entries.ContainsKey(entry.key))
                        throw new InvalidOperationException("Invalid or duplicate localization key: " + entry.key);
                    entries.Add(entry.key, entry);
                }
            }
            SetLanguage(PlayerPrefs.GetString("language", Application.systemLanguage == SystemLanguage.Turkish ? "tr" : "ru"));
        }
        public static void SetLanguage(string language)
        {
            Language = language == "tr" ? "tr" : "ru";
            // Otomatik inceleme kullanıcının kalıcı dil tercihini değiştirmez.
            if (IsReviewSession) return;
            PlayerPrefs.SetString("language", Language);
            PlayerPrefs.Save();
        }
        public static string Text(string key, params object[] args)
        {
            if (key == null) return "";
            // Editor hot reload clears static tables while the live scene can survive.
            if (entries.Count == 0) Initialize();
            if (!entries.TryGetValue(key, out var entry)) return key;
            var value = Language == "tr" ? entry.tr : entry.ru;
            if (args == null || args.Length == 0) return value;
            var culture = CultureInfo.GetCultureInfo(Language == "tr" ? "tr-TR" : "ru-RU");
            var translated = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is string argument && entries.ContainsKey(argument)) translated[i] = Text(argument);
                // Sunumun açık artı işaretini koru; ör. siyasi değişim +5, düz 5 olmamalı.
                else if (args[i] is string number && !number.StartsWith("+", StringComparison.Ordinal) &&
                    long.TryParse(number, NumberStyles.Integer, CultureInfo.InvariantCulture, out long integer))
                    translated[i] = integer.ToString("N0", culture);
                else translated[i] = args[i];
            }
            return string.Format(culture, value, translated);
        }
    }
}
