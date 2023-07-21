/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Developer.MSGraph;

public static class GraphConfigurationExtensions
{
    public static IServiceCollection AddMsDeveloperMsGraph(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services
            .TryAddSingleton<IGraphService, GraphService>();

        return services;
    }
}
