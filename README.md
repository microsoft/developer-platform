# Developer Platform

[![developer-platform](https://img.shields.io/github/v/release/microsoft/developer-platform?logo=github)](https://github.com/microsoft/developer-platform/releases)

> [!IMPORTANT] 
> This project is currently in the incubation phase, and as such, it's important to note that aspects of the project are still under active development. We will do our best to conduct all development openly by documenting features and requirements.

This repository contains the source code of the Developer Platform API for the Developer Self-service Foundation (DFS) and common components (e.g., NuGets packages) used by the Developer Platform providers.

![Developer Self-service Foundation](/doc/img/self-service-foundation.svg)

The Developer Platform API serves as the single point of contact for user experiences. It is the Developer Platform's contract with other systems. The API is deployed to Azure and calls the providers (sample providers are listed in the next [providers section](#providers).) 

## How to deploy

TODO

## Providers

Developer Platform providers are a set of components that encapsulates logic needed to integrate with downstream systems to support CRUD operations on entities and/or fulfullment of template-based action requests.

List of sample providers and the corresponding repository containing the source codes:

| Repo                                                             | Description                                                              |                                                                                                                                                                                                         |
| ---------------------------------------------------------------- | ------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [developer-platform-github][developer-platform-github]           | The GitHub provider(s)                                                   | [![developer-platform-github](https://img.shields.io/github/v/release/microsoft/developer-platform-github?logo=github)](https://github.com/microsoft/developer-platform-github/releases)                |
| [developer-platform-devcenter][developer-platform-devcenter]     | The Dev Center [Azure Deployment Environments][ade] provider             | [![developer-platform-devcenter](https://img.shields.io/github/v/release/microsoft/developer-platform-devcenter?logo=github)](https://github.com/microsoft/developer-platform-devcenter/releases)       |
| [developer-platform-vscode][developer-platform-vscode]           | Developer Platform VS Code extension (not started)                       | [![developer-platform-vscode](https://img.shields.io/badge/release-none-e05d44?logo=github)](https://github.com/microsoft/developer-platform-vscode/releases)                                           |
| [developer-platform-vscode-chat][developer-platform-vscode-chat] | Developer Platform [GitHub CoPilot chat agent][copilot-chat] for VS Code | [![developer-platform-vscode-chat](https://img.shields.io/github/v/release/microsoft/developer-platform-vscode-chat?logo=github)](https://github.com/microsoft/developer-platform-vscode-chat/releases) |
| [developer-platform-website][developer-platform-website]         | React website imitating a company's portal (for testing)                 | [![developer-platform-website](https://img.shields.io/github/v/release/microsoft/developer-platform-website?logo=github)](https://github.com/microsoft/developer-platform-website/releases)             |

## Packages

### npm

| Package                                                                                    |                                                                                                                                                                     |
| ------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [@developer-platform/entities](https://www.npmjs.com/package/@developer-platform/entities) | [![@developer-platform/entities](https://img.shields.io/npm/v/%40developer-platform/entities?logo=npm)](https://www.npmjs.com/package/@developer-platform/entities) |

### NuGet

| Package                                |                                                                                                                                                                                              |
| -------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Microsoft.Developer.Abstractions       | ![Microsoft.Developer.Abstractions](https://img.shields.io/endpoint?url=https://msdevnuget.blob.core.windows.net/feed/badges/v/microsoft.developer.abstractions.json&logo=nuget)             |
| Microsoft.Developer.Azure              | ![Microsoft.Developer.Azure](https://img.shields.io/endpoint?url=https://msdevnuget.blob.core.windows.net/feed/badges/v/microsoft.developer.azure.json&logo=nuget)                           |
| Microsoft.Developer.Data.Cosmos        | ![Microsoft.Developer.Data.Cosmos](https://img.shields.io/endpoint?url=https://msdevnuget.blob.core.windows.net/feed/badges/v/microsoft.developer.data.cosmos.json&logo=nuget)               |
| Microsoft.Developer.DurableTasks       | ![Microsoft.Developer.DurableTasks](https://img.shields.io/endpoint?url=https://msdevnuget.blob.core.windows.net/feed/badges/v/microsoft.developer.durabletasks.json&logo=nuget)             |
| Microsoft.Developer.Hosting.AspNetCore | ![Microsoft.Developer.Hosting.AspNetCore](https://img.shields.io/endpoint?url=https://msdevnuget.blob.core.windows.net/feed/badges/v/microsoft.developer.hosting.aspnetcore.json&logo=nuget) |
| Microsoft.Developer.Hosting.Functions  | ![Microsoft.Developer.Hosting.Functions](https://img.shields.io/endpoint?url=https://msdevnuget.blob.core.windows.net/feed/badges/v/microsoft.developer.hosting.functions.json&logo=nuget)   |
| Microsoft.Developer.Hosting            | ![Microsoft.Developer.Hosting](https://img.shields.io/endpoint?url=https://msdevnuget.blob.core.windows.net/feed/badges/v/microsoft.developer.hosting.json&logo=nuget)                       |
| Microsoft.Developer.MSGraph            | ![Microsoft.Developer.MSGraph](https://img.shields.io/endpoint?url=https://msdevnuget.blob.core.windows.net/feed/badges/v/microsoft.developer.msgraph.json&logo=nuget)                       |
| Microsoft.Developer.Providers          | ![Microsoft.Developer.Providers](https://img.shields.io/endpoint?url=https://msdevnuget.blob.core.windows.net/feed/badges/v/microsoft.developer.providers.json&logo=nuget)                   |
| Microsoft.Developer.Serialization.Json | ![Microsoft.Developer.Serialization.Json](https://img.shields.io/endpoint?url=https://msdevnuget.blob.core.windows.net/feed/badges/v/microsoft.developer.serialization.json.json&logo=nuget) |

### `nuget.config`

These nuget packages are currently hosted in a private feed. To consume them in your provider, add a [`nuget.config`](https://learn.microsoft.com/en-us/nuget/reference/nuget-config-file) file to your solution:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <!--To inherit the global NuGet package sources remove the <clear/> line below -->
    <clear />
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
    <add key="msdev" value="https://msdevnuget.blob.core.windows.net/feed/index.json" />
  </packageSources>

  <!-- Microsoft.Developer.* packages will be restored from msdev, everything else from nuget.org. -->
  <packageSourceMapping>
    <packageSource key="nuget">
      <package pattern="*" />
    </packageSource>
    <packageSource key="msdev">
      <package pattern="Microsoft.Developer.*" />
    </packageSource>
  </packageSourceMapping>
</configuration>
```

## Additional resources

For more on Platform Engineering and Developer Self-service Foundation, refer to [Microsoft Platform engineering guide](https://aka.ms/plat-eng-learn).

## Contributing

This project welcomes contributions and suggestions. Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft
trademarks or logos is subject to and must follow
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.

[developer-platform-devcenter]: https://github.com/microsoft/developer-platform-devcenter
[developer-platform-github]: https://github.com/microsoft/developer-platform-github
[developer-platform-providers]: https://github.com/microsoft/developer-platform-providers
[developer-platform-vscode]: https://github.com/microsoft/developer-platform-vscode
[developer-platform-vscode-chat]: https://github.com/microsoft/developer-platform-vscode-chat
[developer-platform-website]: https://github.com/microsoft/developer-platform-website
[ade]: https://azure.microsoft.com/en-us/products/deployment-environments
[copilot-chat]: https://code.visualstudio.com/docs/editor/github-copilot#_agents-and-slash-commands
