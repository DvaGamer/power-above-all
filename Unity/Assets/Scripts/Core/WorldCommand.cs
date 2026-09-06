using System;

namespace PowerAboveAll
{
    public static class WorldCommand
    {
        public static void Deploy(WorldState w,WorldArmy army,WorldArmy enemy)
        {
            army.Deployed=true;army.Forward=WorldTerrain.Normal(enemy.Position-army.Position);
            army.FrontAnchor=army.Position;army.RetreatPoint=army.Position-army.Forward*1600;
            var hq=w.Headquarters.Find(h=>h.Id==army.HeadquartersId);
            hq.Destination=army.Position-army.Forward*250;hq.OrderReceivedAt=w.Clock.Milliseconds+1500;hq.Moving=true;
            // Şehirde duran savunucu savunur; yürüyüşteki ordu kendi planıyla ilerler.
            foreach(var unit in army.Units)
            {
                unit.Routed=false;unit.Withdrawal=WorldWithdrawal.None;unit.ManualOrder=false;
                unit.Fatigue=army.Fatigue;unit.Formation=WorldFormation.Line;
                unit.Intent=unit.Role==WorldRole.Reserve?WorldIntent.Reserve:WorldIntent.Advance;
                unit.AssignedPosition=Slot(army,unit);unit.Destination=unit.AssignedPosition;
                unit.ReorganizeUntil=w.Clock.Seconds+2+(100-unit.Cohesion)*.06;
            }
        }
        public static WorldPoint Slot(WorldArmy a,WorldUnit u)
        {
            double side=u.Role==WorldRole.Left?-150:u.Role==WorldRole.Right?150:u.Role==WorldRole.Screen?360:u.Role==WorldRole.Battery?-290:0;
            double depth=u.Role==WorldRole.Reserve?-280:u.Role==WorldRole.Battery?-100:u.Role==WorldRole.Screen?-100:0;
            return a.FrontAnchor+new WorldPoint(a.Forward.Z,-a.Forward.X)*side+a.Forward*depth;
        }
        public static void Update(WorldState w,WorldArmy a,WorldArmy enemy,double dt)
        {
            var hq=w.Headquarters.Find(h=>h.Id==a.HeadquartersId);
            if(hq.Moving&&w.Clock.Milliseconds>=hq.OrderReceivedAt)
            {var delta=hq.Destination-hq.Position;double d=WorldPoint.Distance(hq.Position,hq.Destination);hq.Position+=WorldTerrain.Normal(delta)*Math.Min(d,2.4*dt);if(d<1)hq.Moving=false;}
            a.RearBlocked=false;
            foreach(var e in enemy.Units)
            {
                if(!WorldCombat.Fighting(e)||e.Withdrawal!=WorldWithdrawal.None)continue;
                if(WorldTerrain.SegmentDistance(e.Position,a.FrontAnchor,a.RetreatPoint)<180&&WorldTerrain.Dot(e.Position-a.FrontAnchor,a.Forward)<-120)a.RearBlocked=true;
                if(WorldPoint.Distance(e.Position,hq.Position)<90)hq.Integrity=Math.Max(0,hq.Integrity-(float)(dt*8));
                if(WorldPoint.Distance(e.Position,a.WagonPosition)<90)a.WagonIntegrity=Math.Max(0,a.WagonIntegrity-(float)(dt*8));
            }
            if(a.WagonIntegrity<=0)a.AmmunitionWagon=0;
            var rear=a.FrontAnchor-a.Forward*330;double wagonDistance=WorldPoint.Distance(a.WagonPosition,rear);
            if(a.WagonIntegrity>0&&wagonDistance>5)a.WagonPosition+=WorldTerrain.Normal(rear-a.WagonPosition)*Math.Min(wagonDistance,.8*dt);
            if(a.Posture==WorldPosture.Advance)
            {
                double gap=WorldTerrain.Dot(enemy.FrontAnchor-a.FrontAnchor,a.Forward);
                bool ready=true;
                foreach(var u in a.Units)if(WorldCombat.Fighting(u)&&u.Withdrawal==WorldWithdrawal.None&&!u.ManualOrder&&(u.Role==WorldRole.Centre||u.Role==WorldRole.Left||u.Role==WorldRole.Right)&&WorldPoint.Distance(u.Position,Slot(a,u))>90)ready=false;
                if(ready&&gap>255)a.FrontAnchor+=a.Forward*(1.05*dt);
            }
            // Yerel AI yedeği tek bir açığa gönderir; düşmanın gizli verisini hedef seçmekte kullanmaz.
            if(a.Id!=w.PlayerArmyId&&w.Clock.Seconds>=a.ReserveDecisionAt)
            {
                WorldUnit gap=null;
                foreach(var u in a.Units)if(u.Role==WorldRole.Centre||u.Role==WorldRole.Left||u.Role==WorldRole.Right)
                    if(u.Routed||u.Morale<38||u.Cohesion<30){gap=u;break;}
                if(gap!=null)foreach(var reserve in a.Units)if(reserve.Role==WorldRole.Reserve&&reserve.Intent==WorldIntent.Reserve&&WorldCombat.Fighting(reserve)&&reserve.Orders.Count==0)
                {
                    Queue(w,a,reserve,Slot(a,gap),WorldIntent.Hold,WorldFormation.Line);
                    a.ReserveDecisionAt=w.Clock.Seconds+45;break;
                }
            }
        }
        public static long Delay(WorldState w,WorldArmy a,WorldUnit u)
        {
            var hq=w.Headquarters.Find(h=>h.Id==a.HeadquartersId);var c=w.Commanders.Find(p=>p.Id==a.CommanderId);
            double terrain=WorldTerrain.ClearSight(w,hq.Position,u.Position)?0:1000;
            return (long)(1000+Math.Min(3500,WorldPoint.Distance(hq.Position,u.Position)*2+(100-c.Competence)*4+(100-u.Cohesion)*8+terrain+(hq.Integrity<=0?2000:0)));
        }
        public static void Queue(WorldState w,WorldArmy a,WorldUnit u,WorldPoint p,WorldIntent intent,WorldFormation formation)
        {
            long received=w.Clock.Milliseconds+Delay(w,a,u);
            if(u.Orders.Count>0)received=Math.Max(received,u.Orders[u.Orders.Count-1].ReceivedAt+450);
            u.Orders.Add(new WorldUnitOrder{Destination=p,Intent=intent,Formation=formation,IssuedAt=w.Clock.Milliseconds,ReceivedAt=received});
        }
        public static void Observe(WorldState w)
        {
            var observer=w.Army(w.PlayerArmyId);
            foreach(var a in w.Armies)
            {
                if(a.FactionId==observer.FactionId)continue;
                var known=w.Sightings.Find(s=>s.ArmyId==a.Id);if(known!=null)known.Visible=false;
                int seen=0;WorldPoint centre=new WorldPoint();
                foreach(var u in a.Units)if(u.Men>0&&WorldTerrain.Visible(w,observer,u)){seen+=u.Men;centre+=u.Position*u.Men;}
                if(seen==0)continue;
                if(known==null){known=new WorldSighting{ArmyId=a.Id};w.Sightings.Add(known);}
                known.Visible=true;known.ObservedAt=w.Clock.Milliseconds;known.Position=centre*(1d/seen);
                known.Minimum=Math.Max(100,seen*7/1000*100);known.Maximum=(seen*13/1000+1)*100;
            }
        }
    }
}
