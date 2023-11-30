// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace Microsoft.Developer.Hosting.Middleware;

/// <summary>
/// Updates the <see cref="HttpRequest.Host"/> and <see cref="HttpRequest.Scheme"/> to point to the public function entry point. This
/// is the functions worker, which proxies to the functions app. By default, the HOST and PROTO are pointing to the (internal) hosted
/// functions app and doesn't automatically update for the forwarded headers.
/// </summary>
internal sealed class UpdateHostMiddleware : IFunctionsWorkerMiddleware
{
    public Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        if (context.GetHttpContext() is { } httpContext)
        {
            if (httpContext.Request.Headers.TryGetValue("X-FORWARDED-HOST", out var forwardedHost) && forwardedHost is [{ } host])
            {
                httpContext.Request.Host = new HostString(host);
            }

            if (httpContext.Request.Headers.TryGetValue("X-FORWARDED-PROTO", out var forwardedProto) && forwardedProto is [{ } proto])
            {
                httpContext.Request.Scheme = proto;
            }
        }

        return next(context);
    }
}
