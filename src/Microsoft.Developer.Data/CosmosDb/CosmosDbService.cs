/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Collections.Concurrent;
using Microsoft.Azure.Cosmos;
using Microsoft.Developer.Configuration.Options;
using Microsoft.Developer.Data.CosmosDb.Serialization;
using Microsoft.Developer.Entities;
using Microsoft.Developer.Entities.Serialization;
using Microsoft.Extensions.Options;

namespace Microsoft.Developer.Data.CosmosDb;

public interface ICosmosDbService
{
    CosmosClient Client { get; }

    CosmosOptions Options { get; }

    Task<Database> GetDatabaseAsync(string databaseName, CancellationToken cancellationToken = default);

    Task<Container> GetEntitiesContainerAsync(string databaseName, CancellationToken cancellationToken = default);

    Task<Container> GetEntityContainerAsync<T>(string databaseName, CancellationToken cancellationToken = default)
        where T : class, IEntity, new();

}

public class CosmosDbService : ICosmosDbService
{
    public CosmosClient Client { get; private set; }

    public CosmosOptions Options { get; private set; }

    private readonly ConcurrentDictionary<string, Database> databases = new();

    private readonly ConcurrentDictionary<Type, Container> containers = new();

    public CosmosDbService(IOptions<CosmosOptions> options)
    {
        Options = options.Value;

        if (string.IsNullOrEmpty(options.Value.ConnectionString))
            throw new ArgumentException("ConnectionString cannot be null or empty.", nameof(options));

        if (string.IsNullOrEmpty(options.Value.DatabaseName))
            throw new ArgumentException("DatabaseName cannot be null or empty.", nameof(options));

        Client = new CosmosClient(options.Value.ConnectionString, new CosmosClientOptions()
        {
            Serializer = new CosmosDbSerializer(EntitySerializerOptions.Database)
        });
    }

    public async Task<Database> GetDatabaseAsync(string databaseName, CancellationToken cancellationToken = default)
    {
        databaseName ??= Options.DatabaseName;

        if (databases.TryGetValue(databaseName, out var database) && database is not null)
            return database;

        var response = await Client
            .CreateDatabaseIfNotExistsAsync(databaseName, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        database = response.Database;

        databases[databaseName] = database;

        return database;
    }


    public async Task<Container> GetEntityContainerAsync<T>(string databaseName, CancellationToken cancellationToken = default)
        where T : class, IEntity, new()
    {
        databaseName ??= Options.DatabaseName;

        var database = await GetDatabaseAsync(databaseName, cancellationToken)
            .ConfigureAwait(false);

        if (containers.TryGetValue(typeof(T), out var container) && container is not null)
            return container;

        container = await CreateContainerAsync(database, typeof(T), cancellationToken)
            .ConfigureAwait(false);

        containers[typeof(T)] = container;

        return container;
    }

    public async Task<Container> GetEntitiesContainerAsync(string databaseName, CancellationToken cancellationToken = default)
    {
        databaseName ??= Options.DatabaseName;

        var database = await GetDatabaseAsync(databaseName, cancellationToken)
            .ConfigureAwait(false);

        if (containers.TryGetValue(typeof(IEntity), out var container) && container is not null)
            return container;

        container = await CreateContainerAsync(database, typeof(IEntity), cancellationToken)
            .ConfigureAwait(false);

        containers[typeof(IEntity)] = container;

        return container;
    }

    private static async Task<Container> CreateContainerAsync(Database database, Type containerType, CancellationToken cancellationToken = default)
    {
        var containerName = ContainerNameAttribute.GetNameOrDefault(containerType);

        var containerPartitionKey = PartitionKeyAttribute.GetPath(containerType, true);
        var containerUniqueKeys = UniqueKeyAttribute.GetPaths(containerType, true);

        var containerBuilder = database.DefineContainer(containerName, containerPartitionKey);

        if (containerUniqueKeys.Any())
            foreach (var containerUniqueKey in containerUniqueKeys)
                containerBuilder = containerBuilder
                    .WithUniqueKey()
                    .Path(containerUniqueKey)
                    .Attach();

        if (typeof(ISoftDelete).IsAssignableFrom(containerType) && containerType.IsDefined(typeof(SoftDeleteAttribute), false))
            containerBuilder = containerBuilder
                .WithDefaultTimeToLive(-1);

        _ = await containerBuilder
            .CreateIfNotExistsAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return database.GetContainer(containerName);
    }
}