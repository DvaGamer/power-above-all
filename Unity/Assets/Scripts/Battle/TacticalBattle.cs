using System;
using System.Collections.Generic;
using UnityEngine;

namespace PowerAboveAll
{
    [Serializable]
    public class BattleSetup
    {
        public int Troops;
        public float Supply = 75, Morale = 75, Fatigue = 10, CommanderCompetence = 60;
        public int Seed = 1789;
        public string RegionNameKey = "";
        [NonSerialized] public Func<bool, float, float> CampaignMoraleAfterBattle;
    }

    [Serializable]
    public class BattleOutcome
    {
        public bool Won;
        public int Casualties;
        public float EndingMorale;
        public int MilitarySuppliesRecovered;
    }

    // Original, deliberately compact diorama. A miniature represents several people.
    // All battle state and random combat rolls advance at a seeded, fixed 20 Hz.
    public sealed class TacticalBattle : MonoBehaviour
    {
        enum Kind { Line, Militia, Cavalry, Artillery }
        enum Formation { Line, Column, Square }
        enum Condition { Steady, Pressured, Shaken, Wavering, Routing }

        sealed class Miniature
        {
            public Transform Root, Musket, Bayonet, LeftBoot, RightBoot;
            public bool Falling, Resting;
            public float FallAt, WeaponRaise;
            public Vector3 FallStart;
            public Quaternion FallRotation;
        }

        sealed class Regiment
        {
            public int Id, Original, Men, Ammo;
            public bool Player, FireAtWill = true, Moving, Routed, Withdrawn, WasHit;
            public Kind Kind;
            public Formation Formation;
            public Condition Condition;
            public float Morale, Fatigue, Cohesion = 90, Experience, Reload, Quiet, Facing;
            public float LastVolley = -100, LastHit = -100;
            public bool VisualReady;
            public Vector3 Position, Destination;
            public GameObject Root, Flag;
            public LineRenderer SelectionOutline, FireArc, OrderPath, DestinationMark;
            public readonly Vector3[] Footprint = new Vector3[5];
            public readonly Vector3[] Arc = new Vector3[27];
            public readonly Vector3[] Route = new Vector3[13];
            public readonly Vector3[] DestinationPoints = new Vector3[5];
            public readonly List<Miniature> Figures = new List<Miniature>();
        }

        sealed class Puff
        {
            public GameObject Object;
            public Vector3 Start, End, Drift, Scale;
            public float Born, Lifetime, Delay;
            public bool Projectile, Flash;
            public Renderer Renderer;
            public string Cue;
        }

        public bool Active { get; private set; }
        public event Action<string> Feedback;
        readonly List<Regiment> regiments = new List<Regiment>();
        readonly List<Regiment> selected = new List<Regiment>();
        readonly List<Puff> effects = new List<Puff>();
        readonly List<Material> materials = new List<Material>();
        readonly List<Mesh> meshes = new List<Mesh>();
        readonly List<Texture2D> hudTextures = new List<Texture2D>();
        readonly Vector3 convoy = new Vector3(4, 0, 3);
        Camera battleCamera;
        BattleSetup setup;
        Action<BattleOutcome> completion;
        BattleOutcome outcome;
        System.Random rng;
        GameObject world;
        Material grass, soil, blue, red, cream, wood, iron, skin, leaf, water, gold, smoke, flash;
        Material blueRing, redRing, orderInk;
        float accumulator, elapsed, playerHold, enemyHold, messageUntil, visualClock, campaignReturnMorale;
        bool paused, ended, delivered;
        Regiment hovered;
        string messageKey = "battle.hint";
        int originalTroops;
        const float Tick = .05f;
        GUIStyle bodyStyle, titleStyle, smallStyle, cardStyle, buttonStyle;
        GUIStyle dispatchTitle, dispatchBody, dispatchSmall, dispatchNumber;
        Font dispatchFont;
        MaterialPropertyBlock smokeProperties;

        public void Begin(BattleSetup battleSetup, Camera camera, Action<BattleOutcome> callback)
        {
            Stop();
            if (smokeProperties == null) smokeProperties = new MaterialPropertyBlock();
            setup = battleSetup ?? new BattleSetup();
            battleCamera = camera;
            if (battleCamera == null) throw new ArgumentNullException(nameof(camera));
            completion = callback;
            originalTroops = Mathf.Max(0, setup.Troops);
            rng = new System.Random(setup.Seed);
            accumulator = elapsed = playerHold = enemyHold = visualClock = campaignReturnMorale = 0;
            paused = ended = delivered = false;
            messageKey = "battle.hint";
            messageUntil = 8;
            outcome = null;
            hovered = null;
            Active = true;
            world = new GameObject("Power Above All - Crossing Diorama");
            CreateMaterials();
            BuildLandscape();
            DeployArmy(true, originalTroops);
            DeployArmy(false, Mathf.Max(200, Mathf.RoundToInt(originalTroops * .9f)));
            if (regiments.Count > 0) selected.Add(regiments[0]);
            battleCamera.rect = ViewLayout.CameraRect(new Rect(0, .18f, 1, .73f));
            battleCamera.orthographic = true;
            battleCamera.orthographicSize = 37;
            battleCamera.transform.position = new Vector3(0, 55, -40);
            battleCamera.transform.LookAt(new Vector3(0, 0, 5));
            battleCamera.backgroundColor = new Color(.69f, .75f, .71f);
            foreach (Regiment regiment in regiments) UpdateVisual(regiment, 1);
            if (originalTroops <= 0) Finish(false, false);
        }

        public void Stop()
        {
            Active = false;
            completion = null;
            if (world != null) { world.SetActive(false); ReleaseObject(world); }
            world = null;
            foreach (Material material in materials) if (material != null) ReleaseObject(material);
            foreach (Mesh mesh in meshes) if (mesh != null) ReleaseObject(mesh);
            foreach (Texture2D texture in hudTextures) if (texture != null) ReleaseObject(texture);
            materials.Clear(); meshes.Clear(); regiments.Clear(); selected.Clear(); effects.Clear();
            hudTextures.Clear(); bodyStyle = titleStyle = smallStyle = cardStyle = buttonStyle = null;
            dispatchTitle = dispatchBody = dispatchSmall = dispatchNumber = null;
            if (dispatchFont != null) ReleaseObject(dispatchFont); dispatchFont = null;
            accumulator = 0;
        }

        static void ReleaseObject(UnityEngine.Object item)
        {
            if (Application.isPlaying) Destroy(item);
            else DestroyImmediate(item);
        }

        void OnDestroy() { Stop(); }
        void OnApplicationFocus(bool focused) { accumulator = 0; if (!focused && Active && !ended) paused = true; }

        void Update()
        {
            if (!Active) return;
            if (!ended) HandleInput();
            float visualDelta = paused ? 0 : Mathf.Min(Time.unscaledDeltaTime, .1f);
            visualClock += visualDelta;
            if (!ended)
            {
                if (!paused)
                {
                    accumulator = Mathf.Min(accumulator + Mathf.Min(Time.unscaledDeltaTime, .2f), Tick * 4);
                    int steps = 0;
                    while (accumulator >= Tick && steps++ < 4 && !ended)
                    {
                        Simulate(Tick);
                        accumulator -= Tick;
                    }
                }
                else accumulator = 0;
            }
            foreach (Regiment regiment in regiments) UpdateVisual(regiment, visualDelta);
            UpdateEffects();
        }

        void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Space)) paused = !paused;
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectIndex(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectIndex(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectIndex(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SelectIndex(3);
            Vector2 pointer = ViewLayout.ToCanvas(Input.mousePosition);
            float uiY = pointer.y;
            hovered = null;
            if (pointer.x < 0 || pointer.x > ViewLayout.Width || uiY < 195 || uiY > 733 || ended) return;
            float best = 46f * ViewLayout.Scale;
            foreach (Regiment regiment in regiments)
            {
                if (!regiment.Player || regiment.Withdrawn || regiment.Routed || regiment.Men <= 0) continue;
                Vector3 screen = battleCamera.WorldToScreenPoint(regiment.Root.transform.position + Vector3.up * 2);
                float d = Vector2.Distance(screen, Input.mousePosition);
                if (screen.z > 0 && d < best) { best = d; hovered = regiment; }
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (hovered != null)
                {
                    bool additive = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                    if (!additive) selected.Clear();
                    if (selected.Contains(hovered) && additive) selected.Remove(hovered);
                    else if (!selected.Contains(hovered)) selected.Add(hovered);
                    Feedback?.Invoke("select");
                }
            }
            if (Input.GetMouseButtonDown(1) && selected.Count > 0)
            {
                Ray ray = battleCamera.ScreenPointToRay(Input.mousePosition);
                Plane ground = new Plane(Vector3.up, Vector3.zero);
                if (ground.Raycast(ray, out float distance))
                {
                    Vector3 point = ray.GetPoint(distance);
                    Vector3 centre = Vector3.zero;
                    int commandCount = 0;
                    foreach (Regiment regiment in selected)
                        if (Commandable(regiment)) { centre += regiment.Position; commandCount++; }
                    if (commandCount == 0) return;
                    centre /= commandCount;
                    foreach (Regiment regiment in selected)
                    {
                        if (!Commandable(regiment)) continue;
                        Vector3 offset = commandCount > 1 ? regiment.Position - centre : Vector3.zero;
                        regiment.Destination = Bound(point + offset);
                        regiment.Moving = true;
                    }
                    Feedback?.Invoke("move");
                }
            }
        }

        void SelectIndex(int index)
        {
            if (index >= regiments.Count || !Commandable(regiments[index])) return;
            if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) selected.Clear();
            if (!selected.Contains(regiments[index])) selected.Add(regiments[index]);
            Feedback?.Invoke("select");
        }

        void Simulate(float dt)
        {
            elapsed += dt;
            foreach (Regiment regiment in regiments)
            {
                if (regiment.Withdrawn || regiment.Men <= 0) continue;
                regiment.Reload = Mathf.Max(0, regiment.Reload - dt);
                regiment.Quiet += dt;
                if (regiment.Routed)
                {
                    regiment.Position += new Vector3(0, 0, regiment.Player ? -1 : 1) * dt * 3.8f;
                    if (Mathf.Abs(regiment.Position.z) > 35) regiment.Withdrawn = true;
                    continue;
                }
                Regiment enemy = FindEnemy(regiment);
                if (!regiment.Player) EnemyOrders(regiment, enemy);
                Move(regiment, dt);
                if (!regiment.Moving && enemy != null)
                {
                    float facing = Heading(enemy.Position - regiment.Position);
                    regiment.Facing = Mathf.MoveTowardsAngle(regiment.Facing, facing, dt * 38);
                }
                regiment.Cohesion = Mathf.Clamp(regiment.Cohesion + (regiment.Moving ? regiment.Formation == Formation.Column ? -.12f : -.7f : 2.8f) * dt, 20, 100);
                regiment.Fatigue = Mathf.Clamp(regiment.Fatigue + (regiment.Moving ? .5f : -.65f) * dt, 0, 100);
                if (regiment.Quiet > 7 && !regiment.Moving && regiment.Morale > 20)
                    regiment.Morale = Mathf.Min(regiment.Player ? Mathf.Clamp(setup.Morale, 30, 100) : 78, regiment.Morale + dt * .65f * (regiment.Player ? .5f + setup.CommanderCompetence / 100 : 1));
                if (enemy != null && regiment.FireAtWill && CanAttack(regiment, enemy)) Shoot(regiment, enemy, false);
                SetCondition(regiment);
            }
            UpdateObjective(dt);
            if (ended) return;
            int allied = 0, opposing = 0;
            foreach (Regiment regiment in regiments)
            {
                if (regiment.Routed || regiment.Withdrawn || regiment.Men <= 0) continue;
                if (regiment.Player) allied += regiment.Men; else opposing += regiment.Men;
            }
            if (allied <= originalTroops * .12f) Finish(false, false);
            else if (opposing == 0) Finish(true, false);
        }

        void UpdateEffects()
        {
            if (paused) return;
            for (int i = effects.Count - 1; i >= 0; i--)
            {
                Puff effect = effects[i];
                float age = visualClock - effect.Born - effect.Delay;
                if (age < 0) continue;
                effect.Object.SetActive(true);
                if (effect.Cue != null) { Feedback?.Invoke(effect.Cue); effect.Cue = null; }
                if (age >= effect.Lifetime) { ReleaseObject(effect.Object); effects.RemoveAt(i); continue; }
                if (effect.Projectile) effect.Object.transform.position = Vector3.Lerp(effect.Start, effect.End, age / effect.Lifetime);
                else if (effect.Flash) effect.Object.transform.localScale = effect.Scale * (1 - age / effect.Lifetime);
                else
                {
                    float life = age / effect.Lifetime;
                    effect.Object.transform.position = effect.Start + effect.Drift * age + Vector3.up * (.12f * age * age);
                    effect.Object.transform.localScale = effect.Scale * (.42f + age * .72f);
                    effect.Object.transform.rotation = Quaternion.Euler(age * 7, age * 12 + effect.Start.x * 19, age * 4);
                    Color color = new Color(.82f, .81f, .74f, Mathf.Min(1, age * 12) * .34f * Mathf.Pow(1 - life, .85f));
                    smokeProperties.SetColor("_Color", color);
                    smokeProperties.SetColor("_BaseColor", color);
                    effect.Renderer.SetPropertyBlock(smokeProperties);
                }
            }
        }

        void Move(Regiment regiment, float dt)
        {
            if (!regiment.Moving) return;
            Vector3 difference = regiment.Destination - regiment.Position; difference.y = 0;
            if (difference.magnitude < .3f) { regiment.Moving = false; return; }
            float speed = regiment.Kind == Kind.Cavalry ? 3.6f : regiment.Kind == Kind.Artillery ? .85f : 1.65f;
            speed *= regiment.Formation == Formation.Column ? 1.3f : regiment.Formation == Formation.Square ? .43f : .84f;
            speed *= Mathf.Lerp(1, .52f, regiment.Fatigue / 100);
            if (InOrchard(regiment.Position)) speed *= .65f;
            if (InCreek(regiment.Position)) { speed *= .42f; regiment.Cohesion = Mathf.Max(20, regiment.Cohesion - dt * 2); }
            Vector3 direction = difference.normalized;
            foreach (Regiment other in regiments)
            {
                if (other == regiment || other.Withdrawn || other.Routed || other.Player != regiment.Player) continue;
                Vector3 away = regiment.Position - other.Position; away.y = 0;
                float gap = away.magnitude;
                if (gap < 4.6f && gap > .01f) direction += away.normalized * ((4.6f - gap) / 4.6f) * .75f;
            }
            regiment.Position = Bound(regiment.Position + direction.normalized * Mathf.Min(speed * dt, difference.magnitude));
            regiment.Facing = Mathf.MoveTowardsAngle(regiment.Facing, Heading(difference), dt * 65);
        }

        void EnemyOrders(Regiment regiment, Regiment enemy)
        {
            if (enemy == null) return;
            bool commit = elapsed > 35 || playerHold > 8 || OwnRouted(false) > 0;
            if (regiment.Kind == Kind.Cavalry && !commit) return;
            float distance = FlatDistance(regiment.Position, enemy.Position);
            if (regiment.Kind == Kind.Artillery)
            {
                if (distance > 30 && elapsed > 14) { regiment.Destination = new Vector3(18, 0, 15); regiment.Moving = true; }
                else regiment.Moving = false;
                return;
            }
            if (playerHold > 3 && FlatDistance(regiment.Position, convoy) > 7)
            {
                regiment.Destination = convoy + new Vector3(regiment.Id % 2 == 0 ? -3 : 3, 0, 2);
                regiment.Moving = true;
                return;
            }
            float desired = regiment.Kind == Kind.Cavalry ? 2.4f : regiment.Kind == Kind.Militia ? 12 : 16;
            if (distance > desired)
            {
                regiment.Destination = enemy.Position;
                regiment.Moving = true;
            }
            else regiment.Moving = false;
            if (regiment.Kind == Kind.Cavalry && distance < 10) regiment.Formation = Formation.Line;
        }

        Regiment FindEnemy(Regiment regiment)
        {
            Regiment best = null; float minimum = float.MaxValue;
            foreach (Regiment other in regiments)
            {
                if (other.Player == regiment.Player || other.Men <= 0 || other.Withdrawn || other.Routed) continue;
                float distance = FlatDistance(regiment.Position, other.Position);
                if (distance < minimum) { best = other; minimum = distance; }
            }
            return best;
        }

        float Range(Regiment regiment) { return regiment.Kind == Kind.Artillery ? 34 : regiment.Kind == Kind.Cavalry ? 3.7f : regiment.Kind == Kind.Militia ? 15 : 19; }
        bool CanAttack(Regiment regiment, Regiment enemy)
        {
            if (regiment.Routed || regiment.Withdrawn || regiment.Reload > 0 || enemy.Routed || enemy.Men <= 0) return false;
            if (regiment.Kind != Kind.Cavalry && regiment.Ammo <= 0) return false;
            if (regiment.Moving && regiment.Kind != Kind.Cavalry) return false;
            if (FlatDistance(regiment.Position, enemy.Position) > Range(regiment)) return false;
            float arc = regiment.Formation == Formation.Square ? 180 : regiment.Kind == Kind.Cavalry ? 75 : 45;
            return Mathf.Abs(Mathf.DeltaAngle(regiment.Facing, Heading(enemy.Position - regiment.Position))) <= arc;
        }

        void Shoot(Regiment attacker, Regiment target, bool aimed)
        {
            if (!CanAttack(attacker, target)) return;
            if (attacker.Kind != Kind.Cavalry) attacker.Ammo--;
            attacker.Reload = attacker.Kind == Kind.Artillery ? 13 : attacker.Kind == Kind.Cavalry ? 3.4f : attacker.Kind == Kind.Militia ? 10.5f : 8;
            attacker.Reload *= 1 + attacker.Fatigue / 180;
            attacker.Fatigue = Mathf.Min(100, attacker.Fatigue + (aimed ? 4 : 2));
            float power = Mathf.Sqrt(attacker.Men) * (attacker.Kind == Kind.Artillery ? .57f : attacker.Kind == Kind.Cavalry ? .34f : .43f);
            power *= .76f + (float)rng.NextDouble() * .48f;
            power *= Mathf.Lerp(.62f, 1, attacker.Cohesion / 100) * Mathf.Lerp(1, .62f, attacker.Fatigue / 100);
            power *= 1 + attacker.Experience / 300;
            if (aimed) power *= 1.22f;
            if (attacker.Formation == Formation.Column) power *= .48f;
            if (attacker.Formation == Formation.Square) power *= .57f;
            if (attacker.Kind == Kind.Cavalry && target.Formation == Formation.Square) power *= .23f;
            if (attacker.Kind == Kind.Artillery && target.Formation == Formation.Square) power *= 1.65f;
            if (attacker.Kind == Kind.Cavalry && target.Reload > 4) power *= 1.35f;
            if (TerrainHeight(attacker.Position.x, attacker.Position.z) > TerrainHeight(target.Position.x, target.Position.z) + 1) power *= 1.2f;
            if (InOrchard(target.Position) && attacker.Kind != Kind.Cavalry) power *= .66f;
            float flank = Mathf.Abs(Mathf.DeltaAngle(target.Facing, Heading(attacker.Position - target.Position))) > 100 ? 1.7f : 1;
            int casualties = Mathf.Clamp(Mathf.RoundToInt(power * (flank > 1 ? 1.2f : 1)), 1, target.Men);
            target.Men -= casualties;
            float shock = attacker.Kind == Kind.Artillery ? 5 : attacker.Kind == Kind.Cavalry ? 6 : 2.6f;
            float command = target.Player ? Mathf.Clamp(setup.CommanderCompetence, 0, 100) : 55;
            target.Morale = Mathf.Max(0, target.Morale - (casualties * 130f / Mathf.Max(1, target.Original) + shock) * flank * (1.13f - command / 400));
            target.Cohesion = Mathf.Max(0, target.Cohesion - shock * flank);
            target.Quiet = 0; target.WasHit = true;
            target.LastHit = visualClock;
            SetCondition(target);
            if (attacker.Kind != Kind.Cavalry)
            {
                attacker.LastVolley = visualClock;
                VolleyEffects(attacker, target);
            }
        }

        void SetCondition(Regiment regiment)
        {
            if (regiment.Routed) { regiment.Condition = Condition.Routing; return; }
            if (regiment.Men <= regiment.Original * .25f) regiment.Morale = Mathf.Min(regiment.Morale, 19);
            regiment.Condition = regiment.Morale >= 72 ? Condition.Steady : regiment.Morale >= 55 ? Condition.Pressured : regiment.Morale >= 36 ? Condition.Shaken : regiment.Morale >= 20 ? Condition.Wavering : Condition.Routing;
            if (regiment.Condition != Condition.Routing) return;
            regiment.Routed = true; regiment.Moving = false;
            foreach (Regiment other in regiments)
                if (other != regiment && other.Player == regiment.Player && !other.Routed && FlatDistance(other.Position, regiment.Position) < 15)
                    other.Morale = Mathf.Max(0, other.Morale - 5);
        }

        int OwnRouted(bool player)
        {
            int count = 0;
            foreach (Regiment regiment in regiments) if (regiment.Player == player && regiment.Routed) count++;
            return count;
        }

        void UpdateObjective(float dt)
        {
            bool playerNear = false, enemyNear = false, playerContest = false, enemyContest = false;
            foreach (Regiment regiment in regiments)
            {
                if (regiment.Routed || regiment.Withdrawn || regiment.Men <= 0) continue;
                float distance = FlatDistance(regiment.Position, convoy);
                if (distance < 9) { if (regiment.Player) playerContest = true; else enemyContest = true; }
                if (distance < 6.5f && regiment.Kind != Kind.Artillery)
                { if (regiment.Player) playerNear = true; else enemyNear = true; }
            }
            playerHold = playerNear && !enemyContest ? Mathf.Min(45, playerHold + dt) : Mathf.Max(0, playerHold - dt * 1.5f);
            enemyHold = enemyNear && !playerContest ? Mathf.Min(60, enemyHold + dt) : Mathf.Max(0, enemyHold - dt * 1.5f);
            if (playerHold >= 45) Finish(true, false);
            else if (enemyHold >= 60) Finish(false, false);
        }

        void Finish(bool won, bool retreat)
        {
            if (!Active || ended) return;
            ended = true; paused = false;
            Feedback?.Invoke(retreat ? "retreat" : won ? "victory" : "defeat");
            int survivors = 0; float morale = 0;
            foreach (Regiment regiment in regiments)
            {
                if (!regiment.Player) continue;
                survivors += regiment.Men;
                morale += regiment.Morale * regiment.Men;
            }
            int loss = Mathf.Max(0, originalTroops - survivors);
            if (retreat) loss += Mathf.RoundToInt(survivors * .035f);
            // Capturing the convoy, rather than simply winning elsewhere, creates supplies.
            int recovered = won && playerHold >= 45 ? 24 : 0;
            outcome = new BattleOutcome {
                Won = won, Casualties = Mathf.Clamp(loss, 0, originalTroops),
                EndingMorale = Mathf.Clamp(survivors > 0 ? morale / survivors + (won ? 5 : -8) : 0, 0, 100),
                MilitarySuppliesRecovered = recovered
            };
            campaignReturnMorale = setup.CampaignMoraleAfterBattle != null ? setup.CampaignMoraleAfterBattle(won, outcome.EndingMorale) : outcome.EndingMorale;
        }

        void AcceptOutcome()
        {
            if (!Active || !ended || delivered || outcome == null) return;
            delivered = true;
            Action<BattleOutcome> callback = completion;
            BattleOutcome result = outcome;
            Stop();
            callback?.Invoke(result);
        }

        void OrderFormation(Formation formation)
        {
            foreach (Regiment regiment in selected)
            {
                if (!Commandable(regiment)) continue;
                if (formation == Formation.Square && (regiment.Kind == Kind.Cavalry || regiment.Kind == Kind.Artillery)) continue;
                if (regiment.Formation == formation) continue;
                regiment.Formation = formation;
                regiment.Cohesion = Mathf.Max(20, regiment.Cohesion - 12);
                regiment.Reload = Mathf.Max(regiment.Reload, 2.5f);
            }
            Feedback?.Invoke("formation");
        }

        void OrderVolley()
        {
            bool fired = false;
            foreach (Regiment regiment in selected)
            {
                Regiment enemy = FindEnemy(regiment);
                if (enemy == null || !CanAttack(regiment, enemy)) continue;
                Shoot(regiment, enemy, true); fired = true;
            }
            messageKey = fired ? "battle.volley_fired" : "battle.volley_unavailable";
            messageUntil = elapsed + 6;
        }

        static bool Commandable(Regiment regiment)
        {
            return regiment.Player && !regiment.Routed && !regiment.Withdrawn && regiment.Men > 0;
        }

        Regiment FirstCommandable()
        {
            foreach (Regiment regiment in selected) if (Commandable(regiment)) return regiment;
            return null;
        }

        bool PreparingVolley(Regiment regiment)
        {
            if (ended || regiment.Kind == Kind.Cavalry || regiment.Routed || regiment.Withdrawn ||
                regiment.Moving || regiment.Ammo <= 0 || regiment.Reload > .6f) return false;
            Regiment enemy = FindEnemy(regiment);
            if (enemy == null || FlatDistance(regiment.Position, enemy.Position) > Range(regiment)) return false;
            float arc = regiment.Formation == Formation.Square ? 180 : 45;
            return Mathf.Abs(Mathf.DeltaAngle(regiment.Facing, Heading(enemy.Position - regiment.Position))) <= arc;
        }

        Vector3 Bound(Vector3 position) { return new Vector3(Mathf.Clamp(position.x, -36, 36), 0, Mathf.Clamp(position.z, -28, 30)); }
        static float FlatDistance(Vector3 a, Vector3 b) { a.y = b.y = 0; return Vector3.Distance(a, b); }
        static float Heading(Vector3 vector) { return Mathf.Atan2(vector.x, vector.z) * Mathf.Rad2Deg; }
        static bool InOrchard(Vector3 point) { return point.x > -27 && point.x < -10 && point.z > -5 && point.z < 10; }
        static bool InCreek(Vector3 point) { return Mathf.Abs(point.x - 6) < 2 && Mathf.Abs(point.z - 1) > 4; }
        static float TerrainHeight(float x, float z)
        {
            float distance = new Vector2((x + 20) / 13, (z - 15) / 12).magnitude;
            float hill = distance < 1 ? (1 + Mathf.Cos(distance * Mathf.PI)) * 1.45f : 0;
            return Mathf.Abs(x - 6) < 1.6f && Mathf.Abs(z - 1) > 4 ? -.42f : hill;
        }

        Material MakeMaterial(string name, Color color, bool transparent = false, bool emissive = false)
        {
            // Açık Resources başvuruları, çalışma zamanında seçilen varyantları oyuncuda tutar.
            string resource = "BattleMaterials/" + (transparent ? "DioramaTransparent" : emissive ? "DioramaEmission" : "DioramaOpaque");
            Material template = Resources.Load<Material>(resource);
            Material material;
            if (template != null && template.shader != null) material = new Material(template);
            else
            {
                Shader shader = Shader.Find("Standard");
                if (shader == null) shader = Shader.Find("Unlit/Color");
                if (shader == null) shader = Shader.Find("Sprites/Default");
                if (shader == null) throw new InvalidOperationException("Battle render resource is missing: " + resource);
                material = new Material(shader);
            }
            material.name = name; material.color = color;
            if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", .05f);
            if (transparent && material.HasProperty("_Mode"))
            {
                material.SetFloat("_Mode", 2); material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0); material.EnableKeyword("_ALPHABLEND_ON"); material.renderQueue = 3000;
                material.SetOverrideTag("RenderType", "Transparent");
            }
            materials.Add(material); return material;
        }

        void CreateMaterials()
        {
            grass = MakeMaterial("Muted meadow", new Color(.50f, .58f, .38f));
            soil = MakeMaterial("Warm earth", new Color(.60f, .52f, .35f));
            blue = MakeMaterial("Royal blue coat", new Color(.19f, .31f, .42f));
            red = MakeMaterial("Opposing ochre red coat", new Color(.54f, .27f, .21f));
            cream = MakeMaterial("Linen", new Color(.83f, .79f, .63f));
            wood = MakeMaterial("Oiled timber", new Color(.32f, .24f, .16f));
            iron = MakeMaterial("Blackened iron", new Color(.17f, .19f, .18f));
            skin = MakeMaterial("Miniature face", new Color(.77f, .59f, .43f));
            leaf = MakeMaterial("Orchard crown", new Color(.30f, .43f, .26f));
            water = MakeMaterial("Shallow creek", new Color(.38f, .53f, .56f));
            gold = MakeMaterial("Convoy ochre", new Color(.84f, .67f, .32f));
            smoke = MakeMaterial("Powder smoke", new Color(.82f, .81f, .73f, .42f), true);
            flash = MakeMaterial("Brief powder flash", new Color(1, .80f, .36f), false, true);
            flash.EnableKeyword("_EMISSION"); flash.SetColor("_EmissionColor", new Color(1, .62f, .18f) * 1.5f);
            blueRing = MakeMaterial("Friendly formation marker", new Color(.50f, .69f, .72f));
            redRing = MakeMaterial("Opposing formation marker", new Color(.65f, .37f, .28f));
            orderInk = MakeMaterial("Field order brass", new Color(.74f, .67f, .44f, .72f), true);
        }

        LineRenderer FieldLine(string name, Transform parent, Material material, int points, float width)
        {
            GameObject line = new GameObject(name); line.transform.SetParent(parent, false);
            LineRenderer renderer = line.AddComponent<LineRenderer>();
            renderer.useWorldSpace = true; renderer.positionCount = points;
            renderer.widthMultiplier = width; renderer.sharedMaterial = material;
            renderer.numCornerVertices = 2; renderer.numCapVertices = 2;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false; renderer.enabled = false;
            return renderer;
        }

        GameObject Primitive(string name, PrimitiveType type, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            GameObject item = GameObject.CreatePrimitive(type);
            item.name = name; item.transform.SetParent(parent, false);
            item.transform.localPosition = position; item.transform.localScale = scale;
            Collider collider = item.GetComponent<Collider>(); if (collider != null) ReleaseObject(collider);
            Renderer renderer = item.GetComponent<Renderer>(); renderer.sharedMaterial = material;
            return item;
        }

        void BuildLandscape()
        {
            const int columns = 85, rows = 69;
            Vector3[] vertices = new Vector3[columns * rows];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[(columns - 1) * (rows - 1) * 6];
            for (int z = 0; z < rows; z++) for (int x = 0; x < columns; x++)
            {
                float px = x - 42, pz = z - 32; int index = z * columns + x;
                vertices[index] = new Vector3(px, TerrainHeight(px, pz), pz);
                uv[index] = new Vector2(x / (float)columns, z / (float)rows);
            }
            int cursor = 0;
            for (int z = 0; z < rows - 1; z++) for (int x = 0; x < columns - 1; x++)
            {
                int index = z * columns + x;
                triangles[cursor++] = index; triangles[cursor++] = index + columns; triangles[cursor++] = index + 1;
                triangles[cursor++] = index + 1; triangles[cursor++] = index + columns; triangles[cursor++] = index + columns + 1;
            }
            Mesh mesh = new Mesh { name = "Hand-authored crossing and hill", vertices = vertices, uv = uv, triangles = triangles };
            mesh.RecalculateNormals(); meshes.Add(mesh);
            GameObject terrain = new GameObject("Terrain"); terrain.transform.SetParent(world.transform, false);
            terrain.AddComponent<MeshFilter>().sharedMesh = mesh; terrain.AddComponent<MeshRenderer>().sharedMaterial = grass;
            Primitive("Diorama earth edge", PrimitiveType.Cube, world.transform, new Vector3(0, -1.15f, 2), new Vector3(84, 1.5f, 68), soil);
            Primitive("Creek north", PrimitiveType.Cube, world.transform, new Vector3(6, -.2f, 20), new Vector3(2.9f, .1f, 30), water);
            Primitive("Creek south", PrimitiveType.Cube, world.transform, new Vector3(6, -.2f, -18), new Vector3(2.9f, .1f, 28), water);
            Primitive("Crossing dirt", PrimitiveType.Cube, world.transform, new Vector3(6, .035f, 1), new Vector3(6, .06f, 7), soil);
            BuildRuralApproach();
            for (int i = 0; i < 20; i++)
            {
                float x = -27 + i % 5 * 3.5f, z = -4 + i / 5 * 4;
                float y = TerrainHeight(x, z);
                Primitive("Orchard trunk", PrimitiveType.Cylinder, world.transform, new Vector3(x, y + .8f, z), new Vector3(.28f, .8f, .28f), wood);
                Primitive("Orchard foliage", PrimitiveType.Sphere, world.transform, new Vector3(x, y + 2.2f, z), new Vector3(2.2f, 2.3f, 2.2f), leaf);
            }
            // Fences are scenery; the clearly marked orchard, hill and creek own terrain rules.
            for (int i = 0; i < 10; i++)
            {
                float x = 12 + i * 2;
                Primitive("Fence post", PrimitiveType.Cube, world.transform, new Vector3(x, .65f, 12), new Vector3(.18f, 1.3f, .18f), wood);
                if (i < 9) Primitive("Fence rail", PrimitiveType.Cube, world.transform, new Vector3(x + 1, .8f, 12), new Vector3(2, .12f, .12f), wood);
            }
            for (int cart = 0; cart < 2; cart++)
            {
                Vector3 position = convoy + new Vector3(cart * 3 - 1.5f, 0, cart * 2 - 1);
                Primitive("Supply cart", PrimitiveType.Cube, world.transform, position + Vector3.up * .85f, new Vector3(1.6f, .65f, 2.3f), wood);
                for (int wheel = 0; wheel < 4; wheel++)
                {
                    Vector3 wheelPosition = position + new Vector3(wheel % 2 == 0 ? -.95f : .95f, .5f, wheel < 2 ? -.75f : .75f);
                    GameObject circle = Primitive("Cart wheel", PrimitiveType.Cylinder, world.transform, wheelPosition, new Vector3(.9f, .10f, .9f), iron);
                    circle.transform.localRotation = Quaternion.Euler(0, 0, 90);
                }
                for (int sack = 0; sack < 5; sack++) Primitive("Grain sack", PrimitiveType.Capsule, world.transform, position + new Vector3((sack % 2 - .5f) * .65f, 1.55f, (sack / 2 - 1) * .60f), new Vector3(.6f, .4f, .7f), cream);
            }
            for (int i = 0; i < 40; i++)
            {
                float angle = i * Mathf.PI * 2 / 40;
                Primitive("Convoy capture boundary", PrimitiveType.Cube, world.transform, convoy + new Vector3(Mathf.Sin(angle) * 6.5f, .12f, Mathf.Cos(angle) * 6.5f), new Vector3(.45f, .1f, .45f), gold);
            }
        }

        void BuildRuralApproach()
        {
            // Yol ve ekili tarla yalnızca araziyi okutur; hareket hesabına katılmaz.
            GroundRibbon("Convoy approach road", new[] {
                new Vector3(-41, 0, -9), new Vector3(-30, 0, -7), new Vector3(-16, 0, -4),
                new Vector3(-3, 0, 0), new Vector3(6, 0, 1), new Vector3(17, 0, 3), new Vector3(41, 0, 8)
            }, 1.65f, soil);
            for (int row = 0; row < 7; row++)
                GroundRibbon("Cultivated field furrow", new[] {
                    new Vector3(15 + row * .3f, 0, -7 + row * 1.3f),
                    new Vector3(34, 0, -5 + row * 1.3f)
                }, .11f, soil);
            for (int house = 0; house < 3; house++)
            {
                Vector3 basePosition = new Vector3(8 + house * 5.2f, 0, 32 + (house % 2) * 1.4f);
                Primitive("Distant farmhouse wall", PrimitiveType.Cube, world.transform, basePosition + Vector3.up * .9f, new Vector3(2.8f, 1.8f, 2.5f), cream);
                for (int side = -1; side <= 1; side += 2)
                {
                    GameObject roof = Primitive("Farmhouse roof slope", PrimitiveType.Cube, world.transform, basePosition + new Vector3(side * .71f, 2.02f, 0), new Vector3(1.75f, .17f, 2.9f), wood);
                    roof.transform.localRotation = Quaternion.Euler(0, 0, -side * 26);
                }
                Primitive("Farmhouse door", PrimitiveType.Cube, world.transform, basePosition + new Vector3(.35f, .48f, -1.27f), new Vector3(.46f, .96f, .04f), wood);
                Primitive("Farmhouse chimney", PrimitiveType.Cube, world.transform, basePosition + new Vector3(-.6f, 2.42f, .5f), new Vector3(.34f, .88f, .4f), soil);
            }
        }

        void GroundRibbon(string name, Vector3[] bends, float width, Material material)
        {
            const int subdivisions = 8;
            int samples = (bends.Length - 1) * subdivisions + 1;
            Vector3[] vertices = new Vector3[samples * 2];
            int[] triangles = new int[(samples - 1) * 6];
            for (int i = 0; i < samples; i++)
            {
                int segment = Mathf.Min(i / subdivisions, bends.Length - 2);
                float fraction = (i - segment * subdivisions) / (float)subdivisions;
                Vector3 point = Vector3.Lerp(bends[segment], bends[segment + 1], fraction);
                Vector3 across = Vector3.Cross(Vector3.up, (bends[segment + 1] - bends[segment]).normalized) * width * .5f;
                vertices[i * 2] = OnSurface(point - across, .055f);
                vertices[i * 2 + 1] = OnSurface(point + across, .055f);
                if (i == samples - 1) continue;
                int triangle = i * 6, vertex = i * 2;
                triangles[triangle] = vertex; triangles[triangle + 1] = vertex + 2; triangles[triangle + 2] = vertex + 1;
                triangles[triangle + 3] = vertex + 1; triangles[triangle + 4] = vertex + 2; triangles[triangle + 5] = vertex + 3;
            }
            Mesh mesh = new Mesh { name = name, vertices = vertices, triangles = triangles };
            mesh.RecalculateNormals(); meshes.Add(mesh);
            GameObject ribbon = new GameObject(name); ribbon.transform.SetParent(world.transform, false);
            ribbon.AddComponent<MeshFilter>().sharedMesh = mesh;
            ribbon.AddComponent<MeshRenderer>().sharedMaterial = material;
        }

        void DeployArmy(bool player, int total)
        {
            int assigned = 0;
            float[] shares = { .32f, .28f, .22f, .18f };
            for (int i = 0; i < 4; i++)
            {
                int men = i == 3 ? total - assigned : Mathf.FloorToInt(total * shares[i]); assigned += men;
                Regiment regiment = new Regiment {
                    Id = regiments.Count, Player = player, Kind = (Kind)i, Formation = Formation.Line,
                    Original = men, Men = men, Morale = player ? Mathf.Clamp(setup.Morale - (100 - setup.Supply) * .11f, 0, 100) : 77,
                    Fatigue = player ? Mathf.Clamp(setup.Fatigue, 0, 100) : 12, Experience = i == 1 ? 15 : 45,
                    Ammo = i == 2 ? 0 : Mathf.Max(1, Mathf.FloorToInt((i == 3 ? 3 : 5) + (player ? setup.Supply : 65) / 10)),
                    Position = new Vector3(-24 + i * 16, 0, player ? -18 : i == 2 ? 29 : 23), Facing = player ? 0 : 180,
                    Reload = (float)rng.NextDouble() * 2
                };
                regiment.Destination = regiment.Position;
                regiment.Root = new GameObject((player ? "Royal " : "Opposing ") + regiment.Kind);
                regiment.Root.transform.SetParent(world.transform, false);
                regiment.SelectionOutline = FieldLine("Formation frontage", world.transform, player ? blueRing : redRing, 5, .10f);
                regiment.SelectionOutline.loop = true;
                regiment.FireArc = FieldLine("Selected regiment firing arc", world.transform, orderInk, 27, .065f);
                regiment.OrderPath = FieldLine("Issued movement order", world.transform, orderInk, 13, .075f);
                regiment.DestinationMark = FieldLine("Order destination", world.transform, orderInk, 5, .10f);
                int count = i == 2 ? 8 : i == 3 ? 6 : 14;
                for (int figure = 0; figure < count; figure++) regiment.Figures.Add(BuildFigure(regiment, figure));
                GameObject pole = Primitive("Regimental standard pole", PrimitiveType.Cylinder, regiment.Root.transform, new Vector3(0, 2.2f, 0), new Vector3(.07f, 2.2f, .07f), wood);
                regiment.Flag = Primitive("Regimental standard", PrimitiveType.Cube, regiment.Root.transform, new Vector3(.7f, 3.7f, 0), new Vector3(1.4f, .95f, .07f), player ? cream : red);
                Primitive("Standard colour", PrimitiveType.Cube, regiment.Flag.transform, new Vector3(-.27f, 0, -.65f), new Vector3(.22f, .9f, .08f), player ? blue : gold);
                if (i == 3)
                {
                    for (int gun = 0; gun < 2; gun++)
                    {
                        Vector3 position = new Vector3(gun * 3 - 1.5f, .65f, 1.6f);
                        GameObject barrel = Primitive("Field gun barrel", PrimitiveType.Cylinder, regiment.Root.transform, position, new Vector3(.4f, 1.3f, .4f), iron);
                        barrel.transform.localRotation = Quaternion.Euler(80, 0, 0);
                        for (int side = -1; side <= 1; side += 2)
                        {
                            GameObject wheel = Primitive("Field gun wheel", PrimitiveType.Cylinder, regiment.Root.transform, position + new Vector3(side * .7f, -.15f, -.3f), new Vector3(1.1f, .12f, 1.1f), wood);
                            wheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
                        }
                    }
                }
                SetCondition(regiment); regiments.Add(regiment);
            }
        }

        Miniature BuildFigure(Regiment regiment, int index)
        {
            GameObject miniature = new GameObject("Miniature " + index); miniature.transform.SetParent(regiment.Root.transform, false);
            float saddle = regiment.Kind == Kind.Cavalry ? .75f : 0;
            if (regiment.Kind == Kind.Cavalry)
            {
                Primitive("Horse", PrimitiveType.Capsule, miniature.transform, new Vector3(0, .65f, 0), new Vector3(.7f, .45f, 1.5f), wood);
                Primitive("Horse neck", PrimitiveType.Capsule, miniature.transform, new Vector3(0, 1.0f, .6f), new Vector3(.33f, .5f, .4f), wood);
            }
            Primitive("Coat", PrimitiveType.Capsule, miniature.transform, new Vector3(0, .65f + saddle, 0), new Vector3(.48f, .39f, .38f), regiment.Kind == Kind.Militia ? soil : regiment.Player ? blue : red);
            Primitive("Breeches", PrimitiveType.Cube, miniature.transform, new Vector3(0, .26f + saddle, 0), new Vector3(.29f, .5f, .25f), cream);
            Transform leftBoot = Primitive("Left boot", PrimitiveType.Cube, miniature.transform, new Vector3(-.12f, .08f + saddle, .04f), new Vector3(.16f, .18f, .30f), iron).transform;
            Transform rightBoot = Primitive("Right boot", PrimitiveType.Cube, miniature.transform, new Vector3(.12f, .08f + saddle, .04f), new Vector3(.16f, .18f, .30f), iron).transform;
            Primitive("Head", PrimitiveType.Sphere, miniature.transform, new Vector3(0, 1.13f + saddle, 0), Vector3.one * .31f, skin);
            Primitive("Hat brim", PrimitiveType.Cube, miniature.transform, new Vector3(0, 1.32f + saddle, 0), new Vector3(.53f, .09f, .37f), iron);
            Primitive("Hat crown", PrimitiveType.Cube, miniature.transform, new Vector3(0, 1.40f + saddle, 0), new Vector3(.30f, .17f, .25f), iron);
            Transform musket = Primitive("Musket", PrimitiveType.Cylinder, miniature.transform, new Vector3(.31f, .80f + saddle, .16f), new Vector3(.05f, .60f, .05f), wood).transform;
            Transform bayonet = Primitive("Bayonet", PrimitiveType.Cylinder, miniature.transform, new Vector3(.31f, 1.51f + saddle, .16f), new Vector3(.026f, .16f, .026f), iron).transform;
            return new Miniature { Root = miniature.transform, Musket = musket, Bayonet = bayonet, LeftBoot = leftBoot, RightBoot = rightBoot };
        }

        void UpdateVisual(Regiment regiment, float dt)
        {
            regiment.Root.SetActive(!regiment.Withdrawn && regiment.Men > 0);
            float blend = regiment.VisualReady ? 1 - Mathf.Exp(-dt * 9) : 1;
            Vector3 desired = new Vector3(regiment.Position.x, TerrainHeight(regiment.Position.x, regiment.Position.z), regiment.Position.z);
            regiment.Root.transform.position = Vector3.Lerp(regiment.Root.transform.position, desired, blend);
            Quaternion facing = Quaternion.Euler(0, regiment.Routed ? regiment.Player ? 180 : 0 : regiment.Facing, 0);
            regiment.Root.transform.rotation = Quaternion.Slerp(regiment.Root.transform.rotation, facing, blend);
            int visible = Mathf.CeilToInt(regiment.Figures.Count * regiment.Men / (float)Mathf.Max(1, regiment.Original));
            float presentationTime = visualClock;
            bool preparing = PreparingVolley(regiment);
            bool marching = !ended && (regiment.Moving || regiment.Routed) && !regiment.Withdrawn;
            for (int i = 0; i < regiment.Figures.Count; i++)
            {
                Miniature figure = regiment.Figures[i]; Transform miniature = figure.Root;
                if (i >= visible && !figure.Falling)
                {
                    figure.Falling = true; figure.FallAt = presentationTime;
                    figure.FallStart = miniature.position; figure.FallRotation = miniature.rotation;
                    miniature.SetParent(world.transform, true);
                }
                if (figure.Falling)
                {
                    float fall = Mathf.Clamp01((presentationTime - figure.FallAt) / .65f);
                    float ease = fall * fall * (3 - 2 * fall);
                    Vector3 end = figure.FallStart + new Vector3(i % 2 == 0 ? .35f : -.35f, 0, -.2f);
                    end.y = TerrainHeight(end.x, end.z) + .10f;
                    miniature.position = Vector3.Lerp(figure.FallStart, end, ease);
                    miniature.rotation = Quaternion.Slerp(figure.FallRotation, figure.FallRotation * Quaternion.Euler(0, 0, i % 2 == 0 ? 88 : -88), ease);
                    figure.Resting = fall >= 1;
                    continue;
                }
                Vector3 slot;
                if (regiment.Formation == Formation.Column) slot = new Vector3((i % 3 - 1) * 1.05f, 0, -(i / 3) * 1.3f);
                else if (regiment.Formation == Formation.Square)
                {
                    float perimeter = i * 4f / regiment.Figures.Count;
                    int side = Mathf.FloorToInt(perimeter);
                    float along = Mathf.Lerp(-2.3f, 2.3f, perimeter - side);
                    slot = side == 0 ? new Vector3(along, 0, 2.3f) : side == 1 ? new Vector3(2.3f, 0, -along) : side == 2 ? new Vector3(-along, 0, -2.3f) : new Vector3(-2.3f, 0, along);
                }
                else slot = new Vector3((i % 7 - 3) * 1.05f, 0, -(i / 7) * 1.25f);
                Vector3 worldSlot = regiment.Root.transform.TransformPoint(slot);
                slot.y = TerrainHeight(worldSlot.x, worldSlot.z) - regiment.Root.transform.position.y;
                float visualBlend = regiment.VisualReady ? 1 - Mathf.Exp(-dt * 4.5f) : 1;
                miniature.localPosition = Vector3.Lerp(miniature.localPosition, slot, visualBlend);
                float stride = marching ? Mathf.Sin(presentationTime * (regiment.Routed ? 10 : 7.5f) + i * .7f) : 0;
                float stepSway = stride * (regiment.Kind == Kind.Cavalry ? 1.8f : 2.5f);
                float hitAge = presentationTime - regiment.LastHit;
                float flinch = hitAge >= 0 && hitAge < .45f ? Mathf.Sin(hitAge / .45f * Mathf.PI) * (i % 2 == 0 ? 10 : -7) : 0;
                Quaternion miniatureFacing = regiment.Formation == Formation.Square ? Quaternion.Euler(0, Mathf.FloorToInt(i * 4f / regiment.Figures.Count) * 90, stepSway + flinch) : Quaternion.Euler(0, 0, stepSway + flinch);
                miniature.localRotation = Quaternion.Slerp(miniature.localRotation, miniatureFacing, visualBlend);
                float volleyAge = presentationTime - regiment.LastVolley;
                float desiredRaise = preparing || (volleyAge >= 0 && volleyAge < .30f) ? 1 : 0;
                figure.WeaponRaise = Mathf.MoveTowards(figure.WeaponRaise, desiredRaise, dt * (desiredRaise > 0 ? 5 : 1.7f));
                // Hasar tick'inde namlu ve tepki birlikte görünür; hazırlık yalnızca görseldir.
                if (volleyAge == 0) figure.WeaponRaise = 1;
                float raised = figure.WeaponRaise;
                if (regiment.Kind == Kind.Cavalry) raised = 0;
                float saddle = regiment.Kind == Kind.Cavalry ? .75f : 0;
                figure.Musket.localRotation = Quaternion.Euler(raised * 82, 0, 0);
                figure.Musket.localPosition = new Vector3(.31f, .80f + saddle + raised * .14f, .16f + raised * .25f);
                if (volleyAge >= 0 && volleyAge < .18f)
                    figure.Musket.localPosition -= Vector3.forward * (.09f * Mathf.Sin(volleyAge / .18f * Mathf.PI));
                Vector3 muzzleDirection = figure.Musket.localRotation * Vector3.up;
                figure.Bayonet.localPosition = figure.Musket.localPosition + muzzleDirection * .71f;
                figure.Bayonet.localRotation = figure.Musket.localRotation;
                figure.LeftBoot.localPosition = new Vector3(-.12f, .08f + saddle + Mathf.Max(0, stride) * .08f, .04f + stride * .14f);
                figure.RightBoot.localPosition = new Vector3(.12f, .08f + saddle + Mathf.Max(0, -stride) * .08f, .04f - stride * .14f);
            }
            regiment.Flag.transform.localRotation = Quaternion.Euler(Mathf.Sin(presentationTime * 1.3f + regiment.Id) * 3, Mathf.Sin(presentationTime * 2 + regiment.Id) * 9, regiment.Routed ? 28 : 0);
            UpdateOrderVisuals(regiment);
            regiment.VisualReady = true;
        }

        void UpdateOrderVisuals(Regiment regiment)
        {
            bool active = !ended && !regiment.Routed && !regiment.Withdrawn && regiment.Men > 0;
            bool chosen = active && selected.Contains(regiment);
            regiment.SelectionOutline.enabled = active && (chosen || hovered == regiment);
            regiment.SelectionOutline.widthMultiplier = chosen ? .10f : .055f;
            float width = regiment.Formation == Formation.Column ? 2 : regiment.Formation == Formation.Square ? 3 : 4.3f;
            float back = regiment.Formation == Formation.Column ? -6.5f : regiment.Formation == Formation.Square ? -3 : -2.2f;
            float front = regiment.Formation == Formation.Square ? 3 : 1;
            regiment.Footprint[0] = new Vector3(-width, 0, back);
            regiment.Footprint[1] = new Vector3(width, 0, back);
            regiment.Footprint[2] = new Vector3(width, 0, front);
            regiment.Footprint[3] = new Vector3(0, 0, front + .8f);
            regiment.Footprint[4] = new Vector3(-width, 0, front);
            for (int i = 0; i < regiment.Footprint.Length; i++)
                regiment.Footprint[i] = OnSurface(regiment.Root.transform.TransformPoint(regiment.Footprint[i]), .14f);
            regiment.SelectionOutline.SetPositions(regiment.Footprint);

            regiment.FireArc.enabled = chosen && FirstCommandable() == regiment;
            if (regiment.FireArc.enabled)
            {
                float arc = regiment.Formation == Formation.Square ? 180 : regiment.Kind == Kind.Cavalry ? 75 : 45;
                for (int i = 0; i < regiment.Arc.Length; i++)
                {
                    bool spoke = arc < 180 && (i == 0 || i == regiment.Arc.Length - 1);
                    float along = arc == 180 ? i / 26f : (i - 1) / 24f;
                    float angle = (regiment.Facing + Mathf.Lerp(-arc, arc, along)) * Mathf.Deg2Rad;
                    Vector3 point = regiment.Position + new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * (spoke ? 0 : Range(regiment));
                    regiment.Arc[i] = OnSurface(point, .11f);
                }
                regiment.FireArc.SetPositions(regiment.Arc);
            }
            regiment.OrderPath.enabled = regiment.DestinationMark.enabled = chosen && regiment.Moving;
            if (!regiment.OrderPath.enabled) return;
            for (int i = 0; i < regiment.Route.Length; i++)
                regiment.Route[i] = OnSurface(Vector3.Lerp(regiment.Position, regiment.Destination, i / 12f), .14f);
            regiment.OrderPath.SetPositions(regiment.Route);
            for (int i = 0; i < regiment.DestinationPoints.Length; i++)
            {
                float angle = i * Mathf.PI * .5f;
                regiment.DestinationPoints[i] = OnSurface(regiment.Destination + new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * .75f, .16f);
            }
            regiment.DestinationMark.SetPositions(regiment.DestinationPoints);
        }

        static Vector3 OnSurface(Vector3 point, float lift)
        {
            point.y = TerrainHeight(point.x, point.z) + lift;
            return point;
        }

        void VolleyEffects(Regiment regiment, Regiment target)
        {
            bool cannon = regiment.Kind == Kind.Artillery;
            for (int i = 0; i < (cannon ? 2 : 5); i++)
            {
                float lateral = cannon ? i * 3 - 1.5f : (i - 2) * 1.35f;
                Vector3 start = regiment.Root.transform.position + regiment.Root.transform.right * lateral + regiment.Root.transform.forward * (cannon ? 2.7f : 1) + Vector3.up * (cannon ? .85f : 1);
                Vector3 end = target.Root.transform.position + Vector3.up;
                GameObject cloud = Primitive("Powder cloud", PrimitiveType.Sphere, world.transform, start, Vector3.one * .4f, smoke);
                cloud.SetActive(false);
                effects.Add(new Puff { Object = cloud, Start = start, Born = visualClock, Lifetime = cannon ? 5.2f : 4.1f,
                    Delay = i * .012f, Drift = new Vector3(.48f + i * .055f, .12f, .13f),
                    Scale = new Vector3(cannon ? 2.1f : 1.6f, .85f + (i % 2) * .15f, 1.25f), Renderer = cloud.GetComponent<Renderer>() });
                GameObject muzzle = Primitive("Muzzle flash", PrimitiveType.Sphere, world.transform, start, Vector3.one * .45f, flash);
                muzzle.SetActive(false);
                effects.Add(new Puff { Object = muzzle, Start = start, Born = visualClock, Scale = Vector3.one * (cannon ? .75f : .30f),
                    Lifetime = .075f, Delay = i * .012f, Flash = true, Cue = i == 0 ? cannon ? "cannon" : "volley" : null });
                if (cannon)
                {
                    GameObject projectile = Primitive("Cannon shot", PrimitiveType.Sphere, world.transform, start, Vector3.one * .12f, iron);
                    projectile.SetActive(false);
                    effects.Add(new Puff { Object = projectile, Start = start, End = end, Born = visualClock, Lifetime = .10f, Projectile = true });
                }
            }
        }

        void InitStyles()
        {
            if (bodyStyle != null) return;
            bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, wordWrap = true };
            bodyStyle.normal.textColor = new Color(.91f, .88f, .76f);
            titleStyle = new GUIStyle(bodyStyle) { fontSize = 23, fontStyle = FontStyle.Bold };
            smallStyle = new GUIStyle(bodyStyle) { fontSize = 12 };
            cardStyle = new GUIStyle(bodyStyle) { fontSize = 12 };
            buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 14, wordWrap = true, border = new RectOffset(0, 0, 0, 0) };
            buttonStyle.normal.background = HudTexture(new Color(.29f, .34f, .27f));
            buttonStyle.hover.background = HudTexture(new Color(.39f, .43f, .32f));
            buttonStyle.active.background = HudTexture(new Color(.48f, .43f, .29f));
            buttonStyle.normal.textColor = new Color(.94f, .91f, .8f);
            buttonStyle.hover.textColor = buttonStyle.active.textColor = buttonStyle.normal.textColor;
            dispatchFont = Font.CreateDynamicFontFromOSFont(new[] { "Georgia", "Times New Roman", "Liberation Serif" }, 23);
            titleStyle.font = dispatchFont;
            dispatchTitle = new GUIStyle(titleStyle) { font = dispatchFont, fontSize = 27 };
            dispatchBody = new GUIStyle(bodyStyle) { font = dispatchFont, fontSize = 18 };
            dispatchSmall = new GUIStyle(smallStyle) { fontSize = 12 };
            dispatchNumber = new GUIStyle(bodyStyle) { fontSize = 27, fontStyle = FontStyle.Bold };
            dispatchTitle.normal.textColor = dispatchBody.normal.textColor = dispatchSmall.normal.textColor = dispatchNumber.normal.textColor = new Color(.20f, .25f, .19f);
        }

        Texture2D HudTexture(Color colour)
        {
            Texture2D texture = new Texture2D(1, 1) { name = "Battle field-order surface" };
            texture.SetPixel(0, 0, colour); texture.Apply(); hudTextures.Add(texture); return texture;
        }

        void Panel(Rect rect, Color color)
        {
            Color previous = GUI.color; GUI.color = color; GUI.DrawTexture(rect, Texture2D.whiteTexture); GUI.color = previous;
        }
        void Text(Rect rect, string key, GUIStyle style, params object[] args) { GUI.Label(rect, L.Text(key, args), style); }
        bool Button(Rect rect, string key) { return GUI.Button(rect, L.Text(key), buttonStyle); }

        public void DrawHud()
        {
            if (!Active) return;
            InitStyles();
            if (ended) { DrawResult(); return; }
            Panel(new Rect(20, 96, 640, 94), new Color(.15f, .20f, .17f, .94f));
            Text(new Rect(34, 105, 605, 28), "battle.title", titleStyle);
            Text(new Rect(34, 136, 605, 22), "battle.objective", cardStyle, Mathf.FloorToInt(playerHold), Mathf.FloorToInt(enemyHold));
            Text(new Rect(34, 160, 605, 30), "battle.objective_rule", smallStyle);
            Panel(new Rect(934, 97, 485, 89), new Color(.15f, .20f, .17f, .94f));
            Text(new Rect(946, 106, 270, 45), "battle.terrain_rules", smallStyle);
            GUI.enabled = !ended;
            if (Button(new Rect(1230, 105, 176, 31), paused ? "battle.resume" : "battle.pause")) paused = !paused;
            if (Button(new Rect(1230, 143, 176, 31), "battle.retreat")) Finish(false, true);
            GUI.enabled = true;
            DrawRegimentLabels();
            Panel(new Rect(0, 738, 1440, 162), new Color(.13f, .18f, .16f, .99f));
            for (int i = 0; i < 4 && i < regiments.Count; i++)
            {
                Regiment regiment = regiments[i]; float x = 18 + i * 215;
                Panel(new Rect(x, 750, 207, 104), selected.Contains(regiment) ? new Color(.29f, .36f, .28f) : new Color(.20f, .25f, .21f));
                GUI.enabled = Commandable(regiment);
                if (GUI.Button(new Rect(x + 5, 754, 197, 25), (i + 1) + "  " + L.Text("battle.kind." + regiment.Kind.ToString().ToLowerInvariant()), buttonStyle)) SelectIndex(i);
                GUI.enabled = true;
                Text(new Rect(x + 10, 781, 190, 22), "battle.regiment_strength", cardStyle, regiment.Men, L.Text("battle.condition." + regiment.Condition.ToString().ToLowerInvariant()));
                Text(new Rect(x + 10, 804, 190, 20), "battle.regiment_morale", smallStyle, Mathf.RoundToInt(regiment.Morale), Mathf.RoundToInt(regiment.Cohesion));
                string ammo = regiment.Kind == Kind.Cavalry ? L.Text("battle.melee") : regiment.Ammo.ToString();
                Text(new Rect(x + 10, 824, 194, 26), "battle.regiment_reload", smallStyle, ammo, Mathf.CeilToInt(regiment.Reload), Mathf.RoundToInt(regiment.Fatigue));
            }
            Regiment primary = FirstCommandable();
            GUI.enabled = primary != null;
            if (Button(new Rect(892, 749, 166, 30), "battle.formation.line")) OrderFormation(Formation.Line);
            if (Button(new Rect(1068, 749, 166, 30), "battle.formation.column")) OrderFormation(Formation.Column);
            bool squareAvailable = false;
            foreach (Regiment regiment in selected)
                if (Commandable(regiment) && regiment.Kind != Kind.Cavalry && regiment.Kind != Kind.Artillery) squareAvailable = true;
            GUI.enabled = squareAvailable;
            if (Button(new Rect(1244, 749, 175, 30), "battle.formation.square")) OrderFormation(Formation.Square);
            GUI.enabled = primary != null;
            if (Button(new Rect(892, 786, 166, 31), "battle.fire_at_will")) SetFireOrder(true);
            if (Button(new Rect(1068, 786, 166, 31), "battle.hold_fire")) SetFireOrder(false);
            bool volleyAvailable = false;
            foreach (Regiment regiment in selected)
            {
                Regiment enemy = FindEnemy(regiment);
                if (Commandable(regiment) && enemy != null && CanAttack(regiment, enemy)) volleyAvailable = true;
            }
            GUI.enabled = !paused && volleyAvailable;
            if (Button(new Rect(1244, 786, 175, 31), "battle.volley")) OrderVolley();
            GUI.enabled = true;
            if (primary != null)
            {
                Text(new Rect(892, 822, 527, 19), "battle.selected_order", smallStyle,
                    L.Text("battle.formation." + primary.Formation.ToString().ToLowerInvariant()),
                    L.Text(primary.Moving ? "battle.order_march" : primary.FireAtWill ? "battle.fire_at_will" : "battle.hold_fire"));
                Text(new Rect(892, 842, 527, 20), VolleyReason(primary), smallStyle);
            }
            else Text(new Rect(892, 827, 527, 30), "battle.select_to_command", smallStyle);
            Text(new Rect(21, 862, 1395, 32), elapsed < messageUntil ? messageKey : "battle.controls", smallStyle);
            ShowUnavailableReason(new Rect(1244, 749, 175, 30), !squareAvailable, primary == null ? "battle.select_to_command" : "battle.square_infantry");
            ShowUnavailableReason(new Rect(1244, 786, 175, 31), paused || !volleyAvailable, primary == null ? "battle.select_to_command" : VolleyReason(primary));
            if (paused && !ended)
            {
                Panel(new Rect(510, 216, 420, 1), new Color(.89f, .83f, .65f, .8f));
                Text(new Rect(548, 223, 365, 38), "battle.paused", bodyStyle);
            }
        }

        void SetFireOrder(bool fire)
        {
            foreach (Regiment regiment in selected) if (Commandable(regiment)) regiment.FireAtWill = fire;
            Feedback?.Invoke("formation");
        }

        string VolleyReason(Regiment regiment)
        {
            if (paused) return "battle.volley_reason_pause";
            if (regiment.Kind != Kind.Cavalry && regiment.Moving) return "battle.volley_reason_moving";
            if (regiment.Kind != Kind.Cavalry && regiment.Ammo <= 0) return "battle.volley_reason_ammo";
            if (regiment.Reload > 0) return "battle.volley_reason_reload";
            Regiment enemy = FindEnemy(regiment);
            if (enemy == null || FlatDistance(regiment.Position, enemy.Position) > Range(regiment)) return "battle.volley_reason_range";
            if (!CanAttack(regiment, enemy)) return "battle.volley_reason_facing";
            return "battle.volley_reason_ready";
        }

        void ShowUnavailableReason(Rect rect, bool unavailable, string key)
        {
            if (!unavailable || !rect.Contains(Event.current.mousePosition)) return;
            Panel(new Rect(949, 691, 470, 39), new Color(.17f, .23f, .19f, .98f));
            Text(new Rect(963, 698, 442, 28), key, smallStyle);
        }

        void DrawRegimentLabels()
        {
            foreach (Regiment regiment in regiments)
            {
                if (regiment.Withdrawn || regiment.Men <= 0) continue;
                bool selectedRegiment = selected.Contains(regiment);
                if (!selectedRegiment && hovered != regiment && visualClock - regiment.LastHit > 4 && regiment.Morale >= 36) continue;
                Vector3 position = battleCamera.WorldToScreenPoint(regiment.Root.transform.position + Vector3.up * 4.8f);
                if (position.z <= 0) continue;
                Vector2 canvasPosition = ViewLayout.ToCanvas(position);
                float x = canvasPosition.x, y = canvasPosition.y;
                if (y < 195 || y > 715) continue;
                Panel(new Rect(x - 78, y - 18, 156, 36), regiment.Player ? new Color(.15f, .26f, .31f, .90f) : new Color(.39f, .23f, .18f, .90f));
                Text(new Rect(x - 71, y - 17, 145, 18), "battle.strength_label", smallStyle, regiment.Men, Mathf.RoundToInt(regiment.Morale));
                Text(new Rect(x - 71, y, 145, 20), "battle.condition." + regiment.Condition.ToString().ToLowerInvariant(), smallStyle);
                Panel(new Rect(x - 78, y + 18, 156, 3), new Color(.15f, .19f, .15f));
                Panel(new Rect(x - 78, y + 18, 156 * regiment.Men / Mathf.Max(1f, regiment.Original), 3), selectedRegiment ? new Color(.86f, .74f, .43f) : new Color(.68f, .68f, .48f));
            }
        }

        void DrawResult()
        {
            Panel(new Rect(0, 96, 1440, 804), new Color(.10f, .14f, .12f, .48f));
            Panel(new Rect(377, 243, 700, 431), new Color(.08f, .12f, .10f, .32f));
            Panel(new Rect(370, 236, 700, 431), new Color(.90f, .88f, .78f));
            Panel(new Rect(370, 236, 6, 431), new Color(.28f, .37f, .35f));
            Text(new Rect(404, 257, 625, 23), "battle.dispatch_header", dispatchSmall);
            if (!string.IsNullOrEmpty(setup.RegionNameKey))
                Text(new Rect(404, 280, 625, 20), "battle.dispatch_place", dispatchSmall, L.Text(setup.RegionNameKey));
            Text(new Rect(404, 310, 630, 44), outcome.Won ? "battle.victory" : "battle.defeat", dispatchTitle);
            Panel(new Rect(404, 360, 630, 1), new Color(.60f, .58f, .44f));
            Text(new Rect(404, 375, 188, 20), "battle.result_started", dispatchSmall);
            Text(new Rect(622, 375, 188, 20), "battle.result_survived", dispatchSmall);
            Text(new Rect(841, 375, 188, 20), "battle.result_casualties", dispatchSmall);
            Text(new Rect(404, 397, 188, 39), "battle.result_count", dispatchNumber, originalTroops);
            Text(new Rect(622, 397, 188, 39), "battle.result_count", dispatchNumber, Mathf.Max(0, originalTroops - outcome.Casualties));
            Text(new Rect(841, 397, 188, 39), "battle.result_count", dispatchNumber, outcome.Casualties);
            Panel(new Rect(404, 443, 630, 1), new Color(.60f, .58f, .44f, .6f));
            Text(new Rect(404, 457, 625, 29), "battle.result_field_morale", dispatchBody, Mathf.RoundToInt(outcome.EndingMorale));
            Text(new Rect(404, 487, 625, 29), "battle.result_return_morale", dispatchBody, Mathf.RoundToInt(campaignReturnMorale));
            Text(new Rect(404, 529, 625, 49), outcome.MilitarySuppliesRecovered > 0 ? "battle.result_convoy" : "battle.result_no_convoy", dispatchBody, outcome.MilitarySuppliesRecovered);
            Text(new Rect(404, 578, 625, 22), "battle.result_note", dispatchSmall);
            if (Button(new Rect(404, 611, 630, 35), "battle.continue")) AcceptOutcome();
        }
    }
}
