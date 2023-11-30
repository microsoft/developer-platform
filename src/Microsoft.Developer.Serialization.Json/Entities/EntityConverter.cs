// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Serialization.Json.Entities;

public class EntityConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsAssignableTo(typeof(PropertiesBase<>).MakeGenericType(typeToConvert));

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => (JsonConverter?)Activator.CreateInstance(typeof(EntityBaseConverter<>).MakeGenericType(typeToConvert));

    private sealed class EntityBaseConverter<T> : JsonConverter<T>, IPropertyBagOptions
        where T : PropertiesBase<T>, new()
    {
        private readonly HashSet<string> ignoredKeys = new(StringComparer.OrdinalIgnoreCase);

        public ICollection<string> IgnoredKeys => ignoredKeys;

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = JsonSerializer.Deserialize<JsonEntityProperties>(ref reader, options);
            (result ??= new()).Options = options;

            return new T { Properties = result };
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var property in value.Properties)
            {
                if (!ignoredKeys.Contains(property.Key))
                {
                    var key = options.PropertyNamingPolicy is { } p ? p.ConvertName(property.Key) : property.Key;
                    writer.WritePropertyName(key);
                    JsonSerializer.Serialize(writer, property.Value, options);
                }
            }

            writer.WriteEndObject();
        }
    }

    private sealed class JsonEntityProperties : EntityProperties
    {
        public JsonSerializerOptions Options { get; set; } = null!;

        protected override T? Convert<T>(string key, object obj) where T : default
            => obj switch
            {
                JsonElement json => MaterializeObject<T>(key, json),
                T t => t,
                _ => default,
            };

        private T? MaterializeObject<T>(string key, JsonElement json)
        {
            var result = json.Deserialize<T>(Options);
            Set(result, key);
            return result;
        }
    }
}
