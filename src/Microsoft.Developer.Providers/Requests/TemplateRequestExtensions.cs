// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.Developer.Entities;
using Microsoft.Developer.Serialization.Json.Entities;

namespace Microsoft.Developer.Requests;

public static class TemplateRequestExtensions
{
    public static async Task<TemplateRequest> GetTemplateRequestAsync(this HttpRequest req, CancellationToken cancellationToken = default)
    {
        var payload = await JsonSerializer.DeserializeAsync<TemplateRequest>(req.Body, EntitySerializerOptions.Default, cancellationToken)
            .ConfigureAwait(false) ?? throw new BadHttpRequestException($"Could not get template request from body");

        var templateRef = payload.TemplateRef ?? throw new BadHttpRequestException($"Could not get TemplateRef from body");

        if (templateRef.Namespace == Entity.Defaults.Namespace)
        {
            throw new BadHttpRequestException($"templateRef namespace {Entity.Defaults.Namespace} is invalid");
        }

        return payload;
    }

    public static Dictionary<string, JsonElement> GetInputDictionary(this TemplateRequest request)
    {
        if (request is null)
        {
            throw new BadHttpRequestException($"Could not get template request payload from body");
        }

        return request.InputJson is not null
            && JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.InputJson) is Dictionary<string, JsonElement> json
            ? json : throw new BadHttpRequestException($"Could not deserialize inputs from body InputJason");
    }

    public static JsonNode GetInputs(this TemplateRequest request)
    {
        if (request is null)
        {
            throw new BadHttpRequestException($"Could not get template request payload from body");
        }

        return request.InputJson is not null && JsonNode.Parse(request.InputJson) is JsonNode json ? json
            : throw new BadHttpRequestException($"Could not get InputJason from body");
    }

    public static T GetRequiredInput<T>(this TemplateRequest request, string property)
        => request.GetInputs().GetRequiredInput<T>(property);

    public static string? GetInput(this JsonNode json, string property)
    {
        try
        {
            return json[property]?.ToString();
        }
        catch (Exception e) when (e is FormatException or InvalidOperationException)
        {
            return null;
        }
    }

    public static T? GetInput<T>(this JsonNode json, string property, T? @default = default)
    {
        try
        {
            var prop = json[property];
            return prop is null ? @default : prop.GetValue<T>();
        }
        catch (Exception e) when (e is FormatException or InvalidOperationException)
        {
            return @default;
        }
    }

    [return: NotNull]
    public static string GetRequiredInput(this JsonNode json, string property)
    {
        try
        {
            return json[property]?.ToString() ?? throw new BadHttpRequestException($"Could not get {property} from inputJson");
        }
        catch (Exception e) when (e is FormatException or InvalidOperationException)
        {
            throw new BadHttpRequestException($"Could not get {property} from inputJson");
        }
    }

    public static T GetRequiredInput<T>(this JsonNode json, string property)
    {
        try
        {
            var prop = json[property] ?? throw new BadHttpRequestException($"Could not get {property} from inputJson");
            return prop.GetValue<T>() ?? throw new BadHttpRequestException($"Could not get {property} from inputJson");
        }
        catch (Exception e) when (e is FormatException or InvalidOperationException)
        {
            throw new BadHttpRequestException($"Could not get {property} from inputJson");
        }
    }
}