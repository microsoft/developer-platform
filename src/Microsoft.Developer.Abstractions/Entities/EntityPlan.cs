// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Microsoft.Developer.Entities;

public class EntityPlan
{
    private EntityNamespace? @namespace;

    [Description("The kind of the resulting entity.")]
    public EntityKind Kind { get; set; }

    [Description("The namespace that the resulting entity will belong to.")]
    [AllowNull]
    public EntityNamespace Namespace
    {
        get => @namespace ?? Entity.Defaults.Namespace;
        init => @namespace = value;
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("A map of key-value pairs of identifying information for the resulting entity.")]
    public IDictionary<ProviderKey, string>? Labels { get; set; }
}