/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class DomainSpec : Spec, IDomainSpec
{
    public EntityRef Owner { get; set; } = default!;
}
