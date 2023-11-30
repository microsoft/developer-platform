// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using System.Text.Json.Nodes;
using Yaml2JsonNode;

namespace Microsoft.Developer.Api;

internal static class YamlExtensions
{
    private const string YamlContentType = "application/yaml";

    public static T AddYamlContentType<T>(this T builder)
        where T : IEndpointConventionBuilder
    {
        builder.AddEndpointFilter(async (ctx, next) =>
        {
            if (ctx.HttpContext.Request.Headers.Accept.Equals(YamlContentType))
            {
                var result = await next(ctx);

                if (result is IValueHttpResult { Value: { } value })
                {
                    var statusCode = (result as IStatusCodeHttpResult)?.StatusCode ?? 200;

                    return new YamlResult(value, statusCode);
                }

                return result;
            }
            else
            {
                return await next(ctx);
            }
        })
        .Add(builder =>
        {
            var existing = builder.Metadata.OfType<ProducesResponseTypeMetadata>().FirstOrDefault();

            if (existing is null)
            {
                builder.Metadata.Add(new ProducesResponseTypeMetadata(200, contentTypes: [YamlContentType]));
            }
            else
            {
                var contentTypes = existing.ContentTypes.Concat([YamlContentType]).ToArray();
                builder.Metadata.Add(new ProducesResponseTypeMetadata(existing.StatusCode, existing.Type, contentTypes));
            }
        });

        return builder;
    }

    private sealed class YamlResult(object value, int statusCode) : IResult, IValueHttpResult, IContentTypeHttpResult, IStatusCodeHttpResult
    {
        object? IValueHttpResult.Value => value;

        string? IContentTypeHttpResult.ContentType => YamlContentType;

        int? IStatusCodeHttpResult.StatusCode => statusCode;

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.ContentType = YamlContentType;
            httpContext.Response.StatusCode = statusCode;

            var manager = httpContext.RequestServices.GetRequiredService<RecyclableMemoryStreamManager>();
            var options = httpContext.RequestServices.GetRequiredService<IOptions<JsonOptions>>().Value.SerializerOptions;
            var node = await GetJsonNodeAsync(manager, options, httpContext.RequestAborted);

            if (node?.GetValueKind() == JsonValueKind.Array && node.AsArray() is JsonArray array)
            {
                await httpContext.Response.WriteAsync(
                    string.Join("---\n", array.Select(i => YamlSerializer.Serialize(i))));
            }
            else
            {
                await httpContext.Response.WriteAsync(YamlSerializer.Serialize(node));
            }
        }

        /// <summary>
        /// Get a <see cref="JsonNode"/> via an intermediary <see cref="Stream"/> since we are expecting responses to have <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        private async ValueTask<JsonNode?> GetJsonNodeAsync(RecyclableMemoryStreamManager manager, JsonSerializerOptions options, CancellationToken token)
        {
            using var stream = new RecyclableMemoryStream(manager);
            await JsonSerializer.SerializeAsync(stream, value, options, token);
            stream.Position = 0;
            return JsonNode.Parse(stream);
        }
    }
}
