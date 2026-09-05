using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PowerAboveAll.Editor
{
    // Local, explicit file commands make repeatable checks possible in the already open editor.
    [InitializeOnLoad]
    public static class LocalVerification
    {
        [Serializable] public sealed class Request { public string id, action, value; public int revision; }
        [Serializable] public sealed class Status
        {
            public string id, action, message, savePath, language, state, view;
            public int revision;
            public bool playing, paused, compiling, battle;
        }
        private static readonly string CommandPath = Path.GetFullPath("Temp/polish-command.json");
        private static readonly string ResultPath = Path.GetFullPath("../output/unity-verification.json");
        private static double nextPoll;
        static LocalVerification() { EditorApplication.update += Poll; }
        private static void Poll()
        {
            if (EditorApplication.timeSinceStartup < nextPoll || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            nextPoll = EditorApplication.timeSinceStartup + .5;
            if (!File.Exists(CommandPath)) return;
            Request request = null;
            try
            {
                request = JsonUtility.FromJson<Request>(File.ReadAllText(CommandPath));
                if (request.revision > 6) return;
                File.Delete(CommandPath);
                switch (request.action)
                {
                    case "status": break;
                    case "view": ConfigureGameView(); break;
                    case "tests":
                        if (EditorApplication.isPlaying) throw new InvalidOperationException("Exit Play before Edit Mode tests");
                        Type.GetType("PowerAboveAll.Tests.ValidationRunner, PowerAboveAll.EditModeTests", true).GetMethod("Run").Invoke(null, null);
                        break;
                    case "pause": EditorApplication.isPaused = request.value == "true"; break;
                    case "play": EditorApplication.isPlaying = request.value == "true"; break;
                    case "language": L.Initialize(); L.SetLanguage(request.value); break;
                    case "save": UnityEngine.Object.FindFirstObjectByType<GameApp>()?.Save(); break;
                    case "sandbox":
                        var current = UnityEngine.Object.FindFirstObjectByType<GameApp>();
                        if (current.Busy) throw new InvalidOperationException("Return to atlas before isolating verification");
                        SessionState.SetString("PowerAboveAll.OriginalLanguage", L.Language);
                        Environment.SetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE", Path.GetFullPath("../output/verification-save"));
                        current.NewCampaign();
                        break;
                    case "restore":
                        var restored = UnityEngine.Object.FindFirstObjectByType<GameApp>();
                        if (restored.Busy) throw new InvalidOperationException("Return to atlas before restoring the personal profile");
                        Environment.SetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE", null);
                        restored.Load(); restored.SetLanguage(SessionState.GetString("PowerAboveAll.OriginalLanguage", "ru"));
                        break;
                    case "select": TestApp().SelectRegion(request.value); break;
                    case "act": TestApp().Act(request.value); break;
                    case "week": TestApp().NextWeek(); break;
                    case "petition": TestApp().ChoosePetition(request.value); break;
                    case "march": TestApp().March(); break;
                    case "panel":
                        typeof(CabinetHud).GetField("document", Members).SetValue(TestApp().GetComponent<CabinetHud>(), request.value);
                        break;
                    case "battle-retreat":
                        var retreat = TestApp().GetComponent<TacticalBattle>();
                        typeof(TacticalBattle).GetMethod("Finish", Members).Invoke(retreat, new object[] { false, true });
                        break;
                    case "battle-accept":
                        var report = TestApp().GetComponent<TacticalBattle>();
                        typeof(TacticalBattle).GetMethod("AcceptOutcome", Members).Invoke(report, null);
                        break;
                    case "capture": ScreenCapture.CaptureScreenshot(Path.GetFullPath("../output/" + Path.GetFileName(request.value))); break;
                    case "build": BuildTools.BuildWindows(); break;
                    case "build-review": BuildTools.BuildWindowsPreview(); break;
                    case "refresh": AssetDatabase.Refresh(); break;
                    default: throw new InvalidOperationException("Unknown local verification action");
                }
                WriteStatus(request, "ok");
            }
            catch (Exception error) { WriteStatus(request, error.ToString()); Debug.LogException(error); }
        }
        private static GameApp TestApp()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE")))
                throw new InvalidOperationException("Gameplay verification requires an isolated save profile");
            return UnityEngine.Object.FindFirstObjectByType<GameApp>();
        }
        private const BindingFlags Members = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static void ConfigureGameView()
        {
            var assembly = typeof(EditorWindow).Assembly;
            var viewType = assembly.GetType("UnityEditor.GameView");
            var view = EditorWindow.GetWindow(viewType);
            view.maximized = true;
            var sizesType = assembly.GetType("UnityEditor.GameViewSizes");
            var sizes = sizesType.GetProperty("instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).GetValue(null);
            var groupMethod = sizesType.GetMethod("GetGroup", Members);
            var group = groupMethod.Invoke(sizes, new[] { Enum.Parse(groupMethod.GetParameters()[0].ParameterType, "Standalone") });
            var groupType = group.GetType();
            int count = (int)groupType.GetMethod("GetTotalCount", Members).Invoke(group, null), selected = -1;
            var get = groupType.GetMethod("GetGameViewSize", Members);
            for (int i = 0; i < count; i++)
            {
                var size = get.Invoke(group, new object[] { i });
                if ((int)size.GetType().GetProperty("width", Members).GetValue(size) == 1440 &&
                    (int)size.GetType().GetProperty("height", Members).GetValue(size) == 900) { selected = i; break; }
            }
            if (selected < 0)
            {
                var sizeType = assembly.GetType("UnityEditor.GameViewSize");
                var constructor = sizeType.GetConstructors(Members).First(c => c.GetParameters().Length == 4);
                var fixedResolution = Enum.Parse(constructor.GetParameters()[0].ParameterType, "FixedResolution");
                var size = constructor.Invoke(new object[] { fixedResolution, 1440, 900, "Power Above All" });
                groupType.GetMethod("AddCustomSize", Members).Invoke(group, new[] { size });
                selected = count;
            }
            viewType.GetProperty("selectedSizeIndex", Members).SetValue(view, selected);
            var zoom = viewType.GetField("m_ZoomArea", Members)?.GetValue(view);
            if (zoom != null)
                for (var type = zoom.GetType(); type != null; type = type.BaseType)
                {
                    var scale = type.GetField("m_Scale", Members | BindingFlags.DeclaredOnly);
                    if (scale != null) { scale.SetValue(zoom, Vector2.one); break; }
                }
            view.Focus(); view.Repaint();
        }
        private static void WriteStatus(Request request, string message)
        {
            var app = UnityEngine.Object.FindFirstObjectByType<GameApp>();
            var status = new Status { id = request?.id, action = request?.action, message = message, revision = 6,
                playing = EditorApplication.isPlaying, paused = EditorApplication.isPaused,
                compiling = EditorApplication.isCompiling, savePath = Application.persistentDataPath,
                language = L.Language, battle = app != null && app.BattleActive,
                state = app != null && app.State != null ? JsonUtility.ToJson(app.State) : null,
                view = Screen.width + "x" + Screen.height + "; MSAA=" + QualitySettings.antiAliasing + "; dpi=" + Screen.dpi };
            Directory.CreateDirectory(Path.GetDirectoryName(ResultPath));
            File.WriteAllText(ResultPath, JsonUtility.ToJson(status, true));
        }
    }
}
