// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Data;

public sealed class EntitiesRepository(string tenantId, IDocumentRepository<Entity> entities) : IEntitiesRepository
{
    public Task<Entity> AddAsync(Entity entity, CancellationToken cancellationToken = default)
    {
        ValidateTenant(entity);
        return entities.AddAsync(entity.Metadata.Tenant, entity, cancellationToken);
    }

    public Task<Entity?> GetAsync(string entityId, CancellationToken cancellationToken = default)
        => entities.GetAsync(tenantId, entityId, cancellationToken);

    public IAsyncEnumerable<Entity> ListAsync(EntityKind kind, CancellationToken cancellationToken = default)
        => entities.QueryAsync(tenantId, q => q.Where(q => q.Kind == kind), cancellationToken);

    public IAsyncEnumerable<Entity> QueryAsync(Func<IQueryable<Entity>, IQueryable<Entity>> filter, CancellationToken cancellationToken = default)
        => entities.QueryAsync(tenantId, filter, cancellationToken);

    public Task<bool> RemoveAsync(Entity entity, CancellationToken cancellationToken = default)
    {
        ValidateTenant(entity);
        return entities.RemoveAsync(entity.Metadata.Tenant, entity.Metadata.Uid, cancellationToken);
    }

    public Task<bool> RemoveAsync(string id, CancellationToken cancellationToken = default)
        => entities.RemoveAsync(tenantId, id, cancellationToken);

    public Task<Entity> SetAsync(Entity entity, CancellationToken cancellationToken = default)
    {
        ValidateTenant(entity);
        return entities.SetAsync(tenantId, entity, cancellationToken);
    }

    private void ValidateTenant(string tenant)
    {
        if (!string.Equals(tenant, tenantId, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Must only operate on expected tenant.");
        }
    }

    private void ValidateTenant(Entity entity)
    {
        if (string.IsNullOrEmpty(entity.Metadata.Tenant))
        {
            entity.Metadata.Tenant = tenantId;
        }
        else
        {
            ValidateTenant(entity.Metadata.Tenant);
        }
    }
}
