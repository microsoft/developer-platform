// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Developer.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Hosting.Middleware;

public class DeveloperPlatformRequestMiddleware : IFunctionsWorkerMiddleware
{
    public Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        if (context.Features.Get<IDeveloperPlatformRequestFeature>() is null)
        {
            var httpContext = context.GetRequiredHttpContext();

            var platformFeature = new DevPlatformFeature
            {
                Kind = httpContext.GetRouteValue("kind") as string,
                Name = httpContext.GetRouteValue("name") as string,
                Namespace = httpContext.GetRouteValue("namespace") as string,
            };

            context.Features.Set<IDeveloperPlatformRequestFeature>(platformFeature);
        }

        return next(context);
    }

    private sealed record DevPlatformFeature : IDeveloperPlatformRequestFeature
    {
        public EntityKind Kind { get; set; }

        public EntityNamespace Namespace { get; set; }

        public EntityName Name { get; set; }
    }
}
