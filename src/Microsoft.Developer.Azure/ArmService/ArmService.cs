/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Collections.Concurrent;
using Azure.ResourceManager;

namespace Microsoft.Developer.Azure;

public interface IArmService
{
    ArmEnvironment ArmEnvironment { get; }

    TokenCredential GetTokenCredential();

    ArmClient GetArmClient(string? subscriptionId = null);
}

public abstract class ArmService : IArmService
{
    private readonly ConcurrentDictionary<string, ArmClient> clients = new(StringComparer.OrdinalIgnoreCase);

    public ArmEnvironment ArmEnvironment => ArmEnvironment.AzurePublicCloud;

    public static bool IsAzureEnvironment =>
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IDENTITY_ENDPOINT"));

    public ArmClient GetArmClient(string? subscriptionId = null)
    {
        var subscriptionKey = subscriptionId ?? string.Empty;

        if (!clients.TryGetValue(subscriptionKey, out var armClient))
        {
            armClient = new ArmClient(GetTokenCredential(), subscriptionId);

            clients[subscriptionKey] = armClient;
        }

        return armClient;
    }

    public virtual TokenCredential GetTokenCredential() => throw new NotImplementedException();
}