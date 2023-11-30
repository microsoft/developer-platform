// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Entities;
using Microsoft.Developer.Serialization.Json.Entities;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Microsoft.Developer.Serialization.Json.Tests;

public class EntitiesSerializationTests
{
    private const string String1 = "link1";
    private const string String2 = "link2";

    private static readonly Link link1 = new()
    {
        Url = String1,
        Title = String1,
        Icon = String1,
        Type = String1
    };

    private static readonly Link link2 = new()
    {
        Url = String2,
    };

    [Fact]
    public void ReadKind()
    {
        // Arrange
        var kind = new EntityKind("EntityKind");
        var json = /*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "{{kind}}",
              "metadata": {
                "Hello": "there"
              }
            }
            """;

        // Act
        var entity = Deserialize(json);

        // Assert
        Assert.Equal(kind, entity.Kind);
    }

    [Fact]
    public void StronglyTyped()
    {
        // Arrange
        var kind = new EntityKind("EntityKind");
        var entity = new Entity(kind);
        entity.Metadata.As<Metadata<string>>().Example = "world";
        entity.Spec.As<MySpec>().Value = 5;

        // Act
        var result = Serialize(entity);

        // Assert
        Assert.Equal(/*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "{{kind}}",
              "metadata": {
                "{{Metadata<string>.PropertyName}}": "world"
              },
              "spec": {
                "value": 5
              }
            }
            """, result);
    }

    [Fact]
    public void RoundTripsExtraMetadata()
    {
        // Arrange
        var json = /*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "someKind",
              "metadata": {
                "hello": "there"
              },
              "spec": {}
            }
            """;

        // Act
        var entity = Deserialize(json);
        var result = Serialize(entity!);

        // Assert
        Assert.Equal(json, result);
    }

    [Fact]
    public void ReadExtraMetadata()
    {
        // Arrange
        var kind = new EntityKind("EntityKind");
        var json = /*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "{{kind}}",
              "metadata": {
                "{{Metadata<string>.PropertyName}}": "there"
              }
            }
            """;

        // Act
        var entity = Deserialize(json).Metadata.As<Metadata<string>>();

        // Assert
        Assert.Equal("there", entity.Example);
    }

    [Fact]
    public void NoLoss()
    {
        // Arrange
        var kind = new EntityKind("EntityKind");
        var entity = new Entity(kind);

        // Act
        var result = Serialize(entity);

        // Assert
        Assert.Equal(/*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "{{kind}}",
              "metadata": {},
              "spec": {}
            }
            """, result);
    }

    [Fact]
    public void SerializeIntProperty()
    {
        // Arrange
        var kind = new EntityKind("EntityKind");
        var entity = new Entity(kind);
        entity.Metadata.Properties["key"] = 1;

        // Act
        var result = Serialize(entity);

        // Assert
        Assert.Equal(/*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "{{kind}}",
              "metadata": {
                "key": 1
              },
              "spec": {}
            }
            """, result);
    }

    [Fact]
    public void DeserializeExtraData()
    {
        // Arrange
        var json = /*lang=json*/ """
            {
              "metadata": {
                "test": "value"
              }
            }
            """;
        const string expected = /*lang=json*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "",
              "metadata": {
                "test": "value"
              },
              "spec": {}
            }
            """;

        // Act
        var deserialized = Deserialize(json);
        var result = Serialize(deserialized);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void DeserializeExtraComplex()
    {
        // Arrange
        var value = "hello";
        var json = /*lang=json*/ $$"""
            {
              "metadata": {
                "{{Metadata<ComplexObject>.PropertyName}}": {
                  "name": "{{value}}"
                }
              }
            }
            """;

        // Act
        var entity = Deserialize(json);
        var obj = entity.Metadata.As<Metadata<ComplexObject>>().Example!;

        // Assert
        Assert.Equal(value, obj.Name);
    }

    [Fact]
    public void UpdateComplex()
    {
        // Arrange
        var before = /*lang=json*/ $$"""
            {
              "metadata": {
                "{{Metadata<ComplexObject>.PropertyName}}": {
                  "{{nameof(ComplexObject.Name)}}": "hello"
                }
              }
            }
            """;
        var after = /*lang=json*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "",
              "metadata": {
                "{{Metadata<ComplexObject>.PropertyName}}": {
                  "{{nameof(ComplexObject.Name).ToLower()}}": "hello2"
                }
              },
              "spec": {}
            }
            """;

        // Act
        var entity = Deserialize(before);
        entity.Metadata.As<Metadata<ComplexObject>>().Example!.Name = "hello2";
        var result = Serialize(entity);

        // Assert
        Assert.Equal(after, result);
    }

    [Fact]
    public void GetTags()
    {
        // Arrange
        var json = /*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "",
              "metadata": {
                "Tags": [
                    "tag1"
                ]
              }
            }
            """;

        // Act
        var entity = Deserialize(json);

        // Assert
        Assert.Collection(entity.Metadata.Tags,
            t => Assert.Equal("tag1", t));
    }

    [Fact]
    public void NoTags()
    {
        // Arrange
        var json = /*lang=json,strict*/ """
            {
            }
            """;

        // Act
        var entity = Deserialize(json);

        // Assert
        Assert.Empty(entity.Metadata.Tags);
    }

    [Fact]
    public void NoExistingTagsAddOne()
    {
        // Arrange
        var json = /*lang=json,strict*/ """
            {
            }
            """;
        const string expected = /*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "",
              "metadata": {
                "tags": [
                  "{{String1}}"
                ]
              },
              "spec": {}
            }
            """;

        // Act
        var entity = Deserialize(json);
        entity.Metadata.Tags.Add(String1);
        var result = Serialize(entity);

        // Assert
        Assert.Collection(entity.Metadata.Tags,
            l => Assert.Equal(String1, l));
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ExistingTagsAddOne()
    {
        // Arrange
        var json = /*lang=json,strict*/ $$"""
            {
              "metadata": {
                "Tags": [
                  "{{String1}}"
                ]
              }
            }
            """;
        const string expected = /*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "",
              "metadata": {
                "tags": [
                  "{{String1}}",
                  "{{String2}}"
                ]
              },
              "spec": {}
            }
            """;

        // Act
        var entity = Deserialize(json);
        entity.Metadata.Tags.Add(String2);
        var result = Serialize(entity);

        // Assert
        Assert.Collection(entity.Metadata.Tags,
            l => Assert.Equal(String1, l),
            l => Assert.Equal(String2, l));
        Assert.Equal(expected, result);
    }

    [Fact]
    public void NoLinks()
    {
        // Arrange
        var json = /*lang=json,strict*/ """
            {
            }
            """;

        // Act
        var entity = Deserialize(json);

        // Assert
        Assert.Empty(entity.Metadata.Links);
    }

    [Fact]
    public void NoExistingLinksAddOne()
    {
        // Arrange
        var json = /*lang=json,strict*/ """
            {
            }
            """;
        var expected = /*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "",
              "metadata": {
                "links": [
                  {
                    "url": "{{link1.Url}}",
                    "title": "{{link1.Title}}",
                    "icon": "{{link1.Icon}}",
                    "type": "{{link1.Type}}"
                  }
                ]
              },
              "spec": {}
            }
            """;

        // Act
        var entity = Deserialize(json);
        entity.Metadata.Links.Add(link1);
        var result = Serialize(entity);

        // Assert
        Assert.Collection(entity.Metadata.Links,
            l => Assert.Equal(link1, l));
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ExistingLinksAddOne()
    {
        // Arrange
        var json = /*lang=json,strict*/ $$"""
            {
              "metadata": {
                "links": [
                  {
                    "url": "{{link1.Url}}",
                    "title": "{{link1.Title}}",
                    "icon": "{{link1.Icon}}",
                    "type": "{{link1.Type}}"
                  }
                ]
              }
            }
            """;
        var expected = /*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "",
              "metadata": {
                "links": [
                  {
                    "url": "{{link1.Url}}",
                    "title": "{{link1.Title}}",
                    "icon": "{{link1.Icon}}",
                    "type": "{{link1.Type}}"
                  },
                  {
                    "url": "{{link2.Url}}"
                  }
                ]
              },
              "spec": {}
            }
            """;

        // Act
        var entity = Deserialize(json);
        entity.Metadata.Links.Add(link2);
        var result = Serialize(entity);

        // Assert
        Assert.Collection(entity.Metadata.Links,
            l => Assert.Equal(link1, l),
            l => Assert.Equal(link2, l));
        Assert.Equal(expected, result);
    }

    [Fact]
    public void SerializeIdToTop()
    {
        // Arrange
        const string Id = "someId";
        var kind = new EntityKind("EntityKind");
        var entity = new Entity(kind)
        {
            Metadata =
            {
                Uid = Id,
            }
        };

        // Act
        var result = Serialize(entity, EntityModifiers.ForDatabase);

        // Assert
        var expected = /*lang=json,strict*/ $$"""
            {
              "id": "{{Id}}",
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "{{kind}}",
              "metadata": {},
              "spec": {}
            }
            """;

        Assert.Equal(expected, result);
    }

    [Fact]
    public void DeserializeIdToMetadataFromTop()
    {
        // Arrange
        const string Id = "someId";
        const string Serialized = /*lang=json,strict*/ $$"""
            {
              "id": "{{Id}}"
            }
            """;

        // Act
        var entity = Deserialize(Serialized, EntityModifiers.ForDatabase);

        // Assert
        Assert.Equal(Id, entity.Metadata.Uid);
    }

    [Fact]
    public void DeserializeIdToMetadataFromTopWithMetadataIdExisting()
    {
        // Arrange
        const string Id = "someId";
        const string Serialized = /*lang=json,strict*/ $$"""
            {
              "id": "{{Id}}",
              "metadata": {
                "uuid": "someOtherId"
              }
            }
            """;

        // Act
        var entity = Deserialize(Serialized, EntityModifiers.ForDatabase);

        // Assert
        Assert.Equal(Id, entity.Metadata.Uid);
    }

    [Fact]
    public void DeserializeIdToMetadataFromTopWithMetadataItems()
    {
        // Arrange
        const string Id = "someId";
        const string Serialized = /*lang=json,strict*/ $$"""
            {
              "id": "{{Id}}",
              "metadata": {
                "item": "value"
              }
            }
            """;

        // Act
        var entity = Deserialize(Serialized, EntityModifiers.ForDatabase);

        // Assert
        Assert.Equal(Id, entity.Metadata.Uid);
    }

    [Fact]
    public void SerializeTenantToTop()
    {
        // Arrange
        const string Id = "someId";
        var kind = new EntityKind("EntityKind");
        var entity = new Entity(kind)
        {
            Metadata =
            {
                Tenant = Id,
            }
        };

        // Act
        var result = Serialize(entity, EntityModifiers.ForDatabase);

        // Assert
        var expected = /*lang=json,strict*/ $$"""
            {
              "tenant": "{{Id}}",
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "{{kind}}",
              "metadata": {},
              "spec": {}
            }
            """;

        Assert.Equal(expected, result);
    }

    [Fact]
    public void DeserializeTenantToMetadataFromTop()
    {
        // Arrange
        const string Id = "someId";
        const string Serialized = /*lang=json,strict*/ $$"""
            {
              "tenant": "{{Id}}"
            }
            """;

        // Act
        var entity = Deserialize(Serialized, EntityModifiers.ForDatabase);

        // Assert
        Assert.Equal(Id, entity.Metadata.Tenant);
    }

    [Fact]
    public void DeserializeTenantToMetadataFromTopWithMetadataIdExisting()
    {
        // Arrange
        const string Id = "someId";
        const string Serialized = /*lang=json,strict*/ $$"""
            {
              "tenant": "{{Id}}",
              "metadata": {
                "tenant": "someOtherId"
              }
            }
            """;

        // Act
        var entity = Deserialize(Serialized, EntityModifiers.ForDatabase);

        // Assert
        Assert.Equal(Id, entity.Metadata.Tenant);
    }

    [Fact]
    public void DeserializeTenantToMetadataFromTopWithMetadataItems()
    {
        // Arrange
        const string Id = "someId";
        const string Serialized = /*lang=json,strict*/ $$"""
            {
              "tenant": "{{Id}}",
              "metadata": {
                "item": "value"
              }
            }
            """;

        // Act
        var entity = Deserialize(Serialized, EntityModifiers.ForDatabase);

        // Assert
        Assert.Equal(Id, entity.Metadata.Tenant);
    }

    [Fact]
    public void EntityRefSerialize()
    {
        // Arrange
        var kind = new EntityKind("EntityKind");
        var entity = new Entity(kind);
        var entityRef = new EntityRef("kind") { Name = "entityId", Namespace = "namespace" };
        entity.Metadata.As<Metadata<EntityRef>>().Example = entityRef;

        // Act
        var result = Serialize(entity);

        // Assert
        Assert.Equal(/*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "{{kind}}",
              "metadata": {
                "example": "{{entityRef}}"
              },
              "spec": {}
            }
            """, result);
    }

    [Fact]
    public void EntityRefDeserialize()
    {
        // Arrange
        var kind = new EntityKind("kind");
        const string Name = "name";
        const string Namespace = "namespace";
        var json =/*lang=json,strict*/ $$"""
            {
              "apiVersion": "",
              "kind": "",
              "metadata": {
                "example": "{{kind}}:{{Namespace}}/{{Name}}"
              }
            }
            """;

        // Act
        var result = Deserialize(json);

        // Assert
        var entityRef = result.Metadata.As<Metadata<EntityRef>>().Example;

        Assert.NotNull(entityRef);
        Assert.Equal(kind, entityRef.Kind);
        Assert.Equal(Name, entityRef.Name);
        Assert.Equal(Namespace, entityRef.Namespace);
    }

    [Fact]
    public void CustomEntityRefSerialize()
    {
        // Arrange
        const string Name = "entityId";
        const string Namespace = "namespace";

        var kind = new EntityKind("EntityKind");
        var entity = new Entity(kind);
        var entityRef = new CustomEntityRef { Name = Name, Namespace = Namespace };
        entity.Metadata.As<Metadata<CustomEntityRef>>().Example = entityRef;

        // Act
        var result = Serialize(entity);

        // Assert
        Assert.Equal(/*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "{{kind}}",
              "metadata": {
                "example": "{{Namespace}}/{{Name}}"
              },
              "spec": {}
            }
            """, result);
    }

    [Fact]
    public void CustomEntityRefDeserialize()
    {
        // Arrange
        const string Name = "name";
        const string Namespace = "namespace";
        var json =/*lang=json,strict*/ $$"""
            {
              "apiVersion": "",
              "kind": "",
              "metadata": {
                "example": "{{Namespace}}/{{Name}}"
              }
            }
            """;

        // Act
        var result = Deserialize(json);

        // Assert
        var entityRef = result.Metadata.As<Metadata<CustomEntityRef>>().Example;

        Assert.NotNull(entityRef);
        Assert.IsType<CustomEntityRef>(entityRef);
        Assert.Equal(CustomEntityRef.DefaultKind, entityRef.Kind);
        Assert.Equal(Name, entityRef.Name);
        Assert.Equal(Namespace, entityRef.Namespace);
    }

    [Fact]
    public void SerializerChecksDerivedEntityRef()
    {
        // Arrange
        const string Name = "name";
        const string Namespace = "namespace";
        var json =/*lang=json,strict*/ $$"""
            {
              "apiVersion": "",
              "kind": "",
              "metadata": {
                "example": "{{Namespace}}/{{Name}}"
              }
            }
            """;
        var deserialized = Deserialize(json);

        // Act/Assert
        Assert.Throws<InvalidOperationException>(() => deserialized.Metadata.As<Metadata<CustomEntityRefWithNoIEntityRef>>().Example);
    }

    private class CustomEntityRef : EntityRef, IEntityRef<CustomEntityRef>
    {
        public CustomEntityRef()
            : base(DefaultKind)
        {
        }

        public static EntityKind DefaultKind => "custom";

        public static CustomEntityRef Create(EntityName name, EntityNamespace @namespace = default) => new() { Name = name, Namespace = @namespace };
    }

    private class CustomEntityRefWithNoIEntityRef : EntityRef
    {
        public CustomEntityRefWithNoIEntityRef()
            : base("custom2")
        {
        }
    }

    [Fact]
    public void ProviderKeySerialize()
    {
        // Arrange
        var kind = new EntityKind("EntityKind");
        var entity = new Entity(kind);
        var provider = new ProviderKey("provider", "name");
        entity.Metadata.As<ValueMetadata<ProviderKey>>().Example = provider;

        // Act
        var result = Serialize(entity);

        // Assert
        Assert.Equal(/*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "{{kind}}",
              "metadata": {
                "example": "{{provider.Provider}}/{{provider.Name}}"
              },
              "spec": {}
            }
            """, result);
    }

    [Fact]
    public void ProviderKeyDeserialize()
    {
        // Arrange
        const string Provider = "provider";
        const string Name = "name";
        var json =/*lang=json,strict*/ $$"""
            {
              "apiVersion": "",
              "kind": "",
              "metadata": {
                "example": "{{Provider}}/{{Name}}"
              }
            }
            """;

        // Act
        var result = Deserialize(json);

        // Assert
        var provider = result.Metadata.As<ValueMetadata<ProviderKey>>().Example;

        Assert.Equal(Name, provider.Name);
        Assert.Equal(Provider, provider.Provider);
    }

    [Fact]
    public void SerializeProviderKeyInLabel()
    {
        // Arrange
        const string Provider = "provider";
        const string Name = "name";
        const string Value = "value";

        var kind = new EntityKind("EntityKind");
        var providerKey = new ProviderKey(Provider, Name);
        var entity = new Entity(kind)
        {
            Metadata =
            {
                Labels =
                {
                    { providerKey, Value }
                }
            }
        };

        // Act
        var result = Serialize(entity);

        // Assert
        Assert.Equal(/*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "{{kind}}",
              "metadata": {
                "labels": {
                  "{{Provider}}/{{Name}}": "{{Value}}"
                }
              },
              "spec": {}
            }
            """, result);
    }

    [Fact]
    public void DeserializeProviderKeyInLabel()
    {
        // Arrange
        const string Provider = "provider";
        const string Name = "name";
        const string Value = "value";
        var json = /*lang=json,strict*/ $$"""
            {
              "metadata": {
                "labels": {
                  "{{Provider}}/{{Name}}": "{{Value}}"
                }
              }
            }
            """;

        // Act
        var result = Deserialize(json);

        // Assert
        Assert.Collection(result.Metadata.Labels,
            item =>
            {
                Assert.Equal(Provider, item.Key.Provider);
                Assert.Equal(Name, item.Key.Name);
                Assert.Equal(Value, item.Value);
            });
    }

    [Fact]
    public void UserRoleSerialize()
    {
        // Arrange
        var kind = new EntityKind("EntityKind");
        var entity = new Entity(kind);
        var role = UserRole.Provider;
        entity.Metadata.As<ValueMetadata<UserRole>>().Example = role;

        // Act
        var result = Serialize(entity);

        // Assert
        Assert.Equal(/*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "{{kind}}",
              "metadata": {
                "example": "{{role}}"
              },
              "spec": {}
            }
            """, result);
    }

    [Fact]
    public void UserRoleDeserialize()
    {
        // Arrange
        var role = UserRole.Provider;
        var json =/*lang=json,strict*/ $$"""
            {
              "apiVersion": "",
              "kind": "",
              "metadata": {
                "example": "{{role}}"
              }
            }
            """;

        // Act
        var result = Deserialize(json);

        // Assert
        Assert.Equal(role, result.Metadata.As<ValueMetadata<UserRole>>().Example);
    }

    [Fact]
    public void DeserializeEntityRefTest()
    {
        // Arrange
        var kind = "kind";
        var name = "name";
        var @namespace = "namespace";
        var json = /*lang=json,strict*/ $$"""
            {
              "kind": "{{kind}}",
              "name": "{{name}}",
              "namespace": "{{@namespace}}"
            }
            """;

        // Act
        var entityRef = JsonSerializer.Deserialize<EntityRef>(json, CreateOptions());

        // Assert
        Assert.NotNull(entityRef);
        Assert.Equal(kind, entityRef.Kind);
        Assert.Equal(name, entityRef.Name);
        Assert.Equal(@namespace, entityRef.Namespace);
    }

    [Fact]
    public void DeserializeCustomEntityRefTest()
    {
        // Arrange
        var name = "name";
        var @namespace = "namespace";
        var json = /*lang=json,strict*/ $$"""
            {
              "name": "{{name}}",
              "namespace": "{{@namespace}}"
            }
            """;

        // Act
        var entityRef = JsonSerializer.Deserialize<CustomEntityRef>(json, CreateOptions());

        // Assert
        Assert.NotNull(entityRef);
        Assert.Equal(CustomEntityRef.DefaultKind, entityRef.Kind);
        Assert.Equal(name, entityRef.Name);
        Assert.Equal(@namespace, entityRef.Namespace);
    }

    [Fact]
    public void SerializeNameAndNamespace()
    {
        // Arrange
        const string kind = "Test";
        const string name = "name";
        const string @namespace = "namespace";

        var entity = new Entity(kind);
        entity.Metadata.Name = name;
        entity.Metadata.Namespace = @namespace;

        // Act
        var result = Serialize(entity);

        // Assert
        Assert.Equal(/*lang=json,strict*/ $$"""
            {
              "apiVersion": "{{Entity.Defaults.ApiVersion}}",
              "kind": "{{kind}}",
              "metadata": {
                "name": "{{name}}",
                "namespace": "{{@namespace}}"
              },
              "spec": {}
            }
            """, result);
    }

    public static Entity Deserialize(string s, params Action<JsonTypeInfo>[] modifiers)
    {
        return JsonSerializer.Deserialize<Entity>(s, CreateOptions(modifiers)) ?? throw new InvalidOperationException("No result");
    }

    public static string Serialize(Entity? obj, params Action<JsonTypeInfo>[] modifiers)
    {
        return JsonSerializer.Serialize(obj, CreateOptions(modifiers));
    }

    private static JsonSerializerOptions CreateOptions(params Action<JsonTypeInfo>[] modifiers)
    {
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        EntitySerializerOptions.AddEntitySerialization(options);

        options.AddModifiers(modifiers);

        return options;
    }

    private class Metadata<T> : Metadata
        where T : class
    {
        public static readonly string PropertyName = nameof(Example).ToLower();

        public T? Example
        {
            get => Get<T>();
            set => Set(value);
        }
    }

    private class ValueMetadata<T> : Metadata
        where T : struct
    {
        public static readonly string PropertyName = nameof(Example).ToLower();

        public T Example
        {
            get => Get<T>();
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

    public class ComplexObject
    {
        public string Name { get; set; } = null!;
    }
}
