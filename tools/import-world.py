"""Offline atlas compiler. Source GIS stays geographic; runtime meshes are disposable.

Run with Python 3.12 and tools/gis-requirements.txt. No game-time network access.
Only downloads redistributable Natural Earth data from the pinned upstream tag.
"""
import hashlib
import io
import json
import math
from pathlib import Path
import urllib.request

import mapbox_earcut as earcut
import numpy as np
import shapefile
from shapely.geometry import shape, box, Polygon, MultiPolygon, LineString
from shapely.ops import unary_union
from shapely.ops import transform
from pyproj import Transformer
import zipfile

ROOT = Path(__file__).resolve().parents[1]
CACHE = ROOT / "output/gis"
DEST = ROOT / "Unity/Assets/Resources/World"
TAG = "v5.1.2"
BASE = f"https://raw.githubusercontent.com/nvkelso/natural-earth-vector/{TAG}"
SOURCES = []


def read_ne(folder, name):
    CACHE.mkdir(parents=True, exist_ok=True)
    streams = {}
    for ext in ("shp", "shx", "dbf"):
        url = f"{BASE}/{folder}/{name}.{ext}"
        target = CACHE / f"{name}.{ext}"
        if not target.exists():
            print(f"Download {name}.{ext}", flush=True)
            with urllib.request.urlopen(url, timeout=90) as response:
                target.write_bytes(response.read())
        data = target.read_bytes()
        SOURCES.append(dict(url=url, sha256=hashlib.sha256(data).hexdigest(), bytes=len(data)))
        streams[ext] = io.BytesIO(data)
    return shapefile.Reader(**streams, encoding="utf-8")


def polygons(geometry):
    if geometry.is_empty:
        return []
    if geometry.geom_type == "Polygon":
        return [geometry]
    return [p for g in geometry.geoms for p in polygons(g)] if hasattr(geometry, "geoms") else []


def lines(geometry):
    if geometry.is_empty:
        return []
    if geometry.geom_type in ("LineString", "LinearRing"):
        return [geometry]
    return [p for g in geometry.geoms for p in lines(g)] if hasattr(geometry, "geoms") else []


def flat(coords):
    return [round(float(v), 6) for p in coords for v in p[:2]]


def mesh(geometry):
    vertices, triangles = [], []
    area = 0.0
    for p in polygons(geometry):
        if p.area < .000001:
            continue
        rings = [np.array(p.exterior.coords[:-1], dtype=np.float64)]
        rings += [np.array(r.coords[:-1], dtype=np.float64) for r in p.interiors]
        ends = np.cumsum([len(r) for r in rings], dtype=np.uint32)
        coords = np.concatenate(rings)
        indices = earcut.triangulate_float64(coords, ends)
        # Earcut XY CCW faces down on Unity XZ. Reverse every triangle for +Y.
        offset = len(vertices) // 2
        for a, b, c in indices.reshape(-1, 3):
            ab, ac = coords[b] - coords[a], coords[c] - coords[a]
            cross = ab[0] * ac[1] - ab[1] * ac[0]
            if cross > 0:
                b, c = c, b
            triangles.extend((int(a) + offset, int(b) + offset, int(c) + offset))
        vertices.extend(flat(coords))
        area += p.area
    return dict(points=vertices, triangles=triangles, area=round(area, 6))


def write(name, data):
    DEST.mkdir(parents=True, exist_ok=True)
    path = DEST / name
    path.write_text(json.dumps(data, ensure_ascii=False, separators=(",", ":")), encoding="utf-8")
    print(f"Generated {path.relative_to(ROOT)}: {path.stat().st_size:,} bytes", flush=True)


def compile_physical():
    chunks = []
    for name, folder, tolerance, level in (("ne_110m_land", "110m_physical", .025, 0),
                                         ("ne_10m_land", "10m_physical", .008, 1)):
        data = read_ne(folder, name)
        land = unary_union([shape(s.__geo_interface__).buffer(0) for s in data.shapes()])
        land = land.simplify(tolerance, preserve_topology=True)
        for lon in range(-180, 180, 20):
            for lat in range(-90, 90, 20):
                tile = box(lon, lat, lon + 20, min(90, lat + 20))
                clipped = land.intersection(tile)
                if clipped.is_empty:
                    continue
                chunk = mesh(clipped)
                if not chunk["triangles"]:
                    continue
                coast = land.boundary.intersection(tile)
                chunk.update(id=f"land-{level}-{lon}-{lat}", layer="land", lod=level,
                             bounds=[lon, lat, lon + 20, min(90, lat + 20)],
                             paths=[dict(points=flat(line.coords)) for line in lines(coast)])
                chunks.append(chunk)
        print(f"Compiled {name}: {len(chunks)} cumulative chunks", flush=True)

    rivers = []
    data = read_ne("10m_physical", "ne_10m_rivers_lake_centerlines")
    for record in data.iterShapeRecords():
        props = record.record.as_dict()
        geometry = shape(record.shape.__geo_interface__).simplify(.009)
        for line in lines(geometry):
            coords = list(line.coords)
            if len(coords) < 2:
                continue
            rivers.append(dict(id=str(props.get("ne_id", len(rivers))), name=props.get("name", "") or "",
                               rank=int(props.get("scalerank", 6) or 6), points=flat(coords),
                               bounds=list(line.bounds)))
    write("physical.json", dict(schema=1, crs="EPSG:4326", sources=TAG, chunks=chunks, rivers=rivers))
    manifest = dict(checked="2026-09-06", license="Public domain",
                    terms="https://www.naturalearthdata.com/about/terms-of-use/", files=SOURCES,
                    derivation="Land clipped into 20-degree tiles; topology-preserving simplification .025/.008 degrees; earcut triangulation with holes. Rivers .009 degrees. Physical current geography, not reconstructed 1789 hydrology.")
    write("physical-provenance.json", manifest)


def inspect_history():
    import zipfile
    for name in ("FRANCE_1789_BRETTE", "BAILLIAGES_1789_BRETTE"):
        archive = zipfile.ZipFile(CACHE / f"{name}.zip")
        print(name, archive.namelist())
        streams = {}
        for ext in ("shp", "shx", "dbf"):
            match = next(n for n in archive.namelist() if n.lower().endswith('.' + ext) and not n.startswith('__MACOSX'))
            streams[ext] = io.BytesIO(archive.read(match))
        data = shapefile.Reader(**streams, encoding="latin1")
        print('FIELDS', data.fields, 'BBOX', data.bbox)
        print('FIRST', [r.as_dict() for r in data.records()[:3]])
        if name.startswith('BAILLIAGES'): print('GENERALITES', sorted(set(r.as_dict()['GEN_N'] for r in data.records())))
        for file in archive.namelist():
            if file.endswith('.prj'): print('CRS', archive.read(file).decode())


def read_history(name):
    archive = zipfile.ZipFile(CACHE / f"{name}.zip")
    return shapefile.Reader(**{ext: io.BytesIO(archive.read(f"{name}.{ext}")) for ext in ('shp','shx','dbf')}, encoding='latin1')


def compile_campaign():
    recipe = json.loads((ROOT / 'tools/atlas-content.json').read_text())
    reproject = Transformer.from_crs('EPSG:2154', 'EPSG:4326', always_xy=True).transform
    source = 'Gay, Gobbi & Goni, Bailliages in 1789 France, V2.0 (2024), doi:10.7910/DVN/T8UXHK; CC BY 4.0'
    history = read_history('BAILLIAGES_1789_BRETTE')
    groups, subregions, seats = {}, [], {}
    unknown = set()
    for record in history.iterShapeRecords():
        props = record.record.as_dict()
        name = props['GEN_N']
        region = recipe['generaliteGroups'].get(name)
        if not region: unknown.add(name)
        geometry = transform(reproject, shape(record.shape.__geo_interface__)).buffer(0)
        if region: groups.setdefault(region, []).append(geometry)
        sub = mesh(geometry.simplify(.006, preserve_topology=True))
        sub.update(id=props['BAIL_ID'], name=props['BAIL_NS'], regionId=region or '', generalite=name,
                   paths=[dict(points=flat(line.coords)) for line in lines(geometry.boundary.simplify(.008))])
        subregions.append(sub)
        if props['CL_X'] and props['CL_Y']:
            lon, lat = reproject(float(props['CL_X']), float(props['CL_Y']))
            seats[props['CL_N2021']] = (lon, lat, region or '')
    print('Unassigned historical generalites (retained, no invented gameplay):', sorted(unknown))
    regions = []
    for id, geometry in groups.items():
        merged = unary_union(geometry).simplify(.01, preserve_topology=True)
        compiled = mesh(merged)
        compiled.update(id=id, regionId=id, layer='region', lod=1, paths=[dict(points=flat(l.coords)) for l in lines(merged.boundary)])
        regions.append(dict(id=id, politicalEntityId='france', seatId=recipe['regionSeats'][id], source=source,
                            confidence='Authored aggregate of reconstructed generalites; NOT historical province boundaries', areas=[compiled]))
    france = unary_union([transform(reproject, shape(s.__geo_interface__)).buffer(0) for s in read_history('FRANCE_1789_BRETTE').shapes()]).simplify(.008)
    fm = mesh(france)
    fm.update(id='france-1789', layer='political', lod=1, paths=[dict(points=flat(l.coords)) for l in lines(france.boundary)])
    settlements = []
    missing = []
    seat_to_region = {v:k for k,v in recipe['regionSeats'].items()}
    for name in list(recipe['regionSeats'].values()) + recipe['additionalSeats']:
        if name not in seats:
            missing.append(name); continue
        lon, lat, region = seats[name]
        settlements.append(dict(id=name, name=name.title(), regionId=seat_to_region.get(name, region), politicalEntityId='france',
                                longitude=round(lon,6), latitude=round(lat,6), rank=0 if name in seat_to_region else 1, source=source+'; chef-lieu geographic coordinates'))
    if missing: raise ValueError(f'Missing seats: {missing}; available candidates {sorted(seats)}')
    by_id = {s['id']:s for s in settlements}
    roads = []
    for a,b in recipe['roadLinks']:
        sa,sb = by_id[a],by_id[b]
        roads.append(dict(id=a+'-'+b, **{'from':a,'to':b}, points=[sa['longitude'],sa['latitude'],sb['longitude'],sb['latitude']],
                          source='PAA authored route graph between sourced historical seats', confidence='Schematic strategic connection; not a surveyed 1789 road alignment'))
    write('campaign.json', dict(schema=1, year=1789, entities=[dict(id='france',name='Royaume de France',source=source,
          confidence='Published historical reconstruction; parish-based polygon precision, not cadastral certainty',areas=[fm])], regions=regions,
          subregions=subregions, settlements=settlements, roads=roads, terrain=recipe['terrain']))
    catalogue=json.loads((CACHE/'bailliages-catalogue.json').read_text())['data']['latestVersion']
    write('campaign-provenance.json',dict(source=source, checked='2026-09-06', version=2, license=catalogue['license'],
          files=[e['dataFile'] for e in catalogue['files'] if e['dataFile']['filename'] in ('FRANCE_1789_BRETTE.zip','BAILLIAGES_1789_BRETTE.zip','README.txt')],
          derivation='Lambert93 EPSG2154 transformed WGS84 EPSG4326. Game regions aggregate generalites per tools/atlas-content.json. Original bailliage identifiers retained as subregions. Roads schematic; forest extents decorative. Unassigned generalites remain physical/political geography without gameplay simulation.'))


def compile_relief():
    from shapely.geometry import Point
    data=read_ne('10m_physical','ne_10m_geography_regions_polys')
    features=[]
    for record in data.iterShapeRecords():
        props=record.record.as_dict()
        if props['FEATURECLA'] not in ('Range/mtn','Plateau'):continue
        geometry=shape(record.shape.__geo_interface__)
        if not geometry.intersects(box(-12,35,25,57)):continue
        # NE label extents are approximate, not a DEM. Symbols are intentional atlas hachures.
        geometry=geometry.intersection(box(-12,35,25,57))
        points=[]
        left,bottom,right,top=geometry.bounds
        for row,lat in enumerate(np.arange(bottom+.12,top,.32)):
            for column,lon in enumerate(np.arange(left+.12+(row%2)*.17,right,.38)):
                if (row+column*3)%5==0:continue
                x=float(lon)+((row*3+column)%3-1)*.025
                y=float(lat)+((row+column*2)%3-1)*.018
                if geometry.contains(Point(x,y)):points.extend([round(x,6),round(y,6)])
        features.append(dict(id=str(props['NE_ID']),name=props['NAME'],rank=int(props['SCALERANK']),points=points,bounds=list(geometry.bounds)))
    write('relief.json',dict(schema=1,features=features))
    write('relief-provenance.json',dict(checked='2026-09-06',license='Public domain',files=SOURCES,
        source='https://www.naturalearthdata.com/downloads/10m-physical-vectors/10m-physical-labels/',
        limits='Natural Earth physical-label area bounds are approximate at 1:50m scale. Decorative hachures locate ranges/plateaux; they are not elevation samples, peaks or traversal geometry. France and neighbouring Europe content window only.'))


if __name__ == "__main__":
    import sys
    if '--inspect-history' in sys.argv: inspect_history()
    elif '--relief' in sys.argv: compile_relief()
    elif '--campaign' in sys.argv: compile_campaign()
    else: compile_physical()
