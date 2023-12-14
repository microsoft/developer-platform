// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Data.Cosmos;
using Microsoft.Extensions.Caching.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Microsoft.Developer.Data;

public static class ServiceCollectionExtensions
{
    public static IDeveloperPlatformBuilder AddCosmos(this IDeveloperPlatformBuilder builder, IConfiguration config, bool removeTrace = true)
    {
        builder.Services.AddTransient<CosmosBuilderFactory>();
        builder.Services
            .AddSingleton(typeof(IDocumentRepositoryFactory<>), typeof(CosmosDocumentRepositoryFactory<>));

        builder.Services
            .Configure<CosmosOptions>(config.GetSection(CosmosOptions.Section));

        builder.Services
            .AddCosmosCache(options =>
            {
                options.ContainerName = "DistributedCache";
                options.CreateIfNotExists = true;
            });

        builder.Services.AddOptions<CosmosCacheOptions>()
            .Configure<IOptions<CosmosOptions>, CosmosBuilderFactory>((options, cosmos, factory) =>
            {
                options.ClientBuilder = factory.GetBuilder();

                options.DatabaseName = $"{cosmos.Value.DatabaseName}Cache";
            });

        if (removeTrace)
        {
            var defaultTrace = Type.GetType("Microsoft.Azure.Cosmos.Core.Trace.DefaultTrace,Microsoft.Azure.Cosmos.Direct");
            var traceSource = (TraceSource?)defaultTrace?.GetProperty("TraceSource")?.GetValue(null);
            traceSource?.Listeners.Remove("Default");
        }

        return builder;
    }
}
