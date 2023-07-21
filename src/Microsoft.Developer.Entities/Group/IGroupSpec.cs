/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Text.Json.Nodes;

namespace Microsoft.Developer.Entities;

public interface IGroupSpec : ISpec
{
    string Type { get; set; }

    JsonNode? Profile { get; set; }

    GroupEntityRef? Parent { get; set; }

    List<GroupEntityRef> Children { get; set; }

    List<UserEntityRef>? Members { get; set; }
}