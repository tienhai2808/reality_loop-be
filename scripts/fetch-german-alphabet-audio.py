#!/usr/bin/env python3
"""Fetch reviewed German letter-name recordings and package attribution with the MP3 bytes."""
import hashlib
import html
import json
import re
import time
import urllib.parse
from pathlib import Path
from foundation_audio_source import request, is_mp3

ROOT = Path(__file__).resolve().parents[1]
DATA = ROOT / 'src/Infrastructure/Persistence/Seeding/German/Audio'
SEED_BASE = 1563703640076578816
LICENSES = {
    'Public domain': 'https://creativecommons.org/publicdomain/mark/1.0/',
    'CC BY-SA 3.0': 'https://creativecommons.org/licenses/by-sa/3.0/',
    'CC BY-SA 4.0': 'https://creativecommons.org/licenses/by-sa/4.0/',
    'CC0': 'https://creativecommons.org/publicdomain/zero/1.0/'
}


def syncsafe(number):
    return bytes((number >> shift) & 127 for shift in (21, 14, 7, 0))


def attributed_mp3(raw, title, author, license_name, license_url, source):
    if raw.startswith(b'ID3'):
        size = sum(raw[6+i] << (7*(3-i)) for i in range(4))
        raw = raw[10+size+(10 if raw[3] == 4 and raw[5] & 16 else 0):]
    fields = {'TIT2': title, 'TPE1': author, 'TCOP': f'{license_name} {license_url} | {source}'}
    frames = []
    for name, value in fields.items():
        text = b'\x01'+value.encode('utf-16')
        frames.append(name.encode()+len(text).to_bytes(4, 'big')+b'\x00\x00'+text)
    body = b''.join(frames)
    return b'ID3\x03\x00\x00'+syncsafe(len(body))+body+raw


def main():
    seed = (DATA.parent / 'FoundationSeedData.cs').read_text()
    letters = re.findall(r'\((\d+), "([^\"]+)", "([^\"]+)", "([^\"]+)", "([^\"]+)"\)', seed)
    if len(letters) != 30:
        raise ValueError('Review German alphabet coverage before refreshing audio.')
    source_names = {'G': 'g', 'H': 'h', 'I': 'i'}
    titles = ['File:De-'+('Eszett' if code == 'eszett' else source_names.get(upper, upper))+'.ogg'
              for key, code, upper, lower, ipa in letters]
    query = urllib.parse.urlencode({'action': 'query', 'format': 'json', 'titles': '|'.join(titles),
                                   'prop': 'videoinfo', 'viprop': 'url|derivatives|extmetadata|sha1|size', 'redirects': 1})
    response = json.loads(request('https://commons.wikimedia.org/w/api.php?'+query))
    if 'error' in response:
        raise ValueError(response['error'])
    pages = {p['title']: p for p in response['query']['pages'].values()}
    redirects = {r['from']: r['to'] for r in response['query'].get('redirects', [])}
    (DATA / 'Assets').mkdir(parents=True, exist_ok=True)
    partial = DATA / 'manifest.json.tmp'
    cache = {e['fileName']: e for e in json.loads(partial.read_text())['entries']} if partial.exists() else {}
    if (DATA / 'manifest.json').exists():
        cache.update({e['fileName']: e for e in json.loads((DATA / 'manifest.json').read_text())['entries']})
    entries = []
    for (key, code, upper, lower, ipa), title in zip(letters, titles):
        info = pages[redirects.get(title, title)]['videoinfo'][0]
        metadata = info['extmetadata']
        value = lambda name: metadata.get(name, {}).get('value', '')
        license_name = value('LicenseShortName')
        if license_name not in LICENSES or value('Restrictions'):
            raise ValueError(f'Unreviewed license for {title}: {license_name}')
        author = html.unescape(re.sub('<[^>]+>', '', value('Artist'))).strip()
        if not author:
            raise ValueError('Missing attribution.')
        source_page = info['descriptionurl']
        url = next(x['src'] for x in info['derivatives'] if x.get('transcodekey') == 'mp3')
        if urllib.parse.urlparse(url).netloc != 'upload.wikimedia.org' or not url.startswith('https://'):
            raise ValueError('Unexpected source host.')
        filename = code+'.mp3'
        file = DATA / 'Assets' / filename
        previous = cache.get(filename)
        data = file.read_bytes() if file.exists() else b''
        if not previous or previous['sourceOriginalSha1'] != info['sha1'] or hashlib.sha256(data).hexdigest() != previous['sha256']:
            data = request(url)
            if not is_mp3(data):
                raise ValueError('Invalid MP3 source.')
            data = attributed_mp3(data, f'German letter {upper}', author, license_name, LICENSES[license_name], source_page)
            file.write_bytes(data)
            time.sleep(6)
        if not 128 <= len(data) <= 1048576:
            raise ValueError('Invalid recording size.')
        digest = hashlib.sha256(data).hexdigest()
        entries.append({'fileName': filename, 'objectKey': f'foundation/de/alphabet/v1/{code}-{digest[:16]}.mp3',
                        'sha256': digest, 'bytes': len(data), 'contentType': 'audio/mpeg',
                        'sourcePage': source_page, 'sourceUrl': url, 'sourceOriginalUrl': info['url'],
                        'sourceOriginalSha1': info['sha1'], 'sourceTimestamp': value('DateTime'),
                        'sourceDescription': html.unescape(re.sub('<[^>]+>', '', value('ImageDescription'))),
                        'author': author, 'license': license_name, 'licenseUrl': LICENSES[license_name],
                        'modifications': 'Wikimedia MP3 transcode with attribution ID3 tags added by RealityLoop; audio frames unchanged.',
                        'readingIds': [SEED_BASE+200+int(key), SEED_BASE+230+int(key)], 'exampleIds': []})
        partial.write_text(json.dumps({'bundle': 'de-alphabet-v1', 'languageCode': 'de', 'expectedRecordings': 30,
                                       'entries': entries}, ensure_ascii=False, indent=2)+'\n')
        print(f'{upper}/{lower}: {license_name}, {author}, {len(data)} bytes', flush=True)
    partial.replace(DATA / 'manifest.json')
    print(f'Packaged {len(entries)} verified recordings with embedded credits.', flush=True)


if __name__ == '__main__':
    main()
