/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class ComponentSpec : Spec, IComponentSpec
{
    public string Type { get; set; } = default!;

    public string Lifecycle { get; set; } = default!;

    public EntityRef Owner { get; set; } = default!;

    // public SystemEntityRef? System { get; set; }

    // public ComponentEntityRef? SubcomponentOf { get; set; }

    // public List<ApiEntityRef>? ProvidesApis { get; set; }

    // public List<ApiEntityRef>? ConsumesApis { get; set; }

    public List<EntityRef>? DependsOn { get; set; }
}
