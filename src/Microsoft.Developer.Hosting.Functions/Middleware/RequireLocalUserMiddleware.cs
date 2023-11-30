// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Developer.Azure;
using Microsoft.Developer.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Microsoft.Developer.Hosting.Functions.Middleware;

/// <summary>
/// Invoked on functions that have <see cref="RequireLocalUserAttribute"/> added that will add appropriate
/// <c>WWW-Authenticate</c> values for the current provider to connect the provider to a user in a different
/// data store.
/// </summary>
/// <param name="azOptions"></param>
internal class RequireLocalUserMiddleware(IOptions<AzureAdOptions> azOptions) : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        if (context.Features.Get<IFunctionMetadataFeature>() is { Metadata: { } metadata }
            && metadata.GetMetadata<RequireLocalUserAttribute>() is { } requirement
            && context.Features.Get<IDeveloperPlatformUserFeature>() is { } user)
        {
            var userManager = context.InstanceServices.GetRequiredService<ILocalUserManager>();

            if (!await userManager.HasLocalUserAsync(user.User, context.CancellationToken))
            {
                var httpContext = context.GetRequiredHttpContext();

                httpContext.Response.OnStarting(() =>
                {
                    // for some reason the Scheme is always http when running in the cloud, even when the
                    // request is https. for now, if we're running locally, use the scheme, otherwise assume https
                    var scheme = httpContext.Request.Host.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ? httpContext.Request.Scheme : "https";

                    var values = new HeaderValueBuilder("Bearer")
                    {
                        { "realm", requirement.Realm },
                        { "authorization_uri", $"{scheme}://{httpContext.Request.Host}{requirement.RedirectPath}" },
                        { "scopes", $"api://{azOptions.Value.ClientId}/.default" },
                    }.Build();

                    httpContext.Response.Headers.Append(HeaderNames.WWWAuthenticate, values);

                    return Task.CompletedTask;
                });
            }
        }

        await next(context);
    }
}
