// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities.Tests;

public class EntityRefTests
{
    [InlineData("test", null)]
    [InlineData(null, null)]
    [InlineData(":", null)]
    [InlineData(":/", null)]
    [InlineData("k:/", null)]
    [InlineData("k:n/", null)]
    [InlineData("namespace/name/other", null)]
    [Theory]
    public void ParseTestsFails(string? id, string? defaultKind)
    {
        Assert.Throws<ArgumentException>(() => EntityRef.Parse(id!, defaultKind));
        Assert.False(EntityRef.TryParse(id, defaultKind, out _));
    }

    [InlineData("kind:name", null, "kind", "name", Entity.Defaults.Namespace)]
    [InlineData("name", "kind", "kind", "name", Entity.Defaults.Namespace)]
    [InlineData("namespace/name", "kind", "kind", "name", "namespace")]
    [Theory]
    public void ParseTests(string id, string? defaultKind, string kind, string name, string @namespace)
    {
        // Parse
        var parseResult = EntityRef.Parse(id, defaultKind);

        Assert.Equal(kind, parseResult.Kind);
        Assert.Equal(name, parseResult.Name);
        Assert.Equal(@namespace, parseResult.Namespace);

        // TryParse
        Assert.True(EntityRef.TryParse(id, defaultKind, out var tryParseResult));
        Assert.Equal(kind, tryParseResult.Kind);
        Assert.Equal(name, tryParseResult.Name);
        Assert.Equal(@namespace, tryParseResult.Namespace);
    }

    [InlineData("k:ns/n", "k:ns/n")]
    [InlineData("k:ns/n", "K:ns/n")]
    [InlineData("k:ns/n", "k:NS/n")]
    [InlineData("k:ns/n", "k:ns/N")]
    [Theory]
    public void EqualityCheck(string id1, string id2)
    {
        // Arrange
        var ref1 = EntityRef.Parse(id1);
        var ref2 = EntityRef.Parse(id2);

        // Assert
        Assert.Equal(ref1, ref2);
        Assert.Equal(ref1.GetHashCode(), ref2.GetHashCode());
    }

    [InlineData("k:ns/n", "k:ns2/n")]
    [InlineData("k:ns/n", "k:ns/n2")]
    [InlineData("k:ns/n", "k2:ns/n")]
    [Theory]
    public void InequalityCheck(string id1, string id2)
    {
        // Arrange
        var ref1 = EntityRef.Parse(id1);
        var ref2 = EntityRef.Parse(id2);

        // Assert
        Assert.NotEqual(ref1, ref2);
        Assert.NotEqual(ref1.GetHashCode(), ref2.GetHashCode());
    }

    [InlineData("name", nameof(DerivedEntityRef), "name", "default")]
    [InlineData("namespace/name", nameof(DerivedEntityRef), "name", "namespace")]
    [InlineData("DerivedEntityRef:namespace/name", nameof(DerivedEntityRef), "name", "namespace")]
    [InlineData("OtherKind:namespace/name", nameof(DerivedEntityRef), "name", "namespace")]
    [Theory]
    public void DeriveEntityRefTests(string id, string kind, string name, string @namespace)
    {
        // Parse
        var parseResult = EntityRef.Parse<DerivedEntityRef>(id);
        Assert.Equal(kind, parseResult.Kind);
        Assert.Equal(name, parseResult.Name);
        Assert.Equal(@namespace, parseResult.Namespace);

        // TryParse
        Assert.True(EntityRef.TryParse<DerivedEntityRef>(id, out var tryParseResult));
        Assert.Equal(kind, tryParseResult.Kind);
        Assert.Equal(name, tryParseResult.Name);
        Assert.Equal(@namespace, tryParseResult.Namespace);
    }

    private class DerivedEntityRef : EntityRef, IEntityRef<DerivedEntityRef>
    {
        public DerivedEntityRef()
            : base(DefaultKind)
        {
        }

        public static EntityKind DefaultKind => nameof(DerivedEntityRef);

        public static DerivedEntityRef Create(EntityName name, EntityNamespace @namespace = default) => new() { Name = name, Namespace = @namespace };
    }
}
