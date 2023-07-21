/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.Azure.Cosmos;
using Microsoft.Developer.Configuration.Options;
using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Data.CosmosDb;

public class CosmosEntitiesRepository : IEntitiesRepository
{
    private readonly ICosmosDbService cosmosService;

    private CosmosClient cosmos => cosmosService.Client;

    private CosmosOptions options => cosmosService.Options;

    public CosmosEntitiesRepository(ICosmosDbService cosmosService)
    {
        this.cosmosService = cosmosService;
    }

    protected static QueryRequestOptions GetQueryRequestOptions(PartitionKey partitionKey, QueryRequestOptions? options = null)
    {
        if (options is not null) options.PartitionKey = partitionKey;

        return options ?? new QueryRequestOptions { PartitionKey = partitionKey };
    }

    protected static QueryRequestOptions GetQueryRequestOptions(string partitionKeyValue, QueryRequestOptions? options = null)
        => GetQueryRequestOptions(GetPartitionKey(partitionKeyValue), options);

    protected static QueryRequestOptions GetQueryRequestOptions(IEntity entity, QueryRequestOptions? options = null)
        => GetQueryRequestOptions(GetPartitionKey(entity.Tenant), options);

    protected static PartitionKey GetPartitionKey(string partitionKeyValue)
        => new(partitionKeyValue);

    protected static PartitionKey GetPartitionKey<T>(T entity)
        where T : class, IEntity, new()
        => GetPartitionKey(entity.Tenant);

    protected Task<Container> GetContainerAsync(CancellationToken cancellationToken = default)
        => cosmosService.GetEntitiesContainerAsync(options.DatabaseName, cancellationToken);

    public async Task<T> AddAsync<T>(T entity, CancellationToken cancellationToken)
        where T : class, IEntity, new()
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        var response = await container
            .CreateItemAsync<IEntity>(entity, GetPartitionKey(entity), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return response.Resource as T ?? throw new InvalidOperationException($"Failed to add {typeof(T)}.");
    }

    public async Task<T?> GetAsync<T>(string partitionId, string entityId, CancellationToken cancellationToken)
        where T : class, IEntity, new()
    {
        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            var response = await container
                .ReadItemAsync<IEntity>(entityId, GetPartitionKey(partitionId), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return response.Resource as T ?? throw new InvalidOperationException($"Failed to add {typeof(T)}.");
        }
        catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async IAsyncEnumerable<T> ListAsync<T>(string partitionId, [EnumeratorCancellation] CancellationToken cancellationToken)
        where T : class, IEntity, new()
    {
        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        var query = new QueryDefinition($"SELECT * FROM e");

        var entites = container
            .GetItemQueryIterator<IEntity>(query, requestOptions: GetQueryRequestOptions(partitionId))
            .ReadAllAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await foreach (var entity in entites)
            yield return entity as T ?? throw new InvalidOperationException($"Failed to add {typeof(T)}.");

    }

    public async Task<T?> RemoveAsync<T>(T entity, CancellationToken cancellationToken)
        where T : class, IEntity, new()
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            var response = await container
                .DeleteItemAsync<IEntity>(entity.Id, GetPartitionKey(entity), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return response.Resource as T ?? throw new InvalidOperationException($"Failed to add {typeof(T)}.");
        }
        catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == HttpStatusCode.NotFound)
        {
            return null; // already deleted
        }
    }

    public async Task<T> SetAsync<T>(T entity, CancellationToken cancellationToken)
        where T : class, IEntity, new()
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        var response = await container
            .UpsertItemAsync<IEntity>(entity, GetPartitionKey(entity), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return response.Resource as T ?? throw new InvalidOperationException($"Failed to add {typeof(T)}.");

    }
}