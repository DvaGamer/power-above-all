using System;
using System.IO;
using UnityEngine;

namespace PowerAboveAll
{
    public sealed class GameApp : MonoBehaviour
    {
        [Serializable] private class SaveFile { public int Version = 1; public CampaignState State; }
        public CampaignState State { get; private set; }
        public string Mode { get; private set; } = "control";
        public string Message => L.Text(messageKey, messageArgs);
        public CampaignMap Map { get; private set; }
        public Camera Camera { get; private set; }
        public bool BattleActive => battle != null && battle.Active;
        public bool Busy => BattleActive || dispatchPending;
        private CabinetHud hud;
        private TacticalBattle battle;
        private CabinetAudio sound;
        private PetitionDocument petition;
        private string messageKey = "app.welcome";
        private object[] messageArgs = new object[0];
        private string pendingTarget, pendingBattleId;
        private float dispatchUntil;
        private Action afterDispatch;
        private bool dispatchPending;
        private string dispatchKey;
        private GUIStyle dispatchStyle;
        private string SavePath
        {
            get
            {
                string folder = Application.isEditor ? Environment.GetEnvironmentVariable("POWER_ABOVE_ALL_TEST_PROFILE") : null;
                var arguments = Environment.GetCommandLineArgs();
                int shots = Array.IndexOf(arguments, "-shots");
                if (folder == null && shots >= 0 && shots + 1 < arguments.Length)
                    folder = Path.Combine(Path.GetFullPath(arguments[shots + 1]), ".campaign");
                return Path.Combine(folder ?? Application.persistentDataPath, "campaign-v1.json");
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (FindFirstObjectByType<GameApp>() == null) new GameObject("Power Above All").AddComponent<GameApp>();
        }
        private void Awake()
        {
            L.Initialize();
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
            QualitySettings.antiAliasing = 4;
            State = CampaignCore.Create();
            Camera = new GameObject("Atlas camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
            var light = new GameObject("Afternoon light", typeof(Light)).GetComponent<Light>();
            light.type = LightType.Directional; light.intensity = 1.15f;
            light.transform.rotation = Quaternion.Euler(48, -32, 0);
            light.shadows = LightShadows.Soft;
            RenderSettings.ambientLight = new Color(.66f, .69f, .61f);
            Map = new GameObject("State atlas").AddComponent<CampaignMap>();
            Map.Build(Camera);
            hud = gameObject.AddComponent<CabinetHud>();
            battle = gameObject.AddComponent<TacticalBattle>();
            sound = gameObject.AddComponent<CabinetAudio>();
            sound.SetMuted(PlayerPrefs.GetInt("muted", 0) == 1);
            petition = gameObject.AddComponent<PetitionDocument>();
            battle.Feedback += Feedback;
            RestoreAtlas();
            if (File.Exists(SavePath)) Load();
            Refresh();
        }
        private void Update()
        {
            Camera.rect = ViewLayout.CameraRect(BattleActive ? new Rect(0, .18f, 1, .73f)
                : new Rect(245f / 1440, 100f / 900, 895f / 1440, 665f / 900));
            if (dispatchPending)
            {
                Map.SetHovered(null);
                if (Time.unscaledTime >= dispatchUntil)
                {
                    dispatchPending = false;
                    var action = afterDispatch; afterDispatch = null;
                    try { action?.Invoke(); }
                    catch (Exception error)
                    {
                        Debug.LogException(error); battle.Stop(); pendingTarget = pendingBattleId = null;
                        RestoreAtlas(); messageKey = "app.battle.failed"; messageArgs = new object[0];
                    }
                }
                return;
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                sound.SetMuted(!sound.Muted);
                if (!L.IsReviewSession) { PlayerPrefs.SetInt("muted", sound.Muted ? 1 : 0); PlayerPrefs.Save(); }
            }
            if (BattleActive || hud.BlocksMapInput || State.PendingPetition) { Map.SetHovered(null); return; }
            Map.SetHovered(Map.Pick(Input.mousePosition));
            if (Input.GetKeyDown(KeyCode.F5)) Save();
            if (Input.GetKeyDown(KeyCode.F9)) Load();
            if (Input.GetMouseButtonDown(0))
            {
                var p = Input.mousePosition;
                if (Camera.pixelRect.Contains(p))
                {
                    var id = Map.Pick(p);
                    if (!string.IsNullOrEmpty(id)) SelectRegion(id);
                }
            }
        }
        private void OnGUI()
        {
            var old = GUI.matrix;
            ViewLayout.DrawFrame();
            GUI.matrix = ViewLayout.GuiMatrix;
            if (BattleActive)
            {
                GUI.color = new Color(.10f, .17f, .13f);
                GUI.DrawTexture(new Rect(0, 0, 1440, 96), Texture2D.whiteTexture);
                GUI.color = Color.white;
                battle.DrawHud();
                if (GUI.Button(new Rect(1300, 7, 60, 25), "RU")) SetLanguage("ru");
                if (GUI.Button(new Rect(1365, 7, 60, 25), "TR")) SetLanguage("tr");
            }
            else
            {
                bool enabled = GUI.enabled;
                GUI.enabled = !dispatchPending && !State.PendingPetition;
                hud.Draw(this);
                GUI.enabled = enabled;
                if (State.PendingPetition)
                {
                    petition.Draw(this);
                    if (GUI.Button(new Rect(1015, 116, 55, 26), "RU")) SetLanguage("ru");
                    if (GUI.Button(new Rect(1080, 116, 55, 26), "TR")) SetLanguage("tr");
                }
            }
            if (dispatchPending)
            {
                GUI.color = new Color(.09f, .16f, .12f, .92f);
                GUI.DrawTexture(new Rect(0, 0, 1440, 900), Texture2D.whiteTexture);
                GUI.color = Color.white;
                if (dispatchStyle == null) dispatchStyle = new GUIStyle(GUI.skin.label) { fontSize = 30, alignment = TextAnchor.MiddleCenter, wordWrap = true };
                GUI.Label(new Rect(320, 340, 800, 200), L.Text(dispatchKey, "region." + State.SelectedRegionId), dispatchStyle);
            }
            GUI.matrix = old;
        }
        private void Refresh() { Map.Refresh(State, Mode); }
        public void Feedback(string cue)
        {
            if (sound == null) return;
            switch (cue)
            {
                case "select": cue = "paper"; break;
                case "move": case "formation": cue = "order"; break;
                case "retreat": cue = "march"; break;
                case "cannon": cue = "volley"; break;
            }
            sound.Play(cue);
        }
        public void ChoosePetition(string id)
        {
            var result = CampaignCore.ChoosePetition(State, id); Report(result); if (result.Ok) Feedback("seal");
        }
        private void Report(ActionResult result)
        {
            messageKey = result.Key; messageArgs = result.Args;
            Refresh();
            if (result.Ok) WriteSave(false);
        }
        public void SelectRegion(string id)
        {
            if (BattleActive || dispatchPending || CampaignCore.Region(State, id) == null) return;
            State.SelectedRegionId = id; Refresh(); Feedback("paper");
        }
        public void Act(string action)
        {
            if (BattleActive || dispatchPending) return;
            var result = CampaignCore.Act(State, action, State.SelectedRegionId); Report(result);
            if (result.Ok) { Feedback(action); Map.Pulse(State.SelectedRegionId); }
        }
        public void NextWeek()
        {
            if (BattleActive || dispatchPending) return;
            var result = CampaignCore.NextWeek(State); Report(result); if (result.Ok) Feedback("week");
        }
        public void SetMode(string mode) { Mode = mode; Refresh(); }
        public void SetLanguage(string language) { L.SetLanguage(language); Refresh(); }
        public void NewCampaign()
        {
            if (BattleActive || dispatchPending) return;
            State = CampaignCore.Create(); Mode = "control"; messageKey = "app.welcome"; messageArgs = new object[0];
            Map.ResetPresentation(); Refresh(); WriteSave(false);
        }
        public void March()
        {
            if (BattleActive || dispatchPending) return;
            var result = CampaignCore.March(State, State.SelectedRegionId);
            if (!result.RequiresBattle) { Report(result); if (result.Ok) Feedback("march"); return; }
            var arrival = CampaignCore.PreviewMarch(State, State.SelectedRegionId);
            pendingTarget = State.SelectedRegionId;
            pendingBattleId = "battle-" + State.Week + "-" + State.Moves + "-" + State.ArmyRegionId + "-" + pendingTarget;
            WriteSave(false);
            Dispatch("app.dispatch", () =>
            {
                Map.SetVisible(false);
                var commander = State.Characters.Find(c => c.Id == "dumas");
                battle.Begin(new BattleSetup { Troops = State.Troops, Supply = arrival.Supply, Morale = arrival.Morale,
                    Fatigue = arrival.Fatigue, CommanderCompetence = commander == null ? 60 : commander.Competence,
                    CampaignMoraleAfterBattle = (won, morale) => CampaignCore.BattleReturnMorale(arrival.Morale, morale, won),
                    Seed = 1789 + State.Week * 31 + State.Moves, RegionNameKey = "region." + pendingTarget }, Camera, CompleteBattle);
            });
        }
        private void CompleteBattle(BattleOutcome outcome)
        {
            if (pendingBattleId == null) return;
            var result = CampaignCore.ResolveBattle(State, pendingTarget, pendingBattleId, outcome.Won, outcome.Casualties, outcome.EndingMorale);
            pendingBattleId = null; pendingTarget = null;
            if (result.Ok) CampaignCore.RecoverMilitarySupplies(State, outcome.MilitarySuppliesRecovered);
            battle.Stop(); RestoreAtlas(); Report(result);
            // The report's single return button restores the atlas immediately.
        }
        private void Dispatch(string key, Action action)
        {
            dispatchKey = key; afterDispatch = action; dispatchUntil = Time.unscaledTime + .9f; dispatchPending = true;
        }
        private void RestoreAtlas()
        {
            Camera.orthographic = true; Camera.orthographicSize = 28.3f;
            Camera.transform.SetPositionAndRotation(new Vector3(-5.8f, 65, 2.7f), Quaternion.Euler(90, 0, 0));
            Camera.rect = ViewLayout.CameraRect(new Rect(245f / 1440, 100f / 900, 895f / 1440, 665f / 900));
            Camera.backgroundColor = new Color(.655f, .729f, .69f);
            Camera.nearClipPlane = .1f; Camera.farClipPlane = 200;
            Map.SetVisible(true); Refresh();
        }
        public void Save() { if (!BattleActive && !dispatchPending) WriteSave(true); }
        private void WriteSave(bool notify)
        {
            try
            {
                CampaignCore.Validate(State);
                Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
                string temporary = SavePath + ".tmp";
                File.WriteAllText(temporary, JsonUtility.ToJson(new SaveFile { State = State }, true));
                if (File.Exists(SavePath)) File.Replace(temporary, SavePath, SavePath + ".bak");
                else File.Move(temporary, SavePath);
                if (notify) { messageKey = "app.saved"; messageArgs = new object[0]; }
            }
            catch (Exception error) { Debug.LogException(error); messageKey = "app.save.failed"; messageArgs = new object[0]; }
        }
        public void Load()
        {
            if (BattleActive || dispatchPending) return;
            try
            {
                if (!File.Exists(SavePath)) { messageKey = "app.save.none"; messageArgs = new object[0]; return; }
                var save = JsonUtility.FromJson<SaveFile>(File.ReadAllText(SavePath));
                if (save == null || save.Version != 1 || save.State == null) throw new InvalidDataException("Unsupported save");
                CampaignCore.Validate(save.State); State = save.State;
                messageKey = "app.loaded"; messageArgs = new object[0]; Map.ResetPresentation(); Refresh();
            }
            catch (Exception error) { Debug.LogException(error); messageKey = "app.load.failed"; messageArgs = new object[0]; }
        }
    }
}
