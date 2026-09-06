"""Unity'yi açmadan atlas silüeti ve mevcut yürüyüşlerin geometrisini denetler."""

import math
from pathlib import Path
import re


ROOT = Path(__file__).resolve().parents[2]
SOURCE = (ROOT / "Assets/Scripts/Presentation/CampaignMap.cs").read_text(encoding="utf-8")
CORE = (ROOT / "Assets/Scripts/Core/CampaignCore.cs").read_text(encoding="utf-8")
raw_coast = re.search(r"float\[\] Coast = \{(.*?)\};", SOURCE, re.S).group(1)
values = list(map(float, re.findall(r"\d+(?:\.\d+)?", raw_coast)))
coast = list(zip(values[::2], values[1::2]))
def soften(polygon):
    result = []
    for i, point in enumerate(polygon):
        for neighbor in (polygon[i - 1], polygon[(i + 1) % len(polygon)]):
            result.append(tuple(a * .88 + b * .12 for a, b in zip(point, neighbor)))
    return result


softened = soften(coast)


def cross(a, b, c):
    return (b[0] - a[0]) * (c[1] - a[1]) - (b[1] - a[1]) * (c[0] - a[0])


def area(polygon):
    return sum(a[0] * b[1] - a[1] * b[0] for a, b in zip(polygon, polygon[1:] + polygon[:1])) / 2


def inside(point):
    x, y = point
    hits = 0
    for a, b in zip(softened, softened[1:] + softened[:1]):
        if (a[1] > y) != (b[1] > y):
            if x < a[0] + (y - a[1]) * (b[0] - a[0]) / (b[1] - a[1]):
                hits += 1
    return hits % 2 == 1


assert area(softened) > 0, "Siluet yönü hücre kırpma sözleşmesiyle aynı olmalı."
relative_area_change = abs(area(softened) - area(coast)) / area(coast)
assert relative_area_change < .005, f"Siluet alanı fazla değişti: {relative_area_change:.3%}"


def validate_polygon(polygon, label):
    for i, a in enumerate(polygon):
        b = polygon[(i + 1) % len(polygon)]
        for j in range(i + 2, len(polygon)):
            if i == 0 and j == len(polygon) - 1:
                continue
            c, d = polygon[j], polygon[(j + 1) % len(polygon)]
            assert not (cross(a, b, c) * cross(a, b, d) < -1e-8
                        and cross(c, d, a) * cross(c, d, b) < -1e-8), f"Çokgen kendi kendini kesiyor: {label}"


validate_polygon(softened, "Fransa")
for label, pattern, needs_softening in (
    ("Doğu", r'easternLand.AddRange\(Points\((.*?)\)\);', False),
    ("İberya", r'MakeFlat\("Iberian margin", SoftenCoast\(Points\((.*?)\)\)', True),
    ("Manş", r'MakeFlat\("Channel shore", SoftenCoast\(Points\((.*?)\)\)', True),
):
    raw = re.search(pattern, SOURCE, re.S).group(1)
    coords = list(map(float, re.findall(r"-?\d+(?:\.\d+)?", raw)))
    polygon = list(zip(coords[::2], coords[1::2]))
    polygon = soften(polygon) if needs_softening else softened[:48] + polygon
    validate_polygon(polygon, label)

seeds = {
    name: (float(x), float(y))
    for name, x, y in re.findall(r'new Seed\("(\w+)",(\d+),(\d+),', SOURCE)
}
for name, point in seeds.items():
    assert inside(point), f"Bölge merkezi kara dışında: {name}"

# Veri yalnız okunur: mevcut komşuluklar arasındaki görsel kavis kara üzerinde kalmalı.
marches = 0
for name, rest in re.findall(r'Def\("(\w+)"(.*?)\)', CORE):
    for neighbor in re.findall(r'"(\w+)"', rest):
        start = (seeds[name][0] + 16.2, seeds[name][1] - 10.2)
        end = (seeds[neighbor][0] + 16.2, seeds[neighbor][1] - 10.2)
        dx, dy = end[0] - start[0], end[1] - start[1]
        length = math.hypot(dx, dy)
        bend = min(length * .1, 19.2)
        def point_at(control, t):
            return tuple((1 - t) ** 2 * a + 2 * (1 - t) * t * b + t ** 2 * c
                         for a, b, c in zip(start, control, end))

        control = None
        for amount in (1, -1, 2, -2, 3, -3, 0):
            candidate = ((start[0] + end[0]) / 2 + dy / length * bend * amount,
                         (start[1] + end[1]) / 2 - dx / length * bend * amount)
            if all(inside(point_at(candidate, sample / 32)) for sample in range(1, 32)):
                control = candidate
                break
        assert control is not None, f"Kara üzerinde görsel yay bulunamadı: {name} → {neighbor}"
        for frame in range(101):
            t = frame / 100
            position = point_at(control, t)
            assert inside(position), f"Görsel yürüyüş kara dışına çıktı: {name} → {neighbor}, t={t:.2f}"
        marches += 1

print(f"PASS: {len(softened)} kıyı köşesi, {len(seeds)} bölge merkezi, {marches} yönlü yürüyüş; "
      f"alan değişimi {relative_area_change:.3%}. Unity görüntüsü/oynanış doğrulaması değildir.")
