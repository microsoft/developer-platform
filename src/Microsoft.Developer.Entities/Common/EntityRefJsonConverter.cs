/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class EntityRefJsonConverter : JsonConverter<EntityRef>
{
    public override EntityRef Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, EntityRef value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Id);
}

public class ApiEntityRefJsonConverter : JsonConverter<ApiEntityRef>
{
    public override ApiEntityRef Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, ApiEntityRef value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Id);
}

public class SystemEntityRefJsonConverter : JsonConverter<SystemEntityRef>
{
    public override SystemEntityRef Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, SystemEntityRef value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Id);
}

public class ComponentEntityRefJsonConverter : JsonConverter<ComponentEntityRef>
{
    public override ComponentEntityRef Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, ComponentEntityRef value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Id);
}

public class GroupEntityRefJsonConverter : JsonConverter<GroupEntityRef>
{
    public override GroupEntityRef Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, GroupEntityRef value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Id);
}

public class UserEntityRefJsonConverter : JsonConverter<UserEntityRef>
{
    public override UserEntityRef Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, UserEntityRef value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Id);
}

public class DomainEntityRefJsonConverter : JsonConverter<DomainEntityRef>
{
    public override DomainEntityRef Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, DomainEntityRef value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Id);
}

public class ResourceEntityRefJsonConverter : JsonConverter<ResourceEntityRef>
{
    public override ResourceEntityRef Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, ResourceEntityRef value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Id);
}

public class LocationEntityRefJsonConverter : JsonConverter<LocationEntityRef>
{
    public override LocationEntityRef Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, LocationEntityRef value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Id);
}
