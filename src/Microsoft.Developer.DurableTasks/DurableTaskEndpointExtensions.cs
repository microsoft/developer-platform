// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using DurableTask.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Reflection;

using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace Microsoft.AspNetCore.Routing;

public static class DurableTaskEndpointExtensions
{
    private const string InstanceIdKey = "instanceId";

    private static RouteHandlerBuilder ValidateOrchestrationInstance(this RouteHandlerBuilder builder) => builder
        .Produces(StatusCodes.Status404NotFound)
        .AddEndpointFilterFactory((context, next) =>
        {
            var client = context.ApplicationServices.GetService<TaskHubClient>();
            var index = GetInstanceIdParameter(context.MethodInfo);

            if (client is null)
            {
                return context => next(context);
            }

            return async context =>
            {
                var instanceId = context.GetArgument<string>(index);
                var feature = context.HttpContext.Features.GetRequiredFeature<DurableTaskFeature>();

                feature.State = await client.GetOrchestrationStateAsync(instanceId);

                if (!feature.HasState)
                {
                    return TypedResults.NotFound();
                }

                return await next(context);
            };

            static int GetInstanceIdParameter(MethodInfo method)
            {
                var count = 0;

                foreach (var parameter in method.GetParameters())
                {
                    if (string.Equals(InstanceIdKey, parameter.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return count;
                    }

                    count++;
                }

                throw new InvalidOperationException($"Could not find a parameter named {InstanceIdKey}");
            }
        });

    public static DurableTaskEndpointConventionBuilder<TResult> MapDurableTask<T, TBody, TResult>(this IEndpointRouteBuilder endpoint, string route, string statusRoute)
      where T : TaskOrchestration<TResult, TBody>, IVersionedName
        => endpoint.MapDurableTask<TBody, TResult>(route, statusRoute, T.Name, T.Version);

    public static DurableTaskEndpointConventionBuilder<TResult> WithOrchestrationTag<TResult>(this DurableTaskEndpointConventionBuilder<TResult> builder, string name, Func<HttpContext, string?> func)
        => builder.WithMetadata(new DurableTaskTag(name, ctx => ValueTask.FromResult(func(ctx))));

    public static DurableTaskEndpointConventionBuilder<TResult> WithOrchestrationTag<TResult, T>(this DurableTaskEndpointConventionBuilder<TResult> builder, string name, Func<HttpContext, T> func)
        => builder.WithOrchestrationTag(name, ctx =>
        {
            var result = func(ctx);
            return JsonSerializer.Serialize(result);
        });

    public static DurableTaskEndpointConventionBuilder<TResult> WithOrchestrationTag<TResult>(this DurableTaskEndpointConventionBuilder<TResult> builder, string name, Func<HttpContext, ValueTask<string?>> func)
        => builder.WithMetadata(new DurableTaskTag(name, func));

    public static DurableTaskEndpointConventionBuilder<TResult> MapDurableTask<TBody, TResult>(this IEndpointRouteBuilder endpoint, string route, string statusRoute, string name, string version)
    {
        var post = endpoint
            .MapPost(route, async ([FromServices] TaskHubClient client, [FromServices] TaskHubWorker worker, [FromBody] TBody body, HttpContext context, CancellationToken token) =>
            {
                var feature = context.Features.GetRequiredFeature<DurableTaskFeature>();
                var tagGenerators = context.GetEndpoint()?.Metadata.GetOrderedMetadata<DurableTaskTag>();

                Dictionary<string, string>? tags = default;

                if (tagGenerators is not null)
                {
                    tags = [];

                    foreach (var tag in tagGenerators)
                    {
                        if (await tag.KeyFactory(context) is { } value)
                        {
                            tags.Add(tag.Key, value);
                        }
                    }
                }

                var instance = await client.CreateOrchestrationInstanceAsync(name, version, Guid.NewGuid().ToString(), body, tags);
                var state = await client.GetOrchestrationStateAsync(instance);

                return CreateResult(context, statusRoute, instance.InstanceId);
            })
            .AddEndpointFilter((ctx, next) =>
            {
                ctx.HttpContext.Features.Set(new DurableTaskFeature());
                return next(ctx);
            });

        return new(endpoint, post);
    }

    public static RouteHandlerBuilder MapStatus<TResult>(this DurableTaskEndpointConventionBuilder<TResult> builder, string route)
        => builder
            .MapGet($"{route}/{{instanceId}}", Results<Ok<TResult>, Accepted<DurableTaskResult>, StatusCodeHttpResult> (HttpContext context, string instanceId, IOptions<JsonOptions> options, CancellationToken token) =>
            {
                var state = context.Features.GetRequiredFeature<DurableTaskFeature>().State;

                if (state.OrchestrationStatus is OrchestrationStatus.Running or OrchestrationStatus.Pending or OrchestrationStatus.Suspended or OrchestrationStatus.ContinuedAsNew)
                {
                    return CreateResult(context, route, state.OrchestrationInstance.InstanceId);
                }

                // If it failed, we'll tell them it's gone
                if (state.OrchestrationStatus is OrchestrationStatus.Canceled or OrchestrationStatus.Failed or OrchestrationStatus.Terminated)
                {
                    return TypedResults.StatusCode(StatusCodes.Status410Gone);
                }

                Debug.Assert(state.OrchestrationStatus == OrchestrationStatus.Completed);

                var deserialized = JsonSerializer.Deserialize<TResult>(state.Output, options.Value.SerializerOptions);

                return TypedResults.Ok(deserialized);
            })
            .AddEndpointFilter((ctx, next) =>
            {
                ctx.HttpContext.Features.Set(new DurableTaskFeature());
                return next(ctx);
            })
            .Produces(StatusCodes.Status410Gone)
            .ValidateOrchestrationInstance();

    private static Accepted<DurableTaskResult> CreateResult(HttpContext context, string statusRoute, string instanceId)
    {
        var location = $"{context.Request.Scheme}://{context.Request.Host}{statusRoute}/{instanceId}";

        var result = new DurableTaskResult(instanceId);

        return TypedResults.Accepted(location, result);
    }

    private sealed record DurableTaskTag(string Key, Func<HttpContext, ValueTask<string?>> KeyFactory);

    private sealed class DurableTaskFeature
    {
        private OrchestrationState? state;

        public bool HasState => state is not null;

        public OrchestrationState State
        {
            get => state ?? throw new InvalidOperationException("Must check if state exists with HasState");
            set => state = value;
        }

        public IDictionary<string, string> Tags { get; } = new Dictionary<string, string>();
    };
}
