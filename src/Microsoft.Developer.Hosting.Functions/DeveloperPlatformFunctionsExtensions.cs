// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Developer.Entities.Serialization;
using Microsoft.Developer.Hosting.Functions.Middleware;
using Microsoft.Developer.Hosting.Middleware;
using Microsoft.Developer.Serialization.Json.Entities;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DeveloperPlatformFunctionsExtensions
{
    private const string HttpTriggerType = "httpTrigger";
    private const string EntityTriggerType = "entityTrigger";
    private const string ActivityTriggerType = "activityTrigger";
    private const string OrchestrationTriggerType = "orchestrationTrigger";

    private static bool IsTriggerType(this FunctionContext context, string triggerType)
        => context.FunctionDefinition.InputBindings.Any(p => p.Value.Type.Equals(triggerType, StringComparison.OrdinalIgnoreCase));

    public static bool IsHttpTrigger(this FunctionContext context) => context.IsTriggerType(HttpTriggerType);

    public static bool IsEntityTrigger(this FunctionContext context) => context.IsTriggerType(EntityTriggerType);

    public static bool IsActivityTrigger(this FunctionContext context) => context.IsTriggerType(ActivityTriggerType);

    public static bool IsOrchestrationTrigger(this FunctionContext context) => context.IsTriggerType(OrchestrationTriggerType);

    private static IFunctionsWorkerApplicationBuilder UseWhenHttp<T>(this IFunctionsWorkerApplicationBuilder builder, HashSet<string> excludingFunctions)
        where T : class, IFunctionsWorkerMiddleware
        => builder.UseWhen<T>(context => !excludingFunctions.Contains(context.FunctionDefinition.Name) && context.IsHttpTrigger());

    public static IFunctionsWorkerApplicationBuilder UseDeveloperPlatform(this IFunctionsWorkerApplicationBuilder builder, params string[] excludingFunctions)
    {
        var excluded = new HashSet<string>(excludingFunctions);

        // configure the worker options to use the same serializer as the http options
        // durable functions uses the worker options to serialize input/output
        builder.Services.Configure<WorkerOptions>(o => o.Serializer = EntityObjectSerializer.Default);
        builder.Services.Configure<JsonSerializerOptions>(o =>
        {
            o.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            o.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            EntitySerializerOptions.AddEntitySerialization(o);
        });

        builder
            .UseWhenHttp<UpdateHostMiddleware>(excluded)
            .UseMiddleware<FunctionMetadataMiddleware>()
            .UseWhenHttp<DeveloperPlatformAuthenticationMiddleware>(excluded)
            .UseWhenHttp<DeveloperPlatformAuthorizationMiddleware>(excluded)
            .UseWhenHttp<DeveloperPlatformUserMiddleware>(excluded)
            .UseWhenHttp<DeveloperPlatformRequestMiddleware>(excluded)
            .UseMiddleware<RequireLocalUserMiddleware>();

        return builder;
    }
}