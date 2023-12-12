// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer;

public class MappedUser<TLocalUser>
    where TLocalUser : ILocalUser
{
    public string Id => PlatformUser.UserId;

    public string Tenant => PlatformUser.TenantId;

    public string LocalId => LocalUser.Id;

    public string PlatformId => PlatformUser.Id;

    public required TLocalUser LocalUser { get; set; }

    public required MsDeveloperUserId PlatformUser { get; set; }

    /// <summary>
    /// Gets or sets a value that tracks the key to the refresh token stored in the secrets manager.
    /// </summary>
    public string? OauthTokenSecretName { get; set; }

    /// <summary>
    /// Gets or sets the time until the refresh token will expire.
    /// </summary>
    public DateTimeOffset Expiration { get; set; }
}