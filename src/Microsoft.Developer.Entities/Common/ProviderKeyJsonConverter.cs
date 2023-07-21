/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class ProviderKeyJsonConverter : JsonConverter<ProviderKey>
{
    public override ProviderKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new ProviderKey(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, ProviderKey value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Key);
    }

    public override ProviderKey ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new ProviderKey(reader.GetString()!);
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, ProviderKey value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(value.Key);
    }
}
