// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.Developer.Requests;

public class TemplateRequest
{
    [Required]
    [Description("A reference by name to a template.")]
    [Example("template:[{namespace}/]{name}")]
    public EntityRef TemplateRef { get; set; } = null!;

    [Required]
    [Description("The ID of the provider of the template.")]
    public string Provider { get; set; } = null!;

    [Required]
    [Description("The input JSON to the template.")]
    public string InputJson { get; set; } = null!;
}
