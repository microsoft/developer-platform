// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Microsoft.Developer.Entities;
using Microsoft.Developer.Features;

namespace Microsoft.Developer.Hosting.AspNetCore.Middleware;

public class DeveloperPlatformUserMiddleware(RequestDelegate next, IUserService userProvider)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Features.Get<IDeveloperPlatformUserFeature>() is null)
        {
            var devUser = await userProvider.GetOrCreateUser(context.User);

            if (devUser is not null)
            {
                var platformFeature = new DevPlatformFeature(devUser);

                context.Features.Set<IDeveloperPlatformUserFeature>(platformFeature);
            }
        }

        await next(context);
    }

    private sealed record DevPlatformFeature(Entity Entity) : IDeveloperPlatformUserFeature;
}
