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
        private int expectedWidth=1440,expectedHeight=900;
        private CampaignState battleCampaignBefore;
        private MarchPreview battleArrival;
        private string battleTarget, battleId;
        private BattleSnapshot acceptedBattle;

        [Serializable] private sealed class BattleEvidence
        {
            public string BattleId, OriginRegionId, TargetRegionId;
            public CampaignState CampaignBefore;
            public MarchPreview Arrival;
            public BattleSnapshot Battle;
        }

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
            var launchArgs=Environment.GetCommandLineArgs();
            int wi=Array.IndexOf(launchArgs,"-screen-width"),hi=Array.IndexOf(launchArgs,"-screen-height");
            if(wi>=0&&(wi+1>=launchArgs.Length||!int.TryParse(launchArgs[wi+1],out expectedWidth)))throw new InvalidDataException("Invalid review width.");
            if(hi>=0&&(hi+1>=launchArgs.Length||!int.TryParse(launchArgs[hi+1],out expectedHeight)))throw new InvalidDataException("Invalid review height.");
            if(expectedWidth<640||expectedWidth>7680||expectedHeight<480||expectedHeight>4320)throw new InvalidDataException("Review dimensions outside supported range.");
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
                    case "role-menu": RequireIdle(app); app.BeginRoleSelection(); break;
                    case "role-cancel":
                        if (!app.ChoosingRole || !app.CanCancelRoleSelection) throw new InvalidOperationException("No existing campaign to resume.");
                        app.CancelRoleSelection(); break;
                    case "role-start":
                        RequireChoice(value, "crown", "assembly", "army");
                        if (!app.ChoosingRole) throw new InvalidOperationException("Open role selection before accepting an appointment.");
                        app.StartCampaign(value); break;
                    case "mandate":
                        RequireIdle(app); RequireChoice(value, "issue", "fulfil", "break");
                        if (value == "issue") app.IssueMandate();
                        else app.ResolveMandate(CampaignCore.MandateId(app.State.Obligation), value);
                        break;
                    case "mandate-terms": RequireIdle(app); app.GetComponent<CabinetHud>().ShowMandateTerms(); break;
                    case "first-report":
                        RequireChoice(value,"open","close");
                        if(value=="open")app.GetComponent<CabinetHud>().OpenCommission();else app.GetComponent<CabinetHud>().CloseCommission(app);
                        break;
                    case "patron-repair": RequireIdle(app); app.RepairPatronTrust(); break;
                    case "accord": RequireChoice(value, "grant"); RequireIdle(app); app.GrantRegionalAccord(); break;
                    case "victory":
                        RequireChoice(value, "recognize", "bonus", "decline"); RequireIdle(app);
                        app.ResolveVictory(app.State.PendingVictoryId, value); break;
                    case "victory-close": RequireIdle(app); app.GetComponent<CabinetHud>().CloseVictoryDecision(); break;
                    case "forage":
                        RequireChoice(value, "veto"); RequireIdle(app);
                        app.VetoDumasInitiative(app.State.DumasForageDueWeek); break;
                    case "establishment":
                        RequireIdle(app);
                        string[] establishment = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (establishment.Length != 2 ||
                            (establishment[0] != "budget" && establishment[0] != "campaign") ||
                            !int.TryParse(establishment[1], NumberStyles.None, CultureInfo.InvariantCulture, out int targetTroops) ||
                            targetTroops < 0 || targetTroops > CampaignCore.MaximumArmyTarget ||
                            (establishment[0] == "campaign" && targetTroops != 0))
                            throw new InvalidDataException("establishment requires budget and a nonnegative target, or campaign 0.");
                        app.SetArmyEstablishment(establishment[0], targetTroops); break;
                    case "commission":
                        RequireIdle(app); RequireChoice(value, "grant", "recruit", "revoke");
                        if(value=="grant")app.GrantOfficerCommission();
                        else if(value=="recruit")app.RecruitThroughDumas();
                        else app.RevokeOfficerCommission();
                        break;
                    case "lang": RequireChoice(value, "ru", "tr"); app.SetLanguage(value); break;
                    case "reform":
                        RequireIdle(app);
                        string[] reform = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (reform.Length == 1 && reform[0] == "end") app.EndRegionalReform();
                        else if (reform.Length == 2 && (reform[0] == "begin" || reform[0] == "draft"))
                        {
                            RequireChoice(reform[1], "provisioning", "commerce");
                            if (reform[0] == "begin") app.BeginRegionalReform(app.State.SelectedRegionId, reform[1]);
                            else app.GetComponent<CabinetHud>().PreviewRegionalReform(reform[1]);
                        }
                        else throw new InvalidDataException("reform requires draft/begin provisioning/commerce, or end.");
                        break;
                    case "mode": RequireChoice(value, "control", "unrest", "tax", "army", "food", "influence"); app.SetMode(value); break;
                    case "select": RequireIdle(app); app.SelectRegion(value); break;
                    case "atlas":
                        RequireIdle(app); RequireChoice(value,"world","europe","france","region","oblique","clean","panels");
                        if(value=="world")app.StrategyCamera.SetView(Vector3.zero,3000,0,85);
                        else if(value=="europe")app.StrategyCamera.SetView(AtlasProjection.Project(12,49),570,0,72);
                        else if(value=="france")app.StrategyCamera.SetView(AtlasProjection.Project(2.3f,46.6f),150,0,65);
                        else if(value=="region")app.StrategyCamera.SetView(app.Map.RegionWorld(app.State.SelectedRegionId),35,-25,58);
                        else if(value=="oblique")app.StrategyCamera.SetView(app.Map.RegionWorld(app.State.SelectedRegionId),48,95,40);
                        else app.GetComponent<CabinetHud>().PanelsHidden=value=="clean";
                        yield return new WaitForSecondsRealtime(1.3f); break;
                    case "act": RequireIdle(app); app.Act(value); break;
                    case "desk":
                        RequireIdle(app);
                        if(value=="open")app.OpenBordeauxDesk();
                        else if(value.StartsWith("view ",StringComparison.Ordinal))
                        {
                            string page=value.Substring(5);RequireChoice(page,"report","outbox","draft");
                            app.GetComponent<CabinetHud>().OpenCorrespondencePage(app.State,page);
                        }
                        else
                        {
                            var args=value.Split(' ');Arguments(args,3);
                            RequireChoice(args[0],"bread","tax","order","report");RequireChoice(args[1],"strict","mission");RequireChoice(args[2],"normal","express");
                            app.SendCabinetOrder(args[0],args[1],args[2]=="express");
                        }
                        break;
                    case "week": RequireIdle(app); app.NextWeek(); break;
                    case "march": RequireIdle(app); if(app.State.World==null)RememberBattleContext(app); app.March(); break;
                    case "time":
                        RequireChoice(value,"pause","1","2","3");app.SetWorldSpeed(value=="pause"?0:int.Parse(value,CultureInfo.InvariantCulture));break;
                    case "world": yield return WorldCommand(app,value); break;
                    case "petition": RequireIdle(app); app.ChoosePetition(value); break;
                    case "save": RequireIdle(app); app.Save(); break;
                    case "load": RequireIdle(app); app.Load(); break;
                    case "panel":
                        RequireChoice(value, "council", "economy", "journal", "mandate", "accord", "victory", "initiative", "establishment", "officers", "reform");
                        app.GetComponent<CabinetHud>().OpenDocument(value); break;
                    case "scroll":
                        RequireIdle(app);
                        string[] scroll = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (scroll.Length != 2 || (scroll[0] != "document" && scroll[0] != "province") ||
                            !float.TryParse(scroll[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float offset) ||
                            float.IsNaN(offset) || float.IsInfinity(offset) || offset < 0 || offset > 5000)
                            throw new InvalidDataException("scroll requires document or province and an offset between 0 and 5000.");
                        Field(typeof(CabinetHud), scroll[0] == "document" ? "documentScroll" : "provinceScroll")
                            .SetValue(app.GetComponent<CabinetHud>(), new Vector2(0, offset)); break;
                    case "retreat":
                        if (!app.BattleActive) throw new InvalidOperationException("Cannot retreat outside battle.");
                        CheckOrder(app.GetComponent<TacticalBattle>().Retreat()); break;
                    case "accept":
                        if (!app.BattleActive) throw new InvalidOperationException("Cannot accept outside battle.");
                        var battle = app.GetComponent<TacticalBattle>();
                        acceptedBattle = battle.CaptureSnapshot();
                        CheckOrder(battle.AcceptReport()); break;
                    case "battle": yield return BattleCommand(app, value); break;
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
            object actual = key == "BattlePaused" || key == "BattleEnded" || key == "BattleWon" || key == "BattleHasOutcome" || key == "BattleCanVolley" || key == "BattleSelectionArrived" || key == "BattleEnemyTroops" || key == "BattleOurTroops"
                ? BattleExpectation(app.GetComponent<TacticalBattle>().CaptureSnapshot(), key) :
                key == "PatronRelationship" ? PatronRelationship(app.State) :
                key == "ChoosingRole" ? (object)app.ChoosingRole : key == "MandateDue" ? CampaignCore.MandateDue(app.State) :
                key == "HasObligation" ? app.State.Obligation != null : key == "HasAccord" ? CampaignCore.HasRegionalAccord(app.State) :
                key == "HasPendingVictory" ? CampaignCore.HasPendingVictory(app.State) :
                key == "HasDumasInitiative" ? CampaignCore.HasDumasInitiative(app.State) :
                key == "HasArmyEstablishment" ? CampaignCore.HasArmyEstablishment(app.State) :
                key == "HasOfficerCommission" ? CampaignCore.HasOfficerCommission(app.State) :
                key == "HasRegionalReform" ? CampaignCore.HasRegionalReform(app.State) :
                key == "ReformStatus" ? (object)CampaignCore.GetRegionalReformTerms(app.State).StatusId :
                key == "ResistanceTroops" ? (object)CampaignCore.GetRegionalResistance(app.State,app.State.SelectedRegionId).EnemyTroops :
                key == "UrbanApproval" ? (object)app.State.Factions.Find(faction=>faction.Id=="urban").Approval :
                key == "ResistanceActive" ? (object)CampaignCore.GetRegionalResistance(app.State,app.State.SelectedRegionId).RequiresBattle :
                key == "CommissionRevokeGold" ? (object)CampaignCore.GetOfficerCommissionTerms(app.State).RevokeGoldCost :
                key == "DumasLoyalty" ? (object)app.State.Characters.Find(person=>person.Id=="dumas").Loyalty :
                key == "ArmyCost" ? (object)CampaignCore.Forecast(app.State).ArmyCost :
                key == "ArmyConsumption" ? (object)CampaignCore.Forecast(app.State).ArmyConsumption :
                key == "ForageFood" ? (object)CampaignCore.Forecast(app.State).ForageFood :
                key == "SelectedUnrest" ? (object)CampaignCore.Region(app.State,app.State.SelectedRegionId).Unrest :
                key == "KnownUnrest" ? (object)CampaignCore.Knowledge(app.State,app.State.SelectedRegionId).Unrest :
                key == "ReportAge" ? (object)CampaignCore.Knowledge(app.State,app.State.SelectedRegionId).AgeDays :
                key == "ReportObservedDay" ? (object)CampaignCore.Knowledge(app.State,app.State.SelectedRegionId).ObservedDay :
                key == "SelectedControl" ? CampaignCore.Region(app.State,app.State.SelectedRegionId).Control :
                key == "TaxIncome" ? CampaignCore.Forecast(app.State).TaxIncome :
                key == "WorldSpeed" ? (object)(int)app.State.World.Clock.Speed :
                key == "FirstReportResolved" ? (object)(CampaignCore.Commission(app.State)?.Resolved??false) :
                key == "FirstReportSucceeded" ? (object)(CampaignCore.Commission(app.State)?.Succeeded??false) :
                key == "FirstReportKept" ? (object)(CampaignCore.Commission(app.State)?.Kept??0) :
                key == "FirstReportBroken" ? (object)(CampaignCore.Commission(app.State)?.Broken??0) :
                key == "WorldArmyCount" ? (object)app.State.World.Armies.Count :
                key == "WorldBattleCount" ? (object)app.State.World.Battles.Count :
                key == "WorldConvoyCount" ? (object)app.State.World.Convoys.Count :
                key == "WorldActivity" ? (object)app.State.World.Army(app.State.World.PlayerArmyId).Activity.ToString() :
                key == "WorldMapVisible" ? (object)app.Map.gameObject.activeInHierarchy :
                key == "WorldHasArena" ? (object)(app.GetComponent<TacticalBattle>()!=null) :
                key == "BattleActive" ? (object)app.BattleActive : key == "Busy" ? app.Busy :
                key == "Language" ? L.Language : key == "Mode" ? app.Mode : key == "ResolvedBattleCount" ?
                app.State.ResolvedBattles.Count : Field(typeof(CampaignState), key).GetValue(app.State);
            string observed = Convert.ToString(actual, CultureInfo.InvariantCulture);
            if (observed != expected) throw new InvalidOperationException("Expected " + value + ", observed=" + observed);
            assertions++; report.Add("  PASS " + value);
        }

        private IEnumerator WorldCommand(GameApp app,string value)
        {
            var args=value.Split(' ');
            if(args[0]=="day")
            {
                Arguments(args,2);long target=(long)(Number(args[1],60)*(double)WorldClock.Day);float until=Time.realtimeSinceStartup+120;
                while(app.State.World.Clock.Milliseconds<target&&Time.realtimeSinceStartup<until)yield return null;
                if(app.State.World.Clock.Milliseconds<target)throw new TimeoutException("World did not reach requested day.");
                assertions++;yield break;
            }
            if(args[0]=="focus"){Arguments(args,1);app.FocusWorldArmy();yield return new WaitForSecondsRealtime(1);yield break;}
            if(args[0]=="close")
            {
                Arguments(args,1);var unit=app.State.World.Army(app.State.World.PlayerArmyId).Units.Find(u=>u.Id==app.State.World.SelectedUnitId);
                if(unit==null)throw new InvalidDataException("Select a world unit before the close view.");
                app.StrategyCamera.Focus(WorldMapEntities.Position(unit.Position),.035f);yield return new WaitForSecondsRealtime(1);yield break;
            }
            if(args[0]=="retreat"){Arguments(args,1);app.Simulation.Retreat(app.State.World.PlayerArmyId);yield break;}
            if(args[0]=="supplypanel"){Arguments(args,1);app.OpenWorldSupply();yield break;}
            if(args[0]=="supply"||args[0]=="stock")
            {
                Arguments(args,1);var result=args[0]=="stock"?WorldSupply.Restock(app.State,"royal-depot"):WorldSupply.Dispatch(app.State,"royal-depot",app.State.World.PlayerArmyId);
                if(!result.Ok)throw new InvalidOperationException("Supply action failed: "+result.Key);yield break;
            }
            if(args[0]=="convoy")
            {
                Arguments(args,1);var convoy=app.State.World.Convoys.FindLast(c=>c.ArmyId==app.State.World.PlayerArmyId);
                if(convoy==null)throw new InvalidOperationException("No convoy to inspect.");
                app.StrategyCamera.Focus(WorldMapEntities.Position(convoy.Position),4);yield return new WaitForSecondsRealtime(1);yield break;
            }
            if(args[0]=="unit")
            {
                Arguments(args,2);var unit=app.State.World.Army(app.State.World.PlayerArmyId).Units.Find(u=>u.Id==args[1]);
                if(unit==null)throw new InvalidDataException("Unknown world unit.");app.State.World.SelectedUnitId=unit.Id;yield break;
            }
            if(args[0]=="wait")
            {
                Arguments(args,3);RequireChoice(args[1],"contact","ended","arrived","delivered");float until=Time.realtimeSinceStartup+Number(args[2],1200);
                while(Time.realtimeSinceStartup<until)
                {
                    bool ready=args[1]=="contact"?app.BattleActive:args[1]=="ended"?app.State.World.Battles.Exists(b=>b.Ended):args[1]=="delivered"?app.State.World.Convoys.Exists(c=>c.ArmyId==app.State.World.PlayerArmyId&&c.Status==ConvoyStatus.Delivered):app.State.World.Army(app.State.World.PlayerArmyId).Activity==ArmyActivity.Holding;
                    if(ready){assertions++;yield break;}yield return null;
                }
                File.WriteAllText(Path.Combine(folder,"world-timeout-"+args[1]+".json"),JsonUtility.ToJson(app.State,true));
                throw new TimeoutException("World condition did not occur: "+args[1]);
            }
            throw new InvalidDataException("Unknown world command: "+value);
        }

        private static float PatronRelationship(CampaignState state)
        {
            string patronId = state == null ? null : CampaignCore.PatronIdForRole(state.RoleId);
            var patron = patronId == null ? null : state.Characters.Find(character => character.Id == patronId);
            if (patron == null) throw new InvalidOperationException("PatronRelationship requires a role with an existing patron.");
            return patron.Relationship;
        }

        private static object BattleExpectation(BattleSnapshot snapshot, string key)
        {
            if (!snapshot.Active) throw new InvalidOperationException("Battle expectation requires an active encounter.");
            switch (key)
            {
                case "BattlePaused": return snapshot.Paused;
                case "BattleEnded": return snapshot.Ended;
                case "BattleHasOutcome": return snapshot.HasOutcome;
                case "BattleCanVolley": return snapshot.CanVolley;
                case "BattleSelectionArrived": return snapshot.SelectionArrived;
                case "BattleEnemyTroops": return snapshot.EnemyOriginalTroops;
                case "BattleOurTroops": return snapshot.OriginalTroops;
                case "BattleWon":
                    if (!snapshot.HasOutcome) throw new InvalidOperationException("Battle outcome has not been produced.");
                    return snapshot.Won;
                default: throw new InvalidDataException("Unsupported battle expectation: " + key);
            }
        }

        private void RememberBattleContext(GameApp app)
        {
            var check = CampaignCore.CanMarch(app.State, app.State.SelectedRegionId);
            if (!check.Ok || !check.RequiresBattle) return;
            battleCampaignBefore = CampaignArchive.Deserialize(CampaignArchive.Serialize(app.State, false));
            battleTarget = app.State.SelectedRegionId;
            battleId = "battle-" + app.State.Week.ToString(CultureInfo.InvariantCulture) + "-" + app.State.Moves.ToString(CultureInfo.InvariantCulture) + "-" + app.State.ArmyRegionId + "-" + battleTarget;
            battleArrival = CampaignCore.PreviewMarch(app.State, battleTarget);
            acceptedBattle = null;
        }

        private void CheckOrder(BattleOrderResult result)
        {
            if (!result.Ok) throw new InvalidOperationException("Battle order refused: " + result.ReasonKey);
            report.Add("  order accepted; affected=" + result.AffectedCount.ToString(CultureInfo.InvariantCulture));
        }

        private static void Arguments(string[] parts, int count)
        {
            if (parts.Length != count) throw new InvalidDataException("Wrong battle command argument count.");
        }

        private static float Coordinate(string value)
        {
            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float coordinate) || float.IsNaN(coordinate) || float.IsInfinity(coordinate))
                throw new InvalidDataException("Battle coordinate must be finite: " + value);
            return coordinate;
        }

        private IEnumerator BattleCommand(GameApp app, string value)
        {
            string[] parts = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) throw new InvalidDataException("Battle command is missing.");
            var battle = app.GetComponent<TacticalBattle>();
            switch (parts[0])
            {
                case "select":
                    Arguments(parts, 3); RequireChoice(parts[2], "replace", "add", "toggle");
                    if (!int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out int slot)) throw new InvalidDataException("Battle slot must be 1..4.");
                    CheckOrder(battle.SelectPlayerRegiment(slot, (BattleSelectionMode)Enum.Parse(typeof(BattleSelectionMode), parts[2], true))); break;
                case "move": Arguments(parts, 3); CheckOrder(battle.MoveSelected(new Vector2(Coordinate(parts[1]), Coordinate(parts[2])))); break;
                case "hq": Arguments(parts,3);CheckOrder(battle.MoveHeadquarters(new Vector2(Coordinate(parts[1]),Coordinate(parts[2]))));break;
                case "intent":
                    Arguments(parts,2);RequireChoice(parts[1],"hold","reserve","flank");
                    CheckOrder(battle.SetSelectedIntent(parts[1]=="reserve"?RegimentIntent.PreserveReserve:parts[1]=="flank"?RegimentIntent.GuardFlank:RegimentIntent.Hold));break;
                case "formation":
                    Arguments(parts, 2); RequireChoice(parts[1], "line", "column", "square");
                    CheckOrder(battle.SetSelectedFormation((BattleFormation)Enum.Parse(typeof(BattleFormation), parts[1], true))); break;
                case "fire": Arguments(parts, 2); RequireChoice(parts[1], "hold", "free"); CheckOrder(battle.SetSelectedFireAtWill(parts[1] == "free")); break;
                case "volley": Arguments(parts, 1); CheckOrder(battle.VolleySelected()); break;
                case "pause": Arguments(parts, 2); RequireChoice(parts[1], "on", "off"); CheckOrder(battle.SetPaused(parts[1] == "on")); break;
                case "state":
                    Arguments(parts, 2);
                    if (!battle.Active) throw new InvalidOperationException("Battle state requires an active encounter.");
                    WriteBattleState(parts[1], battle.CaptureSnapshot()); break;
                case "wait":
                    Arguments(parts, 3); RequireChoice(parts[1], "active", "arrived", "volley-ready", "ended");
                    yield return WaitForBattle(app, battle, parts[1], Number(parts[2])); break;
                case "verify-return": Arguments(parts, 1); VerifyBattleReturn(app); break;
                default: throw new InvalidDataException("Unknown battle command: " + parts[0]);
            }
        }

        private void WriteBattleState(string name, BattleSnapshot snapshot)
        {
            var evidence = new BattleEvidence { BattleId = battleId, OriginRegionId = battleCampaignBefore == null ? null : battleCampaignBefore.ArmyRegionId,
                TargetRegionId = battleTarget, CampaignBefore = battleCampaignBefore, Arrival = battleArrival, Battle = snapshot };
            WriteNew(ArtifactPath(name, ".json"), JsonUtility.ToJson(evidence, true));
            states.Add(name + ".json");
        }

        private IEnumerator WaitForBattle(GameApp app, TacticalBattle battle, string condition, float limit)
        {
            float until = Time.realtimeSinceStartup + limit;
            while (Time.realtimeSinceStartup < until)
            {
                BattleSnapshot snapshot = battle.CaptureSnapshot();
                bool ready = condition == "active" ? snapshot.Active : condition == "ended" ? snapshot.Active && snapshot.Ended && snapshot.HasOutcome :
                    condition == "arrived" ? snapshot.Active && !snapshot.Ended && snapshot.SelectionArrived : snapshot.CanVolley;
                if (ready) yield break;
                if ((condition != "active" && !snapshot.Active) || (condition != "active" && condition != "ended" && snapshot.Ended) || (condition == "active" && !snapshot.Active && !app.Busy))
                {
                    WriteBattleState("battle-wait-failed-" + commands.ToString(CultureInfo.InvariantCulture), snapshot);
                    throw new InvalidOperationException("Encounter cannot satisfy battle wait " + condition + ".");
                }
                yield return null;
            }
            BattleSnapshot final = battle.CaptureSnapshot();
            WriteBattleState("battle-timeout-" + commands.ToString(CultureInfo.InvariantCulture), final);
            throw new TimeoutException("Battle wait " + condition + " exceeded " + limit.ToString(CultureInfo.InvariantCulture) + " real seconds; paused=" + final.Paused + ".");
        }

        private void VerifyBattleReturn(GameApp app)
        {
            RequireIdle(app);
            if (battleCampaignBefore == null || battleArrival == null || acceptedBattle == null || !acceptedBattle.HasOutcome)
                throw new InvalidOperationException("No observed battle and accepted report to compare.");
            var resistance = CampaignCore.GetRegionalResistance(battleCampaignBefore, battleTarget);
            int deployedEnemy = 0;
            foreach (var regiment in acceptedBattle.Regiments) if (!regiment.Player) deployedEnemy += regiment.Original;
            if (resistance == null || !resistance.RequiresBattle || acceptedBattle.EnemyOriginalTroops != resistance.EnemyTroops || deployedEnemy != resistance.EnemyTroops)
                throw new InvalidOperationException("Observed enemy deployment does not match the original region's resistance.");
            var after = app.State;
            if (after.Troops != battleCampaignBefore.Troops - acceptedBattle.Casualties ||
                after.ArmyRegionId != (acceptedBattle.Won ? battleTarget : battleCampaignBefore.ArmyRegionId) ||
                Mathf.Abs(after.Morale - acceptedBattle.CampaignReturnMorale) > .001f ||
                after.Food != battleArrival.FoodAfter || after.Moves != battleArrival.MovesAfter ||
                after.MilitarySupplies != Math.Min(100000000, battleArrival.MilitarySuppliesAfter + acceptedBattle.MilitarySuppliesRecovered) ||
                after.ResolvedBattles.Count != battleCampaignBefore.ResolvedBattles.Count + 1 || !after.ResolvedBattles.Contains(battleId))
                throw new InvalidOperationException("Campaign does not match the naturally observed battle report and march costs.");
            foreach (string previous in battleCampaignBefore.ResolvedBattles)
                if (!after.ResolvedBattles.Contains(previous)) throw new InvalidOperationException("Earlier battle history was lost.");
            CampaignCore.Validate(after);
            assertions++; report.Add("  PASS battle return; won=" + acceptedBattle.Won + "; casualties=" + acceptedBattle.Casualties.ToString(CultureInfo.InvariantCulture) + "; enemy=" + deployedEnemy.ToString(CultureInfo.InvariantCulture));
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

        private bool CompletePng(string path)
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
            if (width != expectedWidth || height != expectedHeight) throw new InvalidDataException("Expected "+expectedWidth+"x"+expectedHeight+" frame, observed " + width + "x" + height);
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
        private static float Number(string value,float maximum=120)
        {
            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed) ||
                float.IsNaN(parsed) || float.IsInfinity(parsed) || parsed <= 0 || parsed > maximum)
                throw new InvalidDataException("Duration must be greater than 0 and at most "+maximum+" seconds: " + value);
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
