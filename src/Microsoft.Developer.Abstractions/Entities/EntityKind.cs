// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;

namespace Microsoft.Developer.Entities;

[Description("The high level entity type being described.")]
[Example("kind")]
[DisplayAsString(OnlyString = true, MinLength = 1, MaxLength = 63, RegularExpression = @"^[a-zA-Z][a-z0-9A-Z]*$")]
public readonly record struct EntityKind(string? kind) : IEquatable<EntityKind>, IComparable<EntityKind>, IParsable<EntityKind>
{
    public static EntityKind User { get; } = nameof(User);

    public static EntityKind Template { get; } = nameof(Template);

    public static EntityKind Environment { get; } = nameof(Environment);

    public static EntityKind Repo { get; } = nameof(Repo);

    private readonly string? kind = kind;

    public bool IsEmpty => string.IsNullOrEmpty(kind);

    public int CompareTo(EntityKind other) => StringComparer.OrdinalIgnoreCase.Compare(ToString(), other.kind);

    public bool Equals(EntityKind other) => StringComparer.OrdinalIgnoreCase.Equals(ToString(), other.kind);

    public override string ToString() => kind ?? string.Empty;

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(ToString());

    public static implicit operator EntityKind(string? kind) => new(kind);

    public static implicit operator string(EntityKind kind) => kind.ToString();

    static EntityKind IParsable<EntityKind>.Parse(string s, IFormatProvider? provider) => new(s);

    static bool IParsable<EntityKind>.TryParse(string? s, IFormatProvider? provider, out EntityKind result)
    {
        result = new(s);
        return true;
    }
}
