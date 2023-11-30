// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.Developer.Entities;

public partial class Entity(EntityKind kind)
{
    private Metadata? metadata;
    private Spec? spec;
    private Status? status;
    private IList<Relation>? relations;

    [Required]
    [StringLength(253, MinimumLength = 1)]
    [Description("The version of specification format for this particular entity that this is written against.")]
    [Example(Defaults.ApiVersion)]
    [DefaultValue(Defaults.ApiVersion)]
    public string ApiVersion { get; init; } = Defaults.ApiVersion;

    [Required]
    public EntityKind Kind { get; init; } = kind;

    [Required]
    public Metadata Metadata
    {
        get => metadata ??= new();
        init => metadata = value;
    }

    public EntityRef GetEntityRef() => new(Kind) { Name = Metadata.Name, Namespace = Metadata.Namespace };

    [Required]
    [Description("The specification data describing the entity itself.")]
    public Spec Spec
    {
        get => spec ??= new();
        init => spec = value;
    }

    public Status Status
    {
        get => status ??= new();
        set => status = value;
    }

    [Description("The relations that this entity has with other entities.")]
    public IList<Relation> Relations
    {
        get => relations ??= new List<Relation>();
        set => relations = value;
    }

    public static Entity CreateTemplate()
        => new(EntityKind.Template);
}
