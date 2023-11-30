// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.Developer.Entities;

[Description("A directed relation from one entity to another.")]
public class Relation
{
    [MinLength(1)]
    [Description("The type of relation.")]
    public required string Type { get; set; }

    public required EntityRef TargetRef { get; set; }
}
