/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Azure.Identity;

namespace Microsoft.Developer.Azure;

public interface IAppArmService : IArmService { }

public class AppArmService : ArmService, IAppArmService
{
    public override TokenCredential GetTokenCredential()
        => IsAzureEnvironment ? new ManagedIdentityCredential() : new AzureCliCredential();
}
