// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Identity;

namespace Microsoft.Developer.Azure;

public static class AppTokenCredential
{
    public static TokenCredential GetTokenCredential()
        => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IDENTITY_ENDPOINT")) ? new ManagedIdentityCredential() : new AzureCliCredential();
}