"""Salt okunur koordinat incelemesi: GUI, kamera ve atlas seçimi aynı çerçevede mi?"""

from pathlib import Path
import re


ROOT = Path(__file__).resolve().parents[2]
source = (ROOT / "Assets/Scripts/Presentation/CampaignMap.cs").read_text(encoding="utf-8")
seeds = {name: (float(x), float(y)) for name, x, y in
         re.findall(r'new Seed\("(\w+)",(\d+),(\d+),', source)}
resolutions = [(1440, 900), (1920, 1080), (2560, 1080), (1280, 1024),
               (900, 1440), (640, 360), (3840, 2160)]
camera_center = (-5.8, 2.7)
half_height = 28.3
half_width = half_height * 895 / 665
maximum_error = 0

for width, height in resolutions:
    scale = min(width / 1440, height / 900)
    offset = ((width - 1440 * scale) / 2, (height - 900 * scale) / 2)
    rect = (offset[0] + 245 * scale, offset[1] + 100 * scale, 895 * scale, 665 * scale)
    assert abs(rect[2] / rect[3] - 895 / 665) < 1e-12
    for name, (map_x, map_y) in seeds.items():
        world_x, world_z = (map_x - 450) / 12, (390 - map_y) / 12
        u = .5 + (world_x - camera_center[0]) / (2 * half_width)
        v = .5 + (world_z - camera_center[1]) / (2 * half_height)
        screen = (rect[0] + u * rect[2], rect[1] + v * rect[3])
        assert rect[0] <= screen[0] <= rect[0] + rect[2]
        assert rect[1] <= screen[1] <= rect[1] + rect[3]
        canvas = ((screen[0] - offset[0]) / scale, (height - screen[1] - offset[1]) / scale)
        expected_canvas = (245 + u * 895, 800 - v * 665)
        maximum_error = max(maximum_error, *(abs(a - b) for a, b in zip(canvas, expected_canvas)))
        # Kamera ışınının yatay atlas düzlemiyle kesişimi başlangıç harita noktasını geri vermeli.
        recovered_world = (camera_center[0] + ((screen[0] - rect[0]) / rect[2] - .5) * 2 * half_width,
                           camera_center[1] + ((screen[1] - rect[1]) / rect[3] - .5) * 2 * half_height)
        recovered_map = (recovered_world[0] * 12 + 450, 390 - recovered_world[1] * 12)
        assert all(abs(a - b) < 1e-9 for a, b in zip(recovered_map, (map_x, map_y)))
        closest = min(seeds, key=lambda key: sum((a - b) ** 2 for a, b in zip(seeds[key], recovered_map)))
        assert closest == name

assert maximum_error < 1e-9
print(f"PASS: {len(resolutions)} ekran oranında {len(seeds)} bölge için kamera/GUI/seçim dönüşümü; "
      f"azami tuval hatası {maximum_error:.2e} piksel.")
print("Atlas tuval sınırı: x=245..1140, y=135..800; kaynak görünüm: "
      f"x={450+(camera_center[0]-half_width)*12:.1f}..{450+(camera_center[0]+half_width)*12:.1f}, "
      f"y={390-(camera_center[1]+half_height)*12:.1f}..{390-(camera_center[1]-half_height)*12:.1f}.")
print("Bu kontrol matematiksel eşleşmeyi denetler; Unity input/GUI olay testi değildir.")
