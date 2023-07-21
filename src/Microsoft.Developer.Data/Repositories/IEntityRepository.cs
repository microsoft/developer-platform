/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Data;

public interface IEntityRepository<T>
    where T : class, IEntity, new()
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    Task<T?> GetAsync(string partitionId, string entityId, CancellationToken cancellationToken = default);

    IAsyncEnumerable<T> ListAsync(string partitionId, CancellationToken cancellationToken = default);

    Task<T?> RemoveAsync(T entity, CancellationToken cancellationToken = default);

    Task<T> SetAsync(T entity, CancellationToken cancellationToken = default);
}