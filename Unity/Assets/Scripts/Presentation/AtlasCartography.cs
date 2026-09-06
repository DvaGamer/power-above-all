using System.Collections.Generic;
using UnityEngine;

namespace PowerAboveAll
{
    public static class AtlasCartography
    {
        // Yalnız görüntü çizgisi genelleşir. Kaynak GIS, dolgu ve collider aynı kalır.
        public static GeoPath Simplify(GeoPath source, float tolerance)
        {
            int count = source.points.Length / 2;
            if (count < 4 || tolerance <= 0) return new GeoPath { points = (float[])source.points.Clone() };
            var points = new Vector3[count];
            for (int i=0;i<count;i++) points[i]=AtlasProjection.Project(source.points[i*2],source.points[i*2+1]);
            bool closed=(points[0]-points[count-1]).sqrMagnitude<.000001f;
            var keep=new bool[count];keep[0]=keep[count-1]=true;
            var spans=new Stack<Vector2Int>();
            if(closed)
            {
                int pivot=1;float far=0;
                for(int i=1;i<count-1;i++){float d=(points[i]-points[0]).sqrMagnitude;if(d>far){far=d;pivot=i;}}
                keep[pivot]=true;spans.Push(new Vector2Int(0,pivot));spans.Push(new Vector2Int(pivot,count-1));
            }
            else spans.Push(new Vector2Int(0,count-1));
            while(spans.Count>0)
            {
                var span=spans.Pop();int split=-1;float far=tolerance*tolerance;
                Vector3 a=points[span.x],ab=points[span.y]-a;float length=ab.sqrMagnitude;
                for(int i=span.x+1;i<span.y;i++)
                {
                    float t=length>0?Mathf.Clamp01(Vector3.Dot(points[i]-a,ab)/length):0;
                    float d=(points[i]-(a+ab*t)).sqrMagnitude;
                    if(d>far){far=d;split=i;}
                }
                if(split<0)continue;
                keep[split]=true;spans.Push(new Vector2Int(span.x,split));spans.Push(new Vector2Int(split,span.y));
            }
            var result=new List<float>();
            for(int i=0;i<count;i++)if(keep[i]){result.Add(source.points[i*2]);result.Add(source.points[i*2+1]);}
            if(closed&&result.Count<8)return new GeoPath{points=(float[])source.points.Clone()};
            return new GeoPath{points=result.ToArray()};
        }
        public static GeoPath[] Dashes(float[] coordinates,float dash,float gap)
        {
            if(dash<=0||gap<=0||float.IsNaN(dash)||float.IsNaN(gap)||float.IsInfinity(dash)||float.IsInfinity(gap))
                throw new System.ArgumentOutOfRangeException(nameof(dash));
            var result=new List<GeoPath>();double travelled=0,period=(double)dash+gap;
            for(int i=2;i<coordinates.Length;i+=2)
            {
                Vector3 a=AtlasProjection.Project(coordinates[i-2],coordinates[i-1]),b=AtlasProjection.Project(coordinates[i],coordinates[i+1]);
                float length=Vector3.Distance(a,b);if(length<.00001f)continue;
                double segmentEnd=travelled+length;
                long first=(long)System.Math.Floor(travelled/period);
                for(long mark=first;mark*period<segmentEnd;mark++)
                {
                    double begin=System.Math.Max(travelled,mark*period),end=System.Math.Min(segmentEnd,mark*period+dash);
                    if(end-begin<.000001)continue;
                    Vector2 from=AtlasProjection.Geographic(Vector3.Lerp(a,b,(float)((begin-travelled)/length)));
                    Vector2 to=AtlasProjection.Geographic(Vector3.Lerp(a,b,(float)((end-travelled)/length)));
                    result.Add(new GeoPath{points=new[]{from.x,from.y,to.x,to.y}});
                }
                travelled=segmentEnd;
            }
            return result.ToArray();
        }
    }
}
