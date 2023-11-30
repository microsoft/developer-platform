// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Data;

public interface IEntitiesRepository
{
    IAsyncEnumerable<Entity> ListAsync(EntityKind kind, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Entity entity, CancellationToken cancellationToken = default);

    Task<Entity> AddAsync(Entity entity, CancellationToken cancellationToken = default);

    Task<Entity?> GetAsync(string entityId, CancellationToken cancellationToken = default);

    IAsyncEnumerable<Entity> QueryAsync(Func<IQueryable<Entity>, IQueryable<Entity>> filter, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(string id, CancellationToken cancellationToken = default);

    Task<Entity> SetAsync(Entity entity, CancellationToken cancellationToken = default);
}
