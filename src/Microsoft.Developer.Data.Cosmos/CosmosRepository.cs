// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Net;
using System.Runtime.CompilerServices;

namespace Microsoft.Developer.Data.Cosmos;

internal sealed class CosmosRepository<T>(Func<CancellationToken, ValueTask<Container>> containerFactory) : IDocumentRepository<T>
{
    private static QueryRequestOptions GetQueryRequestOptions(PartitionKey partitionKey, QueryRequestOptions? options = null)
    {
        if (options is not null)
        {
            options.PartitionKey = partitionKey;
        }

        return options ?? new QueryRequestOptions { PartitionKey = partitionKey };
    }

    private static QueryRequestOptions GetQueryRequestOptions(string partitionKeyValue, QueryRequestOptions? options = null)
        => GetQueryRequestOptions(GetPartitionKey(partitionKeyValue), options);

    private static PartitionKey GetPartitionKey(string partitionKeyValue)
        => new(partitionKeyValue);

    private async ValueTask<Container> GetContainerAsync(CancellationToken cancellationToken = default) => await containerFactory(cancellationToken);

    public async Task<T> AddAsync(string partitionId, T entity, CancellationToken cancellationToken)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);
        var response = await container
            .CreateItemAsync(entity, new(partitionId), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return response.Resource;
    }

    public async Task<T?> GetAsync(string partitionId, string entityId, CancellationToken cancellationToken)
    {
        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            var response = await container
                .ReadItemAsync<T>(entityId, GetPartitionKey(partitionId), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return response.Resource;
        }
        catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
    }

    public async IAsyncEnumerable<T> QueryAsync(string partitionId, Func<IQueryable<T>, IQueryable<T>> queryFilter, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        var query = container
            .GetItemLinqQueryable<T>(requestOptions: GetQueryRequestOptions(partitionId));

        await foreach (var item in queryFilter(query).ToFeedIterator().ReadAllAsync(cancellationToken: cancellationToken))
        {
            yield return item;
        }
    }

    public async Task<bool> RemoveAsync(string partitionId, string id, CancellationToken cancellationToken = default)
    {
        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            var response = await container
                .DeleteItemAsync<T>(id, new(partitionId), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return response.Resource is not null;
        }
        catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == HttpStatusCode.NotFound)
        {
            return default; // already deleted
        }
    }

    public async Task<T> SetAsync(string partitionId, T entity, CancellationToken cancellationToken)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        var response = await container
            .UpsertItemAsync(entity, new(partitionId), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return response.Resource;
    }
}
