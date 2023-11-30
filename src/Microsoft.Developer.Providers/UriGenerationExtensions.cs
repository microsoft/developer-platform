// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Developer.Providers;

public static class UriGenerationExtensions
{
    public static Uri GetAbsoluteUri(this HttpContext context, string relativeUri)
    {
        if (context.RequestServices.GetRequiredService<IConfiguration>()["HostUrl"] is { } hostUrl)
        {
            return new Uri(new Uri(hostUrl), relativeUri);
        }
        else
        {
            var req = context.Request;

            // for some reason the Scheme is always http when running in the cloud, even when the
            // request is https. for now, if we're running locally, use the scheme, otherwise assume https
            var scheme = req.Host.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ? req.Scheme : "https";

            return new UriBuilder()
            {
                Scheme = scheme,
                Host = req.Host.Host,
                Port = req.Host.Port ?? -1,
                Path = relativeUri,
            }.Uri;
        }
    }

    public static Uri GetStatusUri(this HttpContext context, string instanceId)
        => context.GetAbsoluteUri($"status/{instanceId}");
}
