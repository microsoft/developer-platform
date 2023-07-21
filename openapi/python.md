# Microsoft Developer Client SDKs

This file contains the configuration for generating client SDKs for the Microsoft Developer API.

> see https://aka.ms/autorest

## Getting Started

To generate the client SDKs for the Microsoft Developer API, simply install AutoRest via `npm` (`[sudo] npm install -g autorest`) and then run:

```shell
cd path/to/client
autorest --v3 python.md
```

For other options on installation see [Installing AutoRest](https://aka.ms/autorest/install) on the AutoRest GitHub page.

## Configuration

The remainder of this file is configuration details used by AutoRest.

### Inputs

```yaml
use: "@autorest/python@latest"
input-file: openapi.yaml
namespace: microsoft-developer
add-credentials: true
credential-scopes: openid
override-client-name: MicrosoftDeveloperClient
license-header: MICROSOFT_MIT_NO_VERSION
output-folder: "./../sdks/python/microsoft-developer"
no-namespace-folders: true
clear-output-folder: true
```
