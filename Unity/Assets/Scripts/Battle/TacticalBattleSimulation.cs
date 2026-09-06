using System;
using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class TacticalBattle
    {
        const float ContactReach = 3.7f;
        const float AttackRecovery = .6f;
        enum AttackMode { None, Ranged, Contact }

        // Görsel nesnelere dokunmadan bir adımın bütün kararlarını aynı durumdan üretir.
        struct StepState
        {
            public Regiment Unit;
            public int Id, Original, Men, Ammo;
            public bool Player, FireAtWill, Moving, Routed, Withdrawn, AimedVolleyPending;
            public Kind Kind;
            public Formation Formation;
            public Condition Condition;
            public float Morale, Fatigue, Cohesion, Experience, Reload, ContactReload, Quiet, Facing;
            public Vector3 Position, Destination;

            public StepState(Regiment unit)
            {
                Unit = unit; Id = unit.Id; Original = unit.Original; Men = unit.Men; Ammo = unit.Ammo;
                Player = unit.Player; FireAtWill = unit.FireAtWill; Moving = unit.Moving;
                Routed = unit.Routed; Withdrawn = unit.Withdrawn; AimedVolleyPending = unit.AimedVolleyPending;
                Kind = unit.Kind; Formation = unit.Formation; Condition = unit.Condition;
                Morale = unit.Morale; Fatigue = unit.Fatigue; Cohesion = unit.Cohesion; Experience = unit.Experience;
                Reload = unit.Reload; ContactReload = unit.ContactReload; Quiet = unit.Quiet; Facing = unit.Facing;
                Position = unit.Position; Destination = unit.Destination;
            }

            public void Apply()
            {
                Unit.Men = Men; Unit.Ammo = Ammo; Unit.Moving = Moving; Unit.Routed = Routed;
                Unit.Withdrawn = Withdrawn; Unit.AimedVolleyPending = AimedVolleyPending;
                Unit.Formation = Formation; Unit.Condition = Condition; Unit.Morale = Morale;
                Unit.Fatigue = Fatigue; Unit.Cohesion = Cohesion; Unit.Reload = Reload;
                Unit.ContactReload = ContactReload; Unit.Quiet = Quiet; Unit.Facing = Facing;
                Unit.Position = Position; Unit.Destination = Destination;
            }
        }

        struct AttackIntent
        {
            public AttackMode Mode;
            public int Target, Casualties;
            public float MoraleLoss, CohesionLoss, Reload, Fatigue;
        }

        Regiment[] StableRegiments()
        {
            Regiment[] result = regiments.ToArray();
            Array.Sort(result, (a, b) => a.Id.CompareTo(b.Id));
            return result;
        }

        void Simulate(float dt)
        {
            if (!Active || ended || paused || dt <= 0 || float.IsNaN(dt) || float.IsInfinity(dt)) return;
            elapsed += dt;
            AdvanceCommandNetwork(dt);
            Regiment[] units = StableRegiments();
            var before = new StepState[units.Length];
            var prepared = new StepState[units.Length];
            for (int i = 0; i < units.Length; i++) before[i] = new StepState(units[i]);
            for (int i = 0; i < units.Length; i++) prepared[i] = PrepareStep(before, i, dt);

            var attacks = new AttackIntent[units.Length];
            bool aimedRequested = false, aimedFired = false;
            for (int i = 0; i < units.Length; i++)
            {
                StepState attacker = prepared[i];
                aimedRequested |= attacker.Player && attacker.AimedVolleyPending;
                int target = FindEnemy(prepared, i);
                if (target < 0) continue;
                AttackMode mode = ContactReady(attacker, prepared[target]) ? AttackMode.Contact :
                    (attacker.FireAtWill || attacker.AimedVolleyPending) && RangedReady(attacker, prepared[target]) ? AttackMode.Ranged : AttackMode.None;
                if (mode == AttackMode.None) continue;
                bool aimed = mode == AttackMode.Ranged && attacker.AimedVolleyPending;
                // Kararlı kimlik sırası yalnız rastgele çekilişi atar; hasar henüz uygulanmaz.
                attacks[i] = PlanAttack(attacker, prepared[target], target, mode, aimed);
                aimedFired |= aimed;
            }

            var losses = new int[units.Length];
            var moraleLoss = new float[units.Length];
            var cohesionLoss = new float[units.Length];
            for (int i = 0; i < units.Length; i++)
            {
                prepared[i].AimedVolleyPending = false;
                AttackIntent attack = attacks[i];
                if (attack.Mode == AttackMode.None) continue;
                prepared[i].Fatigue = attack.Fatigue;
                if (attack.Mode == AttackMode.Ranged)
                {
                    prepared[i].Ammo--;
                    prepared[i].Reload = attack.Reload;
                    prepared[i].ContactReload = Mathf.Max(prepared[i].ContactReload, AttackRecovery);
                }
                else
                {
                    prepared[i].ContactReload = attack.Reload;
                    prepared[i].Reload = Mathf.Max(prepared[i].Reload, AttackRecovery);
                }
                losses[attack.Target] += attack.Casualties;
                moraleLoss[attack.Target] += attack.MoraleLoss;
                cohesionLoss[attack.Target] += attack.CohesionLoss;
            }
            for (int i = 0; i < units.Length; i++)
            {
                if (losses[i] == 0) continue;
                prepared[i].Men = Mathf.Max(0, prepared[i].Men - losses[i]);
                prepared[i].Morale = Mathf.Max(0, prepared[i].Morale - moraleLoss[i]);
                prepared[i].Cohesion = Mathf.Max(0, prepared[i].Cohesion - cohesionLoss[i]);
                prepared[i].Quiet = 0;
            }
            ResolveRoutingWaves(prepared);
            for (int i = 0; i < units.Length; i++) prepared[i].Apply();

            // Aynı adımdaki bütün gerçek atışlar, bozgundan bağımsız olarak aynı görsel saati alır.
            for (int i = 0; i < units.Length; i++)
            {
                if (losses[i] > 0) { units[i].WasHit = true; units[i].LastHit = visualClock; }
                if (attacks[i].Mode != AttackMode.Ranged) continue;
                units[i].LastVolley = visualClock;
                VolleyEffects(units[i], units[attacks[i].Target]);
            }
            if (aimedRequested)
            {
                messageKey = aimedFired ? "battle.volley_fired" : "battle.volley_unavailable";
                messageUntil = elapsed + 6;
            }
            UpdateObjective(dt);
            if (ended) return;
            int allied = 0, opposing = 0;
            foreach (StepState state in prepared)
            {
                if (!Fighting(state)) continue;
                if (state.Player) allied += state.Men; else opposing += state.Men;
            }
            if (allied <= originalTroops * .12f) Finish(false, false);
            else if (opposing == 0) Finish(true, false);
        }

        StepState PrepareStep(StepState[] before, int index, float dt)
        {
            StepState state = before[index];
            if (state.Withdrawn || state.Men <= 0) { state.AimedVolleyPending = false; return state; }
            state.Reload = Mathf.Max(0, state.Reload - dt);
            state.ContactReload = Mathf.Max(0, state.ContactReload - dt);
            state.Quiet += dt;
            if (state.Routed)
            {
                state.AimedVolleyPending = false;
                state.Position += new Vector3(0, 0, state.Player ? -1 : 1) * dt * 3.8f;
                if (Mathf.Abs(state.Position.z) > 35) state.Withdrawn = true;
                return state;
            }
            int enemy = FindEnemy(before, index);
            if (!state.Player && enemy >= 0) EnemyOrders(ref state, before[enemy], before);
            Move(ref state, before, dt);
            if (!state.Moving && enemy >= 0)
                state.Facing = Mathf.MoveTowardsAngle(state.Facing, Heading(before[enemy].Position - state.Position), dt * 38);
            state.Cohesion = Mathf.Clamp(state.Cohesion + (state.Moving ? state.Formation == Formation.Column ? -.12f : -.7f : 2.8f) * dt, 20, 100);
            state.Fatigue = Mathf.Clamp(state.Fatigue + (state.Moving ? .5f : -.65f) * dt, 0, 100);
            if (state.Quiet > 7 && !state.Moving && state.Morale > 20)
                state.Morale = Mathf.Min(state.Player ? Mathf.Clamp(setup.Morale, 30, 100) : 78,
                    state.Morale + dt * .65f * (state.Player ? .5f + setup.CommanderCompetence / 100 : 1));
            return state;
        }

        void Move(ref StepState state, StepState[] before, float dt)
        {
            if (!state.Moving) return;
            Vector3 difference = state.Destination - state.Position; difference.y = 0;
            if (difference.magnitude < .3f) { state.Moving = false; return; }
            float speed = state.Kind == Kind.Cavalry ? 3.6f : state.Kind == Kind.Artillery ? .85f : 1.65f;
            speed *= state.Formation == Formation.Column ? 1.3f : state.Formation == Formation.Square ? .43f : .84f;
            speed *= Mathf.Lerp(1, .52f, state.Fatigue / 100);
            if (InOrchard(state.Position)) speed *= .65f;
            if (InCreek(state.Position)) { speed *= .42f; state.Cohesion = Mathf.Max(20, state.Cohesion - dt * 2); }
            Vector3 direction = difference.normalized;
            foreach (StepState other in before)
            {
                if (other.Id == state.Id || !Fighting(other) || other.Player != state.Player) continue;
                Vector3 away = state.Position - other.Position; away.y = 0;
                float gap = away.magnitude;
                if (gap < 4.6f && gap > .01f) direction += away.normalized * ((4.6f - gap) / 4.6f) * .75f;
            }
            state.Position = Bound(state.Position + direction.normalized * Mathf.Min(speed * dt, difference.magnitude));
            state.Facing = Mathf.MoveTowardsAngle(state.Facing, Heading(difference), dt * 65);
        }

        void EnemyOrders(ref StepState state, StepState enemy, StepState[] before)
        {
            bool commit = elapsed > 35 || playerHold > 8;
            foreach (StepState other in before) if (!other.Player && other.Routed) commit = true;
            if (state.Kind == Kind.Cavalry && !commit) return;
            float distance = FlatDistance(state.Position, enemy.Position);
            if (state.Kind == Kind.Artillery)
            {
                if (distance > 30 && elapsed > 14) { state.Destination = new Vector3(18, 0, 15); state.Moving = true; }
                else state.Moving = false;
                return;
            }
            if (playerHold > 3 && FlatDistance(state.Position, convoy) > 7)
            {
                state.Destination = convoy + new Vector3(state.Id % 2 == 0 ? -3 : 3, 0, 2);
                state.Moving = true;
                return;
            }
            float desired = state.Kind == Kind.Cavalry || state.Ammo <= 0 ? 2.4f : state.Kind == Kind.Militia ? 12 : 16;
            if (distance > desired) { state.Destination = enemy.Position; state.Moving = true; }
            else state.Moving = false;
            if (state.Kind == Kind.Cavalry && distance < 10) state.Formation = Formation.Line;
        }

        static int FindEnemy(StepState[] states, int index)
        {
            int best = -1; float minimum = float.MaxValue;
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].Player == states[index].Player || !Fighting(states[i])) continue;
                float distance = FlatDistance(states[index].Position, states[i].Position);
                if (distance < minimum || (distance == minimum && (best < 0 || states[i].Id < states[best].Id)))
                { best = i; minimum = distance; }
            }
            return best;
        }

        static bool Fighting(StepState state) { return !state.Routed && !state.Withdrawn && state.Men > 0; }
        static float AttackRange(Kind kind) { return kind == Kind.Artillery ? 34 : kind == Kind.Cavalry ? ContactReach : kind == Kind.Militia ? 15 : 19; }
        static bool ContactReady(StepState attacker, StepState target)
        {
            if (!Fighting(attacker) || !Fighting(target) || attacker.Player == target.Player ||
                attacker.Kind == Kind.Artillery || attacker.ContactReload > 0 || FlatDistance(attacker.Position, target.Position) > ContactReach) return false;
            float arc = attacker.Formation == Formation.Square ? 180 : 75;
            return Mathf.Abs(Mathf.DeltaAngle(attacker.Facing, Heading(target.Position - attacker.Position))) <= arc;
        }

        static bool RangedReady(StepState attacker, StepState target)
        {
            if (!Fighting(attacker) || !Fighting(target) || attacker.Player == target.Player ||
                attacker.Kind == Kind.Cavalry || attacker.Ammo <= 0 || attacker.Reload > 0 || attacker.Moving) return false;
            float distance = FlatDistance(attacker.Position, target.Position);
            if (distance > AttackRange(attacker.Kind) || (attacker.Kind != Kind.Artillery && distance <= ContactReach)) return false;
            float arc = attacker.Formation == Formation.Square ? 180 : 45;
            return Mathf.Abs(Mathf.DeltaAngle(attacker.Facing, Heading(target.Position - attacker.Position))) <= arc;
        }

        AttackIntent PlanAttack(StepState attacker, StepState target, int targetIndex, AttackMode mode, bool aimed)
        {
            bool contact = mode == AttackMode.Contact;
            float fatigue = Mathf.Min(100, attacker.Fatigue + (aimed ? 4 : 2));
            float coefficient = contact ? attacker.Kind == Kind.Cavalry ? .34f : attacker.Kind == Kind.Militia ? .14f : .18f : attacker.Kind == Kind.Artillery ? .57f : .43f;
            float power = Mathf.Sqrt(attacker.Men) * coefficient * (.76f + (float)rng.NextDouble() * .48f);
            power *= Mathf.Lerp(.62f, 1, attacker.Cohesion / 100) * Mathf.Lerp(1, .62f, fatigue / 100);
            power *= 1 + attacker.Experience / 300;
            if (aimed) power *= 1.22f;
            if (!contact || attacker.Kind == Kind.Cavalry)
            {
                if (attacker.Formation == Formation.Column) power *= .48f;
                if (attacker.Formation == Formation.Square) power *= .57f;
            }
            if (attacker.Kind == Kind.Cavalry && target.Formation == Formation.Square) power *= .23f;
            if (attacker.Kind == Kind.Cavalry && target.Reload > 4) power *= 1.35f;
            if (!contact)
            {
                if (attacker.Kind == Kind.Artillery && target.Formation == Formation.Square) power *= 1.65f;
                if (TerrainHeight(attacker.Position.x, attacker.Position.z) > TerrainHeight(target.Position.x, target.Position.z) + 1) power *= 1.2f;
                if (InOrchard(target.Position)) power *= .66f;
            }
            float flank = Mathf.Abs(Mathf.DeltaAngle(target.Facing, Heading(attacker.Position - target.Position))) > 100 ? 1.7f : 1;
            int casualties = Mathf.Clamp(Mathf.RoundToInt(power * (flank > 1 ? 1.2f : 1)), 1, target.Men);
            float shock = attacker.Kind == Kind.Artillery ? 5 : attacker.Kind == Kind.Cavalry ? 6 : 2.6f;
            float command = target.Player ? Mathf.Clamp(setup.CommanderCompetence, 0, 100) : 55;
            float reload = contact ? 3.4f : attacker.Kind == Kind.Artillery ? 13 : attacker.Kind == Kind.Militia ? 10.5f : 8;
            return new AttackIntent {
                Mode = mode, Target = targetIndex, Casualties = casualties, Fatigue = fatigue,
                Reload = reload * (1 + attacker.Fatigue / 180),
                MoraleLoss = (casualties * 130f / Mathf.Max(1, target.Original) + shock) * flank * (1.13f - command / 400),
                CohesionLoss = shock * flank
            };
        }

        static void ResolveRoutingWaves(StepState[] states)
        {
            var wave = new bool[states.Length];
            var shock = new float[states.Length];
            for (int round = 0; round < states.Length; round++)
            {
                bool any = false;
                for (int i = 0; i < states.Length; i++)
                {
                    wave[i] = false; shock[i] = 0;
                    if (states[i].Routed) { states[i].Condition = Condition.Routing; continue; }
                    if (states[i].Withdrawn) continue;
                    if (states[i].Men <= states[i].Original * .25f) states[i].Morale = Mathf.Min(states[i].Morale, 19);
                    float morale = states[i].Morale;
                    states[i].Condition = morale >= 72 ? Condition.Steady : morale >= 55 ? Condition.Pressured : morale >= 36 ? Condition.Shaken : morale >= 20 ? Condition.Wavering : Condition.Routing;
                    if (states[i].Condition != Condition.Routing) continue;
                    states[i].Routed = true; states[i].Moving = false; states[i].AimedVolleyPending = false;
                    wave[i] = true; any = true;
                }
                if (!any) break;
                // İlk dalganın bütün alayları işaretlenmeden komşu morali değişmez.
                for (int i = 0; i < states.Length; i++)
                {
                    if (!wave[i]) continue;
                    for (int j = 0; j < states.Length; j++)
                        if (Fighting(states[j]) && states[j].Player == states[i].Player && FlatDistance(states[j].Position, states[i].Position) < 15)
                            shock[j] += 5;
                }
                for (int i = 0; i < states.Length; i++) states[i].Morale = Mathf.Max(0, states[i].Morale - shock[i]);
            }
        }
    }
}
