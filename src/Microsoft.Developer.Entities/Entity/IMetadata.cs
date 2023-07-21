/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public interface IMetadata : IAdditionalProperties
{
    string Name { get; set; }

    string Uid { get; set; }

    string Tenant { get; set; }

    string? Namespace { get; set; }

    string? Title { get; set; }

    string? Description { get; set; }

    Dictionary<ProviderKey, string>? Labels { get; set; }

    Dictionary<ProviderKey, string>? Annotations { get; set; }

    List<string>? Tags { get; set; }

    List<Link>? Links { get; set; }
}