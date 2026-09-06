using System;
using System.Collections.Generic;

namespace PowerAboveAll
{
    public static partial class WorldRouting
    {
        private sealed class Edge
        { public int To; public string RoadId; public double Cost; public List<WorldPoint> Points; }
        private sealed class Cut { public int Node; public double Distance; }
        private sealed class Attachment
        {
            public int Site=-1, Road=-1;
            public double Along, Offset=double.PositiveInfinity;
            public WorldPoint Point;
        }
        private static double Length(List<WorldPoint> points)
        {double n=0;for(int i=1;i<points.Count;i++)n+=WorldPoint.Distance(points[i-1],points[i]);return n;}

        // Gerçek konumları yol parçasına bağla; eski bölge merkezine geri dönüş yok.
        // En fazla 5 km yerel erişim; uzak düğüme veya kıtaya kestirme oluşturulmaz.
        public static WorldRoute Between(WorldState world,WorldPoint start,WorldPoint end)
        {
            var a=Attach(world,start);var b=Attach(world,end);
            if(a.Offset>5000||b.Offset>5000)return null;
            var graph=new List<List<Edge>>();
            for(int i=0;i<world.Sites.Count+4;i++)graph.Add(new List<Edge>());
            var cuts=new List<List<Cut>>();
            foreach(var road in world.Roads)
                cuts.Add(new List<Cut>{new Cut{Node=world.Sites.FindIndex(s=>s.Id==road.From)+4},new Cut{Node=world.Sites.FindIndex(s=>s.Id==road.To)+4,Distance=Length(road)}});
            ConnectAttachment(graph,cuts,a,0,2,start);ConnectAttachment(graph,cuts,b,1,3,end);
            for(int i=0;i<world.Roads.Count;i++)
            {
                var road=world.Roads[i];if(road.Blocked||road.SpeedFactor<=0)continue;
                cuts[i].Sort((x,y)=>x.Distance.CompareTo(y.Distance));
                for(int j=1;j<cuts[i].Count;j++)
                    Connect(graph,cuts[i][j-1].Node,cuts[i][j].Node,Slice(road.Points,cuts[i][j-1].Distance,cuts[i][j].Distance),road.Id,road.SpeedFactor);
            }
            var costs=new double[graph.Count];var visited=new bool[graph.Count];var previous=new int[graph.Count];var chosen=new Edge[graph.Count];
            for(int i=0;i<costs.Length;i++){costs[i]=double.PositiveInfinity;previous[i]=-1;}costs[0]=0;
            while(true)
            {
                int node=-1;double best=double.PositiveInfinity;
                for(int i=0;i<costs.Length;i++)if(!visited[i]&&costs[i]<best){node=i;best=costs[i];}
                if(node<0)return null;if(node==1)break;visited[node]=true;
                foreach(var edge in graph[node])if(!visited[edge.To]&&best+edge.Cost<costs[edge.To])
                {costs[edge.To]=best+edge.Cost;previous[edge.To]=node;chosen[edge.To]=edge;}
            }
            var edges=new List<Edge>();for(int n=1;n!=0;n=previous[n])edges.Insert(0,chosen[n]);
            var result=new WorldRoute();result.Points.Add(start);
            foreach(var edge in edges)foreach(var p in edge.Points)Append(result,p,edge.RoadId);
            return result;
        }
        private static Attachment Attach(WorldState world,WorldPoint point)
        {
            var result=new Attachment();
            for(int i=0;i<world.Sites.Count;i++)
            {double d=WorldPoint.Distance(point,world.Sites[i].Position);if(d<.1)return new Attachment{Site=i,Point=world.Sites[i].Position,Offset=d};}
            for(int r=0;r<world.Roads.Count;r++)
            {
                double along=0;var points=world.Roads[r].Points;
                for(int i=1;i<points.Count;i++)
                {
                    var delta=points[i]-points[i-1];double length=WorldPoint.Distance(points[i-1],points[i]);
                    double t=length<.001?0:Math.Max(0,Math.Min(1,((point.X-points[i-1].X)*delta.X+(point.Z-points[i-1].Z)*delta.Z)/(length*length)));
                    var projected=WorldPoint.Lerp(points[i-1],points[i],t);double offset=WorldPoint.Distance(point,projected);
                    if(offset<result.Offset)result=new Attachment{Road=r,Point=projected,Along=along+t*length,Offset=offset};
                    along+=length;
                }
            }
            return result;
        }
        private static void ConnectAttachment(List<List<Edge>> graph,List<List<Cut>> cuts,Attachment a,int node,int projection,WorldPoint point)
        {
            Connect(graph,node,a.Site>=0?a.Site+4:projection,new List<WorldPoint>{point,a.Point},"",.65);
            if(a.Road>=0)cuts[a.Road].Add(new Cut{Node=projection,Distance=a.Along});
        }
        private static List<WorldPoint> Slice(List<WorldPoint> points,double from,double to)
        {
            var result=new List<WorldPoint>();double along=0;
            for(int i=1;i<points.Count;i++)
            {
                double length=WorldPoint.Distance(points[i-1],points[i]);
                if(length>.00001&&along<=to&&along+length>=from)
                {
                    var a=WorldPoint.Lerp(points[i-1],points[i],Math.Max(0,(from-along)/length));
                    var b=WorldPoint.Lerp(points[i-1],points[i],Math.Min(1,(to-along)/length));
                    if(result.Count==0)result.Add(a);result.Add(b);
                }
                along+=length;
            }
            return result;
        }
        private static void Connect(List<List<Edge>> graph,int a,int b,List<WorldPoint> points,string road,double factor)
        {
            double cost=Length(points)/factor;
            graph[a].Add(new Edge{To=b,Cost=cost,RoadId=road,Points=points});
            var reverse=new List<WorldPoint>(points);reverse.Reverse();
            graph[b].Add(new Edge{To=a,Cost=cost,RoadId=road,Points=reverse});
        }
        private static void Append(WorldRoute route,WorldPoint point,string road)
        {
            if(WorldPoint.Distance(route.Points[route.Points.Count-1],point)<.001)return;
            route.Points.Add(point);route.SegmentRoadIds.Add(road);
            if(road!=""&&!route.RoadIds.Contains(road))route.RoadIds.Add(road);
        }
        public static string CurrentRoad(WorldRoute route)=>route.Segment<route.SegmentRoadIds.Count?route.SegmentRoadIds[route.Segment]:"";
        public static double RoadSpeed(WorldState world,WorldRoute route)
        {var road=world.Roads.Find(r=>r.Id==CurrentRoad(route));return road==null?1:road.Blocked?0:road.SpeedFactor;}
        public static double Remaining(WorldRoute route,WorldPoint position)
        {
            if(route==null||route.Segment>=route.Points.Count-1)return 0;
            double value=WorldPoint.Distance(position,route.Points[route.Segment+1]);
            for(int i=route.Segment+2;i<route.Points.Count;i++)value+=WorldPoint.Distance(route.Points[i-1],route.Points[i]);return value;
        }
    }
}
