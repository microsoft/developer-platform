# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

import argparse
import shutil
import subprocess
import sys

from os import walk
from pathlib import Path

REPO_DIR = Path(__file__).resolve().parent.parent
SRC_DIR = REPO_DIR / 'src'

API_DIR = SRC_DIR / 'Microsoft.Developer.Api'
API_PROJ = API_DIR / 'Microsoft.Developer.Api.csproj'
API_DLL = API_DIR / 'bin' / 'Debug' / 'net7.0' / 'Microsoft.Developer.Api.dll'
OPENAPI_DIR = REPO_DIR / 'openapi'


def dotnet(args, message, cwd=None):

    if not (dnt := shutil.which('dotnet')):
        print('Error: dotnet not found on your PATH.')
        sys.exit(1)

    args_arr = args if isinstance(args, list) else [args]

    # convert Path objects to strings
    for i, arg in enumerate(args):
        if isinstance(arg, Path):
            args[i] = str(arg)

    cmd_arr = [dnt, *args_arr]

    print('')
    print(f'dotnet: {message}')
    print(' '.join(cmd_arr))
    print('')

    subprocess.run(cmd_arr, cwd=cwd)


parser = argparse.ArgumentParser(description='Open API (Swagger) Generator.')
parser.add_argument('--build', '-b', action='store_true', help='Build the API project.')

args = parser.parse_args()

build = args.build


if build:
    dotnet(['build', API_PROJ, '-c', 'Debug'], 'Building API project')


dotnet(['tool', 'restore'], 'Restoring tools', cwd=API_DIR)

dotnet(['swagger', 'tofile', '--output', OPENAPI_DIR / 'openapi.json', API_DLL, 'v1'], 'Generating OpenAPI JSON', cwd=API_DIR)

dotnet(['swagger', 'tofile', '--yaml', '--output', OPENAPI_DIR / 'openapi.yaml', API_DLL, 'v1'], 'Generating OpenAPI YAML', cwd=API_DIR)

print('')
print('Done.')
