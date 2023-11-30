// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using DurableTask.Core;
using DurableTask.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Developer.DurableTasks;

public static class DurableTaskHostingExtensions
{
    public static void AddNamedActivity<T>(this ITaskHubWorkerBuilder builder)
        where T : TaskActivity, IVersionedName
    {
        builder.AddActivity<T>(T.Name, T.Version);
    }

    public static Task<TOutput> ScheduleTask<TActivity, TInput, TOutput>(this OrchestrationContext context, TInput input)
        where TActivity : TaskActivity<TInput, TOutput>, IVersionedName
    {
        return context.ScheduleTask<TOutput>(TActivity.Name, TActivity.Version, input);
    }

    public static Task<TOutput> ScheduleAsyncTask<TActivity, TInput, TOutput>(this OrchestrationContext context, TInput input)
        where TActivity : AsyncTaskActivity<TInput, TOutput>, IVersionedName
    {
        return context.ScheduleTask<TOutput>(TActivity.Name, TActivity.Version, input);
    }

    public static void AddNamedOrchestration<T>(this ITaskHubWorkerBuilder builder)
        where T : TaskOrchestration, IVersionedName
    {
        builder.AddOrchestration<T>(T.Name, T.Version);
    }

    public static void ConfigureTaskHubWorker(this IServiceCollection services, Action<ITaskHubWorkerBuilder> configure)
    {
        configure(GetTaskHubBuilder(services));
    }

    private static ITaskHubWorkerBuilder GetTaskHubBuilder(this IServiceCollection services)
    {
        return services.Single(sd => sd.ServiceType == typeof(ITaskHubWorkerBuilder))
            .ImplementationInstance as ITaskHubWorkerBuilder ?? throw new InvalidOperationException("Could not find a " + nameof(ITaskHubWorkerBuilder));
    }
}
