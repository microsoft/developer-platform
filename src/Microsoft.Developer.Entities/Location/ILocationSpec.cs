/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public interface ILocationSpec : ISpec
{
    string? Type { get; set; }

    string? Target { get; set; }

    List<string>? Targets { get; set; }

    string Presence { get; set; }
}