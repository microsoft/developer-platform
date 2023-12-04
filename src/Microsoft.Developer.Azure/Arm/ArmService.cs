// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Security.Claims;
using Azure.Identity;
using Azure.ResourceManager;
using Microsoft.Identity.Abstractions;

namespace Microsoft.Developer.Azure;

public class ArmService : IArmService
{
    private readonly ConcurrentDictionary<string, ArmClient> appClients = new(StringComparer.OrdinalIgnoreCase);

    public ArmEnvironment ArmEnvironment => ArmEnvironment.AzurePublicCloud;

    public static bool IsAzureEnvironment =>
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IDENTITY_ENDPOINT"));

    public ArmClient GetArmClient(string? subscriptionId = null)
    {
        var subscriptionKey = subscriptionId ?? string.Empty;

        if (!appClients.TryGetValue(subscriptionKey, out var armClient))
        {
            armClient = new ArmClient(GetTokenCredential(), subscriptionId);

            appClients[subscriptionKey] = armClient;
        }

        return armClient;
    }

    // public ArmClient GetUserArmClient(ClaimsPrincipal user, string? subscriptionId = null)
    //     => new(GetUserTokenCredential(user), subscriptionId);

    public TokenCredential GetTokenCredential()
        => IsAzureEnvironment ? new ManagedIdentityCredential() : new AzureCliCredential();
}

public class UserArmService(ITokenAcquirerFactory tokenFactory) : IUserArmService
{
    public ArmEnvironment ArmEnvironment => ArmEnvironment.AzurePublicCloud;

    public static bool IsAzureEnvironment =>
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IDENTITY_ENDPOINT"));

    public ArmClient GetArmClient(ClaimsPrincipal user, string? subscriptionId = null)
        => new(GetTokenCredential(user), subscriptionId);

    public TokenCredential GetTokenCredential(ClaimsPrincipal user)
        => new ClaimsPrincipalTokenCredential(user, tokenFactory.GetTokenAcquirer());
}
