// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;

namespace Microsoft.Developer.Entities;

[Example("namespace")]
[Description("The namespace that the entity belongs to.")]
[DisplayAsString(OnlyString = true, MinLength = 1, MaxLength = 63, RegularExpression = @"^[a-z0-9]+(?:\-+[a-z0-9]+)*$", DefaultValue = "default")]
public readonly record struct EntityNamespace : IEquatable<EntityNamespace>, IComparable<EntityNamespace>, IParsable<EntityNamespace>
{
    private readonly string? ns;

    public EntityNamespace(string? ns)
    {
        this.ns = ns?.ToLowerInvariant();
    }

    public bool IsEmpty => string.IsNullOrEmpty(ns);

    public int CompareTo(EntityNamespace other) => StringComparer.OrdinalIgnoreCase.Compare(ToString(), other.ns);

    public bool Equals(EntityNamespace other) => StringComparer.OrdinalIgnoreCase.Equals(ToString(), other.ns);

    public override string ToString() => ns ?? string.Empty;

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(ToString());

    public static implicit operator EntityNamespace(string? @namespace) => new(@namespace);

    public static implicit operator string(EntityNamespace @namespace) => @namespace.ToString();

    static EntityNamespace IParsable<EntityNamespace>.Parse(string s, IFormatProvider? provider) => new(s);

    static bool IParsable<EntityNamespace>.TryParse(string? s, IFormatProvider? provider, out EntityNamespace result)
    {
        result = new(s);
        return true;
    }
}
