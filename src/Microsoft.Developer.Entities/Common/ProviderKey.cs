/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

[JsonConverter(typeof(ProviderKeyJsonConverter))]
public readonly struct ProviderKey //: IEqualityComparer<ProviderKey>
{
    const string LocalProvider = "local";

    public ProviderKey(string key)
    {
        var parts = key.Split('/');

        if (parts.Length < 1 || parts.Length > 2)
            throw new ArgumentException("Key must be in the format of [Provider/]Name");

        Provider = parts.Length == 2 ? parts[0].ToLower() : LocalProvider;
        Name = parts.Length == 2 ? parts[1] : parts[0];
    }

    public readonly string Name { get; }

    public readonly string Provider { get; } = LocalProvider;

    public readonly string Key
        => Provider.Equals(LocalProvider, StringComparison.OrdinalIgnoreCase)
           ? Name : $"{Provider.ToLower()}/{Name}";

    public static implicit operator ProviderKey(string key) => new(key);

    public static implicit operator string(ProviderKey key) => key.Key;
}
