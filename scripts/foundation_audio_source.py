"""Shared MediaWiki download helpers for offline seed maintenance."""
import time
import urllib.request
import urllib.error

def request(url):
    for attempt in range(4):
        try:
            with urllib.request.urlopen(urllib.request.Request(url, headers={'User-Agent': 'RealityLoopAudioSeed/2.0 (educational audio bundles; Wikimedia Commons reuse)'}), timeout=45) as response:
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

