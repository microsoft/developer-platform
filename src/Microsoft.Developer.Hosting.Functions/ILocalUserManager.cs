// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer;

/// <summary>
/// Non-generic counterpart of <see cref="ILocalUserManager{TLocalUser}"/> for use when the type parameter of the user is not known.
/// </summary>
public interface ILocalUserManager
{
    Task<bool> HasLocalUserAsync(MsDeveloperUserId user, CancellationToken token);
}

/// <summary>
/// An interface that allows the system to translate between a local <typeparamref name="TMappedUser"/> and
/// the Developer Platform user.
/// </summary>
public interface ILocalUserManager<TMappedUser, TLocalUser> : ILocalUserManager
    where TMappedUser : MappedUser<TLocalUser>
    where TLocalUser : ILocalUser
{
    Task<TLocalUser?> GetLocalUserAsync(MsDeveloperUserId user, CancellationToken token);

    Task<MsDeveloperUserId?> GetMsDeveloperUserAsync(TLocalUser user, CancellationToken token);

    Task<TMappedUser?> GetMappedUserAsync(MsDeveloperUserId user, CancellationToken token);
}