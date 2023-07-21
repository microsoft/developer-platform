/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class Provider : Entity<ProviderMetadata, ProviderSpec, ProviderStatus>, IProvider<ProviderMetadata, ProviderSpec, ProviderStatus>
{
    public override string Kind => nameof(Provider);
}
