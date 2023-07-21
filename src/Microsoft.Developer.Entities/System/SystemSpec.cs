/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class SystemSpec : Spec, ISystemSpec
{
    public EntityRef Owner { get; set; } = default!;

    public DomainEntityRef? Domain { get; set; }
}
