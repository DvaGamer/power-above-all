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

        // Perspektif şehir gravürü tek renkli-vertex mesh'te toplanır; yeni collider üretmez.
        private sealed class CityEngraving
        {
            public readonly List<Vector3> Vertices = new List<Vector3>();
            public readonly List<int> Indices = new List<int>();
            public readonly List<Color> Colors = new List<Color>();
            public void Shape(Color color, params float[] coordinates)
            {
                int start = Vertices.Count, count = coordinates.Length / 2;
                for (int i = 0; i < coordinates.Length; i += 2)
                {
                    Vertices.Add(new Vector3(coordinates[i], .12f, coordinates[i + 1]));
                    Colors.Add(color);
                }
                for (int i = 1; i < count - 1; i++)
                { Indices.Add(start); Indices.Add(start + i); Indices.Add(start + i + 1); }
            }
            public void Line(Color color, float width, params float[] coordinates)
            {
                for (int i = 0; i < coordinates.Length - 2; i += 2)
                {
                    Vector2 a = new Vector2(coordinates[i], coordinates[i + 1]);
                    Vector2 b = new Vector2(coordinates[i + 2], coordinates[i + 3]);
                    Vector2 normal = new Vector2(a.y - b.y, b.x - a.x).normalized * (width * .5f);
                    Shape(color, a.x + normal.x, a.y + normal.y, b.x + normal.x, b.y + normal.y,
                        b.x - normal.x, b.y - normal.y, a.x - normal.x, a.y - normal.y);
                }
            }
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
        private readonly Dictionary<string, Color> displayedColors = new Dictionary<string, Color>();
        private readonly List<UnityEngine.Object> owned = new List<UnityEngine.Object>();
        private List<Vector2> mainland;
        private Camera atlasCamera;
        private Transform selectionRoot, hoverRoot, routeRoot, armyRoot, armyCloth;
        private Material borderMat, goldMat, roadMat, hoverMat, cityInkMat;
        private Texture2D paperGrain;
        private string selectedId, hoveredId, armyId;
        private int lastWeek = -1, lastMoves = -1;
        private Vector3 armyFrom, armyControl, armyTarget;
        private float armyMoveStarted;
        private bool armyPositioned;
        private const float MarchDuration = .85f;
        private static readonly Vector3 ArmyOffset = new Vector3(1.35f, 0, .85f);
        private static readonly Color SelectionInk = new Color(.953f, .906f, .792f);
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
            camera.backgroundColor = Hex("#83B0B6");
            mainland = new List<Vector2>();
            for (int i = 0; i < Coast.Length; i += 2) mainland.Add(new Vector2(Coast[i], Coast[i + 1]));
            mainland = SoftenCoast(mainland);
            paperGrain = MakePaperGrain();
            borderMat = MakeMaterial(Hex("#677960"));
            goldMat = MakeMaterial(Hex("#CAB36F"));
            hoverMat = MakeMaterial(Hex("#DCCE9F"));
            roadMat = MakeMaterial(Hex("#536C57"));
            cityInkMat = MakeAtlasMaterial(Color.white);
            cityInkMat.mainTexture = Texture2D.whiteTexture;
            MakeFlat("Atlas sea", new List<Vector2> { new Vector2(-700,-600),new Vector2(1500,-600),new Vector2(1500,1600),new Vector2(-700,1600) }, Hex("#83B0B6"), -.22f);
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
                var renderer = go.AddComponent<MeshRenderer>(); renderer.sharedMaterial = MakeAtlasMaterial(seed.Ink);
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                go.AddComponent<MeshCollider>().sharedMesh = mesh;
                provinces.Add(seed.Id, renderer);
                provinceColors.Add(seed.Id, seed.Ink);
                displayedColors.Add(seed.Id, seed.Ink);
                BorderOfCell(cell, transform, borderMat, .045f, .12f, false);
                MakeCity(seed);
            }
            AddCoastalEngraving();
            DrawLine("Coastline", mainland, true, transform, borderMat, .095f, .14f);
            AddEngraving();
            selectionRoot = NewRoot("Selected province"); hoverRoot = NewRoot("Province under pointer");
            routeRoot = NewRoot("Dispatch routes"); armyRoot = NewRoot("Army standard");
            var pulseObject = new GameObject("Order wax imprint");pulseObject.transform.SetParent(transform,false);
            pulseRing=pulseObject.AddComponent<LineRenderer>();pulseRing.sharedMaterial=goldMat;pulseRing.positionCount=48;pulseRing.loop=true;pulseRing.useWorldSpace=false;pulseRing.widthMultiplier=.1f;pulseRing.enabled=false;
        }

        public void Refresh(CampaignState state, string mode)
        {
            if (!built || state == null) return;
            foreach (Seed seed in Seeds)
            {
                var region = CampaignCore.Region(state, seed.Id);
                float value = 0;
                switch (mode)
                {
                    case "unrest": value = 1f - region.Unrest / 100f; break;
                    case "control": value = region.Control / 100f; break;
                    case "influence": value = region.EliteLoyalty / 100f; break;
                    case "army": value = seed.Id == state.ArmyRegionId ? 1f : .12f; break;
                    case "food": value = BaseFood(seed.Id) * (1f - region.Unrest / 200f) / 22f; break;
                    case "tax": value = BaseTax(seed.Id) * (1f - region.Unrest / 150f) * (.5f + region.Control / 200f) * (.75f + state.Factions.Find(f => f.Id == "assembly").Approval / 200f) / 48f; break;
                    default: provinceColors[seed.Id] = seed.Ink; continue;
                }
                provinceColors[seed.Id] = ModeColor(mode,value);
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
                    if (cells.TryGetValue(selectedId ?? "", out var selectedCell))
                    {
                        BorderOfCell(selectedCell, selectionRoot, borderMat, .20f, .19f);
                        BorderOfCell(selectedCell, selectionRoot, goldMat, .09f, .21f);
                    }
                    RebuildHover();
                }
                if (!armyPositioned)
                {
                    if (armyRoot.childCount == 0) MakeArmy(state.ArmyRegionId);
                    armyRoot.localPosition = ArmyPosition(state.ArmyRegionId);
                    armyTarget = armyFrom = armyRoot.localPosition;
                    armyControl = armyTarget;
                    armyPositioned = true; armyMoveStarted = Time.unscaledTime - MarchDuration;
                }
                else if (armyChanged)
                {
                    armyFrom = armyRoot.localPosition;
                    armyTarget = ArmyPosition(armyId);
                    armyControl = RouteControl(armyFrom, armyTarget);
                    armyMoveStarted = Time.unscaledTime;
                }
                if (selectedId != armyId && selectedId != null && armyId != null)
                {
                    var check = CampaignCore.CanMarch(state, selectedId);
                    if (check.Ok) DrawRoute(ArmyPosition(armyId), ArmyPosition(selectedId));
                }
            }
        }

        private void Update()
        {
            if (!built) return;
            // Presentation only: the authoritative campaign transition has already completed.
            if (armyPositioned)
            {
                float t = Mathf.Clamp01((Time.unscaledTime - armyMoveStarted) / MarchDuration);
                float smooth = t * t * (3f - 2f * t);
                armyRoot.localPosition = RoutePoint(armyFrom, armyControl, armyTarget, smooth) + Vector3.up * (Mathf.Sin(t * Mathf.PI) * .15f);
                armyCloth.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(t * Mathf.PI * 4f) * Mathf.Sin(t * Mathf.PI) * 3.5f);
            }
            float blend = 1f - Mathf.Exp(-Time.unscaledDeltaTime / .07f);
            foreach (var province in provinces)
            {
                var p = province.Value.transform.localPosition;
                p.y = Mathf.Lerp(p.y, province.Key == selectedId ? .06f : province.Key == hoveredId ? .022f : 0f, blend);
                province.Value.transform.localPosition = p;
                float pulse = province.Key == pulseId ? Mathf.Clamp01(1f - (Time.unscaledTime - pulseStarted) / .95f) : 0f;
                displayedColors[province.Key] = Color.Lerp(displayedColors[province.Key], provinceColors[province.Key], blend);
                float highlight = province.Key == selectedId ? .10f : province.Key == hoveredId ? .055f : 0f;
                province.Value.sharedMaterial.color = Color.Lerp(displayedColors[province.Key], SelectionInk, Mathf.Max(highlight, pulse * .48f));
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

        public void SetHovered(string id)
        {
            if (!built) return;
            if (id != null && !provinces.ContainsKey(id)) id = null;
            if (hoveredId == id) return;
            hoveredId = id;
            RebuildHover();
        }

        private void RebuildHover()
        {
            ClearChildren(hoverRoot);
            if (hoveredId != selectedId && cells.TryGetValue(hoveredId ?? "", out var cell))
                BorderOfCell(cell, hoverRoot, hoverMat, .10f, .18f);
        }

        public void ResetPresentation()
        {
            if (!built) return;
            armyPositioned = false;
            lastWeek = lastMoves = -1;
            pulseId = null;
            pulseRing.enabled = false;
            SetHovered(null);
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

        public void SetVisible(bool visible) { if (!visible) SetHovered(null); gameObject.SetActive(visible); }

        private static float BaseFood(string id) { foreach (var d in CampaignCore.Regions) if (d.Id == id) return d.BaseFood; return 0; }
        private static float BaseTax(string id) { foreach (var d in CampaignCore.Regions) if (d.Id == id) return d.BaseTax; return 0; }
        private Transform NewRoot(string name) { var go = new GameObject(name); go.transform.SetParent(transform, false); return go.transform; }
        private static void ClearChildren(Transform root) { for (int i = root.childCount - 1; i >= 0; --i) { var go = root.GetChild(i).gameObject; go.SetActive(false); UnityEngine.Object.Destroy(go); } }
        private static Color Hex(string value) { ColorUtility.TryParseHtmlString(value, out var color); return color; }
        // Renk açıklaması ile yüzey aynı paleti kullanır; veri hesabı Refresh içinde değişmez.
        public static Color ModeColor(string mode,float value)
        {
            value=Mathf.Clamp01(value);
            if(mode=="army")return Color.Lerp(Hex("#E9DCB7"),Hex("#83B0B6"),value);
            if(mode=="food")return Color.Lerp(Hex("#E7CE98"),Hex("#7F9E80"),value);
            if(mode=="tax")return Color.Lerp(Hex("#F0E1BA"),Hex("#B79D71"),value);
            Color low=Hex("#C98270"),middle=Hex("#DDCCA0"),high=Hex("#A9BA88");
            return value<.5f?Color.Lerp(low,middle,value*2):Color.Lerp(middle,high,(value-.5f)*2);
        }
        private Material MakeMaterial(Color color) { var shader = Shader.Find("Unlit/Color"); if (!shader) shader = Shader.Find("Sprites/Default"); var material = new Material(shader) { color = color }; owned.Add(material); return material; }
        private Material MakeAtlasMaterial(Color color)
        {
            var material = new Material(Shader.Find("Sprites/Default")) { color = color, mainTexture = paperGrain };
            owned.Add(material);
            return material;
        }
        private Texture2D MakePaperGrain()
        {
            const int size = 128;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, true)
            {
                name = "Atlas paper fibres", wrapMode = TextureWrapMode.Repeat, filterMode = FilterMode.Trilinear
            };
            var pixels = new Color[size * size];
            uint noise = 1789;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                // Yerel doku dizisi simülasyonun rastgele sayı akışını tüketmez.
                noise = unchecked(noise * 1664525u + 1013904223u);
                // Üst bitler sütunlarda iki satırlık tekrar ve görünür bant üretmez.
                float fibre=(noise >> 24)/255f;
                float wash=.5f+.5f*Mathf.Sin(x*Mathf.PI*2/size+Mathf.Sin(y*Mathf.PI*2/size)*.6f);
                float shade = .975f + wash*.015f + fibre*.01f;
                pixels[y * size + x] = new Color(shade, shade, shade, 1f);
            }
            texture.SetPixels(pixels);
            texture.Apply(true, true);
            owned.Add(texture);
            return texture;
        }
        private static Vector3 World(Vector2 p, float elevation) { return new Vector3((p.x - 450f) / 12f, elevation, (390f - p.y) / 12f); }

        private static List<Vector2> SoftenCoast(List<Vector2> coast)
        {
            // Tek ölçülü köşe geçişi ana silüeti ve bölge merkezleri modelini korur.
            var softened = new List<Vector2>(coast.Count * 2);
            for (int i = 0; i < coast.Count; i++)
            {
                Vector2 previous = coast[(i + coast.Count - 1) % coast.Count];
                Vector2 current = coast[i];
                Vector2 next = coast[(i + 1) % coast.Count];
                softened.Add(Vector2.Lerp(current, previous, .12f));
                softened.Add(Vector2.Lerp(current, next, .12f));
            }
            return softened;
        }

        private void MakeFlat(string name, List<Vector2> points, Color color, float height)
        {
            var vertices = new List<Vector3>(); foreach (var p in points) vertices.Add(World(p, height));
            var go = new GameObject(name); go.transform.SetParent(transform, false);
            go.AddComponent<MeshFilter>().sharedMesh = NewMesh(vertices, Triangulate(points));
            var renderer = go.AddComponent<MeshRenderer>(); renderer.sharedMaterial = MakeAtlasMaterial(color); renderer.shadowCastingMode = ShadowCastingMode.Off;
        }
        private Mesh NewMesh(List<Vector3> vertices, List<int> indices)
        {
            var uv = new List<Vector2>();
            foreach (var vertex in vertices) uv.Add(new Vector2(vertex.x * .2f, vertex.z * .2f));
            var mesh = new Mesh { name = "Atlas polygon" }; mesh.SetVertices(vertices); mesh.SetUVs(0, uv); mesh.SetTriangles(indices, 0); mesh.RecalculateNormals(); mesh.RecalculateBounds(); owned.Add(mesh); return mesh;
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
        private void BorderOfCell(List<Vector2> cell, Transform parent, Material material, float width, float elevation, bool includeCoast = true)
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
            if (!includeCoast) return;
            for (int i = 0; i < mainland.Count; i++)
            {
                Vector2 a = mainland[i], b = mainland[(i + 1) % mainland.Count];
                if (ClipSegmentToCell(ref a, ref b, cell))
                    DrawLine("Province coast", new List<Vector2> { a, b }, false, parent, material, width, elevation);
            }
        }
        private static bool ClipSegmentToCell(ref Vector2 a, ref Vector2 b, List<Vector2> cell)
        {
            for (int i = 0; i < cell.Count; i++)
            {
                Vector2 edge = cell[(i + 1) % cell.Count] - cell[i];
                float da = Cross(edge, a - cell[i]), db = Cross(edge, b - cell[i]);
                if (da < -.001f && db < -.001f) return false;
                if ((da < 0f) != (db < 0f))
                {
                    Vector2 intersection = Vector2.LerpUnclamped(a, b, da / (da - db));
                    if (da < 0f) a = intersection; else b = intersection;
                }
            }
            return (b - a).sqrMagnitude > .00001f;
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
            // Çevre coğrafyası oynanabilir sınır veya tarihî ölçüm iddiası taşımaz.
            var easternLand = new List<Vector2>();
            for (int i = 0; i < 48; i++) easternLand.Add(mainland[i]);
            easternLand.AddRange(Points(660,570,681,562,704,568,728,581,749,599,771,609,786,631,
                805,649,819,667,840,687,858,709,882,722,922,769,990,817,1100,840,
                1200,680,1200,-300,470,-300));
            MakeFlat("Continental margin", easternLand, Hex("#E4DFC0"), -.05f);
            MakeFlat("Iberian margin", SoftenCoast(Points(285,552,300,580,329,587,358,605,392,609,420,620,
                448,626,468,611,486,590,518,577,526,598,513,614,497,623,488,643,477,660,469,674,
                456,688,452,706,446,718,457,746,427,790,200,960,-200,960,-200,760,-95,722,
                18,690,67,657,123,638,163,624,211,619,242,608,272,578)), Hex("#E4DFC0"), -.05f);
            MakeFlat("Channel shore", SoftenCoast(Points(-300,-150,510,-150,465,-30,420,12,383,27,355,20,
                323,46,287,45,261,57,236,54,210,72,174,69,144,85,114,75,95,87,54,91,
                25,84,-20,101,-70,97,-120,110,-300,115)), Hex("#E9E1BF"), -.05f);
            MakeFlat("Corsica inset", new List<Vector2> {new Vector2(776,542),new Vector2(786,566),new Vector2(798,584),new Vector2(788,614),new Vector2(776,638),new Vector2(763,628),new Vector2(755,605),new Vector2(763,573)}, Hex("#C9C296"), .02f);
        }
        private void AddCoastalEngraving()
        {
            var waterInk = MakeMaterial(Hex("#72A0A8"));
            for (int i = 0; i < mainland.Count; i++)
            {
                Vector2 a = mainland[i], b = mainland[(i + 1) % mainland.Count], middle = (a + b) * .5f;
                bool atlantic = middle.x < 315f && middle.y > 215f;
                bool channel = middle.x < 421f && middle.y < 260f;
                bool mediterranean = middle.x > 485f && middle.y > 554f;
                if (!atlantic && !channel && !mediterranean) continue;
                Vector2 edge = (b - a).normalized;
                Vector2 outward = new Vector2(edge.y, -edge.x);
                for (int ring = 1; ring <= 2; ring++)
                {
                    Vector2 offset = outward * (2.4f * ring);
                    DrawLine("Engraved coastal water", new List<Vector2> { a + offset, b + offset }, false,
                        transform, waterInk, .025f, -.12f);
                }
            }
        }
        private void MakeCity(Seed seed)
        {
            Transform city = NewRoot("Engraved town:" + seed.Id);
            city.SetParent(provinces[seed.Id].transform, false);
            city.localPosition = World(seed.Point, .14f);
            var drawing = new CityEngraving();
            Color ink = Hex("#3E5A4E"), faint = Hex("#8FA079"), wall = Hex("#F3E7CA");
            bool capital = seed.Id == "ile";
            bool coastal = seed.Id == "brittany" || seed.Id == "normandy";
            bool frontier = seed.Id == "lorraine" || seed.Id == "champagne";
            bool grain = seed.Id == "orleans" || seed.Id == "poitou" || seed.Id == "picardy";

            // Harflerden ayrılan, yaklaşık 34 × 20 piksel bir kent silüeti.
            // Yapılar özgün işaretlerdir; belirli tarihî bir cephenin kopyası değildir.
            drawing.Line(faint, .045f, -1.52f,-.83f, -.8f,-.9f, .1f,-.86f, 1.49f,-.78f);
            if (grain)
            {
                for (int row = 0; row < 3; row++)
                    drawing.Line(faint, .04f, -1.52f + row * .18f,-.73f, -1.2f + row * .18f,-.40f);
            }
            if (coastal)
            {
                drawing.Line(ink, .055f, -1.28f,-.61f, -1.28f,.62f);
                drawing.Shape(wall, -1.32f,.52f, -1.76f,-.22f, -1.32f,-.15f);
                drawing.Line(ink, .045f, -1.32f,.52f, -1.76f,-.22f, -1.32f,-.15f);
                drawing.Line(ink, .06f, -1.82f,-.52f, -1.64f,-.69f, -1.08f,-.66f);
            }
            if (capital)
            {
                EngravedHouse(drawing, -.26f, -.44f, .45f, .87f, .26f);
                EngravedHouse(drawing, -1.45f, -.72f, .98f, .57f, .29f);
                EngravedHouse(drawing, .47f, -.72f, .93f, .57f, .29f);
                EngravedHouse(drawing, -.50f, -.77f, .97f, .68f, .35f);
                drawing.Line(ink, .05f, -1.45f,-.41f, 1.39f,-.41f);
            }
            else
            {
                EngravedHouse(drawing, -.70f, -.20f, .68f, .35f, .27f);
                EngravedHouse(drawing, .09f, -.68f, frontier ? .40f : .33f, 1.13f, .30f);
                EngravedHouse(drawing, coastal ? -.97f : -1.17f, -.73f, .96f, .48f, .30f);
                EngravedHouse(drawing, .46f, -.66f, seed.Id == "languedoc" ? 1f : .80f, .47f, .28f);
                drawing.Line(ink, .07f, .21f,.14f, .21f,.33f);
            }
            if (frontier)
            {
                drawing.Shape(wall, -1.32f,-.78f, 1.36f,-.78f, 1.36f,-.51f, -1.32f,-.51f);
                drawing.Line(ink, .06f, -1.32f,-.78f, -1.32f,-.45f, -.89f,-.45f, -.89f,-.54f,
                    -.42f,-.54f, -.42f,-.45f, .08f,-.45f, .08f,-.54f, .55f,-.54f,
                    .55f,-.45f, 1.36f,-.45f, 1.36f,-.78f);
                drawing.Shape(ink, -.19f,-.78f, .16f,-.78f, .16f,-.62f, -.02f,-.53f, -.19f,-.62f);
            }
            var mesh = NewMesh(drawing.Vertices, drawing.Indices);
            mesh.name = "Town engraving: " + seed.Id;
            mesh.SetColors(drawing.Colors);
            city.gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            var renderer = city.gameObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = cityInkMat;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }
        private static void EngravedHouse(CityEngraving drawing, float x, float baseline, float width, float height, float roofHeight)
        {
            Color ink = Hex("#3E5A4E"), wall = Hex("#F3E7CA"), shade = Hex("#B79D71");
            Color roof = Hex("#60868B"), roofLight = Hex("#A0BEC0");
            float eaves = baseline + height, ridge = eaves + roofHeight;
            float right = x + width, peak = x + width * .40f, depth = .15f;
            drawing.Shape(shade, right,baseline, right+depth,baseline+.09f, right+depth,eaves+.10f, right,eaves);
            drawing.Shape(wall, x,baseline, right,baseline, right,eaves, x,eaves);
            drawing.Shape(roof, x-.06f,eaves, peak,ridge, right+.06f,eaves);
            drawing.Shape(roofLight, peak,ridge, peak+depth,ridge+.10f, right+depth+.06f,eaves+.10f, right+.06f,eaves);
            drawing.Line(ink, .055f, x,baseline, x,eaves, x-.06f,eaves, peak,ridge, peak+depth,ridge+.10f,
                right+depth+.06f,eaves+.10f, right+depth,baseline+.09f, right,baseline, x,baseline);
            drawing.Line(ink, .045f, peak,ridge, right+.06f,eaves, x-.06f,eaves);
            drawing.Line(ink, .045f, right,eaves, right,baseline);
            float door = x + width * .47f;
            drawing.Line(ink, .075f, door,baseline+.03f, door,baseline+Mathf.Min(height*.48f,.25f));
            if (width > .6f)
            {
                float window = baseline + height * .69f;
                drawing.Line(ink, .065f, x+width*.21f,window-.055f, x+width*.21f,window+.055f);
                drawing.Line(ink, .065f, x+width*.76f,window-.055f, x+width*.76f,window+.055f);
            }
        }
        private void Block(Transform parent, Vector3 position, Vector3 scale, Material material, string name)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube); go.name = name; go.transform.SetParent(parent, false); go.transform.localPosition = position; go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = material; var collider = go.GetComponent<Collider>(); if (collider) { collider.enabled = false; Destroy(collider); }
        }
        private void AddEngraving()
        {
            var river = MakeMaterial(Hex("#64949D"));
            DrawLine("Seine",Points(540,332,511,297,477,280,450,250,398,232,366,225,332,233,301,219),false,transform,river,.12f,.1f);
            DrawLine("Loire",Points(532,425,512,402,483,406,456,390,440,365,417,342,382,344,341,337,310,332,275,328,224,347),false,transform,river,.13f,.1f);
            DrawLine("Rhone",Points(601,365,587,399,588,435,579,472,576,493,588,554),false,transform,river,.14f,.1f);
            DrawLine("Garonne",Points(435,569,408,538,380,525,344,503,319,489,301,459),false,transform,river,.11f,.1f);
            var mountain = MakeMaterial(Hex("#9B9D72"));
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
                DrawLine("Sea engraving",Points(10,z*12+390,23,z*12+390,35,z*12+389),false,transform,MakeMaterial(Hex("#72A0A8")),.04f,-.1f);
            }
        }
        private static List<Vector2> Points(params float[] values) { var p=new List<Vector2>();for(int i=0;i<values.Length;i+=2)p.Add(new Vector2(values[i],values[i+1]));return p; }
        private void MakeArmy(string id)
        {
            if(string.IsNullOrEmpty(id))return;
            armyRoot.localPosition=ArmyPosition(id);
            Block(armyRoot,new Vector3(0,.32f,0),new Vector3(.1f,.7f,.1f),goldMat,"Standard pole");
            armyCloth = NewRoot("Folded standard"); armyCloth.SetParent(armyRoot, false);
            var cloth = new GameObject("Royal field standard"); cloth.transform.SetParent(armyCloth, false);
            cloth.AddComponent<MeshFilter>().sharedMesh = NewMesh(new List<Vector3>
            {
                new Vector3(0,.72f,-.72f), new Vector3(.72f,.82f,-.66f), new Vector3(1.5f,.71f,-.73f),
                new Vector3(0,.72f,.72f), new Vector3(.72f,.82f,.66f), new Vector3(1.5f,.71f,.73f)
            }, new List<int> { 0,3,1,1,3,4,1,4,2,2,4,5 });
            cloth.AddComponent<MeshRenderer>().sharedMaterial = MakeMaterial(Hex("#31535B"));
            Block(armyCloth,new Vector3(.75f,.86f,0),new Vector3(.075f,.04f,1.05f),goldMat,"Standard embroidery");
            Block(armyCloth,new Vector3(.75f,.87f,0),new Vector3(1.05f,.04f,.075f),goldMat,"Standard embroidery");
            var ring = new List<Vector2>();
            for (int i = 0; i < 32; i++)
            {
                float angle = i * Mathf.PI * 2f / 32f;
                ring.Add(new Vector2(450f + Mathf.Cos(angle) * 8f, 390f + Mathf.Sin(angle) * 8f));
            }
            DrawLine("Army station seal", ring, true, armyRoot, roadMat, .065f, .025f);
        }
        private Vector3 ArmyPosition(string id) => transform.InverseTransformPoint(RegionWorld(id)) + ArmyOffset;
        private Vector3 RouteControl(Vector3 from, Vector3 to)
        {
            Vector3 direction = to - from;
            Vector3 bend = new Vector3(-direction.z, 0, direction.x).normalized * Mathf.Min(direction.magnitude * .1f, 1.6f);
            Vector3 middle = (from + to) * .5f;
            // Yalnız mürekkep izi: mevcut komşuluk, maliyet ve varış hesabı değişmez.
            // Kıyıdaki bir körfezi kesmek yerine aynı yumuşak yayı kara tarafına yatırır.
            foreach (float amount in new[] { 1f, -1f, 2f, -2f, 3f, -3f, 0f })
            {
                Vector3 control = middle + bend * amount;
                bool onLand = true;
                for (int sample = 1; sample < 32; sample++)
                {
                    Vector3 point = RoutePoint(from, control, to, sample / 32f);
                    if (Inside(new Vector2(point.x * 12f + 450f, 390f - point.z * 12f), mainland)) continue;
                    onLand = false; break;
                }
                if (onLand) return control;
            }
            return middle;
        }
        private static Vector3 RoutePoint(Vector3 from, Vector3 control, Vector3 to, float t) =>
            (1f - t) * (1f - t) * from + 2f * (1f - t) * t * control + t * t * to;
        private void DrawRoute(Vector3 from,Vector3 to)
        {
            Vector3 control = RouteControl(from, to);
            int segments = Mathf.Clamp(Mathf.CeilToInt(Vector3.Distance(from, to) / .65f), 8, 28);
            for(int i=0;i<segments;i++)
            {
                float t=i/(float)segments,u=(i+.56f)/segments;
                Vector3 a=RoutePoint(from,control,to,t),b=RoutePoint(from,control,to,u);
                DrawLine("March ribbon",Points(a.x*12+450,390-a.z*12,b.x*12+450,390-b.z*12),false,routeRoot,roadMat,.09f,.28f);
            }
            Vector3 tip = RoutePoint(from, control, to, .97f);
            Vector3 tangent = (tip - RoutePoint(from, control, to, .88f)).normalized;
            Vector3 side = new Vector3(-tangent.z, 0, tangent.x) * .32f;
            Vector3 left = tip - tangent * .65f + side, right = tip - tangent * .65f - side;
            DrawLine("Dispatch direction", Points(left.x*12+450,390-left.z*12,tip.x*12+450,390-tip.z*12,
                right.x*12+450,390-right.z*12),false,routeRoot,roadMat,.09f,.28f);
        }
        private void OnDestroy() { foreach (var item in owned) if(item)Destroy(item); owned.Clear(); }
    }
}
