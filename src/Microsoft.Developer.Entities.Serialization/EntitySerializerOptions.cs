/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities.Serialization;

public static class EntitySerializerOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        // TypeInfoResolver =
        // ReferenceHandler =
        // ReadCommentHandling =
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        // PropertyNameCaseInsensitive =
        // NumberHandling
        // MaxDepth =
        // IncludeFields = true,
        // IgnoreReadOnlyProperties =
        // IgnoreReadOnlyFields
        // Encoder =
        // UnknownTypeHandling =
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        // DefaultBufferSize =
        Converters = {
            new JsonStringEnumConverter()
        },
        // AllowTrailingCommas =
        WriteIndented = true
    };

    public static readonly JsonSerializerOptions API = new(Default)
    {
        // TypeInfoResolver = new DefaultJsonTypeInfoResolver // EntityTypeResolver
        TypeInfoResolver = new EntityTypeResolver
        {
            Modifiers = {
                ApiIgnoreModifier.Modify,
                SlugifyModifier.Modify
            }
        }
    };

    public static readonly JsonSerializerOptions Database = new(Default)
    {
        // TypeInfoResolver = new DefaultJsonTypeInfoResolver // EntityTypeResolver
        TypeInfoResolver = new EntityTypeResolver
        {
            Modifiers = {
                DatabaseIgnoreModifier.Modify,
                SlugifyModifier.Modify
            }
        }
    };

    public static JsonSerializerOptions WithConverters(IList<JsonConverter> converters)
    {
        var options = new JsonSerializerOptions(Default);

        foreach (var converter in converters)
            options.Converters.Add(converter);

        return options;
    }

    public static void AddEntitySerialization(this JsonSerializerOptions options)
    {
        options.TypeInfoResolver = API.TypeInfoResolver;
        options.ReferenceHandler = API.ReferenceHandler;
        options.ReadCommentHandling = API.ReadCommentHandling;
        options.PropertyNamingPolicy = API.PropertyNamingPolicy;
        options.PropertyNameCaseInsensitive = API.PropertyNameCaseInsensitive;
        options.MaxDepth = API.MaxDepth;
        options.IncludeFields = API.IncludeFields;
        options.IgnoreReadOnlyProperties = API.IgnoreReadOnlyProperties;
        options.IgnoreReadOnlyFields = API.IgnoreReadOnlyFields;
        options.Encoder = API.Encoder;
        options.UnknownTypeHandling = API.UnknownTypeHandling;
        options.DictionaryKeyPolicy = API.DictionaryKeyPolicy;
        options.DefaultIgnoreCondition = API.DefaultIgnoreCondition;
        options.DefaultBufferSize = API.DefaultBufferSize;
        foreach (var converter in API.Converters)
            options.Converters.Add(converter);
        options.AllowTrailingCommas = API.AllowTrailingCommas;
        options.WriteIndented = API.WriteIndented;
    }
}
