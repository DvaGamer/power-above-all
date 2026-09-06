using System;
using System.Collections.Generic;

namespace PowerAboveAll
{
    public static class WorldCombat
    {
        public const long TickMilliseconds=100;
        public const double ContactMetres=800;
        private struct Attack { public int Target, Loss; public float Shock, Disorder; }
        public static double Range(WorldUnit unit) => unit.Kind==WorldUnitKind.Artillery?680:unit.Kind==WorldUnitKind.Cavalry?60:unit.Kind==WorldUnitKind.Militia?300:380;
        public static bool Fighting(WorldUnit unit) => unit.Men>0&&!unit.Routed;
        public static double Heading(WorldPoint delta) => Math.Atan2(delta.X,delta.Z)*180/Math.PI;
        private static double Angle(double a,double b) {double d=(a-b)%360;if(d>180)d-=360;if(d< -180)d+=360;return d;}
        private static double Turn(double from,double to,double amount)=>from+Math.Max(-amount,Math.Min(amount,Angle(to,from)));
        private static double Random(WorldBattle battle)
        {uint x=battle.RandomState;x^=x<<13;x^=x>>17;x^=x<<5;battle.RandomState=x;return x/(double)uint.MaxValue;}

        public static void Step(WorldState world,WorldBattle battle)
        {
            if(battle.Ended)return;
            const double dt=TickMilliseconds/1000d;
            var first=world.Army(battle.FirstArmyId);var second=world.Army(battle.SecondArmyId);
            WorldCommand.Update(world,first,second,dt);WorldCommand.Update(world,second,first,dt);
            var units=new List<WorldUnit>(first.Units);units.AddRange(second.Units);int divide=first.Units.Count;
            var positions=new WorldPoint[units.Count];for(int i=0;i<units.Count;i++)positions[i]=units[i].Position;
            var targets=new int[units.Count];
            for(int i=0;i<units.Count;i++)
            {
                targets[i]=-1;double nearest=double.PositiveInfinity;
                for(int j=0;j<units.Count;j++)
                {
                    if((i<divide)==(j<divide)||!Fighting(units[j])||!WorldTerrain.Visible(world,i<divide?first:second,units[j]))continue;
                    double distance=WorldPoint.Distance(positions[i],positions[j]);
                    // Hedef bir yürüyüş rotası değildir. Önümüzdeki tehdit ve açık ateş hattı önceliklidir.
                    double angle=Math.Abs(Angle(units[i].Facing,Heading(positions[j]-positions[i])));
                    double score=distance+angle*4;
                    if(!WorldTerrain.ClearSight(world,positions[i],positions[j]))score+=2000;
                    if(!WorldTerrain.FriendlyFireLane(i<divide?first:second,units[i],positions[j]))score+=1200;
                    if(units[i].Kind==WorldUnitKind.Artillery&&units[j].Kind==WorldUnitKind.Artillery)score-=120;
                    if(score<nearest){nearest=score;targets[i]=j;}
                }
            }
            for(int i=0;i<units.Count;i++)
            {
                var u=units[i];var army=i<divide?first:second;var enemy=targets[i]>=0?units[targets[i]]:null;
                u.Reload=Math.Max(0,u.Reload-dt);u.Quiet+=dt;
                if(u.Men==0)continue;
                while(u.Orders.Count>0&&u.Orders[0].ReceivedAt<=world.Clock.Milliseconds)
                {
                    var order=u.Orders[0];u.Orders.RemoveAt(0);
                    if(u.Routed)continue;
                    u.Intent=order.Intent;u.Destination=order.Destination;u.Moving=WorldPoint.Distance(u.Position,u.Destination)>1;
                    u.ManualOrder=true;
                    u.Replenishing=false;
                    if(u.Formation!=order.Formation){u.Formation=order.Formation;u.Cohesion=Math.Max(20,u.Cohesion-12);u.ReorganizeUntil=world.Clock.Seconds+2.5+(100-u.Cohesion)*.035;u.Reload=Math.Max(2.5,u.Reload);}
                }
                u.Pressure=WorldPressure.None;
                var hq=world.Headquarters.Find(h=>h.Id==army.HeadquartersId);
                if(army.RearBlocked)u.Pressure|=WorldPressure.RearBlocked;
                if(hq.Integrity<=0)u.Pressure|=WorldPressure.HeadquartersLost;
                if(u.Ammo==0)u.Pressure|=WorldPressure.Ammunition;
                if(u.Fatigue>65)u.Pressure|=WorldPressure.Exhausted;
                if(u.Cohesion<40)u.Pressure|=WorldPressure.Disordered;
                bool supported=WorldPoint.Distance(hq.Position,u.Position)<320&&hq.Integrity>0;
                foreach(var friend in army.Units)if(friend!=u&&Fighting(friend)&&WorldPoint.Distance(friend.Position,u.Position)<220)supported=true;
                if(!supported)u.Pressure|=WorldPressure.Isolated;
                if(u.Intent==WorldIntent.Withdraw&&u.Withdrawal==WorldWithdrawal.None)u.Withdrawal=WorldWithdrawal.Ordered;
                if(u.Withdrawal!=WorldWithdrawal.None||u.Routed)
                {
                    u.WithdrawalSeconds+=dt;
                    if(army.RearBlocked&&u.Withdrawal==WorldWithdrawal.Ordered)u.Withdrawal=WorldWithdrawal.Disordered;
                    if(u.Withdrawal==WorldWithdrawal.Disordered){u.Cohesion=Math.Max(0,u.Cohesion-(float)dt);u.Morale=Math.Max(0,u.Morale-(float)dt);}
                    if(u.Routed)u.Withdrawal=WorldWithdrawal.Rout;
                    u.Destination=army.RetreatPoint;u.Moving=true;
                    if(army.RearBlocked&&u.Routed&&u.WithdrawalSeconds>30&&enemy!=null&&WorldPoint.Distance(enemy.Position,u.Position)<90)
                    {u.Captured+=u.Men;u.Men=0;u.Withdrawal=WorldWithdrawal.Surrendered;u.Moving=false;}
                }
                else if(!u.ManualOrder)
                {
                    u.AssignedPosition=WorldCommand.Slot(army,u);u.Destination=u.AssignedPosition;
                    u.Moving=WorldPoint.Distance(u.Position,u.Destination)>5;
                }
                else
                {
                    u.Moving=WorldPoint.Distance(u.Position,u.Destination)>5;
                    if(u.Intent==WorldIntent.Advance&&enemy!=null&&WorldPoint.Distance(u.Position,enemy.Position)<Range(u)*.8)u.Moving=false;
                }
                // Yerel komutan boş tüfekle ateş hattında beklemez. Asıl niyet saklanır.
                bool firearm=u.Kind!=WorldUnitKind.Cavalry;
                if(firearm&&u.Withdrawal==WorldWithdrawal.None&&!u.Routed)
                {
                    if(u.Ammo==0&&!u.Replenishing&&army.AmmunitionWagon>0&&army.WagonIntegrity>0&&!army.RearBlocked)
                    {u.Replenishing=true;u.ResumeDestination=u.Destination;}
                    if(u.Replenishing)
                    {
                        if(u.Ammo>=8||(army.AmmunitionWagon==0&&u.Ammo>0))
                        {u.Replenishing=false;u.Destination=u.ResumeDestination;u.Moving=WorldPoint.Distance(u.Position,u.Destination)>5;}
                        else
                        {u.Destination=army.WagonPosition;u.Moving=WorldPoint.Distance(u.Position,u.Destination)>110;}
                    }
                    if(u.Ammo==0&&(army.AmmunitionWagon==0||army.WagonIntegrity<=0||army.RearBlocked)&&enemy!=null&&WorldPoint.Distance(u.Position,enemy.Position)>80)
                    {u.Replenishing=false;u.Withdrawal=WorldWithdrawal.Ordered;u.Destination=army.RetreatPoint;u.Moving=true;}
                }
                if(world.Clock.Seconds<u.ReorganizeUntil)u.Moving=false;
                if(u.Moving)
                {
                    double speed=u.Routed?3.8:u.Kind==WorldUnitKind.Cavalry?3.6:u.Kind==WorldUnitKind.Artillery?.85:1.65;
                    speed*=u.Formation==WorldFormation.Column?1.3:u.Formation==WorldFormation.Square?.43:.84;
                    speed*=(1-u.Fatigue*.0048)*WorldTerrain.MoveFactor(world,u);
                    WorldPoint delta=u.Destination-u.Position;double distance=WorldPoint.Distance(u.Position,u.Destination);
                    if(distance<=.5)u.Moving=false;
                    else{u.Position+=delta*(Math.Min(distance,speed*dt)/distance);u.Facing=Turn(u.Facing,Heading(delta),65*dt);}
                }
                else if(enemy!=null)u.Facing=Turn(u.Facing,Heading(enemy.Position-u.Position),38*dt);
                bool rough=WorldTerrain.In(world,u.Position,WorldTerrainKind.Woodland)||WorldTerrain.In(world,u.Position,WorldTerrainKind.River);
                u.Cohesion=Clamp(u.Cohesion+(u.Moving?(rough?-.35f:-.008f):u.Quiet>5?1.5f:.15f)*(float)dt,0,100);
                u.Fatigue=Clamp(u.Fatigue+(u.Moving?.035f:u.Quiet>10?-.12f:.015f)*(float)dt,0,100);
                if(u.Quiet>10&&!u.Moving&&u.Morale>20&&u.Morale<78)u.Morale=Math.Min(78,u.Morale+(float)(dt*.3));
                if((u.Pressure&WorldPressure.HeadquartersLost)!=0)u.Morale=Math.Max(0,u.Morale-(float)(dt*.09));
                if(u.Ammo<16&&army.AmmunitionWagon>0&&army.WagonIntegrity>0&&WorldPoint.Distance(u.Position,army.WagonPosition)<140&&!u.Moving&&u.Quiet>10&&u.Reload<=0)
                {u.Ammo++;army.AmmunitionWagon--;u.Reload=2;}
            }
            var attacks=new Attack[units.Count];
            for(int i=0;i<units.Count;i++)
            {
                attacks[i].Target=-1;var u=units[i];int index=targets[i];
                if(!Fighting(u)||u.Withdrawal!=WorldWithdrawal.None||index<0||u.Reload>0||world.Clock.Seconds<u.ReorganizeUntil)continue;
                var enemy=units[index];double distance=WorldPoint.Distance(u.Position,enemy.Position);
                bool contact=distance<=60&&u.Kind!=WorldUnitKind.Artillery;
                if(!contact&&(u.Moving||u.Replenishing||u.Ammo==0||u.Kind==WorldUnitKind.Cavalry||distance>Range(u)))continue;
                if(!contact&&(!WorldTerrain.ClearSight(world,u.Position,enemy.Position)||!WorldTerrain.FriendlyFireLane(i<divide?first:second,u,enemy.Position)))
                {u.Pressure|=WorldPressure.Obstructed;continue;}
                if(Math.Abs(Angle(u.Facing,Heading(enemy.Position-u.Position)))>(u.Formation==WorldFormation.Square?180:contact?75:45))continue;
                // Eski taktik formülün moral/düzen, yorgunluk, tecrübe ve yan ateş ilkeleri korunur.
                double coefficient=contact?(u.Kind==WorldUnitKind.Cavalry?.34:u.Kind==WorldUnitKind.Militia?.14:.18):u.Kind==WorldUnitKind.Artillery?.57:.43;
                double power=Math.Sqrt(u.Men)*coefficient*(.76+Random(battle)*.48);
                power*=(.62+.38*u.Cohesion/100)*(1-.38*u.Fatigue/100)*(1+u.Experience/300);
                power*=.55+.45*u.Morale/100;
                power*=1-WorldTerrain.Cover(world,enemy.Position);
                double height=WorldTerrain.Height(world,u.Position)-WorldTerrain.Height(world,enemy.Position);
                power*=Math.Max(.6,Math.Min(1.3,1+height*.012));
                if(u.Kind==WorldUnitKind.Cavalry&&WorldTerrain.In(world,u.Position,WorldTerrainKind.Woodland))power*=.35;
                if(WorldTerrain.In(world,u.Position,WorldTerrainKind.River))power*=.45;
                if(!contact||u.Kind==WorldUnitKind.Cavalry)power*=u.Formation==WorldFormation.Column?.48:u.Formation==WorldFormation.Square?.57:1;
                if(u.Kind==WorldUnitKind.Cavalry&&enemy.Formation==WorldFormation.Square)power*=.23;
                if(u.Kind==WorldUnitKind.Cavalry&&enemy.Reload>4)power*=1.35;
                if(u.Kind==WorldUnitKind.Artillery&&enemy.Formation==WorldFormation.Square)power*=1.65;
                double flank=Math.Abs(Angle(enemy.Facing,Heading(u.Position-enemy.Position)))>100?1.7:1;
                if(flank>1)enemy.Pressure|=WorldPressure.Flanked;
                int loss=Math.Max(1,Math.Min(enemy.Men,(int)Math.Round(power*(flank>1?1.2:1),MidpointRounding.AwayFromZero)));
                float shock=u.Kind==WorldUnitKind.Artillery?5:u.Kind==WorldUnitKind.Cavalry?6:2.6f;
                var commander=world.Commanders.Find(c=>c.Id==(index<divide?first:second).CommanderId);
                double isolation=(enemy.Pressure&WorldPressure.Isolated)!=0?1.25:1;
                attacks[i]=new Attack{Target=index,Loss=loss,Shock=(float)((loss*130d/Math.Max(1,enemy.Original)+shock)*flank*isolation*(1.13-commander.Competence/400)),Disorder=(float)(shock*flank)};
                u.Reload=(contact?3.4:u.Kind==WorldUnitKind.Artillery?13:u.Kind==WorldUnitKind.Militia?10.5:8)*(1+u.Fatigue/180);
                u.Fatigue=Math.Min(100,u.Fatigue+2);
                if(!contact){u.Ammo--;u.LastFiredAt=world.Clock.Seconds;}
            }
            for(int i=0;i<attacks.Length;i++)
            {
                var hit=attacks[i];if(hit.Target<0)continue;var target=units[hit.Target];
                target.Men=Math.Max(0,target.Men-hit.Loss);target.Morale=Math.Max(0,target.Morale-hit.Shock);target.Cohesion=Math.Max(0,target.Cohesion-hit.Disorder);target.Quiet=0;
            }
            // Yeni bozgunlar aynı dalgada işaretlenir; liste sırası komşu paniğini değiştirmez.
            for(int wave=0;wave<units.Count;wave++)
            {
                var broken=new List<int>();
                for(int i=0;i<units.Count;i++)
                {
                    var u=units[i];if(u.Routed)continue;
                    if(u.Men<=u.Original*.25)u.Morale=Math.Min(u.Morale,19);
                    if(u.Morale<20){u.Routed=true;u.Withdrawal=WorldWithdrawal.Rout;u.Orders.Clear();broken.Add(i);}
                    else if((u.Morale<32||u.Cohesion<22)&&u.Withdrawal==WorldWithdrawal.None){u.Withdrawal=WorldWithdrawal.Ordered;u.Orders.Clear();}
                }
                if(broken.Count==0)break;
                foreach(int i in broken)for(int j=0;j<units.Count;j++)
                    if((i<divide)==(j<divide)&&Fighting(units[j])&&WorldPoint.Distance(units[i].Position,units[j].Position)<300)
                        units[j].Morale=Math.Max(0,units[j].Morale-5);
            }
            int a=Effective(first,battle),b=Effective(second,battle);
            // Dağılmış cepheye oyuncunun yedeği sokabileceği kısa karar aralığı tanınır.
            first.DisruptionSeconds=a<battle.FirstOriginal*.33?first.DisruptionSeconds+dt:0;
            second.DisruptionSeconds=b<battle.SecondOriginal*.33?second.DisruptionSeconds+dt:0;
            if(first.DisruptionSeconds>25)a=0;if(second.DisruptionSeconds>25)b=0;
            UpdateArmyPosition(first);UpdateArmyPosition(second);
            if(a==0||b==0)End(world,battle,a==0?(b==0?"":second.Id):first.Id);
        }
        private static void UpdateArmyPosition(WorldArmy army)
        {
            WorldPoint centre=new WorldPoint();int men=0,total=0;double fatigue=0;
            foreach(var u in army.Units)
            {
                fatigue+=u.Fatigue*u.Men;total+=u.Men;
                if(Fighting(u)&&u.Withdrawal==WorldWithdrawal.None){centre+=u.Position*u.Men;men+=u.Men;}
            }
            if(men>0)army.Position=centre*(1d/men);
            if(total>0)army.Fatigue=(float)(fatigue/total);
        }
        private static int Effective(WorldArmy army,WorldBattle battle)
        {int total=0;foreach(var unit in army.Units)if(Fighting(unit)&&unit.Withdrawal==WorldWithdrawal.None&&unit.Intent!=WorldIntent.Withdraw&&WorldPoint.Distance(unit.Position,battle.Contact)<5000)total+=unit.Men;return total;}
        public static void End(WorldState world,WorldBattle battle,string winner)
        {
            if(battle.Ended)return;
            battle.Ended=true;battle.EndedAt=world.Clock.Milliseconds;battle.WinnerId=winner;
            foreach(string id in new[]{battle.FirstArmyId,battle.SecondArmyId})
            {
                var army=world.Army(id);WorldPoint centre=new WorldPoint();int alive=0;
                foreach(var unit in army.Units){centre+=unit.Position*unit.Men;alive+=unit.Men;unit.Orders.Clear();unit.Moving=false;}
                if(alive>0)army.Position=centre*(1d/alive);
                army.Activity=id==winner?ArmyActivity.Holding:ArmyActivity.Retreating;
                if(id==winner)army.RegionId=battle.RegionId;
                army.RecoverUntil=world.Clock.Milliseconds+WorldClock.Day;
                var away=army.Position-battle.Contact;double length=WorldPoint.Distance(army.Position,battle.Contact);
                if(length<1){away=new WorldPoint(id==battle.FirstArmyId?-1:1,0);length=1;}
                army.Route=new WorldRoute{DestinationRegionId=army.RegionId,Points=new List<WorldPoint>{army.Position,army.Position+away*(3000/length)}};
            }
            world.LastNoticeKey=winner==world.PlayerArmyId?"world.victory":"world.defeat";world.LastNoticeRegion=battle.RegionId;
        }
        private static float Clamp(float v,float low,float high)=>Math.Max(low,Math.Min(high,v));
    }
}
