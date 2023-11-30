// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Developer.Entities;

[Description("Metadata common to all versions/kinds of entities.")]
public class Metadata : PropertiesBase<Metadata>
{
    private static readonly Func<IDictionary<ProviderKey, string>> emptyProviderKeyFactory = static () => new Dictionary<ProviderKey, string>();

    [Required]
    [Description("The name of the entity. Must be unique within the catalog at any given point in time, for any given namespace + kind pair.")]
    [Example("name")]
    [StringLength(63, MinimumLength = 1)]
    [RegularExpression(@"^([A-Za-z0-9][-A-Za-z0-9_.]*)?[A-Za-z0-9]$")]
    public EntityName Name
    {
        get => Get<EntityName>();
        set => Set(value);
    }

    [Description("The namespace that the entity belongs to.")]
    [DefaultValue("default")]
    [Example("namespace")]
    [StringLength(63, MinimumLength = 1)]
    [RegularExpression(@"^[a-z0-9]+(?:\-+[a-z0-9]+)*$")]
    public EntityNamespace Namespace
    {
        get => Get<EntityNamespace>();
        set => Set(value);
    }

    [Required]
    [Description("The ID of the provider of the entity.")]
    [Example("github.com")]
    public string? Provider
    {
        get => Get<string>();
        set => Set(value);
    }

    [Description("A unique identifier for the entity.")]
    [Example("6d96252c-a735-497a-b90e-27fbd799e9c6")]
    public string Uid
    {
        get => Get<string>()!;
        set => Set(value);
    }

    [Description("The tenant ID of the user who created the entity.")]
    [Example("6d96252c-a735-497a-b90e-27fbd799e9c6")]
    public string Tenant
    {
        get => Get<string>()!;
        set => Set(value);
    }

    [Description("A display name for the entity that is suitable for presentation to a user.")]
    public string? Title
    {
        get => Get<string>();
        set => Set(value);
    }

    [Description("A short (typically relatively few words, on one line) description of the entity.")]
    public string? Description
    {
        get => Get<string>();
        set => Set(value);
    }

    [Description("A map of key-value pairs of identifying information for the entity.")]
    public IDictionary<ProviderKey, string> Labels
    {
        get => GetOrCreate(emptyProviderKeyFactory);
        set => Set(value);
    }

    [Description("A map of key-value pairs of non-identifying information for the entity.")]
    public IDictionary<ProviderKey, string> Annotations
    {
        get => GetOrCreate(emptyProviderKeyFactory);
        set => Set(value);
    }

    [Description("A list of external hyperlinks related to the entity.")]
    public IList<Link> Links
    {
        get => GetOrCreate(Factory<Link>.List);
        set => Set(value);
    }

    [AllowNull]
    [Description("A list of single-value tags for the entity.")]
    public IList<string> Tags
    {
        get => GetOrCreate(Factory<string>.List);
        set => Set(value);
    }
}
