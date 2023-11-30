// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Microsoft.Developer.Features;
using Microsoft.Extensions.Options;

namespace Microsoft.Developer.Hosting.AspNetCore.Middleware;

internal sealed class DeveloperPlatformRepositoryMiddleware(RequestDelegate next, IOptions<DeveloperPlatformAspNetCoreOptions> options)
{
    public Task InvokeAsync(HttpContext context)
    {
        if (context.Features.Get<IDeveloperPlatformRepositoryFeature>() is null && options.Value.RepositoryFactory is { } factory)
        {
            var feature = factory(context);

            context.Features.Set(feature);
        }

        return next(context);
    }
}