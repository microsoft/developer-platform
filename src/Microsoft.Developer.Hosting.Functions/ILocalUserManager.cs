// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer;

/// <summary>
/// Non-generic counterpart of <see cref="ILocalUserManager{TUser}"/> for use when the type parameter of the user is not known.
/// </summary>
public interface ILocalUserManager
{
    Task<bool> HasLocalUserAsync(MsDeveloperUserId user, CancellationToken token);
}

/// <summary>
/// An interface that allows the system to translate between a local <typeparamref name="TUser"/> and
/// the Developer Platform user.
/// </summary>
public interface ILocalUserManager<TUser> : ILocalUserManager
{
    Task<TUser?> GetLocalUserAsync(MsDeveloperUserId user, CancellationToken token);

    Task<MsDeveloperUserId?> GetMsDeveloperUserAsync(TUser user, CancellationToken token);
}