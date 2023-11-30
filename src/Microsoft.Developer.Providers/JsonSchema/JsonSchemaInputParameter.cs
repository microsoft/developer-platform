// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Providers.JsonSchema;

public class JsonSchemaInputParameter
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Type { get; set; }
    public string? Default { get; set; }

    private List<string>? options;
    public List<string>? Options
    {
        get => options ?? NamedOptions?.Select(o => o.value).ToList();
        set => options = value;
    }
    public List<(string value, string name)>? NamedOptions { get; set; }

    public JsonSchemaInputParameter(string id, string type, string? name = null, string? title = null, string? description = null, string? @default = null)
    {
        Id = id;

        var typeLower = type.ToLowerInvariant();
        Type = JsonSchemaTypes.All.Contains(typeLower) ? typeLower : throw new InvalidOperationException($"{typeLower} is not a valid json schema type");

        Name = name ?? id;
        Title = title ?? name ?? id;

        Description = description;

        Default = @default;
    }

    public JsonSchemaInputParameter(string id, string type, string? name = null, string? title = null, string? description = null, string? @default = null, List<string>? options = null)
        : this(id, type, name, title, description, @default)
    {
        if (options is not null && @default is not null && !options.Contains(@default))
        {
            throw new InvalidOperationException($"input parameter enum options array does not contain params default value {@default}");
        }

        Options = options;
    }

    public JsonSchemaInputParameter(string id, string type, string? name = null, string? title = null, string? description = null, string? @default = null, List<(string value, string name)>? options = null)
        : this(id, type, name, title, description, @default)
    {
        if (options is not null && @default is not null && !options.Any(o => o.value == @default))
        {
            throw new InvalidOperationException($"input parameter enum options array does not contain params default value {@default}");
        }

        NamedOptions = options;
    }
}
