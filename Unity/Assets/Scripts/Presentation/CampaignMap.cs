using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PowerAboveAll
{
    /// <summary>A relief atlas built locally from polygon data; no downloaded artwork or SVG runtime.</summary>
    public sealed class CampaignMap : MonoBehaviour
    {
        private sealed class Seed
        {
            public string Id; public Vector2 Point; public Color Ink;
            public Seed(string id, float x, float y, string color) { Id = id; Point = new Vector2(x, y); ColorUtility.TryParseHtmlString(color, out Ink); }
        }

        private static readonly Seed[] Seeds = {
            new Seed("brittany",185,288,"#ACAA83"), new Seed("normandy",323,240,"#9EA88B"),
            new Seed("picardy",449,173,"#BBB187"), new Seed("ile",439,267,"#C7B98A"),
            new Seed("champagne",534,252,"#BCA18A"), new Seed("lorraine",618,254,"#B1AA8A"),
            new Seed("burgundy",548,367,"#ABA780"), new Seed("orleans",391,353,"#B3B992"),
            new Seed("poitou",304,409,"#B5B080"), new Seed("guyenne",349,511,"#9DA991"),
            new Seed("languedoc",474,543,"#BAB089"), new Seed("provence",588,517,"#C3B291")
        };
        private static readonly float[] Coast = {
            432,89,465,98,479,119,506,120,524,145,547,145,573,172,604,174,631,200,668,196,
            679,222,668,257,650,290,654,327,628,348,624,377,646,399,630,422,645,449,637,474,
            615,489,629,521,653,542,634,563,603,573,579,565,551,582,518,577,486,590,468,611,
            448,626,420,620,392,609,358,605,329,587,300,580,285,552,289,518,301,479,287,466,
            295,448,278,422,283,402,262,383,244,362,211,352,187,334,166,322,130,318,97,300,
            105,288,88,278,95,265,82,253,105,243,137,245,160,253,190,253,216,271,243,269,
            262,249,256,225,245,202,248,177,269,181,283,216,307,219,337,202,359,185,394,176,
            400,153,416,144,420,112
        };
        private readonly Dictionary<string, MeshRenderer> provinces = new Dictionary<string, MeshRenderer>();
        private readonly Dictionary<string, List<Vector2>> cells = new Dictionary<string, List<Vector2>>();
        private readonly Dictionary<string, Color> provinceColors = new Dictionary<string, Color>();
        private readonly List<UnityEngine.Object> owned = new List<UnityEngine.Object>();
        private List<Vector2> mainland;
        private Camera atlasCamera;
        private Transform selectionRoot, routeRoot, armyRoot;
        private Material borderMat, goldMat, roadMat;
        private string selectedId, armyId;
        private int lastWeek = -1, lastMoves = -1;
        private Vector3 armyFrom, armyTarget;
        private float armyMoveStarted;
        private bool armyPositioned;
        private string pulseId;
        private float pulseStarted = -10f;
        private LineRenderer pulseRing;
        private bool built;

        public void Build(Camera camera)
        {
            if (built) return;
            built = true; atlasCamera = camera;
            camera.orthographic = true;
            camera.orthographicSize = 33.5f;
            camera.backgroundColor = Hex("#A7BAB0");
            mainland = new List<Vector2>();
            for (int i = 0; i < Coast.Length; i += 2) mainland.Add(new Vector2(Coast[i], Coast[i + 1]));
            borderMat = MakeMaterial(Hex("#707B61"));
            goldMat = MakeMaterial(Hex("#E0CB84"));
            roadMat = MakeMaterial(new Color(.39f, .43f, .30f, 1f));
            MakeFlat("Atlas sea", new List<Vector2> { new Vector2(-700,-600),new Vector2(1500,-600),new Vector2(1500,1600),new Vector2(-700,1600) }, Hex("#A7BAB0"), -.22f);
            AddSurroundings();
            var triangles = Triangulate(mainland);
            foreach (Seed seed in Seeds)
            {
                var cell = new List<Vector2> { new Vector2(-100,-100),new Vector2(1000,-100),new Vector2(1000,900),new Vector2(-100,900) };
                foreach (Seed other in Seeds)
                {
                    if (other == seed) continue;
                    Vector2 normal = other.Point - seed.Point;
                    float limit = (other.Point.sqrMagnitude - seed.Point.sqrMagnitude) * .5f;
                    cell = Clip(cell, normal, limit);
                }
                cells[seed.Id] = cell;
                var vertices = new List<Vector3>(); var indices = new List<int>();
                for (int t = 0; t < triangles.Count; t += 3)
                {
                    var fragment = new List<Vector2> { mainland[triangles[t]], mainland[triangles[t + 1]], mainland[triangles[t + 2]] };
                    foreach (Seed other in Seeds)
                    {
                        if (other == seed) continue;
                        fragment = Clip(fragment, other.Point - seed.Point, (other.Point.sqrMagnitude - seed.Point.sqrMagnitude) * .5f);
                        if (fragment.Count == 0) break;
                    }
                    AddFan(fragment, vertices, indices, .02f);
                }
                Mesh mesh = NewMesh(vertices, indices);
                var go = new GameObject("Province:" + seed.Id); go.transform.SetParent(transform, false);
                go.AddComponent<MeshFilter>().sharedMesh = mesh;
                var renderer = go.AddComponent<MeshRenderer>(); renderer.sharedMaterial = MakeMaterial(seed.Ink);
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                go.AddComponent<MeshCollider>().sharedMesh = mesh;
                provinces.Add(seed.Id, renderer);
                provinceColors.Add(seed.Id, seed.Ink);
                BorderOfCell(cell, transform, borderMat, .07f, .05f);
                MakeCity(seed);
            }
            DrawLine("Coastline", mainland, true, transform, borderMat, .14f, .07f);
            AddEngraving();
            selectionRoot = NewRoot("Selected province"); routeRoot = NewRoot("Dispatch routes"); armyRoot = NewRoot("Army standard");
            var pulseObject = new GameObject("Order wax imprint");pulseObject.transform.SetParent(transform,false);
            pulseRing=pulseObject.AddComponent<LineRenderer>();pulseRing.sharedMaterial=goldMat;pulseRing.positionCount=48;pulseRing.loop=true;pulseRing.useWorldSpace=false;pulseRing.widthMultiplier=.1f;pulseRing.enabled=false;
        }

        public void Refresh(CampaignState state, string mode)
        {
            if (!built || state == null) return;
            foreach (Seed seed in Seeds)
            {
                var region = CampaignCore.Region(state, seed.Id);
                float value = 0; Color low = Hex("#A76855"), high = Hex("#A5B68A");
                switch (mode)
                {
                    case "unrest": value = 1f - region.Unrest / 100f; break;
                    case "control": value = region.Control / 100f; break;
                    case "influence": value = region.EliteLoyalty / 100f; break;
                    case "army": value = seed.Id == state.ArmyRegionId ? 1f : .12f; low = Hex("#CEC5A6"); high = Hex("#738F8B"); break;
                    case "food": value = BaseFood(seed.Id) * (1f - region.Unrest / 200f) / 22f; low = Hex("#CEB17A"); high = Hex("#8EA675"); break;
                    case "tax": value = BaseTax(seed.Id) * (1f - region.Unrest / 150f) * (.5f + region.Control / 200f) * (.75f + state.Factions.Find(f => f.Id == "assembly").Approval / 200f) / 48f; low = Hex("#CAC1A0"); high = Hex("#B29356"); break;
                    default: provinces[seed.Id].sharedMaterial.color = seed.Ink; provinceColors[seed.Id] = seed.Ink; continue;
                }
                provinceColors[seed.Id] = Color.Lerp(low, high, Mathf.Clamp01(value));
                provinces[seed.Id].sharedMaterial.color = provinceColors[seed.Id];
            }
            bool selectionChanged = selectedId != state.SelectedRegionId;
            bool armyChanged = armyId != state.ArmyRegionId;
            if (selectionChanged || armyChanged || lastWeek != state.Week || lastMoves != state.Moves)
            {
                selectedId = state.SelectedRegionId; armyId = state.ArmyRegionId;
                lastWeek = state.Week; lastMoves = state.Moves;
                ClearChildren(routeRoot);
                if (selectionChanged)
                {
                    ClearChildren(selectionRoot);
                    if (cells.TryGetValue(selectedId ?? "", out var selectedCell)) BorderOfCell(selectedCell, selectionRoot, goldMat, .19f, .17f);
                }
                if (!armyPositioned) { MakeArmy(state.ArmyRegionId); armyTarget = armyFrom = armyRoot.localPosition; armyPositioned = true; armyMoveStarted = Time.unscaledTime - 1f; }
                else if (armyChanged)
                {
                    armyFrom = armyRoot.localPosition;
                    armyTarget = transform.InverseTransformPoint(RegionWorld(armyId)) + new Vector3(1.35f, 0, .85f);
                    armyMoveStarted = Time.unscaledTime;
                }
                if (selectedId != armyId && selectedId != null && armyId != null)
                {
                    var check = CampaignCore.CanMarch(state, selectedId);
                    if (check.Ok) DrawRoute(RegionWorld(armyId), RegionWorld(selectedId));
                }
            }
        }

        private void Update()
        {
            if (!built) return;
            // Presentation only: the authoritative campaign transition has already completed.
            if (armyPositioned)
            {
                float t = Mathf.Clamp01((Time.unscaledTime - armyMoveStarted) / .85f);
                float smooth = t * t * (3f - 2f * t);
                armyRoot.localPosition = Vector3.Lerp(armyFrom, armyTarget, smooth) + Vector3.up * (Mathf.Sin(t * Mathf.PI) * .28f);
            }
            foreach (var province in provinces)
            {
                var p = province.Value.transform.localPosition;
                p.y = Mathf.MoveTowards(p.y, province.Key == selectedId ? .085f : 0f, Time.unscaledDeltaTime * .45f);
                province.Value.transform.localPosition = p;
                float pulse = province.Key == pulseId ? Mathf.Clamp01(1f - (Time.unscaledTime - pulseStarted) / .95f) : 0f;
                province.Value.sharedMaterial.color = Color.Lerp(provinceColors[province.Key], Hex("#E3D69F"), pulse * .6f);
            }
            float elapsed=Time.unscaledTime-pulseStarted;
            if(pulseRing&&pulseRing.enabled)
            {
                if(elapsed>=.95f)pulseRing.enabled=false;
                else
                {
                    Vector3 center=transform.InverseTransformPoint(RegionWorld(pulseId));float radius=.9f+elapsed*2.3f;
                    for(int i=0;i<48;i++){float angle=i*Mathf.PI*2f/48;pulseRing.SetPosition(i,center+new Vector3(Mathf.Cos(angle)*radius,.12f,Mathf.Sin(angle)*radius));}
                    pulseRing.widthMultiplier=.1f*(1f-elapsed/.95f);
                }
            }
        }

        public void Pulse(string id)
        {
            if(!built||string.IsNullOrEmpty(id)||!provinces.ContainsKey(id))return;
            pulseId=id;pulseStarted=Time.unscaledTime;pulseRing.enabled=true;
        }

        public Vector3 RegionWorld(string id)
        {
            foreach (Seed seed in Seeds) if (seed.Id == id) return transform.TransformPoint(World(seed.Point, .4f));
            return transform.position;
        }

        public string Pick(Vector3 screenPosition)
        {
            if (!built || !gameObject.activeInHierarchy || !atlasCamera.pixelRect.Contains(screenPosition)) return null;
            var ray = atlasCamera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out var hit, 500f) && hit.collider.name.StartsWith("Province:", StringComparison.Ordinal)) return hit.collider.name.Substring(9);
            var plane = new Plane(transform.up, transform.position);
            if (!plane.Raycast(ray, out var distance)) return null;
            Vector3 local = transform.InverseTransformPoint(ray.GetPoint(distance));
            Vector2 point = new Vector2(local.x * 12f + 450f, 390f - local.z * 12f);
            if (!Inside(point, mainland)) return null;
            string nearest = null; float best = float.PositiveInfinity;
            foreach (Seed seed in Seeds) { float d = (point - seed.Point).sqrMagnitude; if (d < best) { best = d; nearest = seed.Id; } }
            return nearest;
        }

        public void SetVisible(bool visible) { gameObject.SetActive(visible); }

        private static float BaseFood(string id) { foreach (var d in CampaignCore.Regions) if (d.Id == id) return d.BaseFood; return 0; }
        private static float BaseTax(string id) { foreach (var d in CampaignCore.Regions) if (d.Id == id) return d.BaseTax; return 0; }
        private Transform NewRoot(string name) { var go = new GameObject(name); go.transform.SetParent(transform, false); return go.transform; }
        private static void ClearChildren(Transform root) { for (int i = root.childCount - 1; i >= 0; --i) { var go = root.GetChild(i).gameObject; go.SetActive(false); UnityEngine.Object.Destroy(go); } }
        private static Color Hex(string value) { ColorUtility.TryParseHtmlString(value, out var color); return color; }
        private Material MakeMaterial(Color color) { var shader = Shader.Find("Unlit/Color"); if (!shader) shader = Shader.Find("Sprites/Default"); var material = new Material(shader) { color = color }; owned.Add(material); return material; }
        private static Vector3 World(Vector2 p, float elevation) { return new Vector3((p.x - 450f) / 12f, elevation, (390f - p.y) / 12f); }

        private void MakeFlat(string name, List<Vector2> points, Color color, float height)
        {
            var vertices = new List<Vector3>(); foreach (var p in points) vertices.Add(World(p, height));
            var go = new GameObject(name); go.transform.SetParent(transform, false);
            go.AddComponent<MeshFilter>().sharedMesh = NewMesh(vertices, Triangulate(points));
            var renderer = go.AddComponent<MeshRenderer>(); renderer.sharedMaterial = MakeMaterial(color); renderer.shadowCastingMode = ShadowCastingMode.Off;
        }
        private Mesh NewMesh(List<Vector3> vertices, List<int> indices)
        {
            var mesh = new Mesh { name = "Atlas polygon" }; mesh.SetVertices(vertices); mesh.SetTriangles(indices, 0); mesh.RecalculateNormals(); mesh.RecalculateBounds(); owned.Add(mesh); return mesh;
        }
        private static void AddFan(List<Vector2> polygon, List<Vector3> vertices, List<int> indices, float height)
        {
            if (polygon.Count < 3) return;
            int start = vertices.Count; foreach (var p in polygon) vertices.Add(World(p, height));
            for (int i = 1; i < polygon.Count - 1; i++) { indices.Add(start); indices.Add(start + i); indices.Add(start + i + 1); }
        }
        private static List<Vector2> Clip(List<Vector2> polygon, Vector2 normal, float limit)
        {
            var output = new List<Vector2>(); if (polygon.Count == 0) return output;
            Vector2 a = polygon[polygon.Count - 1]; float da = Vector2.Dot(a, normal) - limit;
            foreach (Vector2 b in polygon)
            {
                float db = Vector2.Dot(b, normal) - limit;
                if ((da <= .001f) != (db <= .001f)) { float divisor = da - db; if (Mathf.Abs(divisor) > .000001f) output.Add(Vector2.LerpUnclamped(a, b, da / divisor)); }
                if (db <= .001f) output.Add(b); a = b; da = db;
            }
            return output;
        }
        private static float Cross(Vector2 a, Vector2 b) { return a.x * b.y - a.y * b.x; }
        private static List<int> Triangulate(List<Vector2> polygon)
        {
            var result = new List<int>(); var remaining = new List<int>(); float area = 0;
            for (int i = 0; i < polygon.Count; i++) area += Cross(polygon[i], polygon[(i + 1) % polygon.Count]);
            for (int i = 0; i < polygon.Count; i++) remaining.Add(area >= 0 ? i : polygon.Count - 1 - i);
            int guard = polygon.Count * polygon.Count;
            while (remaining.Count > 3 && guard-- > 0)
            {
                bool found = false;
                for (int i = 0; i < remaining.Count; i++)
                {
                    int ia = remaining[(i + remaining.Count - 1) % remaining.Count], ib = remaining[i], ic = remaining[(i + 1) % remaining.Count];
                    Vector2 a = polygon[ia], b = polygon[ib], c = polygon[ic];
                    if (Cross(b - a, c - b) <= .0001f) continue;
                    bool occupied = false;
                    for (int j = 0; j < remaining.Count; j++)
                    {
                        int k = remaining[j]; if (k == ia || k == ib || k == ic) continue; Vector2 p = polygon[k];
                        if (Cross(b - a, p - a) >= -.0001f && Cross(c - b, p - b) >= -.0001f && Cross(a - c, p - c) >= -.0001f) { occupied = true; break; }
                    }
                    if (occupied) continue;
                    result.Add(ia); result.Add(ib); result.Add(ic); remaining.RemoveAt(i); found = true; break;
                }
                if (!found) throw new InvalidOperationException("Atlas polygon could not be triangulated.");
            }
            if (remaining.Count == 3) { result.Add(remaining[0]); result.Add(remaining[1]); result.Add(remaining[2]); }
            return result;
        }
        private static bool Inside(Vector2 p, List<Vector2> polygon)
        {
            bool inside = false; for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
                if ((polygon[i].y > p.y) != (polygon[j].y > p.y) && p.x < (polygon[j].x - polygon[i].x) * (p.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x) inside = !inside;
            return inside;
        }
        private void BorderOfCell(List<Vector2> cell, Transform parent, Material material, float width, float elevation)
        {
            for (int i = 0; i < cell.Count; i++)
            {
                Vector2 a = cell[i], b = cell[(i + 1) % cell.Count], delta = b - a; var cuts = new List<float> { 0f, 1f };
                for (int j = 0; j < mainland.Count; j++)
                {
                    Vector2 c = mainland[j], edge = mainland[(j + 1) % mainland.Count] - c; float divisor = Cross(delta, edge);
                    if (Mathf.Abs(divisor) < .00001f) continue;
                    float t = Cross(c - a, edge) / divisor, u = Cross(c - a, delta) / divisor;
                    if (t > 0 && t < 1 && u >= 0 && u <= 1) cuts.Add(t);
                }
                cuts.Sort();
                for (int c = 0; c < cuts.Count - 1; c++)
                {
                    if (cuts[c + 1] - cuts[c] < .00001f || !Inside(a + delta * ((cuts[c] + cuts[c + 1]) * .5f), mainland)) continue;
                    DrawLine("Province boundary", new List<Vector2> { a + delta * cuts[c], a + delta * cuts[c + 1] }, false, parent, material, width, elevation);
                }
            }
        }
        private void DrawLine(string name, List<Vector2> points, bool loop, Transform parent, Material material, float width, float elevation)
        {
            var go = new GameObject(name); go.transform.SetParent(parent, false); var line = go.AddComponent<LineRenderer>();
            line.useWorldSpace = false; line.sharedMaterial = material; line.widthMultiplier = width; line.positionCount = points.Count; line.loop = loop;
            line.alignment = LineAlignment.View; line.numCornerVertices = 2; line.numCapVertices = 1;
            for (int i = 0; i < points.Count; i++) line.SetPosition(i, World(points[i], elevation));
            line.shadowCastingMode = ShadowCastingMode.Off; line.receiveShadows = false;
        }
        private void AddSurroundings()
        {
            MakeFlat("Northern neighbours", new List<Vector2> { new Vector2(432,89),new Vector2(490,-100),new Vector2(1000,-100),new Vector2(1000,670),new Vector2(760,635),new Vector2(650,553),new Vector2(632,447),new Vector2(641,340),new Vector2(678,219),new Vector2(626,191),new Vector2(550,149),new Vector2(480,120) }, Hex("#D1CFB1"), -.05f);
            MakeFlat("Iberian margin", new List<Vector2> { new Vector2(290,556),new Vector2(380,608),new Vector2(446,625),new Vector2(518,577),new Vector2(540,610),new Vector2(454,680),new Vector2(295,752),new Vector2(-90,810),new Vector2(-90,728),new Vector2(200,643) }, Hex("#CBCBA9"), -.05f);
            MakeFlat("England coast", new List<Vector2> { new Vector2(-100,-100),new Vector2(480,-100),new Vector2(420,25),new Vector2(350,58),new Vector2(270,66),new Vector2(198,83),new Vector2(104,91),new Vector2(-100,135) }, Hex("#D2D1B5"), -.05f);
            MakeFlat("Corsica inset", new List<Vector2> {new Vector2(776,542),new Vector2(786,566),new Vector2(798,584),new Vector2(788,614),new Vector2(776,638),new Vector2(763,628),new Vector2(755,605),new Vector2(763,573)}, Hex("#BDB790"), .02f);
        }
        private void MakeCity(Seed seed)
        {
            Transform city = NewRoot("Miniature:" + seed.Id); Vector3 center = World(seed.Point, .14f); city.localPosition = center;
            Material ivory = MakeMaterial(Hex("#E1D9BB")), roof = MakeMaterial(Hex("#697468")), stone = MakeMaterial(Hex("#929679"));
            int count = seed.Id == "ile" ? 7 : 4;
            for (int i = 0; i < count; i++)
            {
                float angle = i * 2.39996f, radius = .35f + (i % 3) * .18f;
                Vector3 position = new Vector3(Mathf.Cos(angle) * radius, .12f, Mathf.Sin(angle) * radius);
                Block(city, position, new Vector3(.34f,.24f,.45f), ivory, "House");
                Pyramid(city, position + Vector3.up * .12f, .26f, .22f, roof);
            }
            Block(city, new Vector3(0,.35f,0), new Vector3(.35f,.7f,.35f), stone, "Bell tower"); Pyramid(city, new Vector3(0,.7f,0), .28f, .4f, roof);
            if (seed.Id == "ile") { Block(city,new Vector3(.4f,.25f,.85f),new Vector3(1.3f,.4f,.3f),ivory,"Palace"); Block(city,new Vector3(.4f,.47f,.85f),new Vector3(1.35f,.09f,.39f),roof,"Palace cornice"); }
            if(seed.Id=="orleans"||seed.Id=="poitou"||seed.Id=="picardy")
            {
                // Engraved strips of cultivated land distinguish the grain provinces.
                var field=MakeMaterial(Hex("#928A59"));
                for(int i=0;i<4;i++)Block(city,new Vector3(-1.25f+i*.22f,.015f,1.2f),new Vector3(.09f,.035f,1.05f),field,"Cultivated strip");
            }
            if(seed.Id=="guyenne"||seed.Id=="provence")
            {
                Block(city,new Vector3(1.45f,.06f,.15f),new Vector3(.19f,.09f,1.3f),stone,"Trading quay");
                for(int i=0;i<3;i++)Block(city,new Vector3(1.7f,.07f,-.3f+i*.4f),new Vector3(.65f,.08f,.11f),roof,"Harbour pier");
            }
            if(seed.Id=="lorraine"||seed.Id=="champagne")
            {
                // Frontier towns carry a heavier, square enclosure.
                Block(city,new Vector3(-.95f,.14f,.2f),new Vector3(.13f,.28f,1.75f),stone,"Western rampart");
                Block(city,new Vector3(.02f,.14f,-.75f),new Vector3(2f,.28f,.13f),stone,"Northern rampart");
            }
        }
        private void Block(Transform parent, Vector3 position, Vector3 scale, Material material, string name)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube); go.name = name; go.transform.SetParent(parent, false); go.transform.localPosition = position; go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = material; var collider = go.GetComponent<Collider>(); if (collider) { collider.enabled = false; Destroy(collider); }
        }
        private void Pyramid(Transform parent, Vector3 position, float radius, float height, Material material)
        {
            var vertices = new List<Vector3> { new Vector3(-radius,0,-radius),new Vector3(radius,0,-radius),new Vector3(radius,0,radius),new Vector3(-radius,0,radius),new Vector3(0,height,0) };
            var go = new GameObject("Roof"); go.transform.SetParent(parent,false); go.transform.localPosition = position;
            go.AddComponent<MeshFilter>().sharedMesh = NewMesh(vertices,new List<int> {0,4,1,1,4,2,2,4,3,3,4,0}); go.AddComponent<MeshRenderer>().sharedMaterial=material;
        }
        private void AddEngraving()
        {
            var river = MakeMaterial(Hex("#849F9D"));
            DrawLine("Seine",Points(540,332,511,297,477,280,450,250,398,232,366,225,332,233,301,219),false,transform,river,.12f,.1f);
            DrawLine("Loire",Points(532,425,512,402,483,406,456,390,440,365,417,342,382,344,341,337,310,332,275,328,224,347),false,transform,river,.13f,.1f);
            DrawLine("Rhone",Points(601,365,587,399,588,435,579,472,576,493,588,554),false,transform,river,.14f,.1f);
            DrawLine("Garonne",Points(435,569,408,538,380,525,344,503,319,489,301,459),false,transform,river,.11f,.1f);
            var mountain = MakeMaterial(Hex("#A4A181"));
            for(int i=0;i<18;i++)
            {
                Vector2 p = i<10 ? new Vector2(601+(i%3)*9,386+i*10) : new Vector2(338+(i-10)*19,582+(i%2)*9);
                if(!Inside(p,mainland))continue;
                DrawLine("Relief hatching",Points(p.x-5,p.y+6,p.x,p.y-7,p.x+6,p.y+6),false,transform,mountain,.055f,.12f);
                DrawLine("Relief shadow",Points(p.x,p.y-7,p.x+1,p.y+5),false,transform,mountain,.05f,.12f);
            }
            for(int i=0;i<17;i++)
            {
                float z=-28+i*3.6f;
                DrawLine("Sea engraving",Points(10,z*12+390,23,z*12+390,35,z*12+389),false,transform,MakeMaterial(Hex("#98AFA4")),.04f,-.1f);
            }
        }
        private static List<Vector2> Points(params float[] values) { var p=new List<Vector2>();for(int i=0;i<values.Length;i+=2)p.Add(new Vector2(values[i],values[i+1]));return p; }
        private void MakeArmy(string id)
        {
            if(string.IsNullOrEmpty(id))return;
            armyRoot.localPosition=transform.InverseTransformPoint(RegionWorld(id))+new Vector3(1.35f,0,.85f);
            Block(armyRoot,new Vector3(0,.32f,0),new Vector3(.1f,.7f,.1f),goldMat,"Standard pole");
            Block(armyRoot,new Vector3(.46f,.72f,0),new Vector3(.87f,.08f,1.06f),MakeMaterial(Hex("#314F55")),"Royal standard");
            Block(armyRoot,new Vector3(.46f,.77f,0),new Vector3(.07f,.04f,.69f),goldMat,"Standard embroidery");
            Block(armyRoot,new Vector3(.46f,.78f,0),new Vector3(.58f,.04f,.07f),goldMat,"Standard embroidery");
        }
        private void DrawRoute(Vector3 from,Vector3 to)
        {
            from=transform.InverseTransformPoint(from);to=transform.InverseTransformPoint(to);
            Vector3 middle=(from+to)*.5f; middle.x+=1.2f;middle.z+=.6f;
            for(int i=0;i<18;i++)
            {
                float t=i/18f,u=(i+.55f)/18f;Vector3 a=(1-t)*(1-t)*from+2*(1-t)*t*middle+t*t*to;Vector3 b=(1-u)*(1-u)*from+2*(1-u)*u*middle+u*u*to;
                DrawLine("March ribbon",Points(a.x*12+450,390-a.z*12,b.x*12+450,390-b.z*12),false,routeRoot,roadMat,.13f,.28f);
            }
        }
        private void OnDestroy() { foreach (var item in owned) if(item)Destroy(item); owned.Clear(); }
    }
}
