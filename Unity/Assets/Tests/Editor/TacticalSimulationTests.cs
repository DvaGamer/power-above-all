#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    // Sınır durumları gerçek alay/diorama nesneleriyle kurulur; kaynak sırası oyun kuralı olamaz.
    public sealed class TacticalSimulationTests
    {
        const BindingFlags Fields = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        GameObject host;
        TacticalBattle battle;
        Camera camera;
        IList Units => (IList)Read(battle, "regiments");

        [SetUp]
        public void SetUp()
        {
            host = new GameObject("Tactical simulation verification") { hideFlags = HideFlags.HideAndDontSave };
            battle = host.AddComponent<TacticalBattle>(); battle.enabled = false;
            camera = host.AddComponent<Camera>(); camera.enabled = false;
        }

        [TearDown]
        public void TearDown()
        {
            if (battle != null) battle.Stop();
            if (host != null) UnityEngine.Object.DestroyImmediate(host);
        }

        static object Read(object value, string name) { return value.GetType().GetField(name, Fields).GetValue(value); }
        static void Write(object value, string name, object data) { value.GetType().GetField(name, Fields).SetValue(value, data); }
        static void Call(object value, string name, params object[] args) { value.GetType().GetMethod(name, Fields).Invoke(value, args); }
        object Unit(int id)
        {
            foreach (object unit in Units) if ((int)Read(unit, "Id") == id) return unit;
            throw new InvalidOperationException("Missing unit " + id);
        }
        static void SetKind(object unit, string kind)
        {
            FieldInfo field = unit.GetType().GetField("Kind", Fields);
            field.SetValue(unit, Enum.Parse(field.FieldType, kind));
        }

        void Begin()
        {
            battle.Begin(new BattleSetup { Troops = 1200, Morale = 90, Supply = 100, Fatigue = 0,
                CommanderCompetence = 55, Seed = 1789 }, camera, null);
            foreach (object unit in Units)
            {
                Write(unit, "Withdrawn", true); Write(unit, "FireAtWill", false);
                Write(unit, "Moving", false); Write(unit, "Reload", 0f); Write(unit, "ContactReload", 0f);
            }
        }

        object Activate(int id, Vector3 position, float morale = 80, int ammo = 3)
        {
            object unit = Unit(id);
            Write(unit, "Withdrawn", false); Write(unit, "Routed", false); Write(unit, "Men", 400);
            Write(unit, "Original", 400); Write(unit, "Morale", morale); Write(unit, "Fatigue", 0f);
            Write(unit, "Cohesion", 90f); Write(unit, "Ammo", ammo); Write(unit, "Position", position);
            Write(unit, "Destination", position); Write(unit, "Facing", id < 4 ? 0f : 180f);
            return unit;
        }

        void ReverseUnits()
        {
            for (int i = 0; i < Units.Count / 2; i++)
            {
                int j = Units.Count - i - 1; object temporary = Units[i]; Units[i] = Units[j]; Units[j] = temporary;
            }
        }

        void Tick(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                Call(battle, "Simulate", .05f);
                if (!(bool)Read(battle, "paused")) Write(battle, "visualClock", (float)Read(battle, "visualClock") + .05f);
                Call(battle, "UpdateEffects");
            }
        }

        object[] Snapshot()
        {
            var result = new List<object>();
            for (int id = 0; id < Units.Count; id++)
                foreach (string field in new[] { "Men", "Morale", "Ammo", "Reload", "ContactReload", "AimedVolleyPending",
                    "Position", "Destination", "Facing", "Moving", "Routed", "Withdrawn", "Condition", "Cohesion", "Fatigue", "Quiet" })
                    result.Add(Read(Unit(id), field));
            foreach (string field in new[] { "elapsed", "playerHold", "enemyHold", "ended" }) result.Add(Read(battle, field));
            BattleOutcome outcome = (BattleOutcome)Read(battle, "outcome");
            result.Add(outcome == null);
            if (outcome != null) { result.Add(outcome.Won); result.Add(outcome.Casualties); result.Add(outcome.EndingMorale); }
            return result.ToArray();
        }

        object[] RunFragileDuel(bool reverse, bool aimed)
        {
            Begin();
            object player = Activate(0, Vector3.zero, 24), enemy = Activate(4, new Vector3(0, 0, 10), 24);
            Write(player, "FireAtWill", true); Write(enemy, "FireAtWill", true);
            if (reverse) ReverseUnits();
            if (aimed) { Call(battle, "OrderVolley"); Call(battle, "OrderVolley"); }
            Assert.That(Read(player, "Ammo"), Is.EqualTo(3), "emir hasar veya cephane tüketmez");
            Assert.That(Read(enemy, "Men"), Is.EqualTo(400));
            Tick();
            Assert.That(Read(player, "Ammo"), Is.EqualTo(2));
            Assert.That(Read(enemy, "Ammo"), Is.EqualTo(2), "aynı adımda bozulan düşmanın hazır atışı da çıkar");
            Assert.That(Read(player, "Routed"), Is.True);
            Assert.That(Read(enemy, "Routed"), Is.True);
            Assert.That(Read(player, "AimedVolleyPending"), Is.False);
            return Snapshot();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void SimultaneousFatalVolleysAndQueuedAimedVolleyIgnoreListOrder(bool aimed)
        {
            object[] normal = RunFragileDuel(false, aimed);
            object[] reversed = RunFragileDuel(true, aimed);
            CollectionAssert.AreEqual(normal, reversed);
        }

        [Test]
        public void SeveralPreparedHitsClampTotalCasualtiesButDoNotCancelVictimsShot()
        {
            Begin();
            object first = Activate(0, new Vector3(-2, 0, 0)), second = Activate(1, new Vector3(2, 0, 0));
            object target = Activate(4, new Vector3(0, 0, 10));
            SetKind(second, "Line"); Write(first, "FireAtWill", true); Write(second, "FireAtWill", true);
            Write(target, "FireAtWill", true); Write(target, "Men", 2); Write(target, "Original", 2);
            Tick();
            Assert.That(Read(first, "Ammo"), Is.EqualTo(2)); Assert.That(Read(second, "Ammo"), Is.EqualTo(2));
            Assert.That(Read(target, "Ammo"), Is.EqualTo(2)); Assert.That(Read(target, "Men"), Is.EqualTo(0));
        }

        object[] RunRoutingChain(bool reverse)
        {
            Begin();
            Activate(0, Vector3.zero, 19, 0); Activate(1, new Vector3(10, 0, 0), 24, 0);
            Activate(2, new Vector3(20, 0, 0), 24, 0); object observer = Activate(3, new Vector3(30, 0, 0), 40, 0);
            Activate(4, new Vector3(-30, 0, 30), 80, 0);
            if (reverse) ReverseUnits();
            Tick();
            for (int i = 0; i < 3; i++) Assert.That(Read(Unit(i), "Routed"), Is.True);
            Assert.That(Read(observer, "Morale"), Is.EqualTo(35f), "üçüncü dalga bir kez ulaşır");
            Tick();
            Assert.That(Read(observer, "Morale"), Is.EqualTo(35f), "eski bozgun her adımda yeniden şok yaymaz");
            return Snapshot();
        }

        [Test]
        public void SecondaryRoutingWavesAreSimultaneousAndEveryRoutShocksOnlyOnce()
        {
            CollectionAssert.AreEqual(RunRoutingChain(false), RunRoutingChain(true));
        }

        object[] RunMovement(bool reverse)
        {
            Begin();
            object first = Activate(0, Vector3.zero), second = Activate(1, new Vector3(2, 0, 0));
            Activate(4, new Vector3(28, 0, 25), 80, 0);
            Write(first, "Moving", true); Write(second, "Moving", true);
            Write(first, "Destination", new Vector3(0, 0, 12)); Write(second, "Destination", new Vector3(2, 0, 12));
            if (reverse) ReverseUnits();
            Tick(30);
            Assert.That(Read(first, "Position"), Is.Not.EqualTo(Vector3.zero));
            return Snapshot();
        }

        [Test]
        public void FriendlyAvoidanceAndEnemyApproachReadTheSameMovementSnapshot()
        {
            CollectionAssert.AreEqual(RunMovement(false), RunMovement(true));
        }

        [TestCase("Line")]
        [TestCase("Militia")]
        public void ExhaustedEmptyInfantryClosesAndResolvesContestedCombat(string kind)
        {
            Begin();
            object player = Activate(0, new Vector3(4, 0, 1), 80, 0), enemy = Activate(4, new Vector3(4, 0, 5), 80, 0);
            SetKind(player, kind); SetKind(enemy, kind);
            Write(player, "Fatigue", 100f); Write(enemy, "Fatigue", 100f);
            Write(player, "Reload", 8f); Write(enemy, "Reload", 8f);
            Tick(90);
            Assert.That(Read(player, "Men"), Is.LessThan(400)); Assert.That(Read(enemy, "Men"), Is.LessThan(400));
            Assert.That(Read(player, "Ammo"), Is.EqualTo(0)); Assert.That(Read(enemy, "Ammo"), Is.EqualTo(0));
            Assert.That(Read(enemy, "Position"), Is.Not.EqualTo(new Vector3(4, 0, 5)), "AI artık tüfek mesafesinde beklemez");
            for (int i = 0; i < 2400 && !(bool)Read(battle, "ended"); i++) Tick();
            Assert.That(Read(battle, "ended"), Is.True, "yakın çatışma zorunlu süre aşımı olmadan bozulur");
            Assert.That(Read(player, "Ammo"), Is.EqualTo(0)); Assert.That(Read(enemy, "Ammo"), Is.EqualTo(0));
        }

        [Test]
        public void HoldFireStopsMusketFireButKeepsCloseDefenseWithoutSpendingAmmo()
        {
            Begin(); object player = Activate(0, Vector3.zero), enemy = Activate(4, new Vector3(0, 0, 10));
            Tick();
            Assert.That(Read(player, "Men"), Is.EqualTo(400)); Assert.That(Read(enemy, "Men"), Is.EqualTo(400));
            Write(enemy, "Position", new Vector3(0, 0, 3));
            Tick();
            Assert.That(Read(player, "Men"), Is.LessThan(400)); Assert.That(Read(enemy, "Men"), Is.LessThan(400));
            Assert.That(Read(player, "Ammo"), Is.EqualTo(3)); Assert.That(Read(enemy, "Ammo"), Is.EqualTo(3));
        }

        [Test]
        public void ArtilleryWithoutAmmoDoesNotAcquireInfantryContactAttack()
        {
            Begin(); object gun = Activate(0, Vector3.zero, 80, 0), enemy = Activate(4, new Vector3(0, 0, 3), 80, 0);
            SetKind(gun, "Artillery"); Tick();
            Assert.That(Read(gun, "Men"), Is.LessThan(400)); Assert.That(Read(enemy, "Men"), Is.EqualTo(400));
            Assert.That(Read(gun, "ContactReload"), Is.EqualTo(0f));
        }

        [TestCase("Routed")]
        [TestCase("Withdrawn")]
        public void UnavailableInfantryCannotAttackInContact(string flag)
        {
            Begin(); object player = Activate(0, Vector3.zero, 80, 0), enemy = Activate(4, new Vector3(0, 0, 3), 80, 0);
            Write(player, flag, true); Tick();
            Assert.That(Read(enemy, "Men"), Is.EqualTo(400)); Assert.That(Read(player, "ContactReload"), Is.EqualTo(0f));
        }

        [Test]
        public void LastMusketShotRequiresBriefRecoveryThenAllowsContactBeforeMusketReload()
        {
            Begin(); object player = Activate(0, Vector3.zero, 80, 1), enemy = Activate(4, new Vector3(0, 0, 10), 80, 0);
            Write(player, "FireAtWill", true); Tick();
            Assert.That(Read(player, "Ammo"), Is.EqualTo(0));
            int afterShot = (int)Read(enemy, "Men");
            Write(enemy, "Position", new Vector3(0, 0, 3)); Tick();
            Assert.That(Read(enemy, "Men"), Is.EqualTo(afterShot), "sınırı geçmek anlık ikinci saldırı vermez");
            Tick(13);
            Assert.That(Read(enemy, "Men"), Is.LessThan(afterShot));
            Assert.That((float)Read(player, "Reload"), Is.GreaterThan(6f));
            Assert.That(Read(player, "Ammo"), Is.EqualTo(0));
        }

        [Test]
        public void PausedTickPreservesQueuedIntentAndEveryCombatResourceUntilResume()
        {
            Begin(); object player = Activate(0, Vector3.zero); Activate(4, new Vector3(0, 0, 10));
            Call(battle, "OrderVolley"); Write(battle, "paused", true);
            object[] before = Snapshot(); Tick(4);
            CollectionAssert.AreEqual(before, Snapshot());
            Assert.That(((IList)Read(battle, "effects")).Count, Is.Zero);
            Assert.That(Read(player, "AimedVolleyPending"), Is.True);
            Write(battle, "paused", false); Tick();
            Assert.That(Read(player, "Ammo"), Is.EqualTo(2)); Assert.That(Read(player, "AimedVolleyPending"), Is.False);
        }

        [Test]
        public void InvalidatedQueuedTargetConsumesNeitherAmmoNorEffects()
        {
            Begin(); object player = Activate(0, Vector3.zero), enemy = Activate(4, new Vector3(0, 0, 10));
            Call(battle, "OrderVolley"); Write(enemy, "Withdrawn", true); Tick();
            Assert.That(Read(player, "Ammo"), Is.EqualTo(3)); Assert.That(Read(player, "AimedVolleyPending"), Is.False);
            Assert.That(((IList)Read(battle, "effects")).Count, Is.Zero);
            Assert.That(Read(battle, "messageKey"), Is.EqualTo("battle.volley_unavailable"));
        }

        object[] RunTiedTargets(bool reverse)
        {
            Begin(); object player = Activate(0, Vector3.zero);
            Activate(4, new Vector3(-2, 0, 10)); Activate(5, new Vector3(2, 0, 10)); SetKind(Unit(5), "Line");
            Write(player, "FireAtWill", true);
            if (reverse) ReverseUnits(); Tick();
            Assert.That(Read(Unit(4), "Men"), Is.LessThan(400)); Assert.That(Read(Unit(5), "Men"), Is.EqualTo(400));
            return Snapshot();
        }

        [Test]
        public void EqualDistanceTargetsUseStableIdentityRatherThanListPosition()
        {
            CollectionAssert.AreEqual(RunTiedTargets(false), RunTiedTargets(true));
        }
    }
}
#endif
