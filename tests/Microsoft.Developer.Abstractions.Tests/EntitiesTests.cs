// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities.Tests;

public class EntitiesTests
{
    private const string Kind = "MyKind";
    private const string Kind2 = "MyKind2";
    private const string Name = "Name";
    private const string Name2 = "Name2";
    private const string Hello = "hello";

    [Fact]
    public void KindTest()
    {
        // Arrange
        var entity = new Entity(Kind);

        // Act
        var kind = entity.Kind;

        // Assert
        Assert.Equal(Kind, kind);
    }

    [Fact]
    public void MetadataNameThroughOtherType()
    {
        // Arrange
        var entity = new Entity(Kind);
        entity.Metadata.Name = Name;

        // Act
        var name = entity.Metadata.As<MyMetadata>().Name;

        // Assert
        Assert.Equal(Name, name);
    }

    [Fact]
    public void MetadataNameChangedThroughOtherType()
    {
        // Arrange
        var entity = new Entity(Kind);
        entity.Metadata.Name = Name;

        // Act
        entity.Metadata.As<MyMetadata>().Name = Name2;

        // Assert
        Assert.Equal(Name2, entity.Metadata.Name);
    }

    [Fact]
    public void OtherMetadataItemIsNotLost()
    {
        // Arrange
        var entity = new Entity(Kind);

        // Act
        var myMetadata = entity.Metadata.As<MyMetadata>();
        myMetadata.Hello = Hello;
        var result = myMetadata.As<Metadata>().As<MyMetadata>();

        // Assert
        Assert.Equal(Hello, result.Hello);
    }

    [Fact]
    public void LabelsInitialization()
    {
        // Arrange
        var key = ProviderKey.Parse("someId");
        var value = "someValue";
        var entity = new Entity(EntityKind.Template)
        {
            Metadata =
            {
                Labels = new Dictionary<ProviderKey, string>
                {
                    { key, value }
                }
            }
        };

        // Act
        var result = entity.Metadata.Labels[key];

        // Assert
        Assert.Equal(value, result);
    }

    public class MyMetadata : Metadata
    {
        public string? Hello
        {
            get => Get<string>();
            set => Set(value);
        }
    }

    public class MySpec : Spec
    {
        public int Value
        {
            get => Get<int>();
            set => Set(value);
        }
    }
}
