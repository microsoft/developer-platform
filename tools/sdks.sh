#!/bin/bash
set -e

cdir=$(cd -P -- "$(dirname -- "$0")" && pwd -P)
rootdir=${cdir%/*}

apiDll=${1:-"bin/Debug/net7.0/Microsoft.Developer.Api.dll"}


log() { echo " " ; echo "[$(date +"%Y-%m-%d-%H%M%S")] $1"; echo " "; }
line() { echo " "; }

log "Microsoft Developer Python & Typescript Client Generator"

# check for autorest
if ! [ -x "$(command -v autorest)" ]; then
    log "[AutoRest] Installing AutoRest"
    npm install -g autorest
    # echo 'Error: autorest cli is not installed.\nAutoRest is required to run this script. To install the AutoRest, run npm install -g autorest, then try again. Aborting.' >&2
    # exit 1
fi

pushd $rootdir/src/Microsoft.Developer.Api

    log "[dotnet] Restoring dotnet tools"
    dotnet tool restore

    log "[OpenAPI] Generating openapi.json"
    dotnet swagger tofile --output ../../openapi/openapi.json $apiDll v1

    log "[OpenAPI] Generating openapi.yaml"
    dotnet swagger tofile --yaml --output ../../openapi/openapi.yaml $apiDll v1

    if [ "$CI" = true ] ; then
        log "[OpenAPI] copying open api files to release_assets"
        cp ../../openapi/openapi.json ../../openapi/openapi.yaml ../../release_assets
    fi

popd

line

pushd $rootdir/openapi

    log "[AutoRest] Reseting autorest"
    autorest --reset

    log "[AutoRest] Generating python client"
    autorest --v3 python.md

    log "[AutoRest] Generating typescript client"
    autorest --v3 typescript.md

popd

log "Done."
