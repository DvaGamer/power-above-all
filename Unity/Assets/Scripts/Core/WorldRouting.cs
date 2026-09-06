using System;
using System.Collections.Generic;

namespace PowerAboveAll
{
    public static partial class WorldRouting
    {
        public static double Length(WorldRoad road)
        {double length=0;for(int i=1;i<road.Points.Count;i++)length+=WorldPoint.Distance(road.Points[i-1],road.Points[i]);return length;}

        // Yol grafiği kimlikle genişler; siyasi komşuluk, yol geometrisinin yerine geçmez.
        public static WorldRoute Find(WorldState world,WorldArmy army,string destination)
        {
            var end=world.Sites.Find(s=>s.Id==destination);if(end==null)return null;
            var route=Between(world,army.Position,end.Position);
            if(route!=null){route.DestinationSiteId=end.Id;route.DestinationRegionId=end.RegionId;}
            return route;
        }
        public static double Remaining(WorldArmy army)
        {
            var route=army.Route;if(route==null||route.Segment>=route.Points.Count-1)return 0;
            double value=WorldPoint.Distance(army.Position,route.Points[route.Segment+1]);
            for(int i=route.Segment+2;i<route.Points.Count;i++)value+=WorldPoint.Distance(route.Points[i-1],route.Points[i]);return value;
        }
    }
}
