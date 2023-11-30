// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Developer.Serialization.Json.Entities;

public sealed class ParsableConverter<T> : JsonConverter<T>
    where T : IParsable<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.GetString() is { } str)
        {
            return T.Parse(str, null);
        }

        return default;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());

    public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => Read(ref reader, typeToConvert, options)!;

    public override void WriteAsPropertyName(Utf8JsonWriter writer, [DisallowNull] T value, JsonSerializerOptions options)
        => writer.WritePropertyName(value.ToString()!);
}
