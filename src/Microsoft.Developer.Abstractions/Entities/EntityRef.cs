// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.Developer.Entities;

[Example("{kind}:[{namespace}/]{name}")]
[Description("A reference by name to another entity.")]
[DisplayAsString]
public class EntityRef(EntityKind kind) : IParsable<EntityRef>, IEquatable<EntityRef>, IComparable<EntityRef>, IComparable
{
    private EntityNamespace? @namespace;

    public EntityKind Kind { get; } = kind;

    public required EntityName Name { get; init; }

    [AllowNull]
    public EntityNamespace Namespace
    {
        get => @namespace ?? Entity.Defaults.Namespace;
        init => @namespace = value;
    }

    [IgnoreDataMember]
    [SuppressMessage("Style", "IDE0072:Add missing cases", Justification = "https://github.com/dotnet/roslyn/issues/60784")]
    public string Id
    {
        get
        {
            var isDefaultNamespace = Namespace.Equals(Entity.Defaults.Namespace);
            var isEntityRef = typeof(EntityRef) == GetType();

            return (isEntityRef, isDefaultNamespace) switch
            {
                (true, true) => $"{Kind}:{Name}",
                (true, false) => $"{Kind}:{Namespace}/{Name}",
                (false, true) => Name,
                (false, false) => $"{Namespace}/{Name}",
            };
        }
    }

    public override string ToString() => Id;

    static EntityRef IParsable<EntityRef>.Parse(string s, IFormatProvider? provider)
        => Parse(s, null);

    static bool IParsable<EntityRef>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out EntityRef result)
        => TryParse(s, null, out result);

    public static T Parse<T>(string s) where T : EntityRef, IEntityRef<T>
    {
        var entity = Parse(s, T.DefaultKind);

        return T.Create(entity.Name, entity.Namespace);
    }

    public static EntityRef Parse(string s, EntityKind defaultKind = default)
    {
        if (!TryParse(s, defaultKind, out var result))
        {
            throw new ArgumentException("Id must be in the format of Kind:[Namespace/]Name");
        }

        return result;
    }

    public static bool TryParse<T>([NotNullWhen(true)] string? id, [MaybeNullWhen(false)] out T result)
        where T : EntityRef, IEntityRef<T>
    {
        if (TryParse(id, T.DefaultKind, out var entityRef))
        {
            result = T.Create(entityRef.Name, entityRef.Namespace);
            return true;
        }

        result = null;
        return false;
    }

    public static bool TryParse([NotNullWhen(true)] string? id, [MaybeNullWhen(false)] out EntityRef result)
        => TryParse(id, null, out result);

    public static bool TryParse([NotNullWhen(true)] string? id, EntityKind defaultKind, [MaybeNullWhen(false)] out EntityRef result)
    {
        result = default;

        if (id is null)
        {
            return false;
        }

        var (kind, rest) = id.Split(':') switch
        {
            [{ } r] => (defaultKind, r),
            [{ } k, { } r] => (new(k), r),
            _ => default,
        };

        if (kind.IsEmpty || string.IsNullOrEmpty(rest))
        {
            return false;
        }

        var (name, @namespace) = rest.Split('/') switch
        {
            [{ } n] => (new EntityName(n), new EntityNamespace(Entity.Defaults.Namespace)),
            [{ } ns, { } n] => (new(n), new(ns)),
            _ => default,
        };

        if (name.IsEmpty || @namespace.IsEmpty)
        {
            return false;
        }

        result = new(kind) { Name = name, Namespace = @namespace };

        return true;
    }

    public override bool Equals(object? obj) => obj is EntityRef e && Equals(e);

    public override int GetHashCode() => HashCode.Combine(Kind, Name, Namespace);

    public bool Equals(EntityRef? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Kind.Equals(other.Kind)
            && Name.Equals(other.Name)
            && Namespace.Equals(other.Namespace);
    }

    public int CompareTo(EntityRef? other) => Id.CompareTo(other?.Id);

    public int CompareTo(object? obj) => obj is EntityRef e ? CompareTo(e) : -1;

    public static implicit operator EntityRef(string id) => Parse(id);

    public static implicit operator string(EntityRef entityRef) => entityRef.Id;
}

