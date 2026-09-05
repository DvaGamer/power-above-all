using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PowerAboveAll
{
    // Yalnız -shots <boş klasör> -script <dosya> ile çalışan doğrulama sürücüsü.
    // İnsan kaydına dokunmaz; eksik kare, yanlış varsayım ve çalışma zamanı hatası başarısızlıktır.
    public sealed class AutoShots : MonoBehaviour
    {
        private const string Protocol = "AUTO_SHOTS_PROTOCOL 2";
        private string folder, scriptPath, originalLanguage;
        private bool ownsFolder, finished;
        private readonly List<string> report = new List<string>();
        private readonly List<string> failures = new List<string>();
        private readonly List<string> captures = new List<string>();
        private readonly List<string> states = new List<string>();
        private readonly Dictionary<string, string> remembered = new Dictionary<string, string>();
        private int commands, assertions;

        [Serializable]
        private sealed class Receipt
        {
            public int protocolVersion = 2;
            public bool success;
            public int commands, assertions;
            public string campaignPath, completedUtc;
            public string[] captures, states, failures;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Boot()
        {
            var args = Environment.GetCommandLineArgs();
            int at = Array.IndexOf(args, "-shots");
            if (at < 0) return;
            var host = new GameObject("Auto shots").AddComponent<AutoShots>();
            host.folder = at + 1 < args.Length ? args[at + 1] : null;
            int scriptAt = Array.IndexOf(args, "-script");
            host.scriptPath = scriptAt >= 0 && scriptAt + 1 < args.Length ? args[scriptAt + 1] : null;
            Application.logMessageReceived += host.OnLog;
        }

        private IEnumerator Start() { return Guard(Run()); }

        // İç içe coroutine hatalarının eksik bir koşuyu başarılı göstermesini engelle.
        private IEnumerator Guard(IEnumerator routine)
        {
            var stack = new Stack<IEnumerator>();
            stack.Push(routine);
            while (stack.Count > 0 && !finished && failures.Count == 0)
            {
                object current = null;
                bool moved = false;
                try { moved = stack.Peek().MoveNext(); if (moved) current = stack.Peek().Current; }
                catch (Exception error) { failures.Add(error.GetBaseException().ToString()); }
                if (failures.Count > 0 || finished) break;
                if (!moved) { stack.Pop(); continue; }
                if (current is IEnumerator nested) stack.Push(nested);
                else yield return current;
            }
            if (!finished) Finish();
        }

        private IEnumerator Run()
        {
            Application.runInBackground = true;
            originalLanguage = L.Language;
            if (string.IsNullOrWhiteSpace(folder)) throw new InvalidDataException("Missing -shots folder.");
            folder = Path.GetFullPath(folder);
            if (Directory.Exists(folder) && Directory.GetFileSystemEntries(folder).Length != 0)
                throw new InvalidDataException("Shots folder must be empty; existing artifacts are preserved: " + folder);
            Directory.CreateDirectory(folder);
            WriteNew(Path.Combine(folder, "shots.running"), Protocol);
            ownsFolder = true;
            report.Add(Protocol);
            var app = FindFirstObjectByType<GameApp>();
            if (app == null) throw new InvalidOperationException("GameApp not found.");
            var saveProperty = typeof(GameApp).GetProperty("SavePath", BindingFlags.Instance | BindingFlags.NonPublic);
            string observedPath = saveProperty?.GetValue(app) as string;
            string campaignPath = Path.Combine(folder, ".campaign", "campaign-v1.json");
            if (observedPath == null || !string.Equals(Path.GetFullPath(observedPath), campaignPath, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Isolated campaign path was not verified; no commands were executed.");
            if (string.IsNullOrWhiteSpace(scriptPath) || !File.Exists(scriptPath))
                throw new FileNotFoundException("An explicit existing -script file is required.", scriptPath);
            var plan = new List<string>();
            foreach (string raw in File.ReadAllLines(scriptPath))
            {
                string line = raw.Trim();
                if (line.Length > 0 && !line.StartsWith("#", StringComparison.Ordinal)) plan.Add(line);
            }
            if (plan.Count < 2 || plan[0] != "new" || plan[plan.Count - 1] != "quit")
                throw new InvalidDataException("Script must start with new and end with quit.");
            yield return Settle(app, 3f);
            foreach (string line in plan)
            {
                int space = line.IndexOf(' ');
                string command = space < 0 ? line : line.Substring(0, space);
                string value = space < 0 ? "" : line.Substring(space + 1).Trim();
                report.Add(line);
                commands++;
                switch (command)
                {
                    case "new": RequireIdle(app); app.NewCampaign(); break;
                    case "lang": RequireChoice(value, "ru", "tr"); app.SetLanguage(value); break;
                    case "mode": RequireChoice(value, "control", "unrest", "tax", "army", "food", "influence"); app.SetMode(value); break;
                    case "select": RequireIdle(app); app.SelectRegion(value); break;
                    case "act": RequireIdle(app); app.Act(value); break;
                    case "week": RequireIdle(app); app.NextWeek(); break;
                    case "march": RequireIdle(app); app.March(); break;
                    case "petition": RequireIdle(app); app.ChoosePetition(value); break;
                    case "save": RequireIdle(app); app.Save(); break;
                    case "load": RequireIdle(app); app.Load(); break;
                    case "panel":
                        RequireChoice(value, "council", "economy", "journal");
                        Field(typeof(CabinetHud), "document").SetValue(app.GetComponent<CabinetHud>(), value); break;
                    case "scroll":
                        RequireIdle(app);
                        string[] scroll = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (scroll.Length != 2 || scroll[0] != "document" ||
                            !float.TryParse(scroll[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float offset) ||
                            float.IsNaN(offset) || float.IsInfinity(offset) || offset < 0 || offset > 5000)
                            throw new InvalidDataException("scroll requires document and an offset between 0 and 5000.");
                        Field(typeof(CabinetHud), "documentScroll").SetValue(app.GetComponent<CabinetHud>(), new Vector2(0, offset)); break;
                    case "retreat":
                        if (!app.BattleActive) throw new InvalidOperationException("Cannot retreat outside battle.");
                        Method(typeof(TacticalBattle), "Finish").Invoke(app.GetComponent<TacticalBattle>(), new object[] { false, true }); break;
                    case "accept":
                        if (!app.BattleActive) throw new InvalidOperationException("Cannot accept outside battle.");
                        Method(typeof(TacticalBattle), "AcceptOutcome").Invoke(app.GetComponent<TacticalBattle>(), null); break;
                    case "expect": Expect(app, value); break;
                    case "remember":
                        RequireName(value); CampaignCore.Validate(app.State);
                        if (remembered.ContainsKey(value)) throw new InvalidDataException("Duplicate remembered state: " + value);
                        remembered.Add(value, JsonUtility.ToJson(app.State)); break;
                    case "same":
                        if (!remembered.TryGetValue(value, out string previous) || previous != JsonUtility.ToJson(app.State))
                            throw new InvalidOperationException("Campaign differs from remembered state: " + value);
                        assertions++; report.Add("  PASS same " + value); break;
                    case "state":
                        CampaignCore.Validate(app.State);
                        WriteNew(ArtifactPath(value, ".json"), JsonUtility.ToJson(app.State, true)); states.Add(value + ".json"); break;
                    case "wait": yield return new WaitForSecondsRealtime(Number(value)); break;
                    case "settle": yield return Settle(app, Number(value)); break;
                    case "shot": yield return Shot(value); break;
                    case "quit":
                        if (commands != plan.Count) throw new InvalidDataException("Commands after quit are not allowed.");
                        if (assertions == 0 || captures.Count == 0) throw new InvalidDataException("Review needs at least one assertion and one frame.");
                        Finish(); yield break;
                    default: throw new InvalidDataException("Unknown command: " + command);
                }
                if (failures.Count > 0) yield break;
                yield return null;
            }
            throw new InvalidDataException("Review ended without quit.");
        }

        private void Expect(GameApp app, string value)
        {
            int space = value.IndexOf(' ');
            if (space < 1 || space == value.Length - 1) throw new InvalidDataException("expect requires a field and value: " + value);
            string key = value.Substring(0, space), expected = value.Substring(space + 1).Trim();
            object actual = key == "BattleActive" ? (object)app.BattleActive : key == "Busy" ? app.Busy :
                key == "Language" ? L.Language : key == "Mode" ? app.Mode : key == "ResolvedBattleCount" ?
                app.State.ResolvedBattles.Count : Field(typeof(CampaignState), key).GetValue(app.State);
            string observed = Convert.ToString(actual, CultureInfo.InvariantCulture);
            if (observed != expected) throw new InvalidOperationException("Expected " + value + ", observed=" + observed);
            assertions++; report.Add("  PASS " + value);
        }

        private IEnumerator Shot(string name)
        {
            string path = ArtifactPath(name, ".png");
            if (File.Exists(path)) throw new IOException("Screenshot path already exists: " + path);
            yield return new WaitForEndOfFrame();
            // Dosya API'si gizli Windows oyuncusunda kareyi render sonrasında zamanlar.
            ScreenCapture.CaptureScreenshot(path);
            float until = Time.realtimeSinceStartup + 10f;
            while (!CompletePng(path) && Time.realtimeSinceStartup < until) yield return null;
            if (!CompletePng(path)) throw new TimeoutException("Complete screenshot was not written within 10 seconds: " + name);
            captures.Add(name + ".png"); report.Add("  wrote " + name + ".png");
        }

        private static bool CompletePng(string path)
        {
            byte[] png;
            try { if (!File.Exists(path)) return false; png = File.ReadAllBytes(path); }
            catch (IOException) { return false; }
            if (png.Length < 100 || png[png.Length - 8] != 'I' || png[png.Length - 7] != 'E' ||
                png[png.Length - 6] != 'N' || png[png.Length - 5] != 'D') return false;
            byte[] signature = { 137, 80, 78, 71, 13, 10, 26, 10 };
            for (int i = 0; i < signature.Length; i++)
                if (png[i] != signature[i]) throw new InvalidDataException("Invalid PNG signature: " + path);
            int width = (png[16] << 24) | (png[17] << 16) | (png[18] << 8) | png[19];
            int height = (png[20] << 24) | (png[21] << 16) | (png[22] << 8) | png[23];
            if (width != 1440 || height != 900) throw new InvalidDataException("Expected 1440x900 frame, observed " + width + "x" + height);
            return true;
        }

        private static IEnumerator Settle(GameApp app, float limit)
        {
            float until = Time.realtimeSinceStartup + limit;
            while (app.Busy && Time.realtimeSinceStartup < until) yield return null;
            if (app.Busy) throw new TimeoutException("Game remained busy past settle limit " + limit + "s.");
            yield return new WaitForSecondsRealtime(.35f);
        }

        private static void RequireIdle(GameApp app) { if (app.Busy) throw new InvalidOperationException("Command requires an idle campaign."); }
        private static void RequireName(string name)
        {
            if (!Regex.IsMatch(name, @"\A[a-zA-Z0-9][a-zA-Z0-9_-]{0,79}\z")) throw new InvalidDataException("Unsafe artifact name: " + name);
        }
        private string ArtifactPath(string name, string extension) { RequireName(name); return Path.Combine(folder, name + extension); }
        private static void RequireChoice(string value, params string[] choices)
        {
            if (Array.IndexOf(choices, value) < 0) throw new InvalidDataException("Unsupported value: " + value);
        }
        private static FieldInfo Field(Type type, string name)
        {
            return type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new MissingFieldException(type.Name, name);
        }
        private static MethodInfo Method(Type type, string name)
        {
            return type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new MissingMethodException(type.Name, name);
        }
        private static float Number(string value)
        {
            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed) ||
                float.IsNaN(parsed) || float.IsInfinity(parsed) || parsed <= 0 || parsed > 120)
                throw new InvalidDataException("Duration must be greater than 0 and at most 120 seconds: " + value);
            return parsed;
        }
        private static void WriteNew(string path, string text)
        {
            using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(stream)) writer.Write(text);
        }
        private void OnLog(string message, string stack, LogType type)
        {
            if (!finished && (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)) failures.Add(message + "\n" + stack);
        }
        private void Finish()
        {
            if (finished) return;
            finished = true;
            Application.logMessageReceived -= OnLog;
            try
            {
                if (originalLanguage != null) L.SetLanguage(originalLanguage);
                if (ownsFolder)
                {
                    foreach (string failure in failures) report.Add("FAILED " + failure);
                    string time = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
                    report.Add((failures.Count == 0 ? "PASS completed " : "FAIL completed ") + time);
                    WriteNew(Path.Combine(folder, "shots.log"), string.Join(Environment.NewLine, report));
                    WriteNew(Path.Combine(folder, "shots-result.json"), JsonUtility.ToJson(new Receipt {
                        success = failures.Count == 0, commands = commands, assertions = assertions,
                        campaignPath = Path.Combine(folder, ".campaign", "campaign-v1.json"), completedUtc = time,
                        captures = captures.ToArray(), states = states.ToArray(), failures = failures.ToArray()
                    }, true));
                }
                else if (failures.Count == 0) failures.Add("No review folder was acquired.");
            }
            catch (Exception error) { failures.Add(error.ToString()); }
            if (failures.Count > 0) Debug.LogError("Auto shots failed: " + string.Join("; ", failures));
            else Debug.Log("Auto shots passed: " + folder);
            Application.Quit(failures.Count == 0 ? 0 : 1);
        }
        private void OnDestroy() { Application.logMessageReceived -= OnLog; }
    }
}
