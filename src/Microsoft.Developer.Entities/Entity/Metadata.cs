/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public abstract class Metadata : IMetadata
{
    // private string? name;

    [Slugify]
    public string Name { get; set; } = null!;
    // {
    //     get => name ?? throw new InvalidOperationException("Name is null");
    //     set => name = IName.CreateSlug(value);
    // }

    public string Uid { get; set; } = null!;

    [ApiIgnore]
    public string Tenant { get; set; } = null!;

    public string? Namespace { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public Dictionary<ProviderKey, string>? Labels { get; set; } = new();

    public Dictionary<ProviderKey, string>? Annotations { get; set; } = new();

    public List<string>? Tags { get; set; } = new();

    public List<Link>? Links { get; set; } = new();

    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}