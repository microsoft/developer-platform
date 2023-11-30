// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Api.Providers;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Abstractions;
using System.Net.Http.Headers;
using Yarp.ReverseProxy.Forwarder;

namespace Microsoft.Developer.Api;

public static class ProviderForwardingExtensions
{
    /// <summary>
    /// Enables calling the API with /{providerId}/{{**catch-all}} and have it forwarded to the provider.
    /// </summary>
    public static IEndpointConventionBuilder MapProviderPassthrough(this IEndpointRouteBuilder endpoints)
    {
        var providers = endpoints.ServiceProvider.GetRequiredService<IOptions<ProviderOptions>>();
        var builder = new CompositeBuilder();

        foreach (var (_, provider) in providers.Value)
        {
            if (provider.Uri is { })
            {
                var config = new ForwarderRequestConfig();
                var transformer = new ProviderOnBehalfOfTransformer(provider);
                var forwarder = endpoints.MapForwarder($"/{provider.Id}/{{**catch-all}}", provider.Uri.AbsoluteUri, config, transformer);

                builder.Add(forwarder);
            }
        }

        return builder;
    }

    private class ProviderOnBehalfOfTransformer(ProviderDefinition provider) : HttpTransformer
    {
        private readonly string pathBase = $"/{provider.Id}";

        public override async ValueTask TransformRequestAsync(HttpContext httpContext, HttpRequestMessage proxyRequest, string destinationPrefix, CancellationToken cancellationToken)
        {
            // Remove the provider prefix so we proxy to the right path
            httpContext.Request.Path = httpContext.Request.Path.ToString()[pathBase.Length..];

            await base.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix, cancellationToken);

            if (httpContext.User.Identity is { IsAuthenticated: true })
            {
                var authHeader = httpContext.RequestServices.GetRequiredService<IAuthorizationHeaderProvider>();

                var result = await authHeader.CreateAuthorizationHeaderForUserAsync(provider.Scopes, null, claimsPrincipal: httpContext.User, cancellationToken);

                proxyRequest.Headers.Authorization = AuthenticationHeaderValue.Parse(result);
            }
        }
    }

    private sealed class CompositeBuilder : List<IEndpointConventionBuilder>, IEndpointConventionBuilder
    {
        void IEndpointConventionBuilder.Add(Action<EndpointBuilder> convention)
        {
            foreach (var builder in this)
            {
                builder.Add(convention);
            }
        }

        void IEndpointConventionBuilder.Finally(Action<EndpointBuilder> convention)
        {
            foreach (var builder in this)
            {
                builder.Finally(convention);
            }
        }
    }
}
