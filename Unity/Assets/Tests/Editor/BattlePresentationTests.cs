#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    // Geçici bir diorama kullanır; GameApp, kayıtlar ve oyuncu kampanyası açılmaz.
    public sealed class BattlePresentationTests
    {
        const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        GameObject host;
        TacticalBattle battle;
        Camera camera;

        [SetUp]
        public void SetUp()
        {
            host = new GameObject("Battle presentation verification") { hideFlags = HideFlags.HideAndDontSave };
            battle = host.AddComponent<TacticalBattle>(); battle.enabled = false;
            camera = host.AddComponent<Camera>(); camera.enabled = false;
        }

        [TearDown]
        public void TearDown()
        {
            if (battle != null) battle.Stop();
            if (host != null) UnityEngine.Object.DestroyImmediate(host);
        }

        static object Read(object instance, string name)
        {
            FieldInfo field = instance.GetType().GetField(name, PrivateInstance);
            Assert.That(field, Is.Not.Null, name);
            return field.GetValue(instance);
        }

        static void Write(object instance, string name, object value)
        {
            FieldInfo field = instance.GetType().GetField(name, PrivateInstance);
            Assert.That(field, Is.Not.Null, name);
            field.SetValue(instance, value);
        }

        static object Call(object instance, string name, params object[] arguments)
        {
            MethodInfo method = instance.GetType().GetMethod(name, PrivateInstance);
            Assert.That(method, Is.Not.Null, name);
            return method.Invoke(instance, arguments);
        }

        void Begin(Action<BattleOutcome> callback = null, Func<bool, float, float> returnMorale = null)
        {
            battle.Begin(new BattleSetup { Troops = 1200, Seed = 1789, Supply = 74, Morale = 76,
                Fatigue = 13, CommanderCompetence = 63, CampaignMoraleAfterBattle = returnMorale }, camera, callback);
        }

        IList Regiments => (IList)Read(battle, "regiments");

        void PrepareShot()
        {
            object attacker = Regiments[0], target = Regiments[4];
            Write(attacker, "Position", Vector3.zero);
            Write(attacker, "Facing", 0f);
            Write(attacker, "Moving", false);
            Write(attacker, "Reload", 0f);
            Write(target, "Position", new Vector3(0, 0, 10));
            Write(target, "Facing", 180f);
            Write(battle, "visualClock", 3f);
            Call(battle, "UpdateVisual", attacker, 1f);
            Call(battle, "UpdateVisual", target, 1f);
        }

        [Test]
        public void BeginCreatesNativePropertyBlockAndStopReleasesTheDiorama()
        {
            Assert.That(Read(battle, "smokeProperties"), Is.Null);
            Assert.DoesNotThrow(() => Begin());
            Assert.That(Read(battle, "smokeProperties"), Is.TypeOf<MaterialPropertyBlock>());
            GameObject firstWorld = (GameObject)Read(battle, "world");
            Assert.That(Regiments.Count, Is.EqualTo(8));
            battle.Stop();
            Assert.That(firstWorld == null, Is.True);
            Assert.That(battle.Active, Is.False);
            Assert.That(((IList)Read(battle, "materials")).Count, Is.Zero);
            Assert.DoesNotThrow(() => Begin());
            Assert.That(battle.Active, Is.True);
        }

        [Test]
        public void CombatIsIdenticalWithAndWithoutIntermediatePresentationUpdates()
        {
            object[] withPresentation = RunBattle(true);
            battle.Stop();
            object[] withoutPresentation = RunBattle(false);
            CollectionAssert.AreEqual(withPresentation, withoutPresentation);
        }

        object[] RunBattle(bool updatePresentation)
        {
            Begin();
            for (int tick = 0; tick < 1000 && !(bool)Read(battle, "ended"); tick++)
            {
                Write(battle, "visualClock", tick * .05f);
                Call(battle, "Simulate", .05f);
                if (updatePresentation)
                    foreach (object regiment in Regiments) Call(battle, "UpdateVisual", regiment, .05f);
                Call(battle, "UpdateEffects");
            }
            var state = new List<object>();
            foreach (object regiment in Regiments)
                foreach (string field in new[] { "Men", "Morale", "Ammo", "Reload", "Position", "Facing", "Routed", "Cohesion", "Fatigue" })
                    state.Add(Read(regiment, field));
            foreach (string field in new[] { "elapsed", "playerHold", "enemyHold", "ended" }) state.Add(Read(battle, field));
            BattleOutcome outcome = (BattleOutcome)Read(battle, "outcome");
            state.Add(outcome == null);
            if (outcome != null)
            {
                state.Add(outcome.Won); state.Add(outcome.Casualties);
                state.Add(outcome.EndingMorale); state.Add(outcome.MilitarySuppliesRecovered);
            }
            return state.ToArray();
        }

        [Test]
        public void DamageFlashAndReactionShareOneTimestamp()
        {
            Begin(); PrepareShot();
            object attacker = Regiments[0], target = Regiments[4];
            int menBefore = (int)Read(target, "Men"), ammoBefore = (int)Read(attacker, "Ammo");
            Call(battle, "Shoot", attacker, target, false);
            Assert.That((int)Read(target, "Men"), Is.LessThan(menBefore));
            Assert.That((int)Read(attacker, "Ammo"), Is.EqualTo(ammoBefore - 1));
            Assert.That(Read(attacker, "LastVolley"), Is.EqualTo(3f));
            Assert.That(Read(target, "LastHit"), Is.EqualTo(3f));
            IList effects = (IList)Read(battle, "effects");
            Assert.That(effects.Count, Is.GreaterThan(0));
            foreach (object effect in effects) Assert.That(Read(effect, "Born"), Is.EqualTo(3f));
            Call(battle, "UpdateEffects");
            object firstFlash = effects[1];
            Assert.That((bool)Read(firstFlash, "Flash"), Is.True);
            Assert.That(Read(firstFlash, "Delay"), Is.EqualTo(0f));
            Assert.That(((GameObject)Read(firstFlash, "Object")).activeSelf, Is.True);
        }

        [Test]
        public void FrozenPresentationClockKeepsSmokeFiguresAndStandardStill()
        {
            Begin(); PrepareShot();
            object attacker = Regiments[0], target = Regiments[4];
            Call(battle, "Shoot", attacker, target, false);
            Write(attacker, "Moving", true);
            Write(battle, "visualClock", 3.23f);
            Call(battle, "UpdateVisual", attacker, .23f);
            Call(battle, "UpdateEffects");
            object puff = ((IList)Read(battle, "effects"))[0];
            Transform cloud = ((GameObject)Read(puff, "Object")).transform;
            Transform standard = ((GameObject)Read(attacker, "Flag")).transform;
            object figure = ((IList)Read(attacker, "Figures"))[0];
            Transform miniature = (Transform)Read(figure, "Root");
            Transform boot = (Transform)Read(figure, "LeftBoot");
            Vector3 smokePosition = cloud.position, smokeScale = cloud.localScale, figurePosition = miniature.position, bootPosition = boot.localPosition;
            Quaternion flagRotation = standard.localRotation, figureRotation = miniature.localRotation;
            Write(battle, "paused", true);
            for (int frame = 0; frame < 4; frame++)
            {
                Call(battle, "UpdateVisual", attacker, 0f);
                Call(battle, "UpdateEffects");
            }
            Assert.That(cloud.position, Is.EqualTo(smokePosition));
            Assert.That(cloud.localScale, Is.EqualTo(smokeScale));
            Assert.That(miniature.position, Is.EqualTo(figurePosition));
            Assert.That(miniature.localRotation, Is.EqualTo(figureRotation));
            Assert.That(boot.localPosition, Is.EqualTo(bootPosition));
            Assert.That(standard.localRotation, Is.EqualTo(flagRotation));
            Write(battle, "paused", false);
            Write(battle, "visualClock", 3.43f);
            Call(battle, "UpdateVisual", attacker, .2f);
            Call(battle, "UpdateEffects");
            Assert.That(cloud.position, Is.Not.EqualTo(smokePosition));
            Assert.That(boot.localPosition, Is.Not.EqualTo(bootPosition));
        }

        [Test]
        public void PausingBeforeFirstEffectUpdateDefersFlashAndAudioUntilResume()
        {
            Begin(); PrepareShot();
            int sounds = 0;
            battle.Feedback += cue => { if (cue == "volley") sounds++; };
            Call(battle, "Shoot", Regiments[0], Regiments[4], false);
            IList effects = (IList)Read(battle, "effects");
            Write(battle, "paused", true);
            Call(battle, "UpdateEffects");
            foreach (object effect in effects)
                Assert.That(((GameObject)Read(effect, "Object")).activeSelf, Is.False);
            Assert.That(sounds, Is.Zero);
            Write(battle, "paused", false);
            Call(battle, "UpdateEffects");
            Assert.That(((GameObject)Read(effects[1], "Object")).activeSelf, Is.True);
            Assert.That(sounds, Is.EqualTo(1));
            Call(battle, "UpdateEffects");
            Assert.That(sounds, Is.EqualTo(1));
        }

        [Test]
        public void FinalReportAndCallbackAreProducedOnceAndIncludeRetreatLosses()
        {
            int deliveries = 0, previews = 0;
            BattleOutcome delivered = null;
            Begin(result => { deliveries++; delivered = result; }, (won, morale) => { previews++; return 41f; });
            Call(battle, "Finish", false, true);
            Call(battle, "Finish", true, false);
            Assert.That(previews, Is.EqualTo(1));
            Assert.That(Read(battle, "campaignReturnMorale"), Is.EqualTo(41f));
            Call(battle, "AcceptOutcome");
            Call(battle, "AcceptOutcome");
            Assert.That(deliveries, Is.EqualTo(1));
            Assert.That(delivered, Is.Not.Null);
            Assert.That(delivered.Won, Is.False);
            Assert.That(delivered.Casualties, Is.EqualTo(Mathf.RoundToInt(1200 * .035f)));
            Assert.That(delivered.EndingMorale, Is.InRange(0f, 100f));
            Assert.That(delivered.MilitarySuppliesRecovered, Is.Zero);
            Assert.That(battle.Active, Is.False);
        }
    }
}
#endif
