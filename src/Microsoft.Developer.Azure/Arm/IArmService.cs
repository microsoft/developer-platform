// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security.Claims;
using Azure.ResourceManager;

namespace Microsoft.Developer.Azure;

public interface IArmService
{
    ArmEnvironment ArmEnvironment { get; }
    ArmClient GetArmClient(string? subscriptionId = null);
    TokenCredential GetTokenCredential();
}

public interface IUserArmService
{
    ArmEnvironment ArmEnvironment { get; }
    ArmClient GetArmClient(ClaimsPrincipal user, string? subscriptionId = null);
    TokenCredential GetTokenCredential(ClaimsPrincipal user);
}
