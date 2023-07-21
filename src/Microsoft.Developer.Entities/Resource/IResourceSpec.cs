/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public interface IResourceSpec : ISpec
{
    string Type { get; set; }

    EntityRef Owner { get; set; }

    SystemEntityRef? System { get; set; }

    string Lifecycle { get; set; }

    List<EntityRef>? DependsOn { get; set; }

    List<EntityRef>? DependencyOf { get; set; }
}