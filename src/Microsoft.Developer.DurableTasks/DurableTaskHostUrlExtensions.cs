// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using DurableTask.Core.Middleware;
using DurableTask.Core;
using DurableTask.DependencyInjection;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.Developer.DurableTasks;

public static class DurableTaskHostUrlExtensions
{
    public const string HostUrlTagKey = "HostUrl";

    public static Uri? GetHostUrl(this DispatchMiddlewareContext context)
        => context.GetProperty<Uri>(HostUrlTagKey);

    public static void UseHostUrl(this ITaskHubWorkerBuilder builder)
    {
        builder.UseActivityMiddleware<HostUrlMiddleware>();
        builder.UseOrchestrationMiddleware<HostUrlMiddleware>();
    }

    private sealed class HostUrlMiddleware : ITaskMiddleware
    {
        public Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
        {
            if (context.GetTags().TryGetValue(HostUrlTagKey, out var existing))
            {
                var uri = new Uri(existing);
                context.SetProperty(HostUrlTagKey, uri);

                if (context.GetProperty<TaskActivity>() is IContainer<Uri> activity)
                {
                    activity.SetItem(uri);
                }

                if (context.GetProperty<TaskOrchestration>() is IContainer<Uri> orchestration)
                {
                    orchestration.SetItem(uri);
                }
            }

            return next();
        }
    }

    public static DurableTaskEndpointConventionBuilder<TResult> WithHostUrl<TResult>(this DurableTaskEndpointConventionBuilder<TResult> builder)
        => builder.WithOrchestrationTag(HostUrlTagKey, context => new Uri($"{context.Request.Scheme}://{context.Request.Host}").ToString());
}
