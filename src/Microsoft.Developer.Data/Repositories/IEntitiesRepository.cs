/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Data;

public interface IEntitiesRepository
{
    Task<T> AddAsync<T>(T entity, CancellationToken cancellationToken = default)
        where T : class, IEntity, new();

    Task<T?> GetAsync<T>(string partitionId, string entityId, CancellationToken cancellationToken = default)
        where T : class, IEntity, new();

    IAsyncEnumerable<T> ListAsync<T>(string partitionId, CancellationToken cancellationToken = default)
        where T : class, IEntity, new();

    Task<T?> RemoveAsync<T>(T entity, CancellationToken cancellationToken = default)
        where T : class, IEntity, new();

    Task<T> SetAsync<T>(T entity, CancellationToken cancellationToken = default)
        where T : class, IEntity, new();
}
