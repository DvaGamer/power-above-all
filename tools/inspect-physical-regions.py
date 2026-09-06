"""Inspect the pinned public Natural Earth physical-region schema."""
import importlib.util
from pathlib import Path
spec=importlib.util.spec_from_file_location('atlas',Path(__file__).with_name('import-world.py'))
atlas=importlib.util.module_from_spec(spec);spec.loader.exec_module(atlas)
data=atlas.read_ne('10m_physical','ne_10m_geography_regions_polys')
print('Fields:',[f[0] for f in data.fields])
for record in data.iterShapeRecords():
    props=record.record.as_dict()
    name=props.get('NAME','')
    if any(term in str(name).lower() for term in ['alps','pyren','massif','vosges','ardenn','jura','ceven','black forest']):
        print({key:props.get(key) for key in ['NAME','FEATURECLA','SCALERANK','MIN_LABEL']},'bounds',record.shape.bbox)
