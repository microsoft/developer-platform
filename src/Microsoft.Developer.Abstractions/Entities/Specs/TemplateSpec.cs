// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;

namespace Microsoft.Developer.Entities;

[Description("The specification data describing the template.")]
public class TemplateSpec : Spec
{
    [Description("A JSON Schema that defines user inputs for a template.")]
    public string? InputJsonSchema
    {
        get => Get<string>();
        set => Set(value);
    }

    public string? InputUiSchema
    {
        get => Get<string>();
        set => Set(value);
    }

    [Description("Details of the entities that the template creates.")]
    public IList<EntityPlan>? Creates
    {
        get => GetOrCreate(Factory<EntityPlan>.List);
        set => Set(value);
    }
}
