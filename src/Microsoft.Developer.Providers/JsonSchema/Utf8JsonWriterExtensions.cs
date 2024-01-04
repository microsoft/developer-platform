// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Providers.JsonSchema;

public static class Utf8JsonWriterExtensions
{
    public static void WriteEnumArray(this Utf8JsonWriter writer, IEnumerable<string> values)
    {
        if (values.Any())
        {
            writer.WriteStartArray(JsonSchemaKeys.Enum);
            foreach (var value in values)
            {
                writer.WriteStringValue(value);
            }
            writer.WriteEndArray();
        }
    }

    public static void WriteEnumArray(this Utf8JsonWriter writer, IEnumerable<bool> values)
    {
        if (values.Any())
        {
            writer.WriteStartArray(JsonSchemaKeys.Enum);
            foreach (var value in values)
            {
                writer.WriteBooleanValue(value);
            }
            writer.WriteEndArray();
        }
    }

    public static void WriteEnumArray(this Utf8JsonWriter writer, IEnumerable<double> values)
    {
        if (values.Any())
        {
            writer.WriteStartArray(JsonSchemaKeys.Enum);
            foreach (var value in values)
            {
                writer.WriteNumberValue(value);
            }
            writer.WriteEndArray();
        }
    }

    public static void WriteEnumArray(this Utf8JsonWriter writer, IEnumerable<(string value, string name)> values)
    {
        writer.WriteStartArray(JsonSchemaKeys.EnumNames);
        foreach (var (_, name) in values)
        {
            writer.WriteStringValue(name);
        }
        writer.WriteEndArray();

        writer.WriteStartArray(JsonSchemaKeys.Enum);
        foreach (var (value, _) in values)
        {
            writer.WriteStringValue(value);
        }
        writer.WriteEndArray();
    }

    public static void WriteEnumArray(this Utf8JsonWriter writer, IEnumerable<(bool value, string name)> values)
    {
        writer.WriteStartArray(JsonSchemaKeys.EnumNames);
        foreach (var (_, name) in values)
        {
            writer.WriteStringValue(name);
        }
        writer.WriteEndArray();

        writer.WriteStartArray(JsonSchemaKeys.Enum);
        foreach (var (value, _) in values)
        {
            writer.WriteBooleanValue(value);
        }
        writer.WriteEndArray();
    }

    public static void WriteEnumArray(this Utf8JsonWriter writer, IEnumerable<(double value, string name)> values)
    {
        writer.WriteStartArray(JsonSchemaKeys.EnumNames);
        foreach (var (_, name) in values)
        {
            writer.WriteStringValue(name);
        }
        writer.WriteEndArray();

        writer.WriteStartArray(JsonSchemaKeys.Enum);
        foreach (var (value, _) in values)
        {
            writer.WriteNumberValue(value);
        }
        writer.WriteEndArray();
    }

    public static void WriteRequiredArray(this Utf8JsonWriter writer, params string[] required)
    {
        if (required.Length > 0)
        {
            writer.WriteStartArray(JsonSchemaKeys.Required);
            foreach (var value in required)
            {
                writer.WriteStringValue(value);
            }
            writer.WriteEndArray();
        }
    }

    public static void WriteRequiredArray(this Utf8JsonWriter writer, IEnumerable<string> required)
        => writer.WriteRequiredArray(required.ToArray());

    public static void WriteStartIfObject(this Utf8JsonWriter writer)
        => writer.WriteStartObject(JsonSchemaKeys.If);

    public static void WriteStartThenObject(this Utf8JsonWriter writer)
        => writer.WriteStartObject(JsonSchemaKeys.Then);

    public static void WriteStartPropertiesObject(this Utf8JsonWriter writer)
        => writer.WriteStartObject(JsonSchemaKeys.Properties);

    public static void WriteStartParametersObject(this Utf8JsonWriter writer)
        => writer.WriteStartObject(JsonSchemaKeys.Parameters);

    public static void WriteStartAllOfArray(this Utf8JsonWriter writer)
        => writer.WriteStartArray(JsonSchemaKeys.AllOf);

    public static void WriteStartOneOfArray(this Utf8JsonWriter writer)
        => writer.WriteStartArray(JsonSchemaKeys.OneOf);


    public static void WriteName(this Utf8JsonWriter writer, string name)
        => writer.WriteString(JsonSchemaKeys.Name, name);

    public static void WriteTitle(this Utf8JsonWriter writer, string title)
        => writer.WriteString(JsonSchemaKeys.Title, title);

    public static void WriteType(this Utf8JsonWriter writer, string type)
        => writer.WriteString(JsonSchemaKeys.Type, type);

    public static void WriteDefault(this Utf8JsonWriter writer, string value)
        => writer.WriteString(JsonSchemaKeys.Default, value);

    public static void WriteDefault(this Utf8JsonWriter writer, bool value)
        => writer.WriteBoolean(JsonSchemaKeys.Default, value);

    public static void WriteDefault(this Utf8JsonWriter writer, double value)
        => writer.WriteNumber(JsonSchemaKeys.Default, value);

    public static void WriteValue(this Utf8JsonWriter writer, string value)
        => writer.WriteString(JsonSchemaKeys.Value, value);

    public static void WriteValue(this Utf8JsonWriter writer, bool value)
        => writer.WriteBoolean(JsonSchemaKeys.Value, value);

    public static void WriteValue(this Utf8JsonWriter writer, double value)
        => writer.WriteNumber(JsonSchemaKeys.Value, value);

    public static void WriteDefaultAndValue(this Utf8JsonWriter writer, string value)
    {
        writer.WriteDefault(value);
        writer.WriteValue(value);
    }

    public static void WriteDefaultAndValue(this Utf8JsonWriter writer, bool value)
    {
        writer.WriteDefault(value);
        writer.WriteValue(value);
    }

    public static void WriteDefaultAndValue(this Utf8JsonWriter writer, double value)
    {
        writer.WriteDefault(value);
        writer.WriteValue(value);
    }

    public static void WriteBooleanAsOneOf(this Utf8JsonWriter writer, string trueTitle, string falseTitle, string? trueDescription = null, string? falseDescription = null)
    {
        writer.WriteStartOneOfArray();

        writer.WriteStartObject();
        writer.WriteBoolean(JsonSchemaKeys.Const, true);
        writer.WriteTitle(trueTitle);
        if (trueDescription is not null)
        {
            writer.WriteDescription(trueDescription);
        }
        writer.WriteEndObject();

        writer.WriteStartObject();
        writer.WriteBoolean(JsonSchemaKeys.Const, false);
        writer.WriteTitle(falseTitle);
        if (falseDescription is not null)
        {
            writer.WriteDescription(falseDescription);
        }
        writer.WriteEndObject();

        writer.WriteEndArray();
    }

    public static void WriteConst(this Utf8JsonWriter writer, string value)
        => writer.WriteString(JsonSchemaKeys.Const, value);

    public static void WriteDescription(this Utf8JsonWriter writer, string description)
        => writer.WriteString(JsonSchemaKeys.Description, description);

    public static void WriteEmptyInputSchema(this Utf8JsonWriter writer)
    {
        writer.WriteStartObject(); // start root

        writer.WriteType(JsonSchemaTypes.Object);

        writer.WriteStartPropertiesObject(); // start properties

        writer.WriteEndObject(); // end properties

        writer.WriteEndObject(); // end root
    }

    public static void WriteInputParameter(this Utf8JsonWriter writer, JsonSchemaInputParameter input)
    {
        writer.WriteStartObject(input.Id);

        writer.WriteName(input.Name);
        writer.WriteTitle(input.Title);

        writer.WriteType(input.Type);

        if (input.Description is not null)
        {
            writer.WriteDescription(input.Description);
        }

        if (input.Default is not null)
        {
            if (input.Type == JsonSchemaTypes.String)
            {
                writer.WriteDefaultAndValue(input.Default);
            }
            else if (input.Type == JsonSchemaTypes.Boolean)
            {
                writer.WriteDefaultAndValue(Convert.ToBoolean(input.Default));
            }
            else if (input.Type == JsonSchemaTypes.Number)
            {
                writer.WriteDefaultAndValue(Convert.ToDouble(input.Default));
            }
            else
            {
                throw new InvalidOperationException($"json schema input param default not supported on input of type {input.Type}");
            }
        }
        // if boolean or number, set the default value
        else if (input.Type == JsonSchemaTypes.Boolean)
        {
            writer.WriteDefaultAndValue(false);
        }
        else if (input.Type == JsonSchemaTypes.Number)
        {
            writer.WriteDefaultAndValue(0);
        }

        if (input.NamedOptions is not null && input.NamedOptions.Count > 0)
        {
            if (input.Type == JsonSchemaTypes.String)
            {
                writer.WriteEnumArray(input.NamedOptions);
            }
            else if (input.Type == JsonSchemaTypes.Boolean)
            {
                writer.WriteEnumArray(input.NamedOptions.Select(o => (value: Convert.ToBoolean(o.value), o.name)));
            }
            else if (input.Type == JsonSchemaTypes.Number)
            {
                writer.WriteEnumArray(input.NamedOptions.Select(o => (value: Convert.ToDouble(o.value), o.name)));
            }
            else
            {
                throw new InvalidOperationException($"json schema input param options not supported on input of type {input.Type}");
            }
        }
        else if (input.Options is not null && input.Options.Count > 0)
        {
            if (input.Type == JsonSchemaTypes.String)
            {
                writer.WriteEnumArray(input.Options);
            }
            else if (input.Type == JsonSchemaTypes.Boolean)
            {
                writer.WriteEnumArray(input.Options.Select(Convert.ToBoolean));
            }
            else if (input.Type == JsonSchemaTypes.Number)
            {
                writer.WriteEnumArray(input.Options.Select(Convert.ToDouble));
            }
            else
            {
                throw new InvalidOperationException($"json schema input param options not supported on input of type {input.Type}");
            }
        }

        writer.WriteEndObject();
    }

    public static void WriteInputParameter(this Utf8JsonWriter writer, string id, string type, string? name = null, string? title = null, string? description = null, string? @default = null, List<string>? options = null)
        => writer.WriteInputParameter(new JsonSchemaInputParameter(id: id, type: type, name: name, title: title, description: description, @default: @default, options: options));

    public static void WriteEnumInputParameter(this Utf8JsonWriter writer, string id, string type, string? name = null, string? title = null, string? description = null, string? @default = null, List<(string value, string name)>? options = null)
        => writer.WriteInputParameter(new JsonSchemaInputParameter(id: id, type: type, name: name, title: title, description: description, @default: @default, options: options));
}
