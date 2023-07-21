/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public interface IComponentSpec : ISpec
{
    string Type { get; set; }

    string Lifecycle { get; set; }

    EntityRef Owner { get; set; }

    // SystemEntityRef? System { get; set; }

    // ComponentEntityRef? SubcomponentOf { get; set; }

    // List<ApiEntityRef>? ProvidesApis { get; set; }

    // List<ApiEntityRef>? ConsumesApis { get; set; }

    List<EntityRef>? DependsOn { get; set; }
}