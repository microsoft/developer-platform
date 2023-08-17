/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Diagnostics;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Developer.Configuration.Options;
using Microsoft.Developer.Data.CosmosDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Developer.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMsDeveloperData(this IServiceCollection services)
    {
        services
            .AddSingleton<ICosmosDbService, CosmosDbService>();

        return services;
    }

    public static IServiceCollection AddMsDeveloperCache(this IServiceCollection services, IConfiguration config, bool removeTrace = true)
    {
        var options = config.GetSection(CosmosOptions.Section);

        var databaseName = options.GetValue<string>(nameof(CosmosOptions.DatabaseName));
        var connectionString = options.GetValue<string>(nameof(CosmosOptions.ConnectionString));

        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException("ConnectionString cannot be null or empty.", nameof(options));

        if (string.IsNullOrEmpty(databaseName))
            throw new ArgumentException("DatabaseName cannot be null or empty.", nameof(options));

        services
            .AddCosmosCache(options =>
            {
                options.ClientBuilder = new CosmosClientBuilder(connectionString);
                options.DatabaseName = $"{databaseName}Cache";
                options.ContainerName = "DistributedCache";
                options.CreateIfNotExists = true;
            });

        if (removeTrace)
        {
            Type? defaultTrace = Type.GetType("Microsoft.Azure.Cosmos.Core.Trace.DefaultTrace,Microsoft.Azure.Cosmos.Direct");
            TraceSource? traceSource = (TraceSource?)defaultTrace?.GetProperty("TraceSource")?.GetValue(null);
            traceSource?.Listeners.Remove("Default");
        }

        return services;
    }
}