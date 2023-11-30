// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using DurableTask.Core.Middleware;
using DurableTask.Core;
using DurableTask.DependencyInjection;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.Developer.DurableTasks;

public static class DurableTaskClaimsPrincipalExtensions
{
    public const string UserTagKey = "User";

    public static ClaimsPrincipal? GetUser(this DispatchMiddlewareContext context)
        => context.GetProperty<ClaimsPrincipal>();

    public static void UseClaimsPrincipal(this ITaskHubWorkerBuilder builder)
    {
        builder.UseActivityMiddleware<ClaimsPrincipalMiddleware>();
        builder.UseOrchestrationMiddleware<ClaimsPrincipalMiddleware>();
    }

    private sealed class ClaimsPrincipalMiddleware : ITaskMiddleware
    {
        public Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
        {
            if (context.GetTags().TryGetValue(UserTagKey, out var existing))
            {
                var user = CreateUser(existing);
                context.SetProperty(user);

                if (context.GetProperty<TaskActivity>() is IContainer<ClaimsPrincipal> activity)
                {
                    activity.SetItem(user);
                }

                if (context.GetProperty<TaskOrchestration>() is IContainer<ClaimsPrincipal> orchestration)
                {
                    orchestration.SetItem(user);
                }
            }

            return next();
        }

        private static ClaimsPrincipal CreateUser(string base64)
        {
            using var stream = new MemoryStream(Convert.FromBase64String(base64));
            using var reader = new BinaryReader(stream);

            return new ClaimsPrincipal(reader);
        }
    }

    public static DurableTaskEndpointConventionBuilder<TResult> WithUserTag<TResult>(this DurableTaskEndpointConventionBuilder<TResult> builder)
        => builder.WithOrchestrationTag(UserTagKey, context =>
        {
            var ms = new MemoryStream();
            var binary = new BinaryWriter(ms);

            context.User.WriteTo(binary);

            return Convert.ToBase64String(ms.ToArray());
        });
}
