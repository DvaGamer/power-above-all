using System;

namespace PowerAboveAll
{
    // Görsel ve savaş aynı fiziksel katmanı sorgular. Varsayılan açık araziye gizli bonus yok.
    public static class WorldTerrain
    {
        public static double Dot(WorldPoint a,WorldPoint b)=>a.X*b.X+a.Z*b.Z;
        public static WorldPoint Normal(WorldPoint v)
        {double length=Math.Sqrt(Dot(v,v));return length<.0001?new WorldPoint(0,1):v*(1/length);}
        public static double SegmentDistance(WorldPoint p,WorldPoint a,WorldPoint b)
        {var v=b-a;double d=Dot(v,v);return WorldPoint.Distance(p,d<.0001?a:WorldPoint.Lerp(a,b,Math.Max(0,Math.Min(1,Dot(p-a,v)/d))));}
        public static bool Contains(WorldTerrainFeature f,WorldPoint p)
        {
            if(f.Kind!=WorldTerrainKind.River)return WorldPoint.Distance(f.Centre,p)<=f.Radius;
            for(int i=1;i<f.Points.Count;i++)if(SegmentDistance(p,f.Points[i-1],f.Points[i])<=f.Radius)return true;
            return false;
        }
        public static bool In(WorldState w,WorldPoint p,WorldTerrainKind kind)
        {foreach(var f in w.Terrain)if(f.Kind==kind&&Contains(f,p))return true;return false;}
        public static double Height(WorldState w,WorldPoint p)
        {double h=0;foreach(var f in w.Terrain)if(f.Kind==WorldTerrainKind.Hill)h=Math.Max(h,f.Height*Math.Max(0,1-WorldPoint.Distance(f.Centre,p)/f.Radius));return h;}
        public static double Cover(WorldState w,WorldPoint p)
        {if(In(w,p,WorldTerrainKind.Town))return .48;if(In(w,p,WorldTerrainKind.Woodland))return .32;return 0;}
        public static double MoveFactor(WorldState w,WorldUnit u)
        {
            double factor=1;
            if(In(w,u.Position,WorldTerrainKind.Woodland))factor*=u.Kind==WorldUnitKind.Cavalry?.38:.67;
            if(In(w,u.Position,WorldTerrainKind.Town))factor*=u.Kind==WorldUnitKind.Artillery?.45:.7;
            if(In(w,u.Position,WorldTerrainKind.River))factor*=.22;
            return factor;
        }
        public static bool ClearSight(WorldState w,WorldPoint a,WorldPoint b)
        {
            double distance=WorldPoint.Distance(a,b);if(distance<70)return true;
            double start=Height(w,a)+2,end=Height(w,b)+2;
            // Işın boyunca dünya listesini her örnekte yeniden tarama. İlgili birkaç kütleyi seç.
            var relevant=new System.Collections.Generic.List<WorldTerrainFeature>();
            foreach(var f in w.Terrain)if(f.Kind!=WorldTerrainKind.River&&SegmentDistance(f.Centre,a,b)<=f.Radius)relevant.Add(f);
            if(relevant.Count==0)return true;
            // 24 ışın örneği üst sınırı; 86 400 render veya fizik çağrısı yapılmaz.
            int steps=Math.Min(24,Math.Max(2,(int)(distance/25)));double woods=0;
            for(int i=1;i<steps;i++)
            {
                double t=i/(double)steps;var p=WorldPoint.Lerp(a,b,t);
                bool wooded=false;
                foreach(var f in relevant)
                {
                    if(!Contains(f,p))continue;
                    if(f.Kind==WorldTerrainKind.Hill&&f.Height*Math.Max(0,1-WorldPoint.Distance(f.Centre,p)/f.Radius)>start+(end-start)*t+1)return false;
                    if(f.Kind==WorldTerrainKind.Town)return false;
                    if(f.Kind==WorldTerrainKind.Woodland)wooded=true;
                }
                if(wooded){woods+=distance/steps;if(woods>90)return false;}
            }
            return true;
        }
        public static bool Visible(WorldState w,WorldArmy observer,WorldUnit target)
            => Visible(w,observer,target.Position);
        public static bool Visible(WorldState w,WorldArmy observer,WorldPoint target)
        {
            foreach(var scout in observer.Units)
            {
                if(!WorldCombat.Fighting(scout))continue;
                double range=scout.Kind==WorldUnitKind.Cavalry?1700:1050;
                range+=Math.Min(800,Height(w,scout.Position)*8);
                if(In(w,target,WorldTerrainKind.Woodland))range*=.5;
                if(WorldPoint.Distance(scout.Position,target)<=range&&ClearSight(w,scout.Position,target))return true;
            }
            return false;
        }
        public static bool FriendlyFireLane(WorldArmy army,WorldUnit shooter,WorldPoint target)
        {
            double length=WorldPoint.Distance(shooter.Position,target);
            foreach(var friend in army.Units)
            {
                if(friend==shooter||!WorldCombat.Fighting(friend))continue;
                double projection=Dot(friend.Position-shooter.Position,Normal(target-shooter.Position));
                if(projection>25&&projection<length-20&&SegmentDistance(friend.Position,shooter.Position,target)<32)return false;
            }
            return true;
        }
    }
}
