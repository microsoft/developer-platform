# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

from os import walk
from pathlib import Path
from shutil import rmtree

REPO_DIR = Path(__file__).resolve().parent.parent
SRC_DIR = REPO_DIR / 'src'

dirs_del = []

# walk the src directory and find all the child directories
for dirpath, dirnames, files in walk(SRC_DIR):
    # os.walk includes the root directory (i.e. repo/src) so we need to skip it and only check immediate children (and Providers)
    if not SRC_DIR.samefile(dirpath) and (Path(dirpath).parent.samefile(SRC_DIR) or Path(dirpath).parent.samefile(SRC_DIR / 'Providers')):
        print(f'checking: {dirpath}')
        dirs_del.extend([Path(dirpath) / d for d in dirnames if d in ['obj', 'bin', 'publish']])

print('')

for d in dirs_del:
    print(f'removing: {d}')
    rmtree(d)

print('Done.')
