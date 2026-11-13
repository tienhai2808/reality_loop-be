#!/usr/bin/env python3
"""Rebuild the pinned German monoline practice references offline, retaining source/license metadata."""
import hashlib
import importlib.util
import json
import re
import sys
from pathlib import Path
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
DATA = ROOT / 'src/Infrastructure/Persistence/Seeding/German/Handwriting'
sys.dont_write_bytecode = True
spec = importlib.util.spec_from_file_location('stroke_import', ROOT / 'tools/HandwritingLab/import_templates.py')
parser = importlib.util.module_from_spec(spec)
spec.loader.exec_module(parser)


def main():
    manifest = json.loads((DATA / 'sources.json').read_text())
    if manifest['sourceRevision'] != '262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1':
        raise ValueError('A source change requires a reviewed template revision and migration.')
    for source in manifest['files']:
        if hashlib.sha256((DATA / 'Sources' / source['file']).read_bytes()).hexdigest() != source['sha256']:
            raise ValueError('Source checksum mismatch.')
    font = ET.parse(DATA / 'Sources/HersheySans1.svg')
    glyphs = {element.get('unicode'): element.get('d') for element in font.iter() if element.tag.endswith('}glyph')}
    custom = ET.parse(DATA / 'Sources/capital-eszett.svg')
    glyphs['ẞ'] = next(element.get('d') for element in custom.iter() if element.tag.endswith('}path'))
    seed = (DATA.parent / 'FoundationSeedData.cs').read_text()
    letters = re.findall(r'\((\d+), "([^\"]+)", "([^\"]+)", "([^\"]+)", "([^\"]+)"\)', seed)
    if len(letters) != 30:
        raise ValueError('Unexpected German alphabet coverage.')
    characters = []
    for letter_case in (0, 1):
        for key, code, upper, lower, name in letters:
            symbol = (upper, lower)[letter_case]
            strokes = [parser.sample(part) for part in re.split(r'(?=[Mm])', glyphs[symbol]) if part.strip()]
            points = [point for stroke in strokes for point in stroke]
            min_x, max_x = min(p['x'] for p in points), max(p['x'] for p in points)
            min_y, max_y = min(p['y'] for p in points), max(p['y'] for p in points)
            scale = .7 / max(max_x-min_x, max_y-min_y)
            center_x, center_y = (min_x+max_x)/2, (min_y+max_y)/2
            drawing = [[{'x': round(.5+(p['x']-center_x)*scale, 6),
                         'y': round(.5-(p['y']-center_y)*scale, 6)} for p in stroke] for stroke in strokes]
            if not 1 <= len(drawing) <= 12 or any(not 2 <= len(stroke) <= 512 for stroke in drawing) or sum(map(len, drawing)) > 4096:
                raise ValueError(f'Reference exceeds drawing bounds: {symbol}')
            if any(not 0 <= p[axis] <= 1 for stroke in drawing for p in stroke for axis in ('x', 'y')):
                raise ValueError('Reference outside canvas.')
            characters.append({'key': int(key)+letter_case*30, 'symbol': symbol, 'strokes': drawing})
    result = {key: value for key, value in manifest.items() if key != 'files'}
    result['characters'] = characters
    (DATA / 'templates.json').write_text(json.dumps(result, ensure_ascii=False, indent=2)+'\n')
    print(f'Validated {len(characters)} German references / {sum(len(c["strokes"]) for c in characters)} strokes.')


if __name__ == '__main__':
    main()
