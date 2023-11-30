// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security.Claims;

namespace Microsoft.Developer;

public interface IUserScopedServiceFactory<T>
{
    T Create(ClaimsPrincipal user);
}
