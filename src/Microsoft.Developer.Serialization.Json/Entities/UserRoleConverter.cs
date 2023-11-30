// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Serialization.Json.Entities;

internal class UserRoleConverter : JsonConverter<UserRole>
{
    public override UserRole Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => Enum.TryParse<UserRole>(reader.GetString(), out var result) ? result : default;

    public override void Write(Utf8JsonWriter writer, UserRole value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}
