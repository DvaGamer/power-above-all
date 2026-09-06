using System.Collections.Generic;
using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class WorldMapEntities
    {
        private readonly List<Transform> localLandscape=new List<Transform>();
        private void BuildLocalLandscape()
        {
            var surface=Material("#FFFFFF");
            foreach(var town in app.Map.WorldData.settlements)
            {
                var root=new GameObject(town.id+" · authored local settlement").transform;root.SetParent(transform,false);root.position=Position(WorldPoint.FromGeographic(town.longitude,town.latitude));localLandscape.Add(root);
                var drawing=AtlasTownSculpture.Draw(town);var mesh=new Mesh{name=town.id+" · close silhouette"};
                mesh.SetVertices(drawing.Vertices);mesh.SetTriangles(drawing.Indices,0);mesh.SetColors(drawing.Colors);mesh.RecalculateBounds();owned.Add(mesh);
                Shape("Settlement buildings",root,Vector3.one*30*Metre,surface,mesh);
                var vertices=new List<Vector3>();var triangles=new List<int>();var colours=new List<Color>();
                // Parseller sanatsal çevre yorumudur; kadastro veya doğrulanmış1789arazi kullanımı değildir.
                Vector2[] centres={new Vector2(-390,-260),new Vector2(-710,-520),new Vector2(-980,-710),new Vector2(380,230),new Vector2(680,450),new Vector2(900,720)};
                string[] palette={"#BEC797","#A7B98B","#C7CBA1","#B4C18F","#CAD0A4","#9FB48B"};
                for(int i=0;i<centres.Length;i++)
                {
                    var p=centres[i];ColorUtility.TryParseHtmlString(palette[i],out var colour);
                    Face(vertices,triangles,colours,colour,new Vector3(p.x-160,0,p.y-220),new Vector3(p.x-148,0,p.y+210),new Vector3(p.x+125,0,p.y+228),new Vector3(p.x+158,0,p.y-208));
                    for(int line=0;line<5;line++)
                    {
                        float x=p.x-105+line*43;
                        Face(vertices,triangles,colours,Color.Lerp(colour,new Color(.42f,.50f,.32f),.2f),new Vector3(x,.4f,p.y-180),new Vector3(x-8,.4f,p.y+180),new Vector3(x-5,.4f,p.y+180),new Vector3(x+3,.4f,p.y-180));
                    }
                }
                for(int cluster=0;cluster<2;cluster++)for(int i=0;i<18;i++)
                {
                    float x=(cluster==0?-680:610)+(i%6)*27,z=(cluster==0?210:-650)+(i/6)*31;
                    Tree(vertices,triangles,colours,x,z,18+(i%3)*2,i%3);
                }
                var fields=new Mesh{name=town.id+" · parcels and shelterbelts"};fields.SetVertices(vertices);fields.SetTriangles(triangles,0);fields.SetColors(colours);fields.RecalculateBounds();owned.Add(fields);
                Shape("Local terrain forms",root,Vector3.one*Metre,surface,fields).localPosition=Vector3.up*Metre;
            }
        }
        private static void Face(List<Vector3> vertices,List<int> triangles,List<Color> colours,Color colour,params Vector3[] points)
        {
            int first=vertices.Count;var ink=QualitySettings.activeColorSpace==ColorSpace.Linear?colour.linear:colour;
            foreach(var point in points){vertices.Add(point);colours.Add(ink);}
            for(int i=1;i<points.Length-1;i++){triangles.Add(first);triangles.Add(first+i);triangles.Add(first+i+1);}
        }
        private static void Tree(List<Vector3> vertices,List<int> triangles,List<Color> colours,float x,float z,float height,int family)
        {
            var bark=new Color(.34f,.35f,.26f);var leaf=new Color(.33f,.47f,.36f);var light=new Color(.47f,.58f,.40f);
            Face(vertices,triangles,colours,bark,new Vector3(x-1,0,z),new Vector3(x+1,0,z),new Vector3(x+1,height*.6f,z),new Vector3(x-1,height*.6f,z));
            Vector3 tip=new Vector3(x-1+family,height,z+1),bottom=new Vector3(x,height*.3f,z);
            for(int side=0;side<7;side++)
            {
                float a=side*Mathf.PI*2/7,b=(side+1)*Mathf.PI*2/7;
                var left=new Vector3(x+Mathf.Cos(a)*height*.42f,height*.65f,z+Mathf.Sin(a)*height*.33f);
                var right=new Vector3(x+Mathf.Cos(b)*height*.42f,height*.65f,z+Mathf.Sin(b)*height*.33f);
                Face(vertices,triangles,colours,side<3?light:leaf,left,tip,right);Face(vertices,triangles,colours,leaf,right,bottom,left);
            }
        }
        private void UpdateLocalLandscape(float distance)
        {
            foreach(var root in localLandscape)
            {bool show=distance<2&&(root.position-app.StrategyCamera.FocusPoint).sqrMagnitude<distance*distance*6;root.gameObject.SetActive(show);}
        }
    }
}
