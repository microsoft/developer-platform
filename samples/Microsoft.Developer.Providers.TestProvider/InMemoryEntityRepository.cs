using Microsoft.Developer.Data;
using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Providers.TestProvider;

public class InMemoryEntityRepository : IEntitiesRepository, IEntitiesRepositoryFactory
{
    public Task<Entity> AddAsync(Entity entity, CancellationToken cancellationToken = default) => Task.FromResult(entity);

    public Task<Entity?> GetAsync(string entityId, CancellationToken cancellationToken = default) => Task.FromResult<Entity?>(null);

    public IAsyncEnumerable<Entity> ListAsync(EntityKind kind, CancellationToken cancellationToken = default) => AsyncEnumerable.Empty<Entity>();

    public IAsyncEnumerable<Entity> QueryAsync(Func<IQueryable<Entity>, IQueryable<Entity>> filter, CancellationToken cancellationToken = default) => AsyncEnumerable.Empty<Entity>();

    public Task<bool> RemoveAsync(Entity entity, CancellationToken cancellationToken = default) => Task.FromResult(false);

    public Task<bool> RemoveAsync(string id, CancellationToken cancellationToken = default) => Task.FromResult(false);

    public Task<Entity> SetAsync(Entity entity, CancellationToken cancellationToken = default) => Task.FromResult(entity);

    IEntitiesRepository IEntitiesRepositoryFactory.Create(string tenantId) => this;
}
