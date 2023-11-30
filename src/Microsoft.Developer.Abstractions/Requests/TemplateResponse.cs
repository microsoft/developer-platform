// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Requests;

public class TemplateResponse
{
    [Required]
    [Description("References to the entities created.")]
    [Example("[repo:[{namespace}/]{name}]")]
    public EntityRef[] Entities { get; set; } = [];
}
