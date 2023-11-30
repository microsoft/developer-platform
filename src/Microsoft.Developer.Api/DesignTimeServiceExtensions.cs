using DurableTask.Core;
using DurableTask.DependencyInjection;
using DurableTask.Emulator;
using DurableTask.Hosting;
using System.Security.Claims;

namespace Microsoft.Developer.Api;

public static class DesignTimeServiceExtensions
{
    public static IHostBuilder ConfigureDesignTime(this IHostBuilder host) => host
        .ConfigureTaskHubWorker((context, builder) =>
        {
            builder.WithOrchestrationService(sp => new LocalOrchestrationService());
            builder.AddClient();
        })
        .ConfigureServices(services =>
        {
            var design = new DesignTimeService();

            services.AddSingleton<IUserService>(design);
        });

    private sealed class DesignTimeService : IUserService, ITaskHubWorkerBuilder
    {
        IServiceCollection ITaskHubWorkerBuilder.Services { get; } = new ServiceCollection();

        IOrchestrationService? ITaskHubWorkerBuilder.OrchestrationService { get; set; }

        IList<TaskMiddlewareDescriptor> ITaskHubWorkerBuilder.ActivityMiddleware { get; } = new List<TaskMiddlewareDescriptor>();

        IList<TaskMiddlewareDescriptor> ITaskHubWorkerBuilder.OrchestrationMiddleware { get; } = new List<TaskMiddlewareDescriptor>();

        IList<TaskActivityDescriptor> ITaskHubWorkerBuilder.Activities { get; } = new List<TaskActivityDescriptor>();

        IList<TaskOrchestrationDescriptor> ITaskHubWorkerBuilder.Orchestrations { get; } = new List<TaskOrchestrationDescriptor>();

        ValueTask<Entity?> IUserService.GetOrCreateUser(ClaimsPrincipal principal) => throw new NotImplementedException();

        ValueTask<string?> IUserService.GetUserIdAsync(string identifier) => throw new NotImplementedException();
    }
}
