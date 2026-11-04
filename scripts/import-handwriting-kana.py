"""Rebuild the 92-character KanjiVG seed offline; validate every pinned source before writing."""
import hashlib
import importlib.util
import json
import sys
from pathlib import Path
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
DATA = ROOT / "src/Infrastructure/Persistence/Seeding/Handwriting"
spec = importlib.util.spec_from_file_location("lab_import", ROOT / "tools/HandwritingLab/import_templates.py")
parser = importlib.util.module_from_spec(spec)
sys.dont_write_bytecode = True
spec.loader.exec_module(parser)


def main():
    manifest = json.loads((DATA / "sources.json").read_text())
    if manifest["sourceRevision"] != parser.REVISION or len(manifest["characters"]) != 92:
        raise ValueError("Unexpected pinned revision or coverage.")
    result = json.loads((ROOT / "tools/HandwritingLab/Data/templates.json").read_text())
    legacy = {c["id"]: c for c in result["characters"]}
    characters = []
    for entry in manifest["characters"]:
        raw = (DATA / "Sources" / entry["file"]).read_bytes()
        if hashlib.sha256(raw).hexdigest() != entry["sha256"]:
            raise ValueError(f"Source checksum mismatch: {entry['file']}")
        document = ET.fromstring(raw)
        if document.attrib["viewBox"] != "0 0 109 109":
            raise ValueError("Unsupported viewBox.")
        paths = list(document.iter("{http://www.w3.org/2000/svg}path"))
        if len(paths) != entry["strokeCount"]:
            raise ValueError("Unexpected stroke count.")
        strokes = [parser.sample(p.attrib["d"]) for p in paths]
        if not 1 <= len(strokes) <= 12 or sum(map(len, strokes)) > 4096:
            raise ValueError("Reference exceeds drawing budget.")
        if any(not 2 <= len(s) <= 512 for s in strokes):
            raise ValueError("Reference exceeds stroke budget.")
        if any(not 0 <= p[axis] <= 1 for s in strokes for p in s for axis in ("x", "y")):
            raise ValueError("Reference is outside normalized canvas.")
        character = {"id": entry["key"], "symbol": entry["symbol"], "strokes": strokes}
        if entry["key"] in legacy and character != legacy[entry["key"]]:
            raise ValueError("Legacy reference must remain unchanged.")
        characters.append(character)
    if len({x["symbol"] for x in characters}) != 92:
        raise ValueError("Duplicate character.")
    result["characters"] = characters
    (DATA / "templates.json").write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n")
    print(f"Validated {len(characters)} characters / {sum(len(c['strokes']) for c in characters)} strokes; legacy data unchanged.")


if __name__ == "__main__":
    main()
