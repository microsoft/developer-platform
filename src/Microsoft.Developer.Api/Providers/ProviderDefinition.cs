// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations;

namespace Microsoft.Developer.Api.Providers;

public class ProviderDefinition
{
    [Required]
    public Uri Uri { get; set; } = null!;

    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool Enabled { get; set; }

    public string[] Scopes { get; set; } = [];
}
