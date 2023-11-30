// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Serialization.Json.Entities;

public class EntityRefConverter(bool ignoreTypes = false) : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsAssignableTo(typeof(EntityRef));

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert == typeof(EntityRef))
        {
            return new DefaultEntityRefConverter();
        }

        if (!ignoreTypes)
        {
            CheckIfTypeIsIEntityRef(typeToConvert);
        }

        return (JsonConverter?)Activator.CreateInstance(typeof(DerivedEntityRefConverter<>).MakeGenericType(typeToConvert));
    }

    private static void CheckIfTypeIsIEntityRef(Type typeToConvert)
    {
        try
        {
            if (!typeToConvert.IsAssignableTo(typeof(IEntityRef<>).MakeGenericType(typeToConvert)))
            {
                ThrowIfFailed(typeToConvert);
            }
        }
        catch (ArgumentException)
        {
            ThrowIfFailed(typeToConvert);
        }

        static void ThrowIfFailed(Type typeToConvert) => throw new InvalidOperationException($"{typeToConvert} must implement IEntityRef<> to be serialized");
    }

    private class InnerEntityRef
    {
        public string Name { get; set; } = null!;

        public string Namespace { get; set; } = null!;

        public string Kind { get; set; } = null!;
    }

    private sealed class DefaultEntityRefConverter : JsonConverter<EntityRef>
    {
        public override EntityRef? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return EntityRef.Parse(reader.GetString()!, null);
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                var result = JsonSerializer.Deserialize<InnerEntityRef>(ref reader, options);

                if (result is not null)
                {
                    return new EntityRef(result.Kind) { Name = result.Name, Namespace = result.Namespace };
                }
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, EntityRef value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.Id);
    }

    private sealed class DerivedEntityRefConverter<T> : JsonConverter<T>
        where T : EntityRef, IEntityRef<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return EntityRef.Parse<T>(reader.GetString()!);
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                var result = JsonSerializer.Deserialize<InnerEntityRef>(ref reader, options);

                if (result is not null)
                {
                    return T.Create(result.Name, result.Namespace);
                }
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.Id);
    }
}
