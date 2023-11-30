// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Developer.Features;

namespace Microsoft.Developer.Hosting.Middleware;

internal sealed class DeveloperPlatformAuthorizationMiddleware : IFunctionsWorkerMiddleware
{
    public Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        if (context.GetHttpContext() is { } httpContext && !IsAuthorized(context))
        {
            httpContext.Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        return next(context);
    }

    private bool IsAuthorized(FunctionContext context)
    {
        if (context.Features.GetRequiredFeature<IFunctionMetadataFeature>().Metadata.GetMetadata<IAuthorizeData>() is { })
        {
            // TODO: handle policies on metadata
            if (context.GetHttpContext() is { User.Identity.IsAuthenticated: true })
            {
                return true;
            }

            return false;
        }

        // Authorization not required
        return true;
    }
}
