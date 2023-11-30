// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Microsoft.Developer.Entities;

[Description("External hyperlink related to an entity.")]
public class Link : IEquatable<Link>
{
    [Required]
    [Description("A url in a standard uri format")]
    [Example("https://example.com/some/page")]
    public string Url { get; set; } = null!;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("A user friendly display name for the link.")]
    [Example("Documentation")]
    public string? Title { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("A key representing a visual icon to be displayed in the UI.")]
    [Example("file")]
    public string? Icon { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("An optional value to categorize links into specific groups.")]
    [Example("docs")]
    public string? Type { get; set; }

    public bool Equals(Link? other) => string.Equals(Url, other?.Url, StringComparison.Ordinal);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Link is { } other && Equals(other);

    public override int GetHashCode()
    {
        var code = default(HashCode);

        code.Add(Url, StringComparer.OrdinalIgnoreCase);

        return code.ToHashCode();
    }
}
