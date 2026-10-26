#!/usr/bin/env python3
"""Refresh the reviewed public-domain Japanese kana bundle from Wikimedia Commons.

Maintainer tool, not required during app startup or database migration.
Uses MP3 playback derivatives returned by MediaWiki's videoinfo API.
"""
import hashlib
import html
import json
import re
import time
import urllib.error
import urllib.parse
import urllib.request
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DEST = ROOT / 'src/Infrastructure/Persistence/Seeding/Audio/Assets'
USER_AGENT = 'RealityLoopAudioSeed/1.0 (educational kana bundle; Wikimedia Commons reuse)'
OVERRIDES = {'a': 'Ja-A.oga', 'e': 'Ja-E.oga', 'i': 'Japanese I.ogg',
             'u': 'Japanese U.ogg', 'o': 'Japanese O.ogg', 'ka': 'Ja-Ka.oga',
             'chi': 'Japanese ti.ogg', 'fu': 'Japanese hu.ogg', 'n': 'Japanese N.ogg'}


def request(url):
    for attempt in range(4):
        try:
            with urllib.request.urlopen(urllib.request.Request(url, headers={'User-Agent': USER_AGENT}), timeout=45) as response:
                return response.read()
        except Exception as error:
            if attempt == 3:
                raise
            delay = 2 ** attempt
            if isinstance(error, urllib.error.HTTPError) and error.code == 429:
                delay = max(30, int(error.headers.get('Retry-After', '30')))
                print(f'Wikimedia rate limit: waiting {delay}s before retry.', flush=True)
            time.sleep(delay)


def is_mp3(content):
    return content.startswith(b'ID3') or (len(content) >= 2 and content[0] == 0xff and content[1] & 0xe0 == 0xe0)


def main():
    seed = (ROOT / 'src/Infrastructure/Persistence/Seeding/FoundationSeedData.cs').read_text()
    kana = re.findall(r'\((\d+), "(kana-r\d+-c\d+)", "([^"]+)", "([^"]+)", "([^"]+)"', seed)
    if len(kana) != 46:
        raise ValueError('Expected the reviewed 46-kana seed; review mapping before refreshing.')
    titles = ['File:' + OVERRIDES.get(row[4], f'Japanese {row[4]}.ogg') for row in kana]
    query = urllib.parse.urlencode({'action': 'query', 'format': 'json', 'titles': '|'.join(titles),
                                   'prop': 'videoinfo', 'viprop': 'url|derivatives|extmetadata|sha1|size', 'redirects': 1})
    data = json.loads(request('https://commons.wikimedia.org/w/api.php?' + query))
    if 'error' in data:
        raise ValueError(data['error'])
    pages = {page['title']: page for page in data['query']['pages'].values()}
    redirects = {item['from']: item['to'] for item in data['query'].get('redirects', [])}
    DEST.mkdir(parents=True, exist_ok=True)
    manifest_path = DEST.parent / 'manifest.json'
    temporary = manifest_path.with_suffix('.json.tmp')
    previous = json.loads(manifest_path.read_text()) if manifest_path.exists() else {'entries': []}
    cache = {entry['reading']: entry for entry in previous['entries']}
    if temporary.exists():
        partial = json.loads(temporary.read_text())
        cache.update({entry['reading']: entry for entry in partial['entries']})
    entries = []
    for row, title in zip(kana, titles):
        info = pages[redirects.get(title, title)]['videoinfo'][0]
        metadata = info['extmetadata']
        value = lambda key: metadata.get(key, {}).get('value', '')
        if value('LicenseShortName') != 'Public domain' or value('Copyrighted').lower() != 'false' or value('AttributionRequired').lower() != 'false' or value('Restrictions'):
            raise ValueError(f'Unreviewed license for {title}: {metadata}')
        description = html.unescape(re.sub('<[^>]+>', '', value('ImageDescription')))
        if row[2] not in description:
            raise ValueError(f'Source description does not identify kana {row[2]}: {title}')
        author = value('Artist')
        if author != 'Hakatanoshio117117':
            raise ValueError(f'Unexpected author for {title}: {author}')
        source_url = next(item['src'] for item in info['derivatives'] if item.get('transcodekey') == 'mp3')
        parsed = urllib.parse.urlparse(source_url)
        if parsed.scheme != 'https' or parsed.netloc != 'upload.wikimedia.org':
            raise ValueError('Unexpected download host')
        filename = f'{row[4]}.mp3'
        local = DEST / filename
        content = local.read_bytes() if local.exists() else b''
        old = cache.get(row[4], {})
        if (not content or old.get('sourceUrl') != source_url or old.get('sourceOriginalSha1') != info['sha1']
                or old.get('sha256') != hashlib.sha256(content).hexdigest()):
            content = request(source_url)
        if not is_mp3(content) or not 128 <= len(content) <= 1_048_576:
            raise ValueError(f'Invalid/oversized MP3 download: {title}')
        sha256 = hashlib.sha256(content).hexdigest()
        local.write_bytes(content)
        entries.append({'reading': row[4], 'itemCode': row[1], 'hiragana': row[2], 'katakana': row[3],
                        'fileName': filename, 'objectKey': f'foundation/ja/kana/v1/{row[4]}-{sha256[:16]}.mp3',
                        'sha256': sha256, 'bytes': len(content), 'contentType': 'audio/mpeg',
                        'sourcePage': info['descriptionurl'], 'sourceUrl': source_url,
                        'sourceOriginalUrl': info['url'].split('?', 1)[0], 'sourceOriginalSha1': info['sha1'],
                        'sourceTimestamp': value('DateTime'), 'sourceDescription': description,
                        'author': author, 'license': 'Public domain (PD-self)',
                        'licenseUrl': 'https://commons.wikimedia.org/wiki/Template:PD-self',
                        'modifications': 'Wikimedia-provided MP3 transcode; no local editing.'})
        cache[row[4]] = entries[-1]
        temporary.write_text(json.dumps({'bundle': 'ja-kana-v1', 'source': 'Wikimedia Commons',
                                         'entries': list(cache.values())}, ensure_ascii=False, indent=2) + '\n')
        print(f'{row[4]}: {len(content)} bytes; public domain; MP3; SHA-256 recorded', flush=True)
        time.sleep(2)
    manifest = {'bundle': 'ja-kana-v1', 'source': 'Wikimedia Commons', 'entries': entries}
    temporary.write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + '\n')
    temporary.replace(manifest_path)
    print(f'Wrote {len(entries)} recordings ({sum(e["bytes"] for e in entries)} bytes) and source manifest.')


if __name__ == '__main__':
    main()
