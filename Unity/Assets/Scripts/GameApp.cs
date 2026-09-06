using System;
using System.IO;
using UnityEngine;

namespace PowerAboveAll
{
    public sealed class GameApp : MonoBehaviour
    {
        public CampaignState State { get; private set; }
        public string Mode { get; private set; } = "control";
        public string Message => L.Text(messageKey, messageArgs);
        public CampaignMap Map { get; private set; }
        public Camera Camera { get; private set; }
        public bool BattleActive => battle != null && battle.Active;
        public bool Busy => BattleActive || dispatchPending;
        public bool ChoosingRole { get; private set; }
        public bool CanCancelRoleSelection => hasCampaign;
        private bool hasCampaign;
        private bool CampaignInputBlocked => Busy || ChoosingRole || State.PendingPetition || CampaignCore.MandateDue(State);
        private CabinetHud hud;
        private TacticalBattle battle;
        private CabinetAudio sound;
        private PetitionDocument petition;
        private RoleSelection roleSelection;
        private MandateDocument mandateDocument;
        private string messageKey = "app.welcome";
        private object[] messageArgs = new object[0];
        private string pendingTarget, pendingBattleId;
        private float dispatchUntil;
        private Action afterDispatch;
        private bool dispatchPending;
        private string dispatchKey;
        private GUIStyle dispatchStyle;
        private GUIStyle languageStyle, battleIdentityStyle;
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
            Camera.clearFlags = CameraClearFlags.SolidColor;
            var light = new GameObject("Afternoon light", typeof(Light)).GetComponent<Light>();
            light.type = LightType.Directional; light.intensity = 1.15f;
            light.transform.rotation = Quaternion.Euler(48, -32, 0);
            light.shadows = LightShadows.Soft;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(.66f, .69f, .61f);
            Map = new GameObject("State atlas").AddComponent<CampaignMap>();
            Map.Build(Camera);
            hud = gameObject.AddComponent<CabinetHud>();
            battle = gameObject.AddComponent<TacticalBattle>();
            sound = gameObject.AddComponent<CabinetAudio>();
            sound.SetMuted(PlayerPrefs.GetInt("muted", 0) == 1);
            petition = gameObject.AddComponent<PetitionDocument>();
            roleSelection = gameObject.AddComponent<RoleSelection>();
            mandateDocument = gameObject.AddComponent<MandateDocument>();
            battle.Feedback += Feedback;
            RestoreAtlas();
            if (File.Exists(SavePath)) Load();
            if (!hasCampaign) BeginRoleSelection();
            Refresh();
        }
        private void Update()
        {
            Camera.rect = ViewLayout.CameraRect(BattleActive ? ViewLayout.BattleViewport
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
            if (CampaignInputBlocked || hud.BlocksMapInput) { Map.SetHovered(null); return; }
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
            if (ChoosingRole)
            {
                roleSelection.Draw(this);
                GUI.matrix = old;
                return;
            }
            if (BattleActive)
            {
                GUI.color = new Color(.141f, .231f, .216f);
                GUI.DrawTexture(new Rect(0, 0, 1440, 36), Texture2D.whiteTexture);
                // Kameranın altındaki aralığı da boya; önceki atlas karesi görünmemeli.
                GUI.DrawTexture(new Rect(0, 729, 1440, 9), Texture2D.whiteTexture);
                GUI.color = Color.white;
                battle.DrawHud();
                DrawBattleLanguageBar();
            }
            else
            {
                bool enabled = GUI.enabled;
                GUI.enabled = !dispatchPending && !State.PendingPetition && !CampaignCore.MandateDue(State);
                hud.Draw(this);
                GUI.enabled = enabled;
                if (State.PendingPetition)
                {
                    petition.Draw(this);
                    petition.DrawLanguageControls(this);
                }
                else if (CampaignCore.MandateDue(State)) mandateDocument.Draw(this);
            }
            if (dispatchPending)
            {
                float arrival = Mathf.SmoothStep(0, 1, Mathf.Clamp01((.9f - (dispatchUntil - Time.unscaledTime)) / .28f));
                GUI.color = new Color(.08f, .14f, .13f, .48f * arrival);
                GUI.DrawTexture(new Rect(0, 0, 1440, 900), Texture2D.whiteTexture);
                var document = new Rect(400 + (1 - arrival) * 30, 318, 640, 230);
                GUI.color = new Color(.04f, .08f, .07f, .22f * arrival);
                GUI.DrawTexture(new Rect(document.x + 6, document.y + 8, document.width, document.height), Texture2D.whiteTexture);
                GUI.color = new Color(.953f, .906f, .792f, arrival);
                GUI.DrawTexture(document, Texture2D.whiteTexture);
                GUI.color = new Color(.792f, .702f, .435f, arrival);
                GUI.DrawTexture(new Rect(document.x + 36, document.y + 27, document.width - 72, 2), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(document.x + 36, document.yMax - 28, document.width - 72, 1), Texture2D.whiteTexture);
                GUI.color = Color.white;
                if (dispatchStyle == null) dispatchStyle = new GUIStyle(GUI.skin.label) { fontSize = 30, alignment = TextAnchor.MiddleCenter, wordWrap = true };
                dispatchStyle.normal.textColor = new Color(.141f, .231f, .216f, arrival);
                GUI.Label(new Rect(document.x + 36, document.y + 35, document.width - 72, document.height - 70),
                    L.Text(dispatchKey, "region." + State.SelectedRegionId), dispatchStyle);
            }
            GUI.matrix = old;
        }
        private void Refresh() { Map.Refresh(State, Mode); }
        private void DrawBattleLanguageBar()
        {
            Color paper = new Color(.953f, .906f, .792f), muted = new Color(.69f, .75f, .68f);
            if (languageStyle == null)
            {
                languageStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleCenter };
                battleIdentityStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleLeft };
            }
            battleIdentityStyle.normal.textColor = paper;
            GUI.Label(new Rect(22, 0, 620, 36), L.Text("app.battle.identity"), battleIdentityStyle);
            for (int i = 0; i < 2; i++)
            {
                string language = i == 0 ? "ru" : "tr";
                Rect rect = new Rect(1300 + i * 64, 3, 58, 28);
                bool selected = L.Language == language;
                languageStyle.normal.textColor = selected ? paper : muted;
                GUI.Label(rect, i == 0 ? "RU" : "TR", languageStyle);
                if (selected)
                {
                    Color old = GUI.color; GUI.color = new Color(.792f, .702f, .435f);
                    GUI.DrawTexture(new Rect(rect.x + 10, rect.yMax - 1, rect.width - 20, 2), Texture2D.whiteTexture);
                    GUI.color = old;
                }
                if (GUI.Button(rect, GUIContent.none, GUIStyle.none)) SetLanguage(language);
            }
        }
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
            if (Busy || ChoosingRole) return;
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
            if (CampaignInputBlocked || CampaignCore.Region(State, id) == null) return;
            State.SelectedRegionId = id; Refresh(); Feedback("paper");
        }
        public void Act(string action)
        {
            if (CampaignInputBlocked) return;
            var result = CampaignCore.Act(State, action, State.SelectedRegionId); Report(result);
            if (result.Ok) { Feedback(action); Map.Pulse(State.SelectedRegionId); }
        }
        public void NextWeek()
        {
            if (Busy || ChoosingRole) return;
            var result = CampaignCore.NextWeek(State); Report(result); if (result.Ok) Feedback("week");
        }
        public void SetMode(string mode) { Mode = mode; Refresh(); }
        public void SetLanguage(string language) { L.SetLanguage(language); Refresh(); }
        public void NewCampaign()
        {
            if (BattleActive || dispatchPending) return;
            State = CampaignCore.Create(); Mode = "control"; messageKey = "app.welcome"; messageArgs = new object[0];
            ChoosingRole = false; hasCampaign = true;
            Map.ResetPresentation(); Refresh(); WriteSave(false);
        }
        public void BeginRoleSelection()
        {
            if (Busy) return;
            ChoosingRole = true;
            roleSelection.Open(State.RoleId);
            Map.SetHovered(null);
        }
        public void CancelRoleSelection()
        {
            if (hasCampaign) ChoosingRole = false;
        }
        public void StartCampaign(string roleId)
        {
            if (Busy || !ChoosingRole) return;
            CampaignState next = CampaignCore.Create(roleId);
            State = next; ChoosingRole = false; hasCampaign = true; Mode = "control";
            messageKey = State.Journal[0].Key; messageArgs = State.Journal[0].Args;
            Map.ResetPresentation(); hud.OpenDocument("mandate"); Refresh(); WriteSave(false); Feedback("seal");
        }
        public void IssueMandate()
        {
            if (CampaignInputBlocked) return;
            var result = CampaignCore.IssueMandate(State, State.SelectedRegionId);
            Report(result);
            if (result.Ok) { hud.OpenDocument("mandate"); Map.Pulse(State.SelectedRegionId); Feedback("seal"); }
        }
        public void ResolveMandate(string expectedId, string choice)
        {
            if (Busy || ChoosingRole || State.PendingPetition) return;
            var result = CampaignCore.ResolveMandate(State, expectedId, choice);
            Report(result);
            if (result.Ok) { hud.OpenDocument("mandate"); Feedback("seal"); }
        }
        public void RepairPatronTrust()
        {
            if (CampaignInputBlocked) return;
            var result = CampaignCore.RepairPatronTrust(State);
            Report(result);
            if (result.Ok) { hud.OpenDocument("mandate"); Feedback("quill"); }
        }
        public void GrantRegionalAccord()
        {
            if (CampaignInputBlocked) return;
            var result = CampaignCore.GrantRegionalAccord(State, State.SelectedRegionId);
            Report(result);
            if (result.Ok) { hud.OpenDocument("accord"); Map.Pulse(State.SelectedRegionId); Feedback("seal"); }
        }
        public void ResolveVictory(string expectedBattleId, string choice)
        {
            if (CampaignInputBlocked) return;
            var result = CampaignCore.ResolveVictory(State, expectedBattleId, choice);
            Report(result);
            if (result.Ok) { hud.CloseVictoryDecision(); Map.Pulse(State.ArmyRegionId); Feedback("seal"); }
        }
        public void March()
        {
            if (CampaignInputBlocked) return;
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
            if (result.Ok && CampaignCore.HasPendingVictory(State)) hud.OpenDocument("victory");
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
        public void Save() { if (!Busy && !ChoosingRole) WriteSave(true); }
        private void WriteSave(bool notify)
        {
            try
            {
                CampaignCore.Validate(State);
                Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
                string temporary = SavePath + ".tmp";
                File.WriteAllText(temporary, CampaignArchive.Serialize(State));
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
                var restored = CampaignArchive.Deserialize(File.ReadAllText(SavePath));
                State = restored; hasCampaign = true; ChoosingRole = false;
                messageKey = "app.loaded"; messageArgs = new object[0]; Map.ResetPresentation(); Refresh();
            }
            catch (Exception error) { Debug.LogException(error); messageKey = "app.load.failed"; messageArgs = new object[0]; }
        }
    }
}
