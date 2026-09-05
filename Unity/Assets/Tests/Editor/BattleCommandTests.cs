#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    // Oyuncu ve script ortak emir API'sini kullanır; özel alay/saat alanına yazılmaz.
    public sealed class BattleCommandTests
    {
        GameObject host;
        TacticalBattle battle;
        Camera camera;

        [SetUp] public void SetUp()
        {
            host = new GameObject("Battle command verification") { hideFlags = HideFlags.HideAndDontSave };
            battle = host.AddComponent<TacticalBattle>(); battle.enabled = false;
            camera = host.AddComponent<Camera>(); camera.enabled = false;
        }

        [TearDown] public void TearDown()
        {
            if (battle != null) battle.Stop();
            if (host != null) Object.DestroyImmediate(host);
        }

        void Begin(System.Action<BattleOutcome> callback = null)
        { battle.Begin(new BattleSetup { Troops = 1200, Supply = 100, Morale = 78, Seed = 1789 }, camera, callback); }

        static void Accepted(BattleOrderResult result) { Assert.IsTrue(result.Ok, result.ReasonKey); }
        string Snapshot() => JsonUtility.ToJson(battle.CaptureSnapshot());
        BattleRegimentSnapshot Unit(int slot) => System.Array.Find(battle.CaptureSnapshot().Regiments, unit => unit.PlayerSlot == slot);

        [Test]
        public void InactiveAndInvalidOrdersRefuseWithoutChangingSnapshot()
        {
            string inactive = Snapshot();
            Assert.IsFalse(battle.SetPaused(true).Ok);
            Assert.IsFalse(battle.MoveSelected(Vector2.zero).Ok);
            Assert.IsFalse(battle.VolleySelected().Ok);
            Assert.IsFalse(battle.Retreat().Ok);
            Assert.AreEqual(inactive, Snapshot());
            Begin();
            string before = Snapshot();
            foreach (int slot in new[] { -1, 0, 5, 8 }) Assert.IsFalse(battle.SelectPlayerRegiment(slot).Ok);
            Assert.IsFalse(battle.SelectPlayerRegiment(1, (BattleSelectionMode)99).Ok);
            Assert.IsFalse(battle.SetSelectedFormation((BattleFormation)99).Ok);
            Assert.IsFalse(battle.MoveSelected(new Vector2(float.NaN, 1)).Ok);
            Assert.IsFalse(battle.MoveSelected(new Vector2(1, float.PositiveInfinity)).Ok);
            Assert.AreEqual(before, Snapshot());
        }

        [Test]
        public void SelectionPreservesReplaceAddAndMouseToggleSemantics()
        {
            Begin();
            Accepted(battle.SelectPlayerRegiment(1));
            Accepted(battle.SelectPlayerRegiment(2, BattleSelectionMode.Add));
            Accepted(battle.SelectPlayerRegiment(2, BattleSelectionMode.Add));
            CollectionAssert.AreEqual(new[] { 0, 1 }, battle.CaptureSnapshot().SelectedIds);
            Accepted(battle.SelectPlayerRegiment(2, BattleSelectionMode.Toggle));
            CollectionAssert.AreEqual(new[] { 0 }, battle.CaptureSnapshot().SelectedIds);
            Accepted(battle.SelectPlayerRegiment(4, BattleSelectionMode.Replace));
            CollectionAssert.AreEqual(new[] { 3 }, battle.CaptureSnapshot().SelectedIds);
            Accepted(battle.SelectPlayerRegiment(4, BattleSelectionMode.Toggle));
            Assert.IsEmpty(battle.CaptureSnapshot().SelectedIds);
            Assert.IsFalse(battle.CaptureSnapshot().SelectionArrived, "Boş seçim varış değildir.");
            string before = Snapshot();
            Assert.IsFalse(battle.MoveSelected(Vector2.zero).Ok);
            Assert.IsFalse(battle.SetSelectedFireAtWill(false).Ok);
            Assert.AreEqual(before, Snapshot());
        }

        [Test]
        public void PausedGroupMoveSetsBoundedDestinationsAndPreservesPositionAndSpacing()
        {
            Begin();
            Accepted(battle.SetPaused(true));
            Accepted(battle.SelectPlayerRegiment(1));
            Accepted(battle.SelectPlayerRegiment(2, BattleSelectionMode.Add));
            BattleRegimentSnapshot first = Unit(1), second = Unit(2);
            Accepted(battle.MoveSelected(new Vector2(-10, 6)));
            Assert.AreEqual(first.PositionX, Unit(1).PositionX);
            Assert.AreEqual(first.PositionZ, Unit(1).PositionZ);
            Assert.AreEqual(second.PositionX - first.PositionX, Unit(2).DestinationX - Unit(1).DestinationX);
            Assert.AreEqual(-10, (Unit(1).DestinationX + Unit(2).DestinationX) / 2);
            Assert.AreEqual(6, Unit(1).DestinationZ);
            Assert.IsTrue(Unit(1).Moving);
            Assert.IsFalse(battle.CaptureSnapshot().SelectionArrived);
            Accepted(battle.MoveSelected(new Vector2(1000, -1000)));
            Assert.AreEqual(36, Unit(1).DestinationX);
            Assert.AreEqual(-28, Unit(1).DestinationZ);
            Assert.AreEqual(first.PositionX, Unit(1).PositionX, "Emir ışınlanma değildir.");
            Assert.AreEqual(0, battle.CaptureSnapshot().ElapsedSeconds);
        }

        [Test]
        public void MixedSquareOrderChangesInfantryOnlyAndRepeatingItDoesNotChargeCohesionAgain()
        {
            Begin();
            Accepted(battle.SetPaused(true));
            Accepted(battle.SelectPlayerRegiment(1));
            Accepted(battle.SelectPlayerRegiment(3, BattleSelectionMode.Add));
            Accepted(battle.SelectPlayerRegiment(4, BattleSelectionMode.Add));
            float before = Unit(1).Cohesion;
            BattleOrderResult square = battle.SetSelectedFormation(BattleFormation.Square);
            Accepted(square); Assert.AreEqual(1, square.AffectedCount);
            Assert.AreEqual("Square", Unit(1).Formation);
            Assert.Less(Unit(1).Cohesion, before);
            Assert.AreEqual("Line", Unit(3).Formation);
            Assert.AreEqual("Line", Unit(4).Formation);
            string snapshot = Snapshot();
            BattleOrderResult repeat = battle.SetSelectedFormation(BattleFormation.Square);
            Accepted(repeat); Assert.AreEqual(0, repeat.AffectedCount);
            Assert.AreEqual(snapshot, Snapshot());
            Accepted(battle.SelectPlayerRegiment(3));
            snapshot = Snapshot();
            Assert.IsFalse(battle.SetSelectedFormation(BattleFormation.Square).Ok);
            Assert.AreEqual(snapshot, Snapshot());
        }

        [Test]
        public void FirePolicyAffectsSelectionAndPausedVolleyCannotQueueOrFire()
        {
            Begin();
            Accepted(battle.SelectPlayerRegiment(1));
            Accepted(battle.SelectPlayerRegiment(4, BattleSelectionMode.Add));
            Accepted(battle.SetSelectedFireAtWill(false));
            Assert.IsFalse(Unit(1).FireAtWill); Assert.IsFalse(Unit(4).FireAtWill);
            Assert.IsTrue(Unit(2).FireAtWill); Assert.IsTrue(Unit(3).FireAtWill);
            Accepted(battle.SetPaused(true));
            string before = Snapshot();
            var volley = battle.VolleySelected();
            Assert.IsFalse(volley.Ok);
            Assert.AreEqual("battle.volley_reason_pause", volley.ReasonKey);
            Assert.AreEqual(before, Snapshot());
            Assert.IsFalse(Unit(1).AimedVolleyPending);
            Assert.IsFalse(Unit(4).AimedVolleyPending);
        }

        [Test]
        public void VolleyWithoutReachableReadyTargetCannotConsumeAmmunitionOrChangeOrders()
        {
            Begin();
            Accepted(battle.SelectPlayerRegiment(4));
            Accepted(battle.SetSelectedFireAtWill(false));
            Assert.IsFalse(battle.CaptureSnapshot().CanVolley);
            string before = Snapshot();
            Assert.IsFalse(battle.VolleySelected().Ok);
            Assert.AreEqual(before, Snapshot());
        }

        [Test]
        public void MutatingAnExportedSnapshotCannotChangeLiveBattleOrItsSelection()
        {
            Begin();
            string before = Snapshot();
            BattleSnapshot exported = battle.CaptureSnapshot();
            exported.SelectedIds[0] = 999;
            exported.Regiments[0].Men = 0;
            exported.Regiments[0].PositionX = 999;
            exported.Regiments[1] = null;
            exported.Paused = true; exported.PlayerHold = 45; exported.Won = true;
            Assert.AreEqual(before, Snapshot());
        }

        [Test]
        public void RealRetreatButtonProducesOneReportAndOnlyAcceptTransfersItOnce()
        {
            int deliveries = 0; BattleOutcome received = null;
            Begin(value => { deliveries++; received = value; });
            Assert.IsFalse(battle.AcceptReport().Ok);
            Accepted(battle.SetPaused(true));
            Accepted(battle.Retreat());
            BattleSnapshot report = battle.CaptureSnapshot();
            Assert.IsTrue(report.Ended); Assert.IsTrue(report.HasOutcome); Assert.IsFalse(report.Won);
            Assert.Greater(report.Casualties, 0); Assert.Less(report.Casualties, report.OriginalTroops);
            Assert.AreEqual(0, deliveries);
            string before = Snapshot();
            Assert.IsFalse(battle.Retreat().Ok); Assert.IsFalse(battle.VolleySelected().Ok);
            Assert.IsFalse(battle.MoveSelected(Vector2.zero).Ok);
            Assert.AreEqual(before, Snapshot());
            Accepted(battle.AcceptReport());
            Assert.AreEqual(1, deliveries); Assert.IsFalse(battle.Active);
            Assert.AreEqual(report.Casualties, received.Casualties);
            Assert.AreEqual(report.EndingMorale, received.EndingMorale);
            Assert.IsFalse(battle.AcceptReport().Ok); Assert.AreEqual(1, deliveries);
        }
    }
}
#endif
