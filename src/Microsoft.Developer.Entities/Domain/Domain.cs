/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class Domain : Entity<DomainMetadata, DomainSpec, DomainStatus>, IDomain<DomainMetadata, DomainSpec, DomainStatus>
{
    public override string Kind => nameof(Domain);
}
