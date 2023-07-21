/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Microsoft.Azure.Cosmos;
using Microsoft.Developer.Configuration.Options;
using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Data.CosmosDb;

public interface ICosmosEntityRepository
{
    Type EntityType { get; }
}

public abstract class CosmosEntityRepository<T> : ICosmosEntityRepository, IEntityRepository<T>
    where T : class, IEntity, new()
{
    private readonly ICosmosDbService cosmosService;

    private CosmosClient cosmos => cosmosService.Client;

    private CosmosOptions options => cosmosService.Options;

    protected CosmosEntityRepository(ICosmosDbService cosmosService)
    {
        this.cosmosService = cosmosService;
    }

    public Type EntityType { get; } = typeof(T);

    protected QueryRequestOptions GetQueryRequestOptions(PartitionKey partitionKey, QueryRequestOptions? options = null)
    {
        if (options is not null) options.PartitionKey = partitionKey;

        return options ?? new QueryRequestOptions { PartitionKey = partitionKey };
    }

    protected QueryRequestOptions GetQueryRequestOptions(string partitionKeyValue, QueryRequestOptions? options = null)
        => GetQueryRequestOptions(GetPartitionKey(partitionKeyValue), options);

    protected QueryRequestOptions GetQueryRequestOptions(T entity, QueryRequestOptions? options = null)
        => GetQueryRequestOptions(GetPartitionKey(entity), options);

    protected PartitionKey GetPartitionKey(string partitionKeyValue)
        => new(partitionKeyValue);

    protected PartitionKey GetPartitionKey(T entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var partitionKeyValue = PartitionKeyAttribute.GetValue(entity) ?? entity.Tenant
            ?? throw new ArgumentException($"{typeof(T)} does not provide a partition key.");

        return GetPartitionKey(partitionKeyValue);
    }

    protected int GetSoftDeleteTTL()
    {
        var softDeleteTTL = SoftDeleteAttribute.GetSoftDeleteTTL<T>();

        if (!softDeleteTTL.HasValue)
            throw new ArgumentException($"{typeof(T)} does not provide a value soft delete TTL.");

        return softDeleteTTL.Value;
    }

    protected Task<Container> GetContainerAsync(CancellationToken cancellationToken = default)
        => cosmosService.GetEntityContainerAsync<T>(options.DatabaseName, cancellationToken);

    public abstract Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    public abstract Task<T?> GetAsync(string partitionId, string entityId, CancellationToken cancellationToken = default);

    public abstract IAsyncEnumerable<T> ListAsync(string partitionId, CancellationToken cancellationToken = default);

    public abstract Task<T?> RemoveAsync(T entity, CancellationToken cancellationToken = default);

    public abstract Task<T> SetAsync(T entity, CancellationToken cancellationToken = default);
}