#if UNITY_EDITOR
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    public sealed class CommandNetworkTests
    {
        const BindingFlags Private = BindingFlags.NonPublic|BindingFlags.Instance;
        GameObject host;TacticalBattle battle;
        [SetUp] public void Begin()
        {
            host=new GameObject("Command network verification"){hideFlags=HideFlags.HideAndDontSave};
            var camera=host.AddComponent<Camera>();camera.enabled=false;
            battle=host.AddComponent<TacticalBattle>();battle.enabled=false;
            battle.Begin(new BattleSetup{Troops=1200,EnemyTroops=1080,Supply=100,Morale=90,Fatigue=0,CommanderCompetence=75,CommandNetwork=true},camera,null);
        }
        [TearDown] public void End(){battle.Stop();Object.DestroyImmediate(host);}
        BattleRegimentSnapshot Unit(int slot=1)=>System.Array.Find(battle.CaptureSnapshot().Regiments,r=>r.PlayerSlot==slot);
        void Step(float seconds)
        {for(int i=0;i<Mathf.CeilToInt(seconds/.05f);i++)typeof(TacticalBattle).GetMethod("Simulate",Private).Invoke(battle,new object[]{.05f});}
        object Live(int slot=1)=>((IList)typeof(TacticalBattle).GetField("regiments",Private).GetValue(battle))[slot-1];
        void Set(string field,object value,int slot=1)=>Live(slot).GetType().GetField(field,BindingFlags.Public|BindingFlags.Instance).SetValue(Live(slot),value);
        void PlaceEnemy(string kind,Vector3 point)
        {
            foreach(object r in (IList)typeof(TacticalBattle).GetField("regiments",Private).GetValue(battle))
            {
                var type=r.GetType();
                if((bool)type.GetField("Player").GetValue(r)||type.GetField("Kind").GetValue(r).ToString()!=kind)continue;
                type.GetField("Position").SetValue(r,point);type.GetField("Destination").SetValue(r,point);return;
            }
            Assert.Fail("Required enemy regiment is missing: "+kind);
        }
        [Test] public void MovementWaitsForReceptionAndThenStarts()
        {
            var before=Unit();Assert.IsTrue(battle.MoveSelected(new Vector2(0,0)).Ok);Assert.AreEqual(before.DestinationZ,Unit().DestinationZ);
            Assert.AreEqual(1,Unit().PendingCommands);Step(.1f);Assert.IsFalse(Unit().Moving);Step(4.5f);
            Assert.IsTrue(Unit().Moving);Assert.AreEqual(0,Unit().PendingCommands);Assert.AreEqual("Move",Unit().LastReceivedOrder);
        }
        [Test] public void RepeatingAnIdenticalMessageDoesNotRestartDelivery()
        {
            battle.MoveSelected(Vector2.zero);float arrival=Unit().NextCommandAt;Step(.1f);
            Assert.AreEqual(0,battle.MoveSelected(Vector2.zero).AffectedCount);Assert.AreEqual(arrival,Unit().NextCommandAt);Assert.AreEqual(1,Unit().PendingCommands);
        }
        [Test] public void NewDestinationDoesNotEraseTheReceivedOrderInFlight()
        {
            battle.MoveSelected(new Vector2(0,0));Step(4.5f);var before=Unit();battle.MoveSelected(new Vector2(30,12));Step(.1f);
            Assert.AreEqual(before.DestinationX,Unit().DestinationX);Assert.AreEqual(before.DestinationZ,Unit().DestinationZ);
            Assert.AreNotEqual(before.PositionZ,Unit().PositionZ);Assert.AreEqual(1,Unit().PendingCommands);
        }
        [Test] public void GroupQueueRefusalCannotPartiallyOrderAnotherRegiment()
        {
            battle.MoveSelected(Vector2.zero);battle.SetSelectedFormation(BattleFormation.Column);battle.SelectPlayerRegiment(2,BattleSelectionMode.Add);
            string before=JsonUtility.ToJson(battle.CaptureSnapshot());Assert.IsFalse(battle.SetSelectedFireAtWill(false).Ok);
            Assert.AreEqual(before,JsonUtility.ToJson(battle.CaptureSnapshot()));Assert.AreEqual(0,Unit(2).PendingCommands);
        }
        [Test] public void PauseFreezesBothCourierAndHeadquarters()
        {
            battle.MoveSelected(Vector2.zero);battle.MoveHeadquarters(new Vector2(20,0));battle.SetPaused(true);
            string before=JsonUtility.ToJson(battle.CaptureSnapshot());Step(5);Assert.AreEqual(before,JsonUtility.ToJson(battle.CaptureSnapshot()));
        }
        [Test] public void HeadquartersPhysicallyTravelsInsteadOfTeleporting()
        {
            var before=battle.HeadquartersPosition;battle.MoveHeadquarters(new Vector2(20,0));Assert.AreEqual(before,battle.HeadquartersPosition);
            Step(.1f);Assert.Greater(Vector3.Distance(before,battle.HeadquartersPosition),0);Assert.Less(Vector3.Distance(before,battle.HeadquartersPosition),.3f);
        }
        [Test] public void FormationIsReceivedLaterAndHasExistingReorganizationCost()
        {
            battle.SetSelectedFormation(BattleFormation.Column);Assert.AreEqual("Line",Unit().Formation);Step(4.5f);
            Assert.AreEqual("Column",Unit().Formation);Assert.Greater(Unit().LastReceivedAt,0);
        }
        [Test] public void ReserveCommanderWithdrawsOnLossesWithoutRouting()
        {
            battle.SetSelectedIntent(RegimentIntent.PreserveReserve);Step(4.5f);Set("Men",100);Step(.1f);
            Assert.IsTrue(Unit().Moving);Assert.IsFalse(Unit().Routed);Assert.AreEqual("battle.command.withdraw_local",Unit().LocalInitiative);
        }
        [Test] public void ReceivedIntentSurvivesHeadquartersThreat()
        {
            battle.SetSelectedIntent(RegimentIntent.PreserveReserve);Step(4.5f);
            PlaceEnemy("Line",battle.HeadquartersPosition+Vector3.right*5);Step(.05f);
            Assert.IsTrue(battle.HeadquartersUnderThreat);
            Assert.AreEqual("PreserveReserve",Unit().Intent);
        }
        [Test] public void FlankCommanderFormsSquareAgainstNearbyCavalryWithoutAnotherOrder()
        {
            battle.SetSelectedIntent(RegimentIntent.GuardFlank);Step(4.5f);
            var r=Unit();PlaceEnemy("Cavalry",new Vector3(r.PositionX+6,0,r.PositionZ));Step(.05f);
            Assert.AreEqual("Square",Unit().Formation);Assert.AreEqual(0,Unit().PendingCommands);
            Assert.AreEqual("battle.command.square_local",Unit().LocalInitiative);
        }
    }
}
#endif
