// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;

namespace Microsoft.Developer.Entities;

[Example("name")]
[Description("The name of the entity. Must be unique within the catalog at any given point in time, for any given namespace + kind pair.")]
[DisplayAsString(OnlyString = true, MinLength = 1, MaxLength = 63, RegularExpression = @"^([A-Za-z0-9][-A-Za-z0-9_.]*)?[A-Za-z0-9]$")]
public readonly record struct EntityName(string? name) : IEquatable<EntityName>, IComparable<EntityName>, IParsable<EntityName>
{
    private readonly string? name = name;

    public bool IsEmpty => string.IsNullOrEmpty(name);

    public bool StartsWith(string prefix) => name?.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ?? false;

    public bool EndsWith(string suffix) => name?.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) ?? false;

    public EntityName Replace(string oldValue, string newValue) => name?.Replace(oldValue, newValue, StringComparison.OrdinalIgnoreCase);

    public int CompareTo(EntityName other) => StringComparer.OrdinalIgnoreCase.Compare(ToString(), other.name);

    public bool Equals(EntityName other) => StringComparer.OrdinalIgnoreCase.Equals(ToString(), other.name);

    public override string ToString() => name ?? string.Empty;

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(ToString());

    public static implicit operator EntityName(string? name) => new(name);

    public static implicit operator string(EntityName name) => name.ToString();

    static EntityName IParsable<EntityName>.Parse(string s, IFormatProvider? provider) => new(s);

    static bool IParsable<EntityName>.TryParse(string? s, IFormatProvider? provider, out EntityName result)
    {
        result = new(s);
        return true;
    }
}
