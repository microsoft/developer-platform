// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Developer.Data.CosmosDb;
using Microsoft.Developer.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Microsoft.Developer.Serialization.Json.Entities;
using Microsoft.Extensions.Caching.Cosmos;
using Microsoft.Extensions.Options;
using Microsoft.Developer.Data.Cosmos;

namespace Microsoft.Developer.Data;

public static class ServiceCollectionExtensions
{
    public static IDeveloperPlatformBuilder AddCosmos(this IDeveloperPlatformBuilder builder, IConfiguration config, bool removeTrace = true)
    {
        builder.Services
            .AddSingleton(typeof(IDocumentRepositoryFactory<>), typeof(CosmosEntitiesRepositoryFactory<>));

        builder.Services
            .Configure<CosmosOptions>(config.GetSection(CosmosOptions.Section));

        builder.AddDocumentRepository<IEntitiesRepositoryFactory, EntitiesRepositoryFactory, Entity>(nameof(Entity), options =>
        {
            options.DatabaseName = "Entities";
            options.SerializerOptions = EntitySerializerOptions.Database;
        });

        builder.Services
            .AddCosmosCache(options =>
            {
                options.ContainerName = "DistributedCache";
                options.CreateIfNotExists = true;
            });

        builder.Services.AddOptions<CosmosCacheOptions>()
            .Configure<IOptions<CosmosOptions>>((options, cosmos) =>
            {
                options.ClientBuilder = new CosmosClientBuilder(cosmos.Value.ConnectionString);
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
