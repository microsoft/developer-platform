/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.Azure.Cosmos;

using User = Microsoft.Developer.Entities.User;

namespace Microsoft.Developer.Data.CosmosDb;

public class CosmosUserRepository : CosmosEntityRepository<User>, IUserRepository
{
    public CosmosUserRepository(ICosmosDbService cosmosService) : base(cosmosService)
    {
    }

    public override async Task<User> AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        var response = await container
            .CreateItemAsync(entity, GetPartitionKey(entity), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return response.Resource;
    }

    public override async Task<User?> GetAsync(string partitionId, string entityId, CancellationToken cancellationToken = default)
    {
        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            var response = await container
                .ReadItemAsync<User>(entityId, GetPartitionKey(partitionId), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return response.Resource;
        }
        catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public override async IAsyncEnumerable<User> ListAsync(string partitionId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        var query = new QueryDefinition($"SELECT * FROM u");

        var users = container
            .GetItemQueryIterator<User>(query, requestOptions: GetQueryRequestOptions(partitionId))
            .ReadAllAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await foreach (var user in users)
            yield return user;
    }

    public override async Task<User?> RemoveAsync(User entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            var response = await container
                .DeleteItemAsync<User>(entity.Id, GetPartitionKey(entity), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return response.Resource;
        }
        catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == HttpStatusCode.NotFound)
        {
            return null; // already deleted
        }
    }


    public override async Task<User> SetAsync(User entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        var response = await container
            .UpsertItemAsync(entity, GetPartitionKey(entity), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return response.Resource;
    }
}
