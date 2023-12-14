// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Data;
using Microsoft.Developer.Entities;
using Microsoft.Developer.Features;
using Microsoft.Identity.Web;

namespace Microsoft.Developer.Providers.TestProvider;

public static class TestProviderExtensions
{
    public static IDeveloperPlatformBuilder AddTestProvider(this IDeveloperPlatformBuilder builder)
    {
        builder.Services.AddSingleton<IEntitiesRepositoryFactory, InMemoryEntityRepository>();
        builder.Services.AddSingleton<IDownstreamApi, TestProviderServices>();

        return builder.ConfigurePlatform(options =>
        {
            options.RepositoryFactory = context =>
            {
                return new TestRepositoryFeature(context.User.GetTenantId()!);
            };
        });
    }

    private sealed class TestRepositoryFeature(string tenantId) : IDeveloperPlatformRepositoryFeature
    {
        private readonly Dictionary<EntityRef, Entity> entities = BuildTestData(tenantId).ToDictionary(t => t.GetEntityRef());

        private static IEnumerable<Entity> BuildTestData(string tenantId)
        {
            yield return new Entity(EntityKind.Template)
            {
                Metadata =
                {
                    Uid = "templateId",
                    Tenant = tenantId,
                }
            };
        }

        public IAsyncEnumerable<Entity> GetAsync(EntityKind kind) => entities.Values.Where(e => e.Kind == kind).ToAsyncEnumerable();

        public ValueTask<Entity?> GetAsync(EntityRef entityRef) => ValueTask.FromResult(Get(entityRef));

        private Entity? Get(EntityRef entityRef) => entities.TryGetValue(entityRef, out var entity) ? entity : null;

        public IAsyncEnumerable<Entity> GetAsync() => entities.Values.ToAsyncEnumerable();
    }

    private sealed class TestProviderServices : IDownstreamApi
    {
        Task<HttpResponseMessage> IDownstreamApi.SendAsync(DownstreamApiOptions options, CancellationToken token) => throw new NotImplementedException();
    }
}
