import json
import os
import shutil
import subprocess

from pathlib import Path
from re import search

ci = os.environ.get('CI', False)

path_root = Path(__file__).resolve().parent.parent.parent
path_assets = path_root / 'release_assets' if ci else path_root / '.local/release_assets'

# Create the release_assets folder if it doesn't exist
if not path_assets.exists():
    path_assets.mkdir(exist_ok=True)

# copy the openapi files into the release_assets folder
for f in ['openapi.json', 'openapi.yaml']:
    shutil.copy2(path_root / 'openapi' / f, path_assets)

assets = []

# add all the files in the root of the assets folder to the assets list
with os.scandir(path_assets) as s:
    for f in s:
        if f.is_file():
            print(f.path)
            # name = f.name.rsplit('.', 1)[0]
            assets.append({'name': f.name, 'path': f.path})


if not ci:  # if working locally, print the assets.json to a file
    with open(f'{path_assets}/assets.json', 'w') as f:
        json.dump(assets, f, ensure_ascii=False, indent=4, sort_keys=True)


if github_output := os.environ.get('GITHUB_OUTPUT', None):
    with open(github_output, 'a+') as f:
        f.write(f'assets={json.dumps(assets)}')
