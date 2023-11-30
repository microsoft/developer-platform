// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Developer.Entities;
using Microsoft.Developer.Features;

namespace Microsoft.Developer.Hosting.AspNetCore.Middleware;

public class DeveloperPlatformRequestMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Features.Get<IDeveloperPlatformRepositoryFeature>() is null)
        {
            var platformFeature = new DevPlatformFeature(context)
            {
                Kind = context.GetRouteValue("kind") as string,
                Name = context.GetRouteValue("name") as string,
                Namespace = context.GetRouteValue("namespace") as string,
            };

            context.Features.Set<IDeveloperPlatformRequestFeature>(platformFeature);
        }

        await next(context);
    }

    private sealed record DevPlatformFeature(HttpContext Context) : IDeveloperPlatformRequestFeature
    {
        public EntityKind Kind { get; init; }

        public EntityNamespace Namespace { get; init; }

        public EntityName Name { get; init; }
    }
}
