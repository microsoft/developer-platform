// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Serialization.Json.Entities;

public static class EntitySerializerOptions
{
    public static JsonSerializerOptions Default { get; } = CreateDefault(isDatabase: false);

    public static JsonSerializerOptions Database { get; } = CreateDefault(isDatabase: true);

    private static JsonSerializerOptions CreateDefault(bool isDatabase)
    {
        var options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };

        AddEntitySerialization(options);

        if (isDatabase)
        {
            options.AddModifiers(EntityModifiers.ForDatabase);
        }

        options.MakeReadOnly();

        return options;
    }

    public static void AddEntitySerialization(JsonSerializerOptions options)
    {
        options.Converters.Add(new ParsableConverter<ProviderKey>());
        options.Converters.Add(new ParsableConverter<EntityKind>());
        options.Converters.Add(new ParsableConverter<EntityName>());
        options.Converters.Add(new ParsableConverter<EntityNamespace>());
        options.Converters.Add(new EntityRefConverter());
        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new EntityConverter());

        options.AddModifiers(EntityModifiers.IgnoreExtraFields);
    }
}
