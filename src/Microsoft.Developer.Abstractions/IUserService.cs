// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Entities;
using System.Security.Claims;

namespace Microsoft.Developer;

public interface IUserService
{
    ValueTask<Entity?> GetOrCreateUser(ClaimsPrincipal principal);

    ValueTask<string?> GetUserIdAsync(string identifier);
}
