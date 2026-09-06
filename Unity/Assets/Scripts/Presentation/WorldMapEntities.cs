using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PowerAboveAll
{
    // Görsel hiyerarşi tek WorldArmy/Unit kaydını okur; savaş için ikinci ordu üretmez.
    public sealed partial class WorldMapEntities : MonoBehaviour
    {
        public const float Ground=.25f;
        private GameApp app;
        private WorldState observed;
        private readonly Dictionary<string,ArmyView> views=new Dictionary<string,ArmyView>();
        private readonly List<UnityEngine.Object> owned=new List<UnityEngine.Object>();
        private Material blue,red,paper,ink,gold,smoke,figureInk;
        private Mesh soldierMesh,enemySoldierMesh,militiaMesh,cavalryMesh,enemyCavalryMesh,smokeMesh;
        private Transform physicalRoads;
        private sealed class ArmyView
        {
            public Transform Root, Marker, Headquarters, Wagon;
            public LineRenderer Route;
            public readonly Dictionary<string,UnitView> Units=new Dictionary<string,UnitView>();
        }
        private sealed class UnitView { public Transform Root;public LineRenderer Outline;public Transform[] Figures;public Transform Flash,Smoke;public double HeardShot; }
        public static Vector3 Position(WorldPoint p)=>new Vector3((float)(p.X/WorldPoint.MetresPerAtlasUnit),Ground,(float)(p.Z/WorldPoint.MetresPerAtlasUnit));
        public static WorldPoint Point(Vector3 p)=>new WorldPoint(p.x*WorldPoint.MetresPerAtlasUnit,p.z*WorldPoint.MetresPerAtlasUnit);
        private const float Metre=(float)(1/WorldPoint.MetresPerAtlasUnit);
        public void Initialize(GameApp host)
        {
            app=host;blue=Material("#5F8DA5");red=Material("#C98270");paper=Material("#E9DCB7");ink=Material("#243B37");gold=Material("#CAB36F");smoke=Material("#DBDCC6");
            figureInk=Material("#FFFFFF");soldierMesh=BuildSoldier("#5F8DA5");enemySoldierMesh=BuildSoldier("#B96F5D");militiaMesh=BuildSoldier("#CDBF92");smokeMesh=BuildSmoke();
            cavalryMesh=BuildCavalry(soldierMesh);enemyCavalryMesh=BuildCavalry(enemySoldierMesh);
            BuildLocalLandscape();
            physicalRoads=new GameObject("Road network · close representation").transform;physicalRoads.SetParent(transform,false);
            foreach(var route in app.Map.WorldData.roads)
            {
                var line=Line(route.id,physicalRoads,ink,route.points.Length/2);line.widthMultiplier=5*Metre;
                for(int i=0;i<route.points.Length;i+=2)line.SetPosition(i/2,AtlasProjection.Project(route.points[i],route.points[i+1],Ground+Metre));
            }
        }
        private Material Material(string color)
        {
            ColorUtility.TryParseHtmlString(color,out var value);
            var material=new Material(Resources.Load<Shader>("World/AtlasInk")){color=value};owned.Add(material);return material;
        }
        private Mesh BuildSoldier(string coat)
        {
            // Kafa, kısa ceket, iki bacak, geniş şapka ve tüfek: tek elle belirlenen siluet.
            var combine=new List<CombineInstance>();
            var colours=new List<Color>();ColorUtility.TryParseHtmlString(coat,out var uniform);
            var cube=GameObject.CreatePrimitive(PrimitiveType.Cube);var source=cube.GetComponent<MeshFilter>().sharedMesh;
            Vector3[] positions={new Vector3(0,1.8f,0),new Vector3(0,1.1f,0),new Vector3(-.25f,.35f,0),new Vector3(.25f,.35f,0),new Vector3(0,2.1f,0),new Vector3(.6f,1.2f,.2f)};
            Vector3[] scales={new Vector3(.5f,.5f,.48f),new Vector3(.85f,.9f,.5f),new Vector3(.27f,.7f,.35f),new Vector3(.27f,.7f,.35f),new Vector3(.9f,.18f,.55f),new Vector3(.1f,1.9f,.12f)};
            for(int i=0;i<positions.Length;i++)
            {
                combine.Add(new CombineInstance{mesh=source,transform=Matrix4x4.TRS(positions[i],Quaternion.identity,scales[i])});
                var colour=i==0?new Color(.77f,.65f,.46f):i==1?uniform:i==2||i==3?new Color(.84f,.81f,.66f):new Color(.15f,.23f,.21f);
                if(QualitySettings.activeColorSpace==ColorSpace.Linear)colour=colour.linear;
                for(int v=0;v<source.vertexCount;v++)colours.Add(colour);
            }
            cube.SetActive(false);Destroy(cube);
            var mesh=new Mesh{name="PAA constructed infantry silhouette"};mesh.CombineMeshes(combine.ToArray());mesh.SetColors(colours);owned.Add(mesh);return mesh;
        }
        private Mesh BuildSmoke()
        {
            // Üç geniş, asimetrik hacim; her atışın şekli aynı alfabenin uzantısıdır.
            var source=GameObject.CreatePrimitive(PrimitiveType.Sphere);var meshSource=source.GetComponent<MeshFilter>().sharedMesh;
            var pieces=new[]{
                new CombineInstance{mesh=meshSource,transform=Matrix4x4.TRS(new Vector3(-.28f,0,0),Quaternion.identity,new Vector3(.72f,.8f,.85f))},
                new CombineInstance{mesh=meshSource,transform=Matrix4x4.TRS(new Vector3(.12f,.12f,.08f),Quaternion.identity,new Vector3(.8f,1,.9f))},
                new CombineInstance{mesh=meshSource,transform=Matrix4x4.TRS(new Vector3(.48f,.05f,-.04f),Quaternion.identity,new Vector3(.45f,.7f,.65f))}};
            source.SetActive(false);Destroy(source);var mesh=new Mesh{name="PAA powder · three authored masses"};mesh.CombineMeshes(pieces);owned.Add(mesh);return mesh;
        }
        private Mesh BuildCavalry(Mesh rider)
        {
            var cube=GameObject.CreatePrimitive(PrimitiveType.Cube);var source=cube.GetComponent<MeshFilter>().sharedMesh;
            var pieces=new List<CombineInstance>{new CombineInstance{mesh=rider,transform=Matrix4x4.TRS(Vector3.up*.95f,Quaternion.identity,Vector3.one*.82f)}};
            var colours=new List<Color>(rider.colors);var brown=new Color(.32f,.27f,.20f);if(QualitySettings.activeColorSpace==ColorSpace.Linear)brown=brown.linear;
            Vector3[] p={new Vector3(0,.95f,0),new Vector3(0,1.5f,.8f),new Vector3(-.28f,.35f,-.6f),new Vector3(.28f,.35f,-.6f),new Vector3(-.28f,.35f,.6f),new Vector3(.28f,.35f,.6f),new Vector3(0,.9f,-1)};
            Vector3[] size={new Vector3(.9f,.65f,1.7f),new Vector3(.5f,1,.6f),new Vector3(.2f,.8f,.22f),new Vector3(.2f,.8f,.22f),new Vector3(.2f,.8f,.22f),new Vector3(.2f,.8f,.22f),new Vector3(.18f,.7f,.24f)};
            for(int i=0;i<p.Length;i++){pieces.Add(new CombineInstance{mesh=source,transform=Matrix4x4.TRS(p[i],Quaternion.identity,size[i])});for(int v=0;v<source.vertexCount;v++)colours.Add(brown);}
            cube.SetActive(false);Destroy(cube);var mesh=new Mesh{name="PAA mounted silhouette"};mesh.CombineMeshes(pieces.ToArray());mesh.SetColors(colours);owned.Add(mesh);return mesh;
        }
        private Transform Shape(string name,Transform parent,Vector3 scale,Material material,Mesh mesh=null)
        {
            GameObject go;
            if(mesh==null){go=GameObject.CreatePrimitive(PrimitiveType.Cube);var col=go.GetComponent<Collider>();col.enabled=false;Destroy(col);}
            else{go=new GameObject();go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>();}
            go.name=name;go.transform.SetParent(parent,false);go.transform.localScale=scale;
            var renderer=go.GetComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;return go.transform;
        }
        private LineRenderer Line(string name,Transform parent,Material material,int count)
        {
            var go=new GameObject(name);go.transform.SetParent(parent,false);var line=go.AddComponent<LineRenderer>();line.sharedMaterial=material;line.useWorldSpace=false;line.positionCount=count;line.shadowCastingMode=ShadowCastingMode.Off;return line;
        }
        private ArmyView Build(WorldArmy army)
        {
            var root=new GameObject(army.Id+" · persistent world army").transform;root.SetParent(transform,false);
            var colour=army.Id==observed.PlayerArmyId?blue:red;
            var view=new ArmyView{Root=root,Marker=new GameObject("Army standard").transform};view.Marker.SetParent(root,false);
            var pole=Shape("Pole",view.Marker,new Vector3(.04f,.8f,.04f),ink);pole.localPosition=Vector3.up*.4f;
            var cloth=Shape("Folded command pennant",view.Marker,new Vector3(.64f,.4f,.055f),colour);cloth.localPosition=new Vector3(.3f,.65f,0);
            var band=Shape("Linen hoist",view.Marker,new Vector3(.10f,.4f,.06f),paper);band.localPosition=new Vector3(.05f,.65f,0);
            view.Route=Line("Actual remaining route",root,gold,0);
            view.Headquarters=new GameObject("Headquarters · command tent").transform;view.Headquarters.SetParent(root,false);
            var tent=Shape("Linen command tent",view.Headquarters,new Vector3(20,10,16)*Metre,paper);tent.localPosition=Vector3.up*5*Metre;
            var ridge=Shape("Tent ridge",view.Headquarters,new Vector3(16,4,18)*Metre,colour);ridge.localPosition=Vector3.up*12*Metre;ridge.localRotation=Quaternion.Euler(0,0,-9);
            var standard=Shape("Headquarters pennant",view.Headquarters,new Vector3(12,8,1)*Metre,colour);standard.localPosition=new Vector3(9,22,0)*Metre;
            view.Wagon=new GameObject("Ammunition wagon").transform;view.Wagon.SetParent(root,false);
            var bed=Shape("Wooden bed",view.Wagon,new Vector3(18,5,9)*Metre,ink);bed.localPosition=Vector3.up*4*Metre;
            for(int side=-1;side<=1;side+=2)for(int end=-1;end<=1;end+=2)
            {var wheel=Shape("Cart wheel",view.Wagon,new Vector3(4,4,1)*Metre,gold);wheel.localPosition=new Vector3(end*6,2,side*5)*Metre;}
            foreach(var unit in army.Units)
            {
                var u=new UnitView{Root=new GameObject(unit.Id+" · regiment").transform,Figures=new Transform[36],HeardShot=unit.LastFiredAt};u.Root.SetParent(root,false);
                u.Outline=Line("Regiment frontage",u.Root,colour,5);
                Mesh figure=unit.Kind==WorldUnitKind.Cavalry?(army.Id==observed.PlayerArmyId?cavalryMesh:enemyCavalryMesh):unit.Kind==WorldUnitKind.Militia?militiaMesh:army.Id==observed.PlayerArmyId?soldierMesh:enemySoldierMesh;
                for(int i=0;i<u.Figures.Length;i++)u.Figures[i]=Shape("Figure "+i,u.Root,Vector3.one*4.5f*Metre,figureInk,figure);
                if(unit.Kind==WorldUnitKind.Artillery)
                    for(int gun=0;gun<3;gun++)
                    {
                        var barrel=Shape("Field piece",u.Root,new Vector3(2,2,8)*Metre,ink);barrel.localPosition=new Vector3((gun-1)*22,3,16)*Metre;
                        for(int side=-1;side<=1;side+=2){var wheel=Shape("Gun wheel",u.Root,new Vector3(1,4,4)*Metre,gold);wheel.localPosition=barrel.localPosition+new Vector3(side*2.2f,-1,-1)*Metre;}
                    }
                u.Flash=Shape("Volley transient",u.Root,new Vector3(70,2,2)*Metre,gold);
                u.Smoke=Shape("Powder mass",u.Root,Vector3.one,smoke,smokeMesh);view.Units.Add(unit.Id,u);
            }
            return view;
        }
        private void LateUpdate()
        {
            if(app==null||app.State.World==null)return;
            if(observed!=app.State.World)
            {foreach(var view in views.Values){view.Root.gameObject.SetActive(false);Destroy(view.Root.gameObject);}views.Clear();observed=app.State.World;foreach(var army in observed.Armies)views.Add(army.Id,Build(army));}
            float distance=app.StrategyCamera.Distance;
            physicalRoads.gameObject.SetActive(distance<2);
            UpdateLocalLandscape(distance);
            UpdateSupplyViews(distance);
            foreach(var army in observed.Armies)
            {
                var view=views[army.Id];view.Root.position=Position(army.Position);
                bool ours=army.FactionId==observed.Army(observed.PlayerArmyId).FactionId;
                var sight=observed.Sightings.Find(s=>s.ArmyId==army.Id);
                view.Marker.gameObject.SetActive(distance>=2&&army.Men>0&&(ours||sight!=null));
                if(!ours&&sight!=null)view.Marker.localPosition=Position(sight.Position)-Position(army.Position);
                view.Marker.localScale=Vector3.one*Mathf.Clamp(distance/90,.1f,2);
                var hq=observed.Headquarters.Find(h=>h.Id==army.HeadquartersId);
                view.Headquarters.gameObject.SetActive(distance<2&&(ours||WorldTerrain.Visible(observed,observed.Army(observed.PlayerArmyId),hq.Position))&&hq.Integrity>0);
                view.Headquarters.localPosition=Position(hq.Position)-Position(army.Position);
                view.Wagon.gameObject.SetActive(distance<2&&(ours||WorldTerrain.Visible(observed,observed.Army(observed.PlayerArmyId),army.WagonPosition))&&army.WagonIntegrity>0);
                view.Wagon.localPosition=Position(army.WagonPosition)-Position(army.Position);
                bool route=army.Activity==ArmyActivity.Marching&&distance<420;view.Route.enabled=route;
                if(route)
                {
                    int remaining=army.Route.Points.Count-army.Route.Segment;
                    view.Route.positionCount=remaining;view.Route.widthMultiplier=Mathf.Clamp(distance*.0006f,.0001f,.09f);view.Route.SetPosition(0,Vector3.zero);
                    for(int i=1;i<remaining;i++){WorldPoint p=army.Route.Points[army.Route.Segment+i]-army.Position;view.Route.SetPosition(i,new Vector3((float)p.X,3,(float)p.Z)*Metre);}
                }
                foreach(var unit in army.Units)
                {
                    var u=view.Units[unit.Id];bool visible=distance<2&&unit.Men>0&&(ours||WorldTerrain.Visible(observed,observed.Army(observed.PlayerArmyId),unit));u.Root.gameObject.SetActive(visible);if(!visible)continue;
                    WorldPoint offset=unit.Position-army.Position;u.Root.localPosition=new Vector3((float)offset.X,0,(float)offset.Z)*Metre;u.Root.localRotation=Quaternion.Euler(0,(float)unit.Facing,0);
                    bool detail=distance<.35f;float width=unit.Formation==WorldFormation.Column?18:unit.Formation==WorldFormation.Square?50:110;float depth=unit.Formation==WorldFormation.Column?100:unit.Formation==WorldFormation.Square?50:22;
                    u.Outline.enabled=unit.Id==observed.SelectedUnitId;u.Outline.widthMultiplier=1.6f*Metre;
                    u.Outline.SetPositions(new[]{new Vector3(-width/2,1,-depth/2+8)*Metre,new Vector3(-width/2,1,-depth/2)*Metre,new Vector3(0,1,-depth/2)*Metre,new Vector3(width/2,1,-depth/2)*Metre,new Vector3(width/2,1,-depth/2+8)*Metre});
                    int count=Mathf.CeilToInt(36f*unit.Men/Math.Max(1,unit.Original));
                    for(int i=0;i<u.Figures.Length;i++)
                    {
                        u.Figures[i].gameObject.SetActive(detail&&i<count);if(!detail)continue;
                        int columns=unit.Formation==WorldFormation.Column?4:12,rows=36/columns;
                        float x=(i%columns/(float)(columns-1)-.5f)*width,z=(i/columns/(float)(rows-1)-.5f)*depth;
                        float facing=0;
                        if(unit.Formation==WorldFormation.Square)
                        {
                            int edge=i/9;float along=(i%9/8f-.5f)*width;
                            x=edge==0?along:edge==1?width/2:edge==2?-along:-width/2;
                            z=edge==0?depth/2:edge==1?-along:edge==2?-depth/2:along;facing=edge*90;
                        }
                        float bob=unit.Moving||army.Activity==ArmyActivity.Marching?Mathf.Sin((float)(observed.Clock.Seconds*5)+i)*.45f:0;
                        u.Figures[i].localPosition=new Vector3(x,bob,z)*Metre;
                        u.Figures[i].localRotation=Quaternion.Euler(unit.Routed?12:0,facing,0);
                    }
                    float age=(float)(observed.Clock.Seconds-unit.LastFiredAt);
                    if(u.HeardShot!=unit.LastFiredAt)
                    {
                        u.HeardShot=unit.LastFiredAt;
                        if(detail&&age>=0&&age<.2f&&observed.Clock.Speed!=WorldSpeed.Pause)
                            app.Feedback("volley");
                    }
                    u.Flash.gameObject.SetActive(detail&&age>=0&&age<.1f);u.Flash.localPosition=new Vector3(0,3,depth/2+3)*Metre;
                    u.Smoke.gameObject.SetActive(detail&&age>=0&&age<4);
                    u.Smoke.localPosition=new Vector3(age*7,5+age*3,depth/2+6+age*2)*Metre;
                    u.Smoke.localScale=new Vector3(width*(.6f+age*.12f),3+age*2,4+age*4)*Metre;
                }
            }
        }
        public string PickArmy(Vector3 screen)
        {
            if(observed==null)return null;string chosen=null;float near=28*ViewLayout.Scale;
            foreach(var army in observed.Armies)
            {var sight=observed.Sightings.Find(s=>s.ArmyId==army.Id);if(army.Men==0||(army.Id!=observed.PlayerArmyId&&sight==null))continue;var p=app.Camera.WorldToScreenPoint(Position(army.Id==observed.PlayerArmyId?army.Position:sight.Position));float d=Vector2.Distance(p,screen);if(p.z>0&&d<near){near=d;chosen=army.Id;}}
            return chosen;
        }
        public string PickUnit(Vector3 screen)
        {
            if(observed==null||app.StrategyCamera.Distance>=2)return null;string chosen=null;float near=30*ViewLayout.Scale;
            foreach(var unit in observed.Army(observed.PlayerArmyId).Units)
            {if(!WorldCombat.Fighting(unit))continue;var p=app.Camera.WorldToScreenPoint(Position(unit.Position));float d=Vector2.Distance(p,screen);if(p.z>0&&d<near){near=d;chosen=unit.Id;}}
            return chosen;
        }
        private void OnDestroy(){foreach(var item in owned)if(item)Destroy(item);}
    }
}
