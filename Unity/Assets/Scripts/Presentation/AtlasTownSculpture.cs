using System.Collections.Generic;
using UnityEngine;

namespace PowerAboveAll
{
    // Elle yerleştirilmiş duvar/çatı/kule kütleleri; tarihî bina rekonstrüksiyonu değil.
    internal sealed class AtlasTownSculpture
    {
        public readonly List<Vector3> Vertices=new List<Vector3>();
        public readonly List<int> Indices=new List<int>();
        public readonly List<Color> Colors=new List<Color>();
        private static readonly Color Paper=ColorOf("#F3E7CA"),Earth=ColorOf("#B79D71"),Ink=ColorOf("#243B37"),Water=ColorOf("#83B0B6"),Coral=ColorOf("#C98270");
        private static Color ColorOf(string hex){ColorUtility.TryParseHtmlString(hex,out var c);return c;}
        private void Face(Color color, params Vector3[] p)
        {
            int start=Vertices.Count;var c=QualitySettings.activeColorSpace==ColorSpace.Linear?color.linear:color;
            foreach(var point in p){Vertices.Add(point);Colors.Add(c);}
            for(int i=1;i<p.Length-1;i++){Indices.Add(start);Indices.Add(start+i);Indices.Add(start+i+1);}
        }
        private void Box(float x,float z,float width,float depth,float height,Color color)
        {
            float a=x-width*.5f,b=x+width*.5f,c=z-depth*.5f,d=z+depth*.5f;
            Face(color,new Vector3(a,0,c),new Vector3(b,0,c),new Vector3(b,height,c),new Vector3(a,height,c));
            Face(Color.Lerp(color,Earth,.28f),new Vector3(b,0,c),new Vector3(b,0,d),new Vector3(b,height,d),new Vector3(b,height,c));
            Face(Color.Lerp(color,Paper,.10f),new Vector3(a,0,d),new Vector3(a,0,c),new Vector3(a,height,c),new Vector3(a,height,d));
            Face(Color.Lerp(color,Earth,.18f),new Vector3(b,0,d),new Vector3(a,0,d),new Vector3(a,height,d),new Vector3(b,height,d));
        }
        public void House(float x,float z,float width,float depth,float height,float roofHeight,bool warm=false)
        {
            Box(x,z,width,depth,height,Paper);
            float a=x-width*.56f,b=x+width*.56f,c=z-depth*.57f,d=z+depth*.57f,ridge=z+depth*.025f;
            Color roof=Color.Lerp(warm?Coral:Water,Ink,.26f),shadow=Color.Lerp(roof,Ink,.20f);
            Face(roof,new Vector3(a,height,c),new Vector3(b,height,c),new Vector3(b,height+roofHeight,ridge),new Vector3(a,height+roofHeight,ridge));
            Face(shadow,new Vector3(a,height+roofHeight,ridge),new Vector3(b,height+roofHeight,ridge),new Vector3(b,height,d),new Vector3(a,height,d));
            Face(Color.Lerp(Paper,Earth,.15f),new Vector3(a,height,c),new Vector3(a,height+roofHeight,ridge),new Vector3(a,height,d));
            Face(Color.Lerp(Paper,Earth,.30f),new Vector3(b,height,c),new Vector3(b,height,d),new Vector3(b,height+roofHeight,ridge));
            float door=Mathf.Min(.18f,width*.16f),front=z-depth*.5f-.009f;
            Face(Color.Lerp(Ink,Earth,.23f),new Vector3(x-door,0,front),new Vector3(x+door,0,front),new Vector3(x+door,height*.56f,front),new Vector3(x-door,height*.56f,front));
            if(width>1.1f)
                for(int side=-1;side<=1;side+=2)
                {
                    float wx=x+side*width*.30f,wy=height*.61f;
                    Face(Color.Lerp(Ink,Water,.28f),new Vector3(wx-.08f,wy-.09f,front),new Vector3(wx+.08f,wy-.09f,front),new Vector3(wx+.08f,wy+.09f,front),new Vector3(wx-.08f,wy+.09f,front));
                }
        }
        public void Tower(float x,float z,float width,float height)
        {
            Box(x,z,width,width*.9f,height,Paper);
            float a=x-width*.59f,b=x+width*.59f,c=z-width*.55f,d=z+width*.55f;
            Vector3 peak=new Vector3(x-width*.035f,height+width*.62f,z);
            Color roof=Color.Lerp(Water,Ink,.34f);
            Face(roof,new Vector3(a,height,c),new Vector3(b,height,c),peak);
            Face(Color.Lerp(roof,Ink,.2f),new Vector3(b,height,c),new Vector3(b,height,d),peak);
            Face(roof,new Vector3(b,height,d),new Vector3(a,height,d),peak);
            Face(Color.Lerp(roof,Paper,.15f),new Vector3(a,height,d),new Vector3(a,height,c),peak);
            float front=z-width*.45f-.01f;
            Face(Ink,new Vector3(x-.12f,height*.62f,front),new Vector3(x+.12f,height*.62f,front),new Vector3(x+.12f,height*.85f,front),new Vector3(x,height*.92f,front),new Vector3(x-.12f,height*.85f,front));
        }
        public static AtlasTownSculpture Draw(AtlasSettlement town)
        {
            var d=new AtlasTownSculpture();
            if(town.rank==0&&town.regionId=="ile")
            {
                d.House(0,.30f,1.55f,1.25f,1.1f,.60f);
                d.Tower(-.92f,-.15f,.60f,1.90f);d.Tower(.87f,-.11f,.57f,1.82f);
                d.House(-1.65f,-.50f,.79f,.70f,.54f,.31f);d.House(1.57f,.35f,.92f,.76f,.60f,.34f);
                d.House(.30f,-1.1f,.81f,.57f,.43f,.27f);
            }
            else if(town.rank==0&&town.regionId=="guyenne")
            {
                d.House(-1.32f,-.5f,.91f,.83f,.64f,.33f);d.House(-.29f,-.48f,.93f,.83f,.69f,.31f);d.House(.76f,-.45f,.94f,.82f,.62f,.35f);
                d.House(-.48f,.58f,1.43f,.73f,.67f,.37f);d.Tower(1.35f,.57f,.52f,1.77f);
            }
            else if(town.name.ToLowerInvariant().Contains("lyon"))
            {
                d.House(-.94f,-.48f,1.16f,.78f,.60f,.36f,true);d.House(.38f,-.14f,1.07f,.84f,.98f,.39f,true);
                d.House(-.57f,.61f,1.05f,.71f,1.31f,.38f,true);d.Tower(.80f,.77f,.5f,1.90f);
            }
            else
            {
                bool south=town.regionId=="provence"||town.regionId=="languedoc";
                d.House(-.74f,-.32f,1.34f,.82f,.65f,.37f,south);
                d.House(.74f,-.12f,1.08f,.93f,.79f,.44f,south);
                d.Tower(-.08f,.58f,.55f,town.rank==0?1.79f:1.40f);
                if(town.rank==0)d.House(-1.09f,.71f,.76f,.63f,.53f,.29f,south);
            }
            return d;
        }
    }
}
