/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class ResourceSpec : Spec, IResourceSpec
{
    public string Type { get; set; } = default!;

    public EntityRef Owner { get; set; } = default!;

    public SystemEntityRef? System { get; set; }

    public string Lifecycle { get; set; } = default!;

    public List<EntityRef>? DependsOn { get; set; }

    public List<EntityRef>? DependencyOf { get; set; }
}
