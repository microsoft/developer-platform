// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using DurableTask.DependencyInjection;
using Microsoft.Developer.Api.Providers;
using Microsoft.Developer.DurableTasks;
using Microsoft.Developer.Features;
using Microsoft.Extensions.Options;

namespace Microsoft.Developer.Api;

public static class ProviderExtensions
{
    public static IDeveloperPlatformBuilder AddProviders(this IDeveloperPlatformBuilder builder, IConfiguration configuration)
        => builder.AddProviders(options => options.Bind(configuration));

    public static IDeveloperPlatformBuilder AddProviders(this IDeveloperPlatformBuilder builder, Action<OptionsBuilder<ProviderOptions>>? configure = null)
    {
        var options = builder.Services.AddOptions<ProviderOptions>();
        configure?.Invoke(options);

        builder.Services.AddScoped<IProviderEntityRepository, RegisteredProvidersRepository>();

        builder.Services.ConfigureTaskHubWorker(builder =>
        {
            builder.AddClient();
            builder.AddJsonSerializer();

            builder.AddNamedActivity<ProviderTemplateActivity>();
            builder.AddNamedActivity<ProviderCheckStatusActivity>();
            builder.AddNamedOrchestration<TemplateOrchestration>();

            builder.UseHostUrl();
            builder.UseJsonSerializer();
            builder.UseClaimsPrincipal();
            builder.UseMicrosoftIdentityWebClaimsPrincipal();
        });

        builder.ConfigurePlatform(options =>
        {
            options.RepositoryFactory = context =>
            {
                var repository = context.RequestServices.GetRequiredService<IProviderEntityRepository>();
                var options = context.RequestServices.GetRequiredService<IOptions<ProviderOptions>>();

                return new ProvidersFeature(context, options, repository);
            };
        });

        return builder;
    }

    private sealed class ProvidersFeature(HttpContext context, IOptions<ProviderOptions> options, IProviderEntityRepository repository) : IDeveloperPlatformRepositoryFeature
    {
        public IAsyncEnumerable<Entity> GetAsync(EntityKind kind)
        {
            var options = PrepareOptions();
            var result = repository.ListAsync(kind, options, context.RequestAborted);

            return FinalizeProvider(result, options);
        }

        public async ValueTask<Entity?> GetAsync(EntityRef entityRef)
        {
            var options = PrepareOptions();
            var result = await repository.GetAsync(entityRef, options, context.RequestAborted);

            options.CustomizeHttpContext?.Invoke(context);

            return result;
        }

        public IAsyncEnumerable<Entity> GetAsync()
        {
            var options = PrepareOptions();
            var result = repository.ListAsync(options, context.RequestAborted);

            return FinalizeProvider(result, options);
        }
        private async IAsyncEnumerable<Entity> FinalizeProvider(Task<IEnumerable<Entity>> e, DownstreamProviderOptions options)
        {
            var result = await e;

            options.CustomizeHttpContext?.Invoke(context);

            foreach (var item in result)
            {
                yield return item;
            }
        }

        private DownstreamProviderOptions PrepareOptions() => new(context.User)
        {
            Providers = context.Request.Headers.Referer switch
            {
                [{ } referer] when Uri.TryCreate(referer, UriKind.Absolute, out var refererUri) => options.Value.Values.Where(p => !p.Uri.IsBaseOf(refererUri)),
                _ => options.Value.Values,
            },
            CustomizeHttpRequestMessage = message =>
            {
                var host = $"{context.Request.Scheme}://{context.Request.Host}";
                message.Headers.Add("Origin", host);
                message.Headers.Referrer = new Uri(host);
            }
        };
    }
}
