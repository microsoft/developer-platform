/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class LocationSpec : Spec, ILocationSpec
{
    public string? Type { get; set; }

    public string? Target { get; set; }

    public List<string>? Targets { get; set; }

    public string Presence { get; set; } = "required";
}