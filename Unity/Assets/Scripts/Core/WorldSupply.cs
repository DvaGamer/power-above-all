using System;
using System.Collections.Generic;

namespace PowerAboveAll
{
    public enum ConvoyStatus { Travelling, Blocked, Waiting, Delivered, Captured }
    [Serializable] public sealed class WorldDepot
    {
        public string Id, SiteId, FactionId;
        public int Food, Ammunition;
    }
    [Serializable] public sealed class WorldConvoy
    {
        public string Id, DepotId, ArmyId, FactionId, CauseId="";
        public WorldPoint Position, Rendezvous;
        public WorldRoute Route=new WorldRoute();
        public int Food, Ammunition;
        public long DispatchedAt, CompletedAt;
        public ConvoyStatus Status;
        public const double MetresPerSecond=.9;
    }

    // İlk yerel ağ: ulusal stok -> başkent deposu -> sonlu yük -> sahra ordusu.
    // Sivil üretim henüz ülke defterindedir; uzaktaki ordu onu doğrudan tüketemez.
    public static class WorldSupply
    {
        public const int FoodLoad=40, AmmunitionLoad=96;
        public static void Initialize(CampaignState campaign)
        {
            var w=campaign.World;
            foreach(var army in w.Armies)
            {
                army.Rations=CampaignCore.ArmyFoodFor(army.Men);
                var site=w.Sites.Find(s=>s.RegionId==army.RegionId);
                int food=FoodLoad*2;
                if(army.Id==w.PlayerArmyId)
                {
                    army.Rations=Math.Min(campaign.Food,army.Rations);campaign.Food-=(int)army.Rations;
                    food=Math.Min(campaign.Food,food);campaign.Food-=food;
                }
                w.Depots.Add(new WorldDepot{Id=army.Id+"-depot",SiteId=site.Id,FactionId=army.FactionId,Food=food,Ammunition=AmmunitionLoad*2});
            }
        }
        public static double DailyNeed(WorldArmy army)=>army.Men/210d;
        public static double DaysLeft(WorldArmy army)=>army.Men==0?0:army.Rations/DailyNeed(army);
        public static bool Active(WorldConvoy c)=>c.Status!=ConvoyStatus.Delivered&&c.Status!=ConvoyStatus.Captured;
        public static ActionResult Dispatch(CampaignState campaign,string depotId,string armyId)
            =>Dispatch(campaign.World,depotId,armyId);
        private static ActionResult Dispatch(WorldState w,string depotId,string armyId)
        {
            var depot=w.Depots.Find(d=>d.Id==depotId);var army=w.Army(armyId);
            if(depot==null||army==null||depot.FactionId!=army.FactionId||army.Men==0)return Result(false,"supply.unavailable");
            if(w.Convoys.Exists(c=>c.ArmyId==armyId&&Active(c)))return Result(false,"supply.already_sent");
            if(depot.Food<FoodLoad||depot.Ammunition<AmmunitionLoad)return Result(false,"supply.empty_depot");
            var site=w.Sites.Find(s=>s.Id==depot.SiteId);
            var route=WorldRouting.Between(w,site.Position,army.Position);
            if(route==null)return Result(false,"world.route_unavailable");
            depot.Food-=FoodLoad;depot.Ammunition-=AmmunitionLoad;
            w.Convoys.Add(new WorldConvoy{Id="convoy-"+w.NextConvoyId++,DepotId=depot.Id,ArmyId=army.Id,FactionId=army.FactionId,Food=FoodLoad,Ammunition=AmmunitionLoad,Position=site.Position,Rendezvous=army.Position,Route=route,DispatchedAt=w.Clock.Milliseconds});
            return Result(true,"supply.sent");
        }
        public static ActionResult Restock(CampaignState campaign,string depotId)
        {
            var w=campaign.World;var depot=w.Depots.Find(d=>d.Id==depotId);
            // Ülke defterindeki merkez ambarı yalnız başkent deposuna aktarılabilir.
            if(depot==null||depot.FactionId!=w.Army(w.PlayerArmyId).FactionId||w.Sites.Find(s=>s.Id==depot.SiteId).RegionId!="ile")return Result(false,"supply.unavailable");
            if(campaign.Food<FoodLoad||campaign.MilitarySupplies<12)return Result(false,"supply.central_shortage");
            if(depot.Food>1000000-FoodLoad||depot.Ammunition>1000000-AmmunitionLoad)return Result(false,"supply.unavailable");
            campaign.Food-=FoodLoad;campaign.MilitarySupplies-=12;depot.Food+=FoodLoad;depot.Ammunition+=AmmunitionLoad;
            return Result(true,"supply.stocked");
        }
        public static void Step(WorldState w,double seconds,List<WorldPoint> armyStart=null)
        {
            foreach(var convoy in w.Convoys)
            {
                if(!Active(convoy))continue;
                double remainingTime=seconds;convoy.CauseId="";
                while(remainingTime>0&&convoy.Route.Segment+1<convoy.Route.Points.Count)
                {
                    double factor=WorldRouting.RoadSpeed(w,convoy.Route);
                    if(factor<=0){convoy.Status=ConvoyStatus.Blocked;convoy.CauseId=WorldRouting.CurrentRoad(convoy.Route);break;}
                    var target=convoy.Route.Points[convoy.Route.Segment+1];double length=WorldPoint.Distance(convoy.Position,target);
                    double used=Math.Min(remainingTime,length/(WorldConvoy.MetresPerSecond*factor));
                    var next=length<.001?target:WorldPoint.Lerp(convoy.Position,target,Math.Min(1,used*WorldConvoy.MetresPerSecond*factor/length));
                    // Süpürülen parça: yüksek hız, yolun üzerindeki düşmanı atlayamaz.
                    WorldArmy enemy=null;double intercept=2;
                    for(int i=0;i<w.Armies.Count;i++)
                    {
                        var candidate=w.Armies[i];if(candidate.FactionId==convoy.FactionId||candidate.Men==0||candidate.Activity==ArmyActivity.Retreating)continue;
                        var before=armyStart==null?candidate.Position:WorldPoint.Lerp(armyStart[i],candidate.Position,(seconds-remainingTime)/seconds);
                        var after=armyStart==null?candidate.Position:WorldPoint.Lerp(armyStart[i],candidate.Position,(seconds-remainingTime+used)/seconds);
                        var p=convoy.Position-before;var q=next-after;var v=q-p;double n=v.X*v.X+v.Z*v.Z;
                        double t=n<1e-9?0:Math.Max(0,Math.Min(1,-(p.X*v.X+p.Z*v.Z)/n));
                        if(WorldPoint.Distance(WorldPoint.Lerp(p,q,t),new WorldPoint())<250&&t<intercept){enemy=candidate;intercept=t;}
                    }
                    if(enemy!=null)
                    {
                        convoy.Position=WorldPoint.Lerp(convoy.Position,next,intercept);convoy.Status=ConvoyStatus.Captured;convoy.CauseId=enemy.Id;convoy.CompletedAt=w.Clock.Milliseconds;
                        // Ele geçen yük aynı yerdeki düşmana geçer; başka depoda çoğaltılmaz.
                        enemy.Rations+=convoy.Food;enemy.AmmunitionWagon+=convoy.Ammunition;convoy.Food=convoy.Ammunition=0;
                        if(convoy.FactionId==w.Army(w.PlayerArmyId).FactionId)w.LastNoticeKey="supply.captured";break;
                    }
                    convoy.Route.TravelledMetres+=WorldPoint.Distance(convoy.Position,next);convoy.Position=next;remainingTime-=used;
                    convoy.Status=ConvoyStatus.Travelling;
                    if(WorldPoint.Distance(next,target)<.001)convoy.Route.Segment++;
                    else break;
                }
                if(!Active(convoy))continue;
                var recipient=w.Army(convoy.ArmyId);
                if(WorldPoint.Distance(convoy.Position,recipient.WagonPosition)<350)
                {
                    recipient.Rations+=convoy.Food;recipient.AmmunitionWagon+=convoy.Ammunition;convoy.Food=convoy.Ammunition=0;
                    convoy.Status=ConvoyStatus.Delivered;convoy.CompletedAt=w.Clock.Milliseconds;
                    if(convoy.FactionId==w.Army(w.PlayerArmyId).FactionId)w.LastNoticeKey="supply.delivered";
                }
                else if(convoy.Route.Segment>=convoy.Route.Points.Count-1)convoy.Status=ConvoyStatus.Waiting;
            }
        }
        public static void QuarterHour(WorldState w)
        {
            foreach(var depot in w.Depots)
            {
                var site=w.Sites.Find(s=>s.Id==depot.SiteId);
                bool guarded=w.Armies.Exists(a=>a.FactionId==depot.FactionId&&a.Men>0&&WorldPoint.Distance(a.Position,site.Position)<500);
                if(guarded)continue;
                var captor=w.Armies.Find(a=>a.FactionId!=depot.FactionId&&a.Men>0&&a.Activity!=ArmyActivity.Retreating&&WorldPoint.Distance(a.Position,site.Position)<250);
                if(captor!=null){bool ours=depot.FactionId==w.Army(w.PlayerArmyId).FactionId;depot.FactionId=captor.FactionId;if(ours)w.LastNoticeKey="supply.depot_lost";}
            }
            foreach(var army in w.Armies)
            {
                if(army.Men<=0)continue;
                double need=DailyNeed(army)/96, eaten=Math.Min(need,army.Rations);army.Rations-=eaten;
                bool hungry=eaten+1e-9<need;
                army.HungrySeconds=hungry?army.HungrySeconds+900:Math.Max(0,army.HungrySeconds-1800);
                army.Supply=(float)Math.Max(0,Math.Min(100,DaysLeft(army)*100/7));
                if(hungry)
                {
                    army.Fatigue=Math.Min(100,army.Fatigue+.3f);
                    foreach(var unit in army.Units){unit.Morale=Math.Max(0,unit.Morale-.08f);unit.Fatigue=Math.Min(100,unit.Fatigue+.3f);}
                    if(army.HungrySeconds>=3*86400&&w.Clock.Milliseconds%WorldClock.Day==0)
                        foreach(var unit in army.Units)unit.Men-=Math.Min(unit.Men,(int)Math.Ceiling(unit.Men*.01));
                }
                if(army.Activity==ArmyActivity.Holding||army.Activity==ArmyActivity.Recovering)
                {
                    foreach(var unit in army.Units)
                    {
                        unit.Fatigue=army.Fatigue;
                        if(!hungry){unit.Cohesion=Math.Min(90,unit.Cohesion+.8f);if(unit.Morale<78)unit.Morale=Math.Min(78,unit.Morale+.3f);}
                        // Ülke ambarından bedava mermi çıkmaz; gerçek araba stokundan doldur.
                        int ammo=Math.Min(16-unit.Ammo,army.AmmunitionWagon);
                        if(ammo>0){unit.Ammo+=ammo;army.AmmunitionWagon-=ammo;}
                    }
                    if(!hungry&&army.WagonIntegrity<100)army.WagonIntegrity=Math.Min(100,army.WagonIntegrity+.5f);
                    var hq=w.Headquarters.Find(h=>h.Id==army.HeadquartersId);
                    if(!hungry&&hq.Integrity<100)hq.Integrity=Math.Min(100,hq.Integrity+.25f);
                }
            }
            // Yerel komutan mevcut deposuyla aynı kurallardan ikmal ister.
            foreach(var army in w.Armies)
                if(army.Id!=w.PlayerArmyId&&DaysLeft(army)<3&&!w.Convoys.Exists(c=>c.ArmyId==army.Id&&Active(c)))
                {
                    var depot=w.Depots.Find(d=>d.FactionId==army.FactionId);
                    if(depot!=null)Dispatch(w,depot.Id,army.Id);
                }
        }
        public static double DistanceToSegment(WorldPoint p,WorldPoint a,WorldPoint b)=>WorldPoint.Distance(p,Closest(p,a,b));
        private static WorldPoint Closest(WorldPoint p,WorldPoint a,WorldPoint b)
        {var v=b-a;double n=v.X*v.X+v.Z*v.Z;return n<1e-9?a:WorldPoint.Lerp(a,b,Math.Max(0,Math.Min(1,((p.X-a.X)*v.X+(p.Z-a.Z)*v.Z)/n)));}
        private static ActionResult Result(bool ok,string key)=>new ActionResult{Ok=ok,Key=key};
    }
}
