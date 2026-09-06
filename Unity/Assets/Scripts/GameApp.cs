using System;
using System.IO;
using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class GameApp : MonoBehaviour
    {
        public CampaignState State { get; private set; }
        public CampaignState ViewState { get; private set; }
        public string Mode { get; private set; } = "control";
        public string Message => L.Text(messageKey, messageArgs);
        public CampaignMap Map { get; private set; }
        public Camera Camera { get; private set; }
        public StrategicCamera StrategyCamera { get; private set; }
        private float lastMapClick;
        private string lastMapClickRegion;
        public bool BattleActive => State?.World!=null && State.World.HasCombat;
        public bool Busy => Simulation==null;
        public bool ChoosingRole { get; private set; }
        public bool CanCancelRoleSelection => hasCampaign;
        private bool hasCampaign;
        private bool CampaignInputBlocked => Busy || ChoosingRole;
        private CabinetHud hud;
        private CabinetAudio sound;
        private PetitionDocument petition;
        private RoleSelection roleSelection;
        private MandateDocument mandateDocument;
        private string messageKey = "app.welcome";
        private object[] messageArgs = new object[0];
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
            StrategyCamera = gameObject.AddComponent<StrategicCamera>();
            StrategyCamera.Initialize(Camera);
            hud = gameObject.AddComponent<CabinetHud>();
            sound = gameObject.AddComponent<CabinetAudio>();
            sound.SetMuted(PlayerPrefs.GetInt("muted", 0) == 1);
            petition = gameObject.AddComponent<PetitionDocument>();
            roleSelection = gameObject.AddComponent<RoleSelection>();
            mandateDocument = gameObject.AddComponent<MandateDocument>();
            InitializeContinuousWorld();
            RestoreAtlas();
            if (File.Exists(SavePath)) Load();
            if (!hasCampaign) BeginRoleSelection();
            Refresh();
        }
        private void Update()
        {
            Camera.rect = ViewLayout.CameraRect(new Rect(0, 0, 1, 1));
            UpdateContinuousWorld();
            bool pointerOnMap = !hud.IsPointerOverInterface(ViewLayout.ToCanvas(Input.mousePosition));
            StrategyCamera.Tick(!CampaignInputBlocked && !ShowPendingDocument && !hud.BlocksMapInput && pointerOnMap);
            if (Input.GetKeyDown(KeyCode.M))
            {
                sound.SetMuted(!sound.Muted);
                if (!L.IsReviewSession) { PlayerPrefs.SetInt("muted", sound.Muted ? 1 : 0); PlayerPrefs.Save(); }
            }
            if (CampaignInputBlocked || ShowPendingDocument || hud.BlocksMapInput) { Map.SetHovered(null); return; }
            Map.SetHovered(Map.Pick(Input.mousePosition));
            if (Input.GetKeyDown(KeyCode.F5)) Save();
            if (Input.GetKeyDown(KeyCode.F9)) Load();
            if (!pointerOnMap) { Map.SetHovered(null); return; }
            if (Input.GetKeyDown(KeyCode.F)) StrategyCamera.Focus(Map.RegionWorld(State.SelectedRegionId));
            if (Input.GetKeyDown(KeyCode.G)) FocusWorldArmy();
            if (Input.GetMouseButtonDown(0))
            {
                var p = Input.mousePosition;
                if (Camera.pixelRect.Contains(p))
                {
                    if(SelectWorldEntity(p))return;
                    var id = Map.Pick(p);
                    if (!string.IsNullOrEmpty(id))
                    {
                        SelectRegion(id);
                        if (lastMapClickRegion == id && Time.unscaledTime - lastMapClick < .32f) StrategyCamera.Focus(Map.RegionWorld(id));
                        lastMapClickRegion = id; lastMapClick = Time.unscaledTime;
                    }
                }
            }
        }
        private void OnGUI()
        {
            HandleWorldTimeEvent(Event.current);
            if (!CampaignInputBlocked && !ShowPendingDocument && !hud.BlocksMapInput &&
                !hud.IsPointerOverInterface(ViewLayout.ToCanvas(Input.mousePosition)))
                StrategyCamera.HandleMapEvent(Event.current,Input.mousePosition);
            var old = GUI.matrix;
            ViewLayout.DrawFrame();
            GUI.matrix = ViewLayout.GuiMatrix;
            if (ChoosingRole)
            {
                roleSelection.Draw(this);
                GUI.matrix = old;
                return;
            }
            {
                bool enabled = GUI.enabled;
                GUI.enabled = !ShowPendingDocument;
                hud.Draw(this);
                GUI.enabled = enabled;
                if (ShowPendingDocument && State.PendingPetition)
                {
                    petition.Draw(this);
                    petition.DrawLanguageControls(this);
                }
                else if (ShowPendingDocument && CampaignCore.MandateDue(State)) mandateDocument.Draw(this);
            }
            GUI.matrix = old;
        }
        private void Refresh()
        {
            ViewState = State;
            var desk = CampaignCore.Desk(State);
            if(desk != null)
            {
                ViewState = CampaignArchive.Deserialize(CampaignArchive.Serialize(State, false));
                var report = CampaignCore.Knowledge(State, desk.RegionId);
                var visible = CampaignCore.Region(ViewState, desk.RegionId);
                visible.Unrest = report.Unrest; visible.Control = report.Control; visible.EliteLoyalty = report.EliteLoyalty;
                // UI forecasts cannot advance or inspect messages that have not reached the cabinet.
                ViewState.Correspondence.Clear();
            }
            Map.Refresh(ViewState, Mode);
        }
        public void OpenBordeauxDesk()
        {
            if(CampaignInputBlocked)return;
            if(CampaignCore.Desk(State)==null)Report(CampaignCore.OpenCorrespondence(State));
            if(CampaignCore.Desk(State)!=null){SelectRegion("guyenne");StrategyCamera.Focus(Map.RegionWorld("guyenne"),100);}
        }
        public void SendCabinetOrder(string intent,string autonomy,bool express)
        {
            if(CampaignInputBlocked)return;
            var result=CampaignCore.SendCabinetOrder(State,intent,autonomy,express);Report(result);
            if(result.Ok)Feedback("seal");
        }
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
            if(result.Ok)Simulation?.ImportPlayerArmy();
            messageKey = result.Key; messageArgs = result.Args;
            Refresh();
            if (result.Ok) WriteSave(false);
        }
        public void SelectRegion(string id)
        {
            if (CampaignInputBlocked || CampaignCore.Region(State, id) == null) return;
            State.SelectedRegionId = id; hud.NotifyRegionSelected(); Refresh(); Feedback("paper");
        }
        public void Act(string action)
        {
            if (CampaignInputBlocked) return;
            var desk=CampaignCore.Desk(State);
            if(desk!=null && State.SelectedRegionId==desk.RegionId)
            {SendCabinetOrder(action,"strict",false);return;}
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
            if (Busy) return;
            State = CampaignCore.Create(); Mode = "control"; messageKey = "app.welcome"; messageArgs = new object[0];
            InitializeContinuousWorld();
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
            InitializeContinuousWorld();
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
        public void VetoDumasInitiative(int expectedDueWeek)
        {
            if (CampaignInputBlocked) return;
            var result = CampaignCore.VetoDumasInitiative(State, expectedDueWeek);
            Report(result);
            if (result.Ok) { hud.OpenDocument("initiative"); Feedback("quill"); }
        }
        public void SetArmyEstablishment(string policyId, int targetTroops)
        {
            if (CampaignInputBlocked) return;
            var result = CampaignCore.SetArmyEstablishment(State, policyId, targetTroops);
            Report(result);
            if (result.Ok) { hud.OpenDocument("establishment"); Feedback("quill"); }
        }
        public void GrantOfficerCommission()
        {
            if (CampaignInputBlocked) return;
            var result = CampaignCore.GrantOfficerCommission(State);
            Report(result);
            if (result.Ok) { hud.OpenDocument("officers"); Feedback("seal"); }
        }
        public void BeginRegionalReform(string regionId, string modeId)
        {
            if (CampaignInputBlocked) return;
            var result = CampaignCore.BeginRegionalReform(State, regionId, modeId);
            Report(result);
            if (result.Ok) { hud.OpenDocument("reform"); Map.Pulse(regionId); Feedback("seal"); }
        }
        public void EndRegionalReform()
        {
            if (CampaignInputBlocked) return;
            string regionId = State.ReformRegionId;
            var result = CampaignCore.EndRegionalReform(State);
            Report(result);
            if (result.Ok) { hud.OpenDocument("reform"); Map.Pulse(regionId); Feedback("quill"); }
        }
        public void RecruitThroughDumas()
        {
            if (CampaignInputBlocked) return;
            var result = CampaignCore.RecruitThroughDumas(State);
            Report(result);
            if (result.Ok) { hud.OpenDocument("officers"); Map.Pulse(State.ArmyRegionId); Feedback("march"); }
        }
        public void RevokeOfficerCommission()
        {
            if (CampaignInputBlocked) return;
            var result = CampaignCore.RevokeOfficerCommission(State);
            Report(result);
            if (result.Ok) { hud.OpenDocument("officers"); Feedback("quill"); }
        }
        public void March()
        {
            if (CampaignInputBlocked) return;
            var region=Array.Find(Map.WorldData.regions,r=>r.id==State.SelectedRegionId);
            if(region==null)return;
            var result=Simulation.March(State.World.PlayerArmyId,region.seatId);Report(result);if(result.Ok)Feedback("march");
        }
        private void RestoreAtlas()
        {
            StrategyCamera.Resume();
            Camera.rect = ViewLayout.CameraRect(new Rect(0, 0, 1, 1));
            Camera.backgroundColor = new Color(.51f, .69f, .71f);
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
            if (Busy) return;
            try
            {
                if (!File.Exists(SavePath)) { messageKey = "app.save.none"; messageArgs = new object[0]; return; }
                var restored = CampaignArchive.Deserialize(File.ReadAllText(SavePath));
                if(restored.World==null){messageKey="world.legacy_save";messageArgs=new object[0];return;}
                State = restored; hasCampaign = true; ChoosingRole = false;
                InitializeContinuousWorld();
                messageKey = "app.loaded"; messageArgs = new object[0]; Map.ResetPresentation(); Refresh();
            }
            catch (NotSupportedException) { messageKey = "world.legacy_save"; messageArgs = new object[0]; }
            catch (Exception error) { Debug.LogException(error); messageKey = "app.load.failed"; messageArgs = new object[0]; }
        }
    }
}
