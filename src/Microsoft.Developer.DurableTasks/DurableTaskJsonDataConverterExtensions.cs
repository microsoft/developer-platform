// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using DurableTask.Core;
using DurableTask.Core.Middleware;
using DurableTask.Core.Serializing;
using DurableTask.DependencyInjection;
using DurableTask.DependencyInjection.Internal;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.Developer.DurableTasks;

public static class DurableTaskJsonDataConverterExtensions
{
    public static void AddJsonSerializer(this ITaskHubWorkerBuilder builder)
    {
        builder.Services.AddSingleton<JsonDataConverter>(sp => new SystemJsonDataConverter(sp.GetRequiredService<IOptions<JsonOptions>>()));
        builder.Services.AddSingleton<DataConverter>(sp => sp.GetRequiredService<JsonDataConverter>());
        builder.Services.AddOptions<TaskHubClientOptions>()
            .Configure<DataConverter>((options, converter) =>
            {
                options.DataConverter = converter;
            });
    }

    public static void UseJsonSerializer(this ITaskHubWorkerBuilder builder)
    {
        builder.UseActivityMiddleware<JsonOptionsMiddleware>();
        builder.UseOrchestrationMiddleware<JsonOptionsMiddleware>();
    }

    private sealed class JsonOptionsMiddleware(JsonDataConverter converter) : ITaskMiddleware
    {
        public Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
        {
            context.SetProperty(converter);
            context.SetProperty<DataConverter>(converter);

            if (context.GetProperty<TaskActivity>() is IContainer<DataConverter> activity)
            {
                activity.SetItem(converter);
            }

            if (context.GetProperty<TaskOrchestration>() is IContainer<DataConverter> orchestration)
            {
                orchestration.SetItem(converter);
            }

            return next();
        }
    }
}
