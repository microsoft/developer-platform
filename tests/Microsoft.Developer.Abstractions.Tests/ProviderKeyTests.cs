// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities.Tests;

public class ProviderKeyTests
{
    [InlineData("multiple/slashes/key")]
    [InlineData("/")]
    [InlineData("/test")]
    [InlineData("/test/other")]
    [InlineData("/test/other/more")]
    [Theory]
    public void InvalidKeys(string key)
    {
        Assert.Throws<ArgumentException>(() => ProviderKey.Parse(key));
    }

    [InlineData("provider/name", "provider", "name", "provider/name")]
    [InlineData("Provider/nAme", "provider", "nAme", "provider/nAme")]
    [InlineData("name", "local", "name", "name")]
    [Theory]
    public void Parsing(string originalKey, string provider, string name, string key)
    {
        // Act
        var result = ProviderKey.Parse(originalKey);

        // Assert
        Assert.Equal(provider, result.Provider);
        Assert.Equal(name, result.Name);
        Assert.Equal(key, result.Key);
    }

    [InlineData("provider", "name", "provider/name")]
    [InlineData("Provider", "name", "provider/name")]
    [InlineData("Provider", "Name", "provider/Name")]
    [Theory]
    public void FromConstructor(string provider, string name, string key)
    {
        // Act
        var result = new ProviderKey(provider, name);

        // Assert
        Assert.Equal(provider.ToLowerInvariant(), result.Provider);
        Assert.Equal(name, result.Name);
        Assert.Equal(key, result.Key);
    }

    [InlineData("provider/name", "provider/name")]
    [InlineData("provider/name", "Provider/name")]
    [Theory]
    public void EqualityCheck(string key1, string key2)
    {
        // Act
        var result1 = ProviderKey.Parse(key1);
        var result2 = ProviderKey.Parse(key2);

        // Assert
        Assert.Equal(result1, result2);
        Assert.Equal(result1.GetHashCode(), result2.GetHashCode());
    }

    [InlineData("provider/name", "provider/Name")]
    [InlineData("provider/name", "other/name")]
    [InlineData("provider/other", "provider/name")]
    [Theory]
    public void InequalityCheck(string key1, string key2)
    {
        // Act
        var result1 = ProviderKey.Parse(key1);
        var result2 = ProviderKey.Parse(key2);

        // Assert
        Assert.NotEqual(result1, result2);
        Assert.NotEqual(result1.GetHashCode(), result2.GetHashCode());
    }
}
