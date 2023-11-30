// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Developer.MSGraph;

public static class GraphConfigurationExtensions
{
    public static IDeveloperPlatformBuilder AddMicrosoftGraph(this IDeveloperPlatformBuilder builder)
    {
        builder.Services.AddMemoryCache();
        builder.Services.TryAddSingleton<IUserService, MsGraphUserProvider>();
        builder.Services.TryAddSingleton<IGraphService, GraphService>();
        builder.Services.TryAddScoped<IDownstreamApi, DeveloperApiDownstreamApi>();

        return builder;
    }
}
