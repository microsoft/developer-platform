// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security.Claims;
using Microsoft.Identity.Abstractions;

namespace Microsoft.Developer.Azure;

public interface IUserTokenCredentialFactory
{
    TokenCredential GetTokenCredential(ClaimsPrincipal user);
}

public sealed class UserTokenCredentialFactory(ITokenAcquirerFactory tokenFactory) : IUserTokenCredentialFactory
{
    public TokenCredential GetTokenCredential(ClaimsPrincipal user) => new UserTokenCredential(user, tokenFactory.GetTokenAcquirer());
}