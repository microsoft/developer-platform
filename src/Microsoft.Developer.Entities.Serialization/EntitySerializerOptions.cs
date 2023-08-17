/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities.Serialization;

public enum EntitySerializer
{
    Default,
    API,
    Database
}

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
        TypeInfoResolver = new EntityTypeResolver
        {
            Modifiers = {
                SlugifyModifier.Modify
            }
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
                SlugifyModifier.Modify,
                ApiIgnoreModifier.Modify
            }
        }
    };

    public static readonly JsonSerializerOptions Database = new(Default)
    {
        // TypeInfoResolver = new DefaultJsonTypeInfoResolver // EntityTypeResolver
        TypeInfoResolver = new EntityTypeResolver
        {
            Modifiers = {
                SlugifyModifier.Modify,
                DatabaseIgnoreModifier.Modify
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

    public static void AddEntitySerialization(this JsonSerializerOptions options, EntitySerializer serializer = EntitySerializer.API)
    {
        var opts = serializer switch
        {
            EntitySerializer.Default => Default,
            EntitySerializer.API => API,
            EntitySerializer.Database => Database,
            _ => throw new ArgumentOutOfRangeException(nameof(serializer), serializer, $"Not exected serializer value: {serializer}.")
        };

        options.TypeInfoResolver = opts.TypeInfoResolver;
        options.ReferenceHandler = opts.ReferenceHandler;
        options.ReadCommentHandling = opts.ReadCommentHandling;
        options.PropertyNamingPolicy = opts.PropertyNamingPolicy;
        options.PropertyNameCaseInsensitive = opts.PropertyNameCaseInsensitive;
        options.MaxDepth = opts.MaxDepth;
        options.IncludeFields = opts.IncludeFields;
        options.IgnoreReadOnlyProperties = opts.IgnoreReadOnlyProperties;
        options.IgnoreReadOnlyFields = opts.IgnoreReadOnlyFields;
        options.Encoder = opts.Encoder;
        options.UnknownTypeHandling = opts.UnknownTypeHandling;
        options.DictionaryKeyPolicy = opts.DictionaryKeyPolicy;
        options.DefaultIgnoreCondition = opts.DefaultIgnoreCondition;
        options.DefaultBufferSize = opts.DefaultBufferSize;
        foreach (var converter in opts.Converters)
            options.Converters.Add(converter);
        options.AllowTrailingCommas = opts.AllowTrailingCommas;
        options.WriteIndented = opts.WriteIndented;
    }
}
