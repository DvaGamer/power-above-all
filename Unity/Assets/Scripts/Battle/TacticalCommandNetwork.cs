using System;
using System.Collections.Generic;
using UnityEngine;

namespace PowerAboveAll
{
    public enum RegimentIntent { Hold, PreserveReserve, GuardFlank }
    public sealed partial class TacticalBattle
    {
        enum CommandKind { Move, Formation, Fire, Volley, Intent }
        sealed class RegimentCommand
        {
            public CommandKind Kind;
            public Vector3 Destination;
            public int Value;
            public float SentAt, ReceiveAt, DistanceDelay, DisorderDelay, TerrainDelay;
        }
        public bool CommandNetwork => setup != null && setup.CommandNetwork;
        public Vector3 HeadquartersPosition { get; private set; }
        public bool HeadquartersUnderThreat { get; private set; }
        private Vector3 headquartersDestination;
        private GameObject headquartersVisual;
        private bool selectingHeadquarters;

        void BuildHeadquarters()
        {
            HeadquartersPosition=headquartersDestination=new Vector3(0,0,-24);selectingHeadquarters=false;HeadquartersUnderThreat=false;
            if(!CommandNetwork)return;
            headquartersVisual=new GameObject("Dumas and command table");headquartersVisual.transform.SetParent(world.transform,false);
            Primitive("Map table",PrimitiveType.Cube,headquartersVisual.transform,new Vector3(0,.75f,0),new Vector3(1.6f,.16f,1.1f),wood);
            Primitive("Campaign sheet",PrimitiveType.Cube,headquartersVisual.transform,new Vector3(0,.85f,0),new Vector3(1.3f,.04f,.85f),cream);
            Primitive("Command standard",PrimitiveType.Cylinder,headquartersVisual.transform,new Vector3(1.2f,1.6f,0),new Vector3(.07f,1.6f,.07f),wood);
            Primitive("Headquarters pennant",PrimitiveType.Cube,headquartersVisual.transform,new Vector3(1.7f,2.75f,0),new Vector3(1,.55f,.06f),gold);
            for(int i=0;i<2;i++)
            {
                Primitive("Staff coat",PrimitiveType.Capsule,headquartersVisual.transform,new Vector3(-.8f+i*1.5f,.7f,-1),new Vector3(.4f,.6f,.4f),blue);
                Primitive("Staff head",PrimitiveType.Sphere,headquartersVisual.transform,new Vector3(-.8f+i*1.5f,1.45f,-1),Vector3.one*.3f,skin);
            }
            headquartersVisual.transform.position=HeadquartersPosition;
        }
        public BattleOrderResult MoveHeadquarters(Vector2 point)
        {
            var gate=OrderGate();if(!gate.Ok)return gate;
            if(!CommandNetwork||float.IsNaN(point.x)||float.IsNaN(point.y)||float.IsInfinity(point.x)||float.IsInfinity(point.y))return OrderResult(false,"battle.order_invalid");
            headquartersDestination=Bound(new Vector3(point.x,0,point.y));selectingHeadquarters=false;
            return OrderResult(true,affected:1);
        }
        public BattleOrderResult SetSelectedIntent(RegimentIntent intent)
        {
            var gate=OrderGate();if(!gate.Ok)return gate;
            if(!CommandNetwork||!Enum.IsDefined(typeof(RegimentIntent),intent))return OrderResult(false,"battle.order_invalid");
            return QueueCommands(CommandKind.Intent,Vector3.zero,(int)intent);
        }
        static bool SameCommand(RegimentCommand a,CommandKind kind,Vector3 destination,int value)
        {return a.Kind==kind&&a.Value==value&&(kind!=CommandKind.Move||(a.Destination-destination).sqrMagnitude<.0001f);}
        BattleOrderResult QueueCommands(CommandKind kind,Vector3 destination,int value=0)
        {
            var recipients=new List<Regiment>();Vector3 centre=Vector3.zero;
            foreach(var r in selected)
            {
                if(!Commandable(r))continue;
                if(kind==CommandKind.Formation&&value==(int)Formation.Square&&(r.Kind==Kind.Artillery||r.Kind==Kind.Cavalry))continue;
                recipients.Add(r);centre+=r.Position;
            }
            if(recipients.Count==0)return OrderResult(false,"battle.select_to_command");
            centre/=recipients.Count;
            foreach(var r in recipients)
            {
                Vector3 point=kind==CommandKind.Move?Bound(destination+(recipients.Count>1?r.Position-centre:Vector3.zero)):destination;
                if(r.Commands.Count>=2&&!SameCommand(r.Commands[r.Commands.Count-1],kind,point,value))return OrderResult(false,"battle.command.queue_full");
            }
            int added=0;
            foreach(var r in recipients)
            {
                Vector3 point=kind==CommandKind.Move?Bound(destination+(recipients.Count>1?r.Position-centre:Vector3.zero)):destination;
                if(r.Commands.Count>0&&SameCommand(r.Commands[r.Commands.Count-1],kind,point,value))continue;
                float distance=FlatDistance(HeadquartersPosition,r.Position)*.045f;
                float disorder=(100-r.Cohesion)*.008f+r.Fatigue*.004f+(100-setup.CommanderCompetence)*.004f;
                float terrain=(InOrchard(r.Position)?.5f:0)+(Mathf.Abs(TerrainHeight(r.Position.x,r.Position.z)-TerrainHeight(HeadquartersPosition.x,HeadquartersPosition.z))>1?.35f:0)+(HeadquartersUnderThreat?.75f:0);
                float due=elapsed+Mathf.Clamp(.65f+distance+disorder+terrain,.65f,4.5f);
                if(r.Commands.Count>0)due=Mathf.Max(due,r.Commands[r.Commands.Count-1].ReceiveAt+.45f);
                r.Commands.Add(new RegimentCommand{Kind=kind,Destination=point,Value=value,SentAt=elapsed,ReceiveAt=due,DistanceDelay=distance,DisorderDelay=disorder,TerrainDelay=terrain});added++;
            }
            if(added>0){messageKey="battle.command.sent";messageUntil=elapsed+4;Feedback?.Invoke("move");}
            return OrderResult(true,affected:added);
        }
        void AdvanceCommandNetwork(float dt)
        {
            if(!CommandNetwork)return;
            HeadquartersPosition=Vector3.MoveTowards(HeadquartersPosition,headquartersDestination,dt*2.1f);
            HeadquartersUnderThreat=false;
            foreach(var enemy in regiments)
                if(!enemy.Player&&!enemy.Routed&&!enemy.Withdrawn&&enemy.Men>0&&FlatDistance(enemy.Position,HeadquartersPosition)<9)HeadquartersUnderThreat=true;
            if(headquartersVisual)headquartersVisual.transform.position=HeadquartersPosition+Vector3.up*TerrainHeight(HeadquartersPosition.x,HeadquartersPosition.z);
            foreach(var r in regiments)
            {
                if(!Commandable(r)){r.Commands.Clear();continue;}
                if(r.Commands.Count>0&&r.Commands[0].ReceiveAt<=elapsed)
                {
                    var command=r.Commands[0];r.Commands.RemoveAt(0);r.LastReceivedOrder=command.Kind.ToString();r.LastReceivedAt=elapsed;r.LocalInitiative="";
                    switch(command.Kind)
                    {
                        case CommandKind.Move:r.Destination=command.Destination;r.Moving=true;r.Intent=RegimentIntent.Hold;break;
                        case CommandKind.Formation:
                            if(r.Formation!=(Formation)command.Value){r.Formation=(Formation)command.Value;r.Cohesion=Mathf.Max(20,r.Cohesion-12);r.Reload=Mathf.Max(r.Reload,2.5f);}break;
                        case CommandKind.Fire:r.FireAtWill=command.Value!=0;break;
                        case CommandKind.Volley:
                            if(CanVolley(r,FindEnemy(r)))r.AimedVolleyPending=true;
                            else r.LocalInitiative="battle.command.volley_expired";break;
                        case CommandKind.Intent:r.Intent=(RegimentIntent)command.Value;r.Moving=false;r.Destination=r.Position;break;
                    }
                }
                if(r.Intent==RegimentIntent.PreserveReserve&&(r.Morale<32||r.Men<r.Original*.7f)&&elapsed-r.LastInitiativeAt>8)
                {r.Destination=Bound(HeadquartersPosition+new Vector3((r.Id%2==0?-1:1)*4,0,2));r.Moving=true;r.LocalInitiative="battle.command.withdraw_local";r.LastInitiativeAt=elapsed;}
                if(r.Intent==RegimentIntent.GuardFlank&&(r.Kind==Kind.Line||r.Kind==Kind.Militia)&&!r.Moving&&r.Formation!=Formation.Square)
                {
                    foreach(var enemy in regiments)
                        if(!enemy.Player&&enemy.Kind==Kind.Cavalry&&!enemy.Routed&&!enemy.Withdrawn&&enemy.Men>0&&FlatDistance(r.Position,enemy.Position)<10)
                        {r.Formation=Formation.Square;r.Cohesion=Mathf.Max(20,r.Cohesion-12);r.Reload=Mathf.Max(2.5f,r.Reload);r.LocalInitiative="battle.command.square_local";break;}
                }
            }
        }
        void DrawCommandDesk()
        {
            if(!CommandNetwork)return;
            Panel(new Rect(673,42,248,91),Paint(0x243B37));
            if(Button(new Rect(683,49,228,31),selectingHeadquarters?"battle.command.place_hq":"battle.command.hq"))selectingHeadquarters=!selectingHeadquarters;
            Text(new Rect(683,85,228,42),HeadquartersUnderThreat?"battle.command.hq_threat":"battle.command.hq_hint",smallStyle);
            var primary=FirstCommandable();if(primary==null)return;
            GUI.enabled=Commandable(primary);
            if(OrderButton(new Rect(892,699,166,29),"battle.command.hold",primary.Intent==RegimentIntent.Hold))ShowOrderResult(SetSelectedIntent(RegimentIntent.Hold));
            if(OrderButton(new Rect(1068,699,166,29),"battle.command.reserve",primary.Intent==RegimentIntent.PreserveReserve))ShowOrderResult(SetSelectedIntent(RegimentIntent.PreserveReserve));
            if(OrderButton(new Rect(1244,699,175,29),"battle.command.flank",primary.Intent==RegimentIntent.GuardFlank))ShowOrderResult(SetSelectedIntent(RegimentIntent.GuardFlank));
            GUI.enabled=true;
            if(primary.Commands.Count>0)
            {
                var command=primary.Commands[0];
                Panel(new Rect(892,640,527,51),Paint(0x243B37));
                Text(new Rect(902,645,507,42),"battle.command.status",smallStyle,Mathf.Max(0,command.ReceiveAt-elapsed).ToString("0.0"),command.DistanceDelay.ToString("0.0"),command.DisorderDelay.ToString("0.0"),command.TerrainDelay.ToString("0.0"));
            }
            else if(!string.IsNullOrEmpty(primary.LocalInitiative))
            {Panel(new Rect(892,648,527,43),Paint(0x243B37));Text(new Rect(902,653,507,34),primary.LocalInitiative,smallStyle);}
            else if(primary.LastReceivedAt>=0)
            {Panel(new Rect(892,648,527,43),Paint(0x243B37));Text(new Rect(902,653,507,34),"battle.command.received",smallStyle,L.Text("battle.command.kind."+primary.LastReceivedOrder.ToLowerInvariant()));}
        }
    }
}
