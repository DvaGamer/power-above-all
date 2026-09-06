using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PowerAboveAll
{
    public sealed partial class CampaignMap
    {
        public AtlasWorld WorldData { get; private set; }
        public GeoRiver[] WorldRivers => physical.rivers;
        public int VisibleGeographicChunks { get; private set; }
        private PhysicalGeography physical;
        private readonly List<GeographicChunk> geographicChunks = new List<GeographicChunk>();
        private readonly List<LocalDetail> localDetails = new List<LocalDetail>();
        private readonly Dictionary<string, GeoMesh[]> regionGeometry = new Dictionary<string, GeoMesh[]>();
        private readonly Dictionary<string, Mesh[]> boundaryMeshes = new Dictionary<string, Mesh[]>();
        private readonly List<GameObject[]> boundaryLevels = new List<GameObject[]>();
        private Transform geographicBorders, roadsRoot, riversRoot, extraCitiesRoot;
        private Material seaMaterial, landMaterial, coastMaterial, riverMaterial, detailMaterial;
        private float lodTimer;
        private sealed class GeographicChunk { public GameObject Root; public int Level; public Bounds Bounds; }
        private sealed class LocalDetail { public GameObject Root; public Vector3 Point; public float Limit, TownScale; }

        public void Build(Camera camera)
        {
            if (built) return;
            atlasCamera = camera;
            var physicalAsset = Resources.Load<TextAsset>("World/physical");
            var campaignAsset = Resources.Load<TextAsset>("World/campaign");
            if (!physicalAsset || !campaignAsset) throw new InvalidOperationException("Offline world atlas data is missing. Run tools/import-world.py.");
            physical = JsonUtility.FromJson<PhysicalGeography>(physicalAsset.text);
            WorldData = JsonUtility.FromJson<AtlasWorld>(campaignAsset.text);
            if (physical.schema != 1 || WorldData.schema != 1) throw new InvalidOperationException("Unsupported world atlas schema.");
            paperGrain = MakePaperGrain();
            seaMaterial = MakeMaterial(Hex("#83B0B6"));
            landMaterial = MakeAtlasMaterial(Hex("#D3D2A8"));
            coastMaterial = MakeMaterial(Hex("#567E77"));
            riverMaterial = MakeMaterial(Hex("#729EA2"));
            borderMat = MakeMaterial(Hex("#A4AE87")); goldMat = MakeMaterial(Hex("#CAB36F"));
            selectionInkMat = MakeMaterial(Hex("#637858")); hoverMat = MakeMaterial(Hex("#CAB36F"));
            roadMat = MakeMaterial(Hex("#B79D71"));
            cityInkMat = new Material(Resources.Load<Shader>("World/AtlasInk")) { color=Color.white };owned.Add(cityInkMat);
            detailMaterial = cityInkMat;
            seaMaterial.color = Hex("#83B0B6");
            MeshObject("Atlas ocean", Rectangle(-180,-90,180,90,-.3f), seaMaterial, transform);
            var graticule = NewRoot("Engraved meridians");
            var grid = new List<GeoPath>();
            for (int lon=-180;lon<=180;lon+=20) grid.Add(new GeoPath{points=new float[]{lon,-90,lon,90}});
            for (int lat=-80;lat<=80;lat+=20) grid.Add(new GeoPath{points=new float[]{-180,lat,180,lat}});
            MeshObject("Graticule", LinesMesh(grid.ToArray(), .11f, -.27f), MakeMaterial(Hex("#8DB6B8")), graticule);
            foreach (GeoMesh chunk in physical.chunks)
            {
                var root = NewRoot(chunk.id);
                MeshObject("Land", GeographicMesh(chunk,0), landMaterial, root);
                MeshObject("Coast engraving", LinesMesh(chunk.paths,chunk.lod==0?.32f:.065f,.018f),coastMaterial,root);
                Vector3 a=AtlasProjection.Project(chunk.bounds[0],chunk.bounds[1]), b=AtlasProjection.Project(chunk.bounds[2],chunk.bounds[3]);
                geographicChunks.Add(new GeographicChunk{Root=root.gameObject,Level=chunk.lod,Bounds=new Bounds((a+b)*.5f,b-a+Vector3.up)});
            }
            geographicBorders=NewRoot("Historical political boundaries");
            foreach (PoliticalEntity entity in WorldData.entities)
            foreach (GeoMesh area in entity.areas)
            {
                MeshObject(entity.id,GeographicMesh(area,.035f),MakeAtlasMaterial(Hex("#BEC99B")),geographicBorders);
                var outline=Array.ConvertAll(area.paths,p=>AtlasCartography.Simplify(p,.10f));
                MeshObject(entity.id+" boundary",LinesMesh(outline,.08f,.08f),borderMat,geographicBorders);
            }
            foreach (AtlasRegion region in WorldData.regions)
            {
                Seed seed=Array.Find(Seeds,s=>s.Id==region.id);
                if(seed==null)continue;
                var settlement=Array.Find(WorldData.settlements,s=>s.id==region.seatId);
                Vector3 position=AtlasProjection.Project(settlement.longitude,settlement.latitude);
                seed.Point=DrawingPoint(position);
                var root=NewRoot("Province:"+region.id);
                Material material=MakeAtlasMaterial(seed.Ink);
                var controller=root.gameObject.AddComponent<MeshRenderer>();controller.sharedMaterial=material;
                provinces[region.id]=controller; provinceColors[region.id]=displayedColors[region.id]=seed.Ink;
                regionGeometry[region.id]=region.areas;
                cells[region.id]=new List<Vector2>();
                foreach(GeoMesh area in region.areas)
                {
                    var mesh=GeographicMesh(area,.055f);
                    var part=MeshObject("Province:"+region.id,mesh,material,root);
                    part.AddComponent<MeshCollider>().sharedMesh=mesh;
                    if(area.paths.Length>0)foreach(Vector3 p in GeographicPoints(area.paths[0].points,0))cells[region.id].Add(DrawingPoint(p));
                }
                GeographicRegionBorder(region.id,root,borderMat,.10f);
            }
            roadsRoot=NewRoot("Strategic route network");
            foreach(AtlasRoute road in WorldData.roads)
                MeshObject(road.id,LinesMesh(AtlasCartography.Dashes(road.points,.28f,.18f),.065f,.13f),roadMat,roadsRoot);
            riversRoot=NewRoot("River systems");
            // Batched within geographic tiles; small rivers enter only near their tile.
            foreach(GeoRiver river in physical.rivers)
            {
                if(river.bounds[2]<-15||river.bounds[0]>35||river.bounds[3]<30||river.bounds[1]>65)continue;
                var go=MeshObject(river.name,LinesMesh(new[]{new GeoPath{points=river.points}},river.rank<=3?.12f:.07f,.12f),riverMaterial,riversRoot);
                localDetails.Add(new LocalDetail{Root=go,Point=AtlasProjection.Project((river.bounds[0]+river.bounds[2])*.5f,(river.bounds[1]+river.bounds[3])*.5f),Limit=river.rank<=3?900:330});
            }
            extraCitiesRoot=NewRoot("Provincial settlements");
            foreach(var settlement in WorldData.settlements)
            {
                Vector3 p=AtlasProjection.Project(settlement.longitude,settlement.latitude,.15f);
                var root=NewRoot(settlement.id);root.SetParent(extraCitiesRoot,false);root.localPosition=p;
                var engraving=AtlasTownSculpture.Draw(settlement);
                var mesh=NewMesh(engraving.Vertices,engraving.Indices);mesh.SetColors(engraving.Colors);
                MeshObject(settlement.id,mesh,cityInkMat,root);
                root.localScale=Vector3.one*(settlement.rank==0?.78f:.58f);
                localDetails.Add(new LocalDetail{Root=root.gameObject,Point=p,Limit=settlement.rank==0?300:100,TownScale=settlement.rank==0?.78f:.58f});
            }
            BuildLocalAtlasDetails();
            BuildGeographicRelief();
            selectionRoot=NewRoot("Selected region");hoverRoot=NewRoot("Hovered region");routeRoot=NewRoot("March route");armyRoot=NewRoot("Army standard");
            var pulseObject=new GameObject("Decision pulse");pulseObject.transform.SetParent(transform,false);
            pulseRing=pulseObject.AddComponent<LineRenderer>();pulseRing.sharedMaterial=goldMat;pulseRing.positionCount=48;pulseRing.loop=true;pulseRing.useWorldSpace=false;pulseRing.widthMultiplier=.1f;pulseRing.enabled=false;
            built=true;
        }

        private Mesh GeographicMesh(GeoMesh data,float height)
        {
            var vertices=new List<Vector3>(data.points.Length/2);
            for(int i=0;i<data.points.Length;i+=2)vertices.Add(AtlasProjection.Project(data.points[i],data.points[i+1],height));
            return NewMesh(vertices,new List<int>(data.triangles));
        }
        private Mesh Rectangle(float left,float bottom,float right,float top,float height)
        {
            return NewMesh(new List<Vector3>{AtlasProjection.Project(left,bottom,height),AtlasProjection.Project(left,top,height),AtlasProjection.Project(right,top,height),AtlasProjection.Project(right,bottom,height)},new List<int>{0,1,2,0,2,3});
        }
        private GameObject MeshObject(string name,Mesh mesh,Material material,Transform parent)
        {
            var go=new GameObject(name);go.transform.SetParent(parent,false);
            go.AddComponent<MeshFilter>().sharedMesh=mesh;
            var renderer=go.AddComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
            return go;
        }
        private static Vector2 DrawingPoint(Vector3 p)=>new Vector2(p.x*12+450,390-p.z*12);
        private static List<Vector3> GeographicPoints(float[] points,float height)
        {
            var result=new List<Vector3>();for(int i=0;i<points.Length;i+=2)result.Add(AtlasProjection.Project(points[i],points[i+1],height));return result;
        }
        private Mesh LinesMesh(GeoPath[] paths,float width,float height)
        {
            var v=new List<Vector3>();var t=new List<int>();
            foreach(var path in paths)
            {
                var p=GeographicPoints(path.points,height);
                if(p.Count<2)continue;
                bool closed=(p[0]-p[p.Count-1]).sqrMagnitude<.000001f;
                if(closed)p.RemoveAt(p.Count-1);
                if(p.Count<2)continue;
                int first=v.Count;
                for(int i=0;i<p.Count;i++)
                {
                    Vector3 before=p[i]-p[i>0?i-1:closed?p.Count-1:0];
                    Vector3 after=p[i< p.Count-1?i+1:closed?0:i]-p[i];
                    if(before.sqrMagnitude<.000001f)before=after;
                    if(after.sqrMagnitude<.000001f)after=before;
                    Vector3 a=new Vector3(-before.z,0,before.x).normalized,b=new Vector3(-after.z,0,after.x).normalized;
                    Vector3 normal=(a+b).normalized;
                    Vector3 side=normal*(width*.5f/Mathf.Max(.5f,Vector3.Dot(normal,b)));
                    v.Add(p[i]-side);v.Add(p[i]+side);
                }
                int segments=closed?p.Count:p.Count-1;
                for(int i=0;i<segments;i++)
                {
                    int a=first+i*2,b=first+((i+1)%p.Count)*2;
                    t.Add(a);t.Add(a+1);t.Add(b+1);t.Add(a);t.Add(b+1);t.Add(b);
                }
            }
            return NewMesh(v,t);
        }
        private void GeographicRegionBorder(string id,Transform root,Material material,float elevation)
        {
            if(!regionGeometry.TryGetValue(id,out var areas))return;
            if(!boundaryMeshes.TryGetValue(id,out var meshes))
            {
                meshes=new Mesh[3];
                float[] tolerance={.018f,.12f,.32f},width={.038f,.08f,.17f};
                for(int level=0;level<3;level++)
                {
                    var paths=new List<GeoPath>();
                    foreach(var area in areas)foreach(var path in area.paths)paths.Add(AtlasCartography.Simplify(path,tolerance[level]));
                    meshes[level]=LinesMesh(paths.ToArray(),width[level],0);
                }
                boundaryMeshes[id]=meshes;
            }
            var levels=new GameObject[3];
            for(int i=0;i<3;i++){levels[i]=MeshObject("Boundary ink LOD "+i,meshes[i],material,root);levels[i].transform.localPosition=Vector3.up*elevation;levels[i].SetActive(i==1);}
            boundaryLevels.Add(levels);
        }
        private bool OnFrenchLand(Vector3 point)
        {
            Vector2 geo=AtlasProjection.Geographic(point);
            foreach(var region in WorldData.regions)foreach(var area in region.areas)foreach(var path in area.paths)
            {
                bool inside=false;int count=path.points.Length/2;
                for(int i=0,j=count-1;i<count;j=i++)
                {
                    float ax=path.points[i*2],ay=path.points[i*2+1],bx=path.points[j*2],by=path.points[j*2+1];
                    if((ay>geo.y)!=(by>geo.y)&&geo.x<(bx-ax)*(geo.y-ay)/(by-ay)+ax)inside=!inside;
                }
                if(inside)return true;
            }
            return false;
        }
        private void UpdateGeographicLod()
        {
            if(Time.unscaledTime<lodTimer)return;lodTimer=Time.unscaledTime+.12f;
            var app=FindFirstObjectByType<GameApp>();if(!app||!app.StrategyCamera)return;
            float distance=app.StrategyCamera.Distance;Vector3 focus=app.StrategyCamera.FocusPoint;
            int level=distance>900?0:1;VisibleGeographicChunks=0;
            int inkLevel=distance<75?0:distance<280?1:2;
            for(int i=boundaryLevels.Count-1;i>=0;i--)
            {
                var levels=boundaryLevels[i];
                if(!levels[0]){boundaryLevels.RemoveAt(i);continue;}
                for(int j=0;j<levels.Length;j++)if(levels[j].activeSelf!=(j==inkLevel))levels[j].SetActive(j==inkLevel);
            }
            Plane[] planes=GeometryUtility.CalculateFrustumPlanes(atlasCamera);
            foreach(var chunk in geographicChunks)
            {
                bool show=chunk.Level==level&&GeometryUtility.TestPlanesAABB(planes,chunk.Bounds);
                if(chunk.Root.activeSelf!=show)chunk.Root.SetActive(show);if(show)VisibleGeographicChunks++;
            }
            foreach(var detail in localDetails)
            {
                bool show=distance>=2&&distance<detail.Limit&&(detail.Point-focus).sqrMagnitude<distance*distance*1.8f;
                if(detail.Root.activeSelf!=show)detail.Root.SetActive(show);
                if(show&&detail.TownScale>0)
                    detail.Root.transform.localScale=Vector3.one*Mathf.Max(30/(float)WorldPoint.MetresPerAtlasUnit,detail.TownScale*Mathf.Pow(Mathf.Min(1,distance/100),1.2f));
            }
            roadsRoot.gameObject.SetActive(distance>=2&&distance<300);
            foreach(var province in provinces)province.Value.gameObject.SetActive(distance<420);
            selectionRoot.gameObject.SetActive(distance>=2&&distance<420);hoverRoot.gameObject.SetActive(distance>=2&&distance<420);
            routeRoot.gameObject.SetActive(!continuousWorld&&distance<300);armyRoot.gameObject.SetActive(!continuousWorld&&lastTroops>0&&distance<600);
        }

        private void BuildLocalAtlasDetails()
        {
            foreach(var terrain in WorldData.terrain)
            {
                Vector3 center=AtlasProjection.Project(terrain.longitude,terrain.latitude,.14f);
                Transform root=NewRoot("Forest engraving:"+terrain.id);
                var drawing=new CityEngraving();
                for(int i=0;i<19;i++)
                {
                    float angle=i*2.39996f,r=Mathf.Sqrt(i/19f)*terrain.radius;
                    float x=Mathf.Cos(angle)*r,z=Mathf.Sin(angle)*r;
                    CanonicalTree(drawing,x,z,.24f+(i%4)*.045f,i%4);
                }
                var mesh=NewMesh(drawing.Vertices,drawing.Indices);mesh.SetColors(drawing.Colors);
                root.localPosition=center;MeshObject("Authored tree forms",mesh,detailMaterial,root);
                localDetails.Add(new LocalDetail{Root=root.gameObject,Point=center,Limit=130});
            }
            foreach(var town in WorldData.settlements)
            {
                Vector3 center=AtlasProjection.Project(town.longitude,town.latitude,.10f);
                Transform root=NewRoot("Fields:"+town.id);root.localPosition=center;
                var draw=new CityEngraving();
                for(int i=0;i<5;i++)
                {
                    float x=1.8f+i*.36f,z=-.5f+(i%2)*.25f;
                    draw.Shape(Hex(i%2==0?"#C8C49B":"#B7BF91"),x,z,x+.31f,z+.05f,x+.34f,z+.75f,x-.04f,z+.66f);
                    draw.Line(Hex("#AAA982"),.018f,x+.10f,z+.07f,x+.08f,z+.59f);
                }
                var mesh=NewMesh(draw.Vertices,draw.Indices);mesh.SetColors(draw.Colors);MeshObject("Small field strips",mesh,detailMaterial,root);
                localDetails.Add(new LocalDetail{Root=root.gameObject,Point=center,Limit=78});
            }
        }
        private void BuildGeographicRelief()
        {
            var asset=Resources.Load<TextAsset>("World/relief");if(!asset)return;
            var relief=JsonUtility.FromJson<AtlasRelief>(asset.text);
            foreach(var area in relief.features)
            {
                var drawing=new CityEngraving();
                Vector3 center=AtlasProjection.Project((area.bounds[0]+area.bounds[2])*.5f,(area.bounds[1]+area.bounds[3])*.5f);
                for(int i=0;i<area.points.Length;i+=2)
                {
                    drawing.BeginGlyph();
                    Vector3 p=AtlasProjection.Project(area.points[i],area.points[i+1])-center;
                    float size=.48f+(i%6)*.038f;
                    Color shade=Hex("#9DA680"),lit=Hex("#DAD3AF"),ridge=Hex("#77866E");
                    drawing.Shape(shade,p.x-size,p.z,p.x-size*.07f,p.z+size*1.3f,p.x+size*.93f,p.z+size*.08f);
                    drawing.Shape(lit,p.x-size,p.z,p.x-size*.07f,p.z+size*1.3f,p.x+size*.07f,p.z+size*.40f);
                    drawing.Line(ridge,.035f,p.x-size,p.z,p.x-size*.07f,p.z+size*1.3f,p.x+size*.93f,p.z+size*.08f);
                }
                var mesh=NewMesh(drawing.Vertices,drawing.Indices);mesh.SetColors(drawing.Colors);
                var root=NewRoot("Geographic relief: "+area.name);root.localPosition=center+Vector3.up*.11f;
                MeshObject(area.name,mesh,detailMaterial,root);
                localDetails.Add(new LocalDetail{Root=root.gameObject,Point=center,Limit=400});
            }
        }
        private static void CanonicalTree(CityEngraving d,float x,float z,float size,int variant)
        {
            d.BeginGlyph();
            Color shadow=Hex("#7F9E80"),leaf=Hex("#4F7361"),lit=Hex("#A9BA88"),trunk=Hex("#B79D71");
            d.Shape(shadow,x-size*.7f,z-size*.18f,x+size*.85f,z-size*.13f,x+size*.6f,z+size*.3f,x-size*.55f,z+size*.25f);
            d.Line(trunk,size*.12f,x,z,x+size*.05f,z+size*.75f);
            d.Shape(leaf,x-size*.65f,z+size*.25f,x-size*.72f,z+size*.64f,x-size*.38f,z+size*.83f,x-size*.1f,z+size*1.2f,x+size*.32f,z+size*1.13f,x+size*.7f,z+size*.73f,x+size*.59f,z+size*.35f,x+size*.15f,z+size*.21f);
            d.Shape(lit,x-size*.45f,z+size*.56f,x-size*.28f,z+size*.91f,x+size*.1f,z+size*(1.05f+variant*.025f),x+size*.37f,z+size*.69f,x+size*.15f,z+size*.44f);
            d.Line(shadow,size*.055f,x,z+size*.33f,x-size*.15f,z+size*.64f);
        }
    }
}
