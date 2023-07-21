/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Data.CosmosDb;

public class CosmosProjectRepository : CosmosEntityRepository<Project>, IProjectRepository
{
    private readonly IMemoryCache cache;

    public CosmosProjectRepository(ICosmosDbService cosmosService, IMemoryCache cache) : base(cosmosService)
    {
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<string?> ResolveIdAsync(string tenantId, string identifier)
    {
        var key = $"{tenantId}_{identifier}";

        if (!cache.TryGetValue(key, out string? id) && !string.IsNullOrEmpty(id))
        {
            var project = await GetAsync(tenantId, identifier)
                .ConfigureAwait(false);

            id = project?.Id;

            if (!string.IsNullOrEmpty(id))
                cache.Set(key, cache, TimeSpan.FromMinutes(10));
        }

        return id;
    }

    private void RemoveCachedIds(Project project)
    {
        // cache.Remove($"{project.Tenant}_{project.DisplayName}");
        cache.Remove($"{project.Tenant}_{project.Metadata.Name}");
        cache.Remove($"{project.Tenant}_{project.Id}");
    }


    public override async Task<Project> AddAsync(Project entity, CancellationToken cancellationToken = default)
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

    public override async Task<Project?> GetAsync(string partitionId, string entity, CancellationToken cancellationToken = default)
    {
        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        Project? project = null;

        try
        {
            var response = await container
                .ReadItemAsync<Project>(entity, GetPartitionKey(partitionId), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            project = response.Resource;
        }
        catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == HttpStatusCode.NotFound)
        {
            var query = new QueryDefinition($"SELECT * FROM e WHERE e.metadata.name = @identifier OFFSET 0 LIMIT 1")
                .WithParameter("@identifier", entity.ToLowerInvariant());

            var queryIterator = container
                .GetItemQueryIterator<Project>(query, requestOptions: GetQueryRequestOptions(partitionId));

            if (queryIterator.HasMoreResults)
            {
                var queryResults = await queryIterator
                    .ReadNextAsync(cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                project = queryResults.FirstOrDefault();
            }
        }

        return project;
    }

    public override async IAsyncEnumerable<Project> ListAsync(string partitionId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        var query = new QueryDefinition($"SELECT * FROM p");

        var projects = container
            .GetItemQueryIterator<Project>(query, requestOptions: GetQueryRequestOptions(partitionId))
            .ReadAllAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await foreach (var project in projects)
            yield return project;
    }

    public async IAsyncEnumerable<Project> ListAsync(string partitionId, IEnumerable<string> entityIds, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        var query = new QueryDefinition($"SELECT * FROM p WHERE ARRAY_CONTAINS(@ids, p.id) OR ARRAY_CONTAINS(@names, p.metadata.name)")
            .WithParameter("@ids", JsonSerializer.Serialize(entityIds.ToArray()))
            .WithParameter("@names", JsonSerializer.Serialize(entityIds.Select(item => item?.ToLowerInvariant()).ToArray()));

        var projects = container
            .GetItemQueryIterator<Project>(query, requestOptions: GetQueryRequestOptions(partitionId))
            .ReadAllAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await foreach (var project in projects)
            yield return project;
    }


    public override async Task<Project?> RemoveAsync(Project entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var container = await GetContainerAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            var response = await container
                .DeleteItemAsync<Project>(entity.Id, GetPartitionKey(entity), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            RemoveCachedIds(entity);

            // await userRepository
            //     .RemoveProjectMembershipsAsync(project.Organization, project.Id)
            //     .ConfigureAwait(false);

            return response.Resource;
        }
        catch (CosmosException cosmosEx) when (cosmosEx.StatusCode == HttpStatusCode.NotFound)
        {
            return null; // already deleted
        }
    }


    public override async Task<Project> SetAsync(Project entity, CancellationToken cancellationToken = default)
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
