// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http.Json;
using Microsoft.Developer.Requests;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace Microsoft.Developer.Api.Providers;

internal sealed class RegisteredProvidersRepository(IDownstreamApi api, IOptions<JsonOptions> json, ILogger<RegisteredProvidersRepository> logger) : IProviderEntityRepository
{
    private readonly JsonSerializerOptions serializerOptions = json.Value.SerializerOptions;

    public ValueTask<Entity?> GetAsync(EntityRef entityRef, DownstreamProviderOptions options, CancellationToken cancellationToken = default)
        => GetEntity(options, entityRef.ToString().Replace(':', '/'), cancellationToken);

    public Task<IEnumerable<Entity>> ListAsync(DownstreamProviderOptions options, CancellationToken cancellationToken = default)
        => GetEntities(options, string.Empty, cancellationToken);

    public Task<IEnumerable<Entity>> ListAsync(EntityKind kind, DownstreamProviderOptions options, CancellationToken cancellationToken = default)
        => GetEntities(options, kind, cancellationToken);

    private async ValueTask<Entity?> GetEntity(DownstreamProviderOptions downstreamOptions, string url, CancellationToken cancellationToken)
    {
        var entities = new List<Task<ProviderResult<Entity?>>>();

        foreach (var provider in downstreamOptions.Providers)
        {
            if (provider.Enabled)
            {
                entities.Add(GetItem(url, downstreamOptions, provider, cancellationToken).AsTask());
            }
        }

        var results = await Task.WhenAll(entities);

        try
        {
            return results.Where(r => r.Result is not null).SingleOrDefault()?.Result;
        }
        catch (InvalidOperationException e)
        {
            logger.LogError(e, "More than one provider returned an entity for url '{url}'", url);
        }

        return null;
    }

    private async Task<IEnumerable<Entity>> GetEntities(DownstreamProviderOptions downstreamOptions, string relativeUrl, CancellationToken cancellationToken)
    {
        var entities = new List<Task<ProviderResult<Entity[]>>>();

        foreach (var provider in downstreamOptions.Providers)
        {
            if (provider.Enabled)
            {
                entities.Add(GetItems(relativeUrl, downstreamOptions, provider, cancellationToken).AsTask());
            }
        }

        var result = await Task.WhenAll(entities);

        var allEntities = new List<Entity>();

        foreach (var resultItem in result)
        {
            AddWwwAuthenticate(downstreamOptions, resultItem.WwwAuthentication);

            if (resultItem.Result is { } resultItemResults)
            {
                allEntities.AddRange(resultItemResults);
            }
        }

        return allEntities;
    }

    private static void AddWwwAuthenticate(DownstreamProviderOptions options, IList<string> wwwAuthenticate)
    {
        if (wwwAuthenticate.Count > 0)
        {
            options.CustomizeHttpContext += context =>
            {
                context.Response.Headers.AppendList(HeaderNames.WWWAuthenticate, wwwAuthenticate);

                // Required for the client to read the WWW-Authenticate header in CORS requests
                context.Response.Headers.Append(HeaderNames.AccessControlExposeHeaders, HeaderNames.WWWAuthenticate);
            };
        }
    }

    public Task<PostedResponse<TemplateResponse>> CheckStatus(ProviderStatusCheck status, DownstreamProviderOptions downstreamOptions, CancellationToken token)
    {
        if (downstreamOptions.Providers.SingleOrDefault() is not { } provider)
        {
            logger.LogWarning("Need a single provider when posting a template");
            return Task.FromResult(PostedResponse<TemplateResponse>.Invalid);
        }

        return ProcessPossibleLongTaskAsync(provider, downstreamOptions, $"/status/{status.Id}", HttpMethod.Get, null, token);
    }

    public async Task<PostedResponse<TemplateResponse>> PostAsync(TemplateRequest request, DownstreamProviderOptions downstreamOptions, CancellationToken cancellationToken)
    {
        if (downstreamOptions.Providers.SingleOrDefault() is not { } provider)
        {
            logger.LogWarning("Need a single provider when posting a template");
            return PostedResponse<TemplateResponse>.Invalid;
        }

        using var content = JsonContent.Create(request, options: serializerOptions);

        return await ProcessPossibleLongTaskAsync(provider, downstreamOptions, "/entities", HttpMethod.Post, content, cancellationToken);
    }

    private async Task<PostedResponse<TemplateResponse>> ProcessPossibleLongTaskAsync(ProviderDefinition provider, DownstreamProviderOptions downstreamOptions, string relativeUrl, HttpMethod method, HttpContent? content, CancellationToken token)
    {
        try
        {
            using var result = await api.SendAsync(new DownstreamApiOptions(provider.Uri.ToString(), relativeUrl, method)
            {
                User = downstreamOptions.User,
                Scopes = provider.Scopes,
                Content = content,
                CustomizeHttpRequestMessage = downstreamOptions.CustomizeHttpRequestMessage,
            }, token);

            if (result.IsSuccessStatusCode)
            {
                using var stream = await result.Content.ReadAsStreamAsync(token);

                if (result.StatusCode == HttpStatusCode.Accepted)
                {
                    if (await JsonSerializer.DeserializeAsync<PostedResponse<TemplateResponse>>(stream, serializerOptions, token) is { Id.Length: > 0 } longRunning)
                    {
                        return longRunning;
                    }

                    return PostedResponse<TemplateResponse>.Error("No content from provider");
                }

                else if (result.StatusCode == HttpStatusCode.OK)
                {
                    if (await JsonSerializer.DeserializeAsync<TemplateResponse>(stream, serializerOptions, token) is { } responseContent)
                    {
                        return PostedResponse<TemplateResponse>.Response(responseContent);
                    }
                }
            }
        }
        catch (MsalUiRequiredException e)
        {
            logger.LogError(e, "Failure to POST to provider {Provider} due to missing claims '{Claims}'", provider.Uri, e.Claims);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failure to POST to provider {Provider}", provider.Uri);
        }

        return PostedResponse<TemplateResponse>.Error("Failed to call provider");
    }

    private async ValueTask<ProviderResult<Entity?>> GetItem(string relativeUrl, DownstreamProviderOptions downstreamOptions, ProviderDefinition provider, CancellationToken cancellationToken)
    {
        var normalizedRelativeUrl = string.IsNullOrEmpty(relativeUrl) ? "/entities" : $"/entities/{relativeUrl}";

        try
        {
            using var result = await api.SendAsync(new(provider.Uri.ToString(), normalizedRelativeUrl, HttpMethod.Get)
            {
                User = downstreamOptions.User,
                Scopes = provider.Scopes,
                CustomizeHttpRequestMessage = downstreamOptions.CustomizeHttpRequestMessage,
            }, cancellationToken);

            if (result.IsSuccessStatusCode)
            {
                using var stream = await result.Content.ReadAsStreamAsync(cancellationToken);

                var entity = await JsonSerializer.DeserializeAsync<Entity>(stream, serializerOptions, cancellationToken) ?? null;

                if (entity is not null)
                {
                    entity.Metadata.Provider = provider.Id;
                }

                return new() { Result = entity, WwwAuthentication = GetWWWAuthenticate(result) };
            }
        }
        catch (MsalUiRequiredException e)
        {
            logger.LogError(e, "Failure to GET to provider {Provider} due to missing claims '{Claims}'", provider.Uri, e.Claims);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failure to get {Endpoint} from provider {Provider}", normalizedRelativeUrl, provider.Uri);
        }

        return new();
    }

    private async ValueTask<ProviderResult<Entity[]>> GetItems(string url, DownstreamProviderOptions downstreamOptions, ProviderDefinition provider, CancellationToken cancellationToken)
    {
        var normalizedUrl = string.IsNullOrEmpty(url) ? "/entities" : $"/entities/{url}";

        try
        {
            using var result = await api.SendAsync(new(provider.Uri.ToString(), normalizedUrl, HttpMethod.Get)
            {
                User = downstreamOptions.User,
                Scopes = provider.Scopes,
                CustomizeHttpRequestMessage = downstreamOptions.CustomizeHttpRequestMessage,
            }, cancellationToken);

            if (result.IsSuccessStatusCode)
            {
                using var stream = await result.Content.ReadAsStreamAsync(cancellationToken);

                var entities = await JsonSerializer.DeserializeAsync<Entity[]>(stream, serializerOptions, cancellationToken) ?? [];

                foreach (var entity in entities)
                {
                    entity.Metadata.Provider = provider.Id;
                }

                return new() { Result = entities, WwwAuthentication = GetWWWAuthenticate(result) };
            }
        }
        catch (MsalUiRequiredException e)
        {
            logger.LogError(e, "Failure to GET to provider {Provider} due to missing claims '{Claims}'", provider.Uri, e.Claims);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failure to get {Endpoint} from provider {Provider}", normalizedUrl, provider.Uri);
        }

        return new() { Result = [] };
    }

    private static ProviderResult<T> Create<T>(T result, HttpResponseMessage response) => new() { Result = result, WwwAuthentication = GetWWWAuthenticate(response) };

    private static IList<string> GetWWWAuthenticate(HttpResponseMessage response) => response.Headers.TryGetValues(HeaderNames.WWWAuthenticate, out var result) ? result.ToList() : Array.Empty<string>();

    private sealed record ProviderResult<T>
    {
        public T? Result { get; init; }

        public IList<string> WwwAuthentication { get; init; } = Array.Empty<string>();
    }
}
