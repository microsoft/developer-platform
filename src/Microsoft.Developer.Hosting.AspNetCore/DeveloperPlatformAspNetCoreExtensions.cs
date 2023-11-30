// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Developer.Entities;
using Microsoft.Developer.Features;
using Microsoft.Developer.Hosting;
using Microsoft.Developer.Hosting.AspNetCore.Middleware;

namespace Microsoft.Extensions.DependencyInjection;

public static class DeveloperPlatformAspNetCoreExtensions
{
    public static IDeveloperPlatformBuilder ConfigurePlatform(this IDeveloperPlatformBuilder builder, Action<DeveloperPlatformAspNetCoreOptions> configure)
    {
        builder.Services.AddOptions<DeveloperPlatformAspNetCoreOptions>()
            .Configure(configure)
            .ValidateDataAnnotations();

        return builder;
    }

    public static IEndpointConventionBuilder MapEntities(this IEndpointRouteBuilder entities)
    {
        var all = entities.MapGet("/", (HttpContext context) => TypedResults.Ok(context.Features.GetRequiredFeature<IDeveloperPlatformRepositoryFeature>().GetAsync()))
            .WithName("GetEntities");
        var kind = entities.MapGet(@"/{kind:regex(^[a-zA-Z][a-z0-9A-Z]*$)}", (HttpContext context, EntityKind kind) => TypedResults.Ok(context.Features.GetRequiredFeature<IDeveloperPlatformRepositoryFeature>().GetAsync(kind)))
            .WithName("GetEntitiesByKind");
        var named = entities.MapGet(@"/{kind:regex(^[a-zA-Z][a-z0-9A-Z]*$)}/{namespace:regex(^[a-z0-9]+(?:\-+[a-z0-9]+)*$)}/{name:regex(^([A-Za-z0-9][-A-Za-z0-9_.]*)[A-Za-z0-9]$)}", async Task<Results<Ok<Entity>, NotFound>> (HttpContext context, string kind, string @namespace, string name) =>
            {
                var result = await context.Features.GetRequiredFeature<IDeveloperPlatformRepositoryFeature>()
                    .GetAsync(new EntityRef(kind) { Name = name, Namespace = @namespace });

                if (result is null)
                {
                    return TypedResults.NotFound();
                }
                else
                {
                    return TypedResults.Ok(result);
                }
            })
            .WithName("GetEntity");

        return new CompositeBuilder(all, kind, named);
    }

    public static void UseDeveloperPlatform(this IApplicationBuilder app)
    {
        app.UseMiddleware<DeveloperPlatformUserMiddleware>();
        app.UseMiddleware<DeveloperPlatformRequestMiddleware>();
        app.UseMiddleware<DeveloperPlatformRepositoryMiddleware>();
    }

    private sealed class CompositeBuilder(params IEndpointConventionBuilder[] builders) : IEndpointConventionBuilder
    {
        public void Add(Action<EndpointBuilder> convention)
        {
            foreach (var builder in builders)
            {
                builder.Add(convention);
            }
        }

        public void Finally(Action<EndpointBuilder> finallyConvention)
        {
            foreach (var builder in builders)
            {
                builder.Finally(finallyConvention);
            }
        }
    }
}
