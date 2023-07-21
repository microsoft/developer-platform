/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Text.Json.Nodes;

namespace Microsoft.Developer.Entities;

public class GroupSpec : Spec, IGroupSpec
{
    public string Type { get; set; } = default!;

    public JsonNode? Profile { get; set; }

    public GroupEntityRef? Parent { get; set; }

    public List<GroupEntityRef> Children { get; set; } = new();

    public List<UserEntityRef>? Members { get; set; }
}
