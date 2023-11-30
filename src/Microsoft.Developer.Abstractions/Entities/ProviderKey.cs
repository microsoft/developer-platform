// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Developer.Entities;

public readonly struct ProviderKey(string provider, string name) : IEquatable<ProviderKey>, IParsable<ProviderKey>
{
    private const string LocalProvider = "local";

    private string CreateKey(bool ignoreLocal)
    {
        if (ignoreLocal && Provider.Equals(LocalProvider, StringComparison.OrdinalIgnoreCase))
        {
            return Name;
        }
        else
        {
            return $"{Provider}/{Name}";
        }
    }

    public readonly string Name { get; } = name;

    public readonly string Provider { get; } = provider.ToLowerInvariant();

    public readonly string Key => CreateKey(ignoreLocal: true);

    public bool Equals(ProviderKey other)
        => string.Equals(Name, other.Name, StringComparison.Ordinal)
        && string.Equals(Provider, other.Provider, StringComparison.OrdinalIgnoreCase);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is ProviderKey is { } other && Equals(other);

    public override int GetHashCode()
    {
        var code = default(HashCode);

        code.Add(Name, StringComparer.Ordinal);
        code.Add(Provider, StringComparer.OrdinalIgnoreCase);

        return code.ToHashCode();
    }

    public static ProviderKey Parse(string s)
    {
        if (!TryParse(s, out var result))
        {
            throw new ArgumentException("Key must be in the format of [Provider/]Name");
        }

        return result;
    }

    static ProviderKey IParsable<ProviderKey>.Parse(string s, IFormatProvider? provider)
        => Parse(s);

    static bool IParsable<ProviderKey>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ProviderKey result)
        => TryParse(s, out result);

    public static bool TryParse([NotNullWhen(true)] string? s, out ProviderKey result)
    {
        result = default;

        if (s is not null)
        {
            result = s.Split('/') switch
            {
                [{ } name] => new(LocalProvider, name),
                [{ } p, { } name] => new(p.ToLowerInvariant(), name),
                _ => default
            };
        }

        if (string.IsNullOrEmpty(result.Name))
        {
            return false;
        }

        return EntityValidation.IsValidLabelKey(result.CreateKey(ignoreLocal: false));
    }

    public override string ToString() => Key;

    public static implicit operator ProviderKey(string key) => Parse(key);

    public static implicit operator string(ProviderKey key) => key.Key;

    public static bool operator ==(ProviderKey left, ProviderKey right) => left.Equals(right);

    public static bool operator !=(ProviderKey left, ProviderKey right) => !(left == right);
}
