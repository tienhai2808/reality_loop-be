"""Rebuild the derived CC BY-SA 3.0 point data from the four bundled KanjiVG SVGs (offline)."""
import hashlib
import json
import pathlib
import re
import xml.etree.ElementTree as ET

ROOT = pathlib.Path(__file__).resolve().parent / "Data"
REVISION = "70a0b7ae0c18ceb5cb358274b029cce0234a43bc"
SOURCES = [
    ("03042", "あ", 3, "3553cd24cba506dffe12e1214f231fc129c2535f719c1bdc86a9fb651aa9ec9b"),
    ("0304a", "お", 3, "2211aa7b4825d5aed8d30b05df5c939a903599f13827531828aeb019efbf1414"),
    ("0306c", "ぬ", 2, "6156f4b03d21fd26ba1d9b5c07135b5e5c1badb0f58714054e3e5928997b780e"),
    ("03081", "め", 2, "c4d6568ee1f0f7429a2c35a85ab39d392f3d87f87f27158446e4859ad1d60008"),
]
TOKEN = re.compile(r"[MmLlCc]|[-+]?(?:\d*\.\d+|\d+\.?\d*)(?:[eE][-+]?\d+)?")


def sample(path):
    if TOKEN.sub("", path).strip(" ,\t\r\n"):
        raise ValueError("Unsupported SVG path command; this importer intentionally supports only M/L/C.")
    tokens = TOKEN.findall(path)
    index, command, current, points = 0, None, (0.0, 0.0), []
    while index < len(tokens):
        if tokens[index].isalpha():
            command = tokens[index]
            index += 1
        if command is None:
            raise ValueError("A path must begin with a command.")
        count = 6 if command.lower() == "c" else 2
        values = list(map(float, tokens[index:index + count]))
        if len(values) != count:
            raise ValueError("Incomplete SVG segment.")
        index += count
        coordinates = [(values[i], values[i + 1]) for i in range(0, count, 2)]
        if command.islower():
            coordinates = [(x + current[0], y + current[1]) for x, y in coordinates]
        if command.lower() == "m":
            if points:
                raise ValueError("Multiple moves within one stroke are not supported.")
            current = coordinates[0]
            points.append(current)
            command = "l" if command == "m" else "L"
        elif command.lower() == "l":
            current = coordinates[0]
            points.append(current)
        else:
            start, control1, control2, end = current, *coordinates
            for step in range(1, 33):
                t = step / 32
                points.append(tuple((1-t)**3 * start[axis] + 3*(1-t)**2*t * control1[axis]
                                    + 3*(1-t)*t*t * control2[axis] + t**3 * end[axis] for axis in (0, 1)))
            current = end
    if len(points) < 2:
        raise ValueError("Stroke needs at least two points.")
    return [{"x": round(x / 109, 6), "y": round(y / 109, 6)} for x, y in points]


def main():
    characters = []
    for codepoint, symbol, count, digest in SOURCES:
        raw = (ROOT / "KanjiVG" / f"{codepoint}.svg").read_bytes()
        if hashlib.sha256(raw).hexdigest() != digest:
            raise ValueError(f"Source hash mismatch: {codepoint}")
        document = ET.fromstring(raw)
        if document.attrib["viewBox"] != "0 0 109 109":
            raise ValueError("Unexpected coordinate system.")
        paths = [element for element in document.iter() if element.tag.endswith("}path")]
        if len(paths) != count:
            raise ValueError("Unexpected stroke count.")
        strokes = [sample(path.attrib["d"]) for path in paths]
        if not all(0 <= point[axis] <= 1 for stroke in strokes for point in stroke for axis in ("x", "y")):
            raise ValueError("Coordinates outside the canvas.")
        characters.append({"id": "u" + codepoint[1:], "symbol": symbol, "strokes": strokes})
    result = {
        "sourceRevision": REVISION,
        "sourceUrl": f"https://github.com/KanjiVG/kanjivg/tree/{REVISION}",
        "attribution": "KanjiVG, copyright Ulrich Apel. Paths sampled into normalized point sequences by RealityLoop.",
        "license": "CC BY-SA 3.0",
        "licenseUrl": "https://creativecommons.org/licenses/by-sa/3.0/",
        "characters": characters,
    }
    (ROOT / "templates.json").write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"Generated {len(characters)} characters / {sum(len(x['strokes']) for x in characters)} strokes (offline).")


if __name__ == "__main__":
    main()
