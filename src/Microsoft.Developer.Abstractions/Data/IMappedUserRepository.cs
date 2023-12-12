// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Data;

public interface IMappedUserRepository<TMappedUser, TLocalUser>
    where TMappedUser : MappedUser<TLocalUser>
    where TLocalUser : ILocalUser
{
    Task<TMappedUser> AddAsync(TMappedUser user, CancellationToken cancellationToken = default);

    Task<TMappedUser?> GetAsync(string tenant, string id, CancellationToken cancellationToken = default);

    Task<TMappedUser?> GetAsync(MsDeveloperUserId user, CancellationToken cancellationToken = default);

    Task<TMappedUser?> GetAsync(string tenant, TLocalUser user, CancellationToken cancellationToken = default);

    Task<TMappedUser?> GetAsync(TLocalUser user, CancellationToken cancellationToken = default);

    IAsyncEnumerable<TMappedUser> QueryAsync(string? tenant, Func<IQueryable<TMappedUser>, IQueryable<TMappedUser>> filter, CancellationToken cancellationToken = default);

    IAsyncEnumerable<TMappedUser> QueryAsync(string? tenant, string query, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(string tenant, string id, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(TMappedUser user, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(MsDeveloperUserId user, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(string tenant, TLocalUser user, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(TLocalUser user, CancellationToken cancellationToken = default);

    Task<TMappedUser> SetAsync(TMappedUser user, CancellationToken cancellationToken = default);
}