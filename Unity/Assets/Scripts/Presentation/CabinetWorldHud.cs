using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class CabinetHud
    {
        private void DrawWorldEntities(GameApp app)
        {
            var world=app.State.World;if(world==null)return;
            if(ShowWorldSupply)foreach(var convoy in world.Convoys)
                if(convoy.FactionId==world.Army(world.PlayerArmyId).FactionId&&WorldSupply.Active(convoy))
                    AtlasLabel(app,WorldMapEntities.Position(convoy.Position),T("supply.status."+convoy.Status.ToString().ToLowerInvariant()),mapLabel,230,false,15);
            foreach(var army in world.Armies)
            {
                if(army.Men==0)continue;
                bool ours=army.FactionId==world.Army(world.PlayerArmyId).FactionId;
                var sight=world.Sightings.Find(s=>s.ArmyId==army.Id);
                if(!ours&&sight==null)continue;
                if(app.StrategyCamera.Distance>=2)
                    AtlasLabel(app,WorldMapEntities.Position(ours?army.Position:sight.Position),T(army.NameKey)+" · "+(ours?Number(army.Men):T("command.estimate",sight.Minimum,sight.Maximum)),mapLabel,270,world.SelectedArmyId==army.Id,12);
                else foreach(var unit in army.Units)
                {
                    if(unit.Men==0||(!ours&&!WorldTerrain.Visible(world,world.Army(world.PlayerArmyId),unit)))continue;
                    AtlasLabel(app,WorldMapEntities.Position(unit.Position),T("world.kind."+unit.Kind.ToString().ToLowerInvariant())+(ours?" · "+unit.Men:""),cityLabel,150,world.SelectedUnitId==unit.Id);
                }
            }
        }
        private void WorldArmyDesk(GameApp app)
        {
            var world=app.State.World;var army=world.Army(world.SelectedArmyId);
            if(army.Id!=world.PlayerArmyId)
            {
                var sight=world.Sightings.Find(s=>s.ArmyId==army.Id);if(sight==null)return;
                Fill(new Rect(385,711,670,80),paper);
                Text(new Rect(400,720,480,25),T(army.NameKey)+" · "+T("command.estimate",sight.Minimum,sight.Maximum),heading);
                Text(new Rect(400,752,620,25),T(sight.Visible?"command.observed":"command.old_report",new System.DateTime(1789,5,5).AddMilliseconds(sight.ObservedAt).ToString("dd MMM HH:mm")),body);
                return;
            }
            var selected=army.Units.Find(u=>u.Id==world.SelectedUnitId);
            bool commanding=selected!=null&&army.Activity==ArmyActivity.Fighting&&army.Id==world.PlayerArmyId;
            if(!commanding)
            {
            Fill(new Rect(385,680,670,148),paper);Fill(new Rect(385,680,3,148),brass);
            Text(new Rect(400,689,400,25),T(army.NameKey)+" · "+Number(army.Men),heading);
            Text(new Rect(400,717,410,22),T("world.activity."+army.Activity.ToString().ToLowerInvariant()),body);
            string detail=army.Activity==ArmyActivity.Marching?T("world.remaining",(WorldRouting.Remaining(army)/1000).ToString("0.0")):T("world.command_hint");
            Text(new Rect(400,744,470,38),detail,small);
            Text(new Rect(400,789,635,30),T("supply.army_stock",WorldSupply.DaysLeft(army).ToString("0.0"),army.AmmunitionWagon)+" · "+T("command.condition",Mathf.RoundToInt(army.Fatigue),army.Units.Count>0?army.Units[0].Ammo:0),small);
            if(Press(new Rect(903,692,139,31),T("world.focus")))app.FocusWorldArmy();
            if(army.Activity!=ArmyActivity.Fighting&&Press(new Rect(903,743,139,31),T("supply.title")))ShowWorldSupply=!ShowWorldSupply;
            if(army.Activity==ArmyActivity.Fighting&&army.Id==world.PlayerArmyId)
                if(Press(new Rect(903,741,139,31),T("world.retreat")))app.Simulation.Retreat(army.Id);
            }
            if(commanding)
            {
                Fill(new Rect(385,681,670,148),paper);Fill(new Rect(385,681,3,148),brass);
                Text(new Rect(399,690,638,22),T("command.unit",T("command.role."+selected.Role.ToString().ToLowerInvariant()),selected.Men,Mathf.RoundToInt(selected.Morale),Mathf.RoundToInt(selected.Cohesion)),body);
                Text(new Rect(399,713,638,22),T("command.condition",Mathf.RoundToInt(selected.Fatigue),selected.Ammo)+" · "+CommandReason(selected),small);
                Text(new Rect(399,736,638,22),selected.Orders.Count>0?T("world.command_eta",Mathf.Max(0,(selected.Orders[0].ReceivedAt-world.Clock.Milliseconds)/1000f).ToString("0.0")):world.Clock.Seconds<selected.ReorganizeUntil?T("command.reorganizing"):T("command.intent."+selected.Intent.ToString().ToLowerInvariant()),small);
                for(int i=0;i<4;i++)if(Press(new Rect(400+i*160,760,152,27),T("command.intent."+((WorldIntent)i).ToString().ToLowerInvariant()),true,selected.Intent==(WorldIntent)i))
                {
                    var goal=i==0?army.FrontAnchor+army.Forward*180:i==2?army.FrontAnchor-army.Forward*280:selected.Position;
                    app.ReportWorldOrder(selected.Id,goal,(WorldIntent)i,selected.Formation);
                }
                for(int i=0;i<3;i++)if(Press(new Rect(400+i*160,794,152,27),T("battle.formation."+((WorldFormation)i).ToString().ToLowerInvariant()),true,selected.Formation==(WorldFormation)i))
                    app.ReportWorldOrder(selected.Id,selected.Destination,selected.Intent,(WorldFormation)i);
                if(Press(new Rect(880,794,152,27),T("command.hq"),true,app.WorldHeadquartersSelected))app.WorldHeadquartersSelected=!app.WorldHeadquartersSelected;
            }
            if(army.Activity==ArmyActivity.Fighting&&app.StrategyCamera.Distance<2)
            {
                for(int i=0;i<army.Units.Count;i++)
                {var unit=army.Units[i];if(Press(new Rect(18,185+i*43,240,36),T("command.role."+unit.Role.ToString().ToLowerInvariant())+" · "+unit.Men,true,unit.Id==world.SelectedUnitId)){world.SelectedUnitId=unit.Id;app.WorldHeadquartersSelected=false;}}
                var hq=world.Headquarters.Find(h=>h.Id==army.HeadquartersId);
                Text(new Rect(20,456,242,42),T("command.rear",Mathf.RoundToInt(hq.Integrity),army.AmmunitionWagon),small);
                if(Press(new Rect(18,637,240,29),T("supply.title")))ShowWorldSupply=!ShowWorldSupply;
                if(army.RearBlocked)Text(new Rect(20,504,240,42),T("command.reason.rearblocked"),body);
                if(Press(new Rect(18,552,240,32),T("world.retreat")))app.Simulation.Retreat(army.Id);
                if(app.WorldHeadquartersSelected)Text(new Rect(20,596,240,54),T("command.hq_hint"),body);
            }
            if(app.State.PendingPetition||CampaignCore.MandateDue(app.State))
                if(Press(new Rect(16,90,230,36),T("world.pending_document")))app.ShowPendingDocument=!app.ShowPendingDocument;
            if(Press(new Rect(1080,747,338,33),T(world.BattlePolicy==BattleTimePolicy.Pause?"world.contact_pause":"world.contact_slow")))
                world.BattlePolicy=world.BattlePolicy==BattleTimePolicy.Pause?BattleTimePolicy.SlowToNormal:BattleTimePolicy.Pause;
            SupplyDesk(app);
        }
        private string CommandReason(WorldUnit u)
        {
            if(u.Withdrawal!=WorldWithdrawal.None)return T("command.withdrawal."+u.Withdrawal.ToString().ToLowerInvariant());
            if(u.Replenishing)return T("supply.unit_replenishing");
            foreach(var reason in new[]{WorldPressure.HeadquartersLost,WorldPressure.RearBlocked,WorldPressure.Flanked,WorldPressure.Disordered,WorldPressure.Ammunition,WorldPressure.Exhausted,WorldPressure.Isolated,WorldPressure.Obstructed})
                if((u.Pressure&reason)!=0)return T("command.reason."+reason.ToString().ToLowerInvariant());
            return T("command.reason.ready");
        }
    }
}
