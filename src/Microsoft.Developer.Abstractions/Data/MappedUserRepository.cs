// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Data;

public class MappedUserRepository<TMappedUser, TLocalUser>(IDocumentRepository<TMappedUser> users) : IMappedUserRepository<TMappedUser, TLocalUser>
    where TMappedUser : MappedUser<TLocalUser>
    where TLocalUser : ILocalUser
{
    public Task<TMappedUser> AddAsync(TMappedUser user, CancellationToken cancellationToken = default)
        => users.AddAsync(user.Tenant, user, cancellationToken);

    public Task<TMappedUser?> GetAsync(string tenant, string id, CancellationToken cancellationToken = default)
        => users.GetAsync(tenant, id, cancellationToken);

    public Task<TMappedUser?> GetAsync(MsDeveloperUserId user, CancellationToken cancellationToken = default)
        => users.GetAsync(user.TenantId, user.UserId, cancellationToken);

    public async Task<TMappedUser?> GetAsync(string tenant, TLocalUser user, CancellationToken cancellationToken = default)
        => await users.QueryAsync(tenant, q => q.Where(q => q.LocalId == user.Id), cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<TMappedUser?> GetAsync(TLocalUser user, CancellationToken cancellationToken = default)
        => await users.QueryAsync(null, q => q.Where(q => q.LocalId == user.Id), cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);

    public IAsyncEnumerable<TMappedUser> QueryAsync(string? tenant, Func<IQueryable<TMappedUser>, IQueryable<TMappedUser>> filter, CancellationToken cancellationToken = default)
        => users.QueryAsync(tenant, filter, cancellationToken);

    public IAsyncEnumerable<TMappedUser> QueryAsync(string? tenant, string query, CancellationToken cancellationToken = default)
        => users.QueryAsync(tenant, query, cancellationToken);

    public Task<bool> RemoveAsync(string tenant, string id, CancellationToken cancellationToken = default)
        => users.RemoveAsync(tenant, id, cancellationToken);

    public Task<bool> RemoveAsync(TMappedUser user, CancellationToken cancellationToken = default)
        => users.RemoveAsync(user.Tenant, user.Id, cancellationToken);

    public Task<bool> RemoveAsync(MsDeveloperUserId user, CancellationToken cancellationToken = default)
        => users.RemoveAsync(user.TenantId, user.UserId, cancellationToken);

    public async Task<bool> RemoveAsync(string tenant, TLocalUser user, CancellationToken cancellationToken = default)
        => await GetAsync(tenant, user, cancellationToken) is { } remove && await RemoveAsync(remove, cancellationToken);

    public async Task<bool> RemoveAsync(TLocalUser user, CancellationToken cancellationToken = default)
        => await GetAsync(user, cancellationToken) is { } remove && await RemoveAsync(remove, cancellationToken);

    public Task<TMappedUser> SetAsync(TMappedUser user, CancellationToken cancellationToken = default)
        => users.SetAsync(user.Tenant, user, cancellationToken);
}
