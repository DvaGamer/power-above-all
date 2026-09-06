"""Portreyi değiştirmeden alfa adalarını ve güvenli UV kenarını ölçer."""
from collections import deque
from pathlib import Path
import struct
import zlib

path = Path(__file__).parents[1] / "Assets/Resources/Art/PoliticalPortraits-v1.png"
data = path.read_bytes()
assert data[:8] == b"\x89PNG\r\n\x1a\n"
offset, compressed = 8, bytearray()
while offset < len(data):
    length = struct.unpack_from(">I", data, offset)[0]
    kind = data[offset + 4:offset + 8]
    payload = data[offset + 8:offset + 8 + length]
    if kind == b"IHDR":
        width, height, depth, colour, compression, filtering, interlace = struct.unpack(">IIBBBBB", payload)
        assert (depth, colour, interlace) == (8, 6, 0), (depth, colour, interlace)
    if kind == b"IDAT":
        compressed.extend(payload)
    offset += length + 12
raw = zlib.decompress(compressed)
stride = width * 4
rows = []
previous = bytearray(stride)
for y in range(height):
    start = y * (stride + 1)
    mode = raw[start]
    row = bytearray(raw[start + 1:start + 1 + stride])
    for i in range(stride):
        left, above, upper_left = (row[i - 4] if i >= 4 else 0), previous[i], (previous[i - 4] if i >= 4 else 0)
        if mode == 0:
            prediction = 0
        elif mode == 1:
            prediction = left
        elif mode == 2:
            prediction = above
        elif mode == 3:
            prediction = (left + above) // 2
        else:
            assert mode == 4
            p = left + above - upper_left
            distances = (abs(p - left), abs(p - above), abs(p - upper_left))
            prediction = (left, above, upper_left)[distances.index(min(distances))]
        row[i] = (row[i] + prediction) & 255
    rows.append(row)
    previous = row
x0, y0 = width // 2, height // 2
print("sheet", width, height, "bottom-right cell", x0, y0, width, height)
for threshold in (1, 16, 64, 128):
    remaining = {(x, y) for y in range(y0, height) for x in range(x0, width) if rows[y][x * 4 + 3] >= threshold}
    islands = []
    while remaining:
        seed = remaining.pop()
        queue = deque([seed])
        count = 0
        min_x = max_x = seed[0]
        min_y = max_y = seed[1]
        while queue:
            x, y = queue.popleft()
            count += 1
            min_x, max_x = min(min_x, x), max(max_x, x)
            min_y, max_y = min(min_y, y), max(max_y, y)
            for dx, dy in ((-1, 0), (1, 0), (0, -1), (0, 1)):
                neighbour = (x + dx, y + dy)
                if neighbour in remaining:
                    remaining.remove(neighbour)
                    queue.append(neighbour)
        islands.append((count, min_x, min_y, max_x, max_y))
    islands.sort(reverse=True)
    print("threshold", threshold, "largest islands (count,x0,y0,x1,y1)", islands[:8])
    print("left-strip islands", [part for part in islands if part[1] < x0 + 30][:12])
