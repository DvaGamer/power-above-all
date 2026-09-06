"""Frame checks for the player review pass.

Reads the PNGs written by the built player, refuses obviously broken frames,
reports how much each frame moved since the previous run, and builds one
contact sheet a human can look at in the morning.

    python tools/shot-check.py output/shots [--baseline output/shots-baseline]
"""
import sys
import os
import json
from PIL import Image
import numpy as np

SHADER_PINK = np.array([255, 0, 255])
EXPECTED = (1440, 900)


def load(path):
    with Image.open(path) as image:
        return np.asarray(image.convert("RGB"), dtype=np.uint8)


def examine(name, pixels):
    problems = []
    height, width = pixels.shape[:2]
    if (width, height) != EXPECTED:
        problems.append(f"size {width}x{height}, expected {EXPECTED[0]}x{EXPECTED[1]}")
    flat = pixels.reshape(-1, 3)
    mean = flat.mean(axis=0)
    spread = float(flat.std())
    if spread < 6:
        problems.append(f"flat frame, colour spread {spread:.1f}")
    if mean.sum() < 12:
        problems.append("frame is black")
    colours, counts = np.unique(flat, axis=0, return_counts=True)
    share = counts.max() / len(flat)
    if share > 0.92:
        problems.append(f"one colour covers {share:.0%} of the frame")
    pink = np.all(np.abs(flat.astype(int) - SHADER_PINK) < 24, axis=1).mean()
    if pink > 0.004:
        problems.append(f"missing-shader magenta on {pink:.1%} of the frame")
    return {
        "name": name,
        "width": width,
        "height": height,
        "mean": [round(float(v), 1) for v in mean],
        "spread": round(spread, 1),
        "top_colour_share": round(float(share), 4),
        "problems": problems,
    }


def moved(current, previous):
    if current.shape != previous.shape:
        return None
    difference = np.abs(current.astype(int) - previous.astype(int)).sum(axis=2)
    return round(float((difference > 24).mean()), 4)


def contact_sheet(folder, names, target):
    if not names:
        return
    columns = 3
    thumb = (480, 300)
    rows = (len(names) + columns - 1) // columns
    sheet = Image.new("RGB", (columns * thumb[0], rows * thumb[1]), (24, 26, 22))
    for index, name in enumerate(names):
        with Image.open(os.path.join(folder, name)) as image:
            image = image.convert("RGB").resize(thumb, Image.LANCZOS)
            sheet.paste(image, ((index % columns) * thumb[0], (index // columns) * thumb[1]))
    sheet.save(target)


def main():
    global EXPECTED
    if '--width' in sys.argv and '--height' in sys.argv:
        EXPECTED=(int(sys.argv[sys.argv.index('--width')+1]),int(sys.argv[sys.argv.index('--height')+1]))
    folder = sys.argv[1] if len(sys.argv) > 1 else "output/shots"
    baseline = None
    if "--baseline" in sys.argv:
        baseline = sys.argv[sys.argv.index("--baseline") + 1]
    names = sorted(n for n in os.listdir(folder) if n.lower().endswith(".png"))
    if not names:
        print("no frames found in " + folder)
        return 1
    results = []
    broken = 0
    for name in names:
        pixels = load(os.path.join(folder, name))
        result = examine(name, pixels)
        if baseline:
            other = os.path.join(baseline, name)
            if os.path.exists(other):
                result["moved"] = moved(pixels, load(other))
        results.append(result)
        if result["problems"]:
            broken += 1
        line = f"{name}  spread={result['spread']}  top={result['top_colour_share']:.0%}"
        if "moved" in result and result["moved"] is not None:
            line += f"  moved={result['moved']:.1%}"
        if result["problems"]:
            line += "  !! " + "; ".join(result["problems"])
        print(line)
    contact_sheet(folder, names, os.path.join(folder, "contact-sheet.jpg"))
    with open(os.path.join(folder, "frames.json"), "w", encoding="utf-8") as handle:
        json.dump(results, handle, indent=2, ensure_ascii=False)
    print(f"{len(names)} frames, {broken} with problems, contact sheet written")
    return 1 if broken else 0


if __name__ == "__main__":
    sys.exit(main())
