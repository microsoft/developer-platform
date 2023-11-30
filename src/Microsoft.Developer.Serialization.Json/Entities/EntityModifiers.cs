// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Serialization.Json.Entities;

/// <summary>
/// A collection of <see href="https://devblogs.microsoft.com/dotnet/system-text-json-in-dotnet-7/#modifying-json-contracts">modifiers</see> for serialization
/// that can be used in different scenarios for customized behavior
/// </summary>
public static class EntityModifiers
{
    /// <summary>
    /// A serialization modifier that moves <see cref="Metadata.Uid"/> and <see cref="Metadata.Tenant"/> to be on the entity itself during serialization,
    /// as well as handling deserialization to move it back to the metadata. This is helpful for scenarios like Cosmos where an 'id' field is required.
    /// </summary>
    public static void ForDatabase(JsonTypeInfo info)
    {
        const string IdField = "id";
        const string TenantField = "tenant";

        if (info.Type == typeof(Entity))
        {
            // We need to use a custom type to store the id data when deserializing
            // Otherwise, we would not be able to track it as a new metadata object
            // would be created *after* we set it effectively deleting it.
            info.CreateObject = () => new IdEntity(string.Empty);
            info.OnDeserialized = obj =>
            {
                // TODO: should we do any validation/reporting if these values are not the same?
                if (obj is IdEntity entity)
                {
                    entity.Metadata.Uid = entity.Id!;
                    entity.Metadata.Tenant = entity.Tenant!;
                    entity.Id = null;
                    entity.Tenant = null;
                }
            };

            var idProperty = info.CreateJsonPropertyInfo(typeof(string), IdField);
            idProperty.Set = (entity, id) => ((IdEntity)entity).Id = (string?)id;
            idProperty.Get = entity => ((Entity)entity).Metadata.Uid;

            var tenantProperty = info.CreateJsonPropertyInfo(typeof(string), TenantField);
            tenantProperty.Set = (entity, id) => ((IdEntity)entity).Tenant = (string?)id;
            tenantProperty.Get = entity => ((Entity)entity).Metadata.Tenant;

            info.Properties.Insert(0, idProperty);
            info.Properties.Insert(1, tenantProperty);
        }
        else if (info.Type == typeof(Metadata) && info.Converter is IPropertyBagOptions options)
        {
            options.IgnoredKeys.Add(nameof(Metadata.Uid));
            options.IgnoredKeys.Add(nameof(Metadata.Tenant));
        }
    }

    private sealed class IdEntity(string key) : Entity(key)
    {
        public string? Id { get; set; }

        public string? Tenant { get; set; }
    }

    public static void AddModifiers(this JsonSerializerOptions options, params Action<JsonTypeInfo>[] modifiers)
    {
        options.TypeInfoResolver ??= new DefaultJsonTypeInfoResolver();

        foreach (var modifier in modifiers)
        {
            options.TypeInfoResolver = options.TypeInfoResolver.WithAddedModifier(modifier);
        }
    }

    /// <summary>
    /// A modifier that minimizes what is deserialized by removing arrays and property bags that are empty and would otherwise end up with
    /// a serialized value similar to <c>item: []</c> that has unnecessary values that are not needed to deserialize correctly.
    /// </summary>
    public static void IgnoreExtraFields(JsonTypeInfo info)
    {
        IgnoreEmptyRelations(info);
        IgnoreEmptyStatus(info);

        static void IgnoreEmptyStatus(JsonTypeInfo info)
            => IgnoreProperty(info, nameof(Entity.Status), e => e.Status);

        static void IgnoreEmptyRelations(JsonTypeInfo info)
        {
            if (info.Type == typeof(Entity))
            {
                if (GetProperty(info, nameof(Entity.Relations)) is { } property)
                {
                    property.Get = entity =>
                    {
                        var relations = ((Entity)entity).Relations;

                        if (relations.Count == 0)
                        {
                            return null;
                        }

                        return relations;
                    };
                }
            }
        }

        static JsonPropertyInfo? GetProperty(JsonTypeInfo info, string name)
        {
            foreach (var property in info.Properties)
            {
                if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return property;
                }
            }

            return null;
        }

        static void IgnoreProperty(JsonTypeInfo info, string name, Func<Entity, PropertiesBase> func)
        {
            if (info.Type == typeof(Entity))
            {
                if (GetProperty(info, name) is { } property)
                {
                    property.Get = entity =>
                    {
                        var properties = func((Entity)entity);

                        if (properties.Properties.Any())
                        {
                            return properties;
                        }

                        return null;
                    };
                }
            }
        }
    }
}
