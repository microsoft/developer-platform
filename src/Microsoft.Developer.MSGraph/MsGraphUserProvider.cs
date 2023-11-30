// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Data;
using Microsoft.Developer.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace Microsoft.Developer.MSGraph;

internal sealed class MsGraphUserProvider(IEntitiesRepositoryFactory repositoryFactory, IMemoryCache cache, IGraphService graph) : IUserService
{
    public async ValueTask<Entity?> GetOrCreateUser(ClaimsPrincipal principal)
    {
        if (principal.GetTenantId() is { } tenantId && principal.GetObjectId() is { } userId)
        {
            var repository = repositoryFactory.Create(tenantId);

            var result = await repository.GetAsync(userId).ConfigureAwait(false)
                ?? await repository.AddAsync(GetNewUser(principal)).ConfigureAwait(false);

            return result;
        }

        return null;
    }

    public async ValueTask<string?> GetUserIdAsync(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new ArgumentNullException(nameof(identifier));
        }

        // TODO: this should be a cross-cutting concern
        var key = $"user_{nameof(GetUserIdAsync)}_{identifier}";

        if (!cache.TryGetValue(key, out string? val))
        {
            var guid = await graph
                .GetUserIdAsync(identifier)
                .ConfigureAwait(false);

            val = guid?.ToString();

            if (!string.IsNullOrEmpty(val))
            {
                return cache.Set(key, val, TimeSpan.FromMinutes(5));
            }
        }

        return val;
    }

    private static Entity GetNewUser(ClaimsPrincipal principal)
    {
        var objectId = principal.GetObjectId() ?? throw new InvalidOperationException();
        var tenantId = principal.GetTenantId() ?? throw new InvalidOperationException();
        var displayName = principal.GetDisplayName() ?? throw new InvalidOperationException();

        var metadata = new Metadata
        {
            Name = displayName,
            Uid = objectId,
            Tenant = tenantId,
            Title = displayName,
            Namespace = Entity.Defaults.Namespace,
            Provider = "graph.microsoft.com"
        };

        metadata.Annotations?.Add("graph.microsoft.com/user-id", objectId);
        metadata.Annotations?.Add("microsoft.com/email", displayName);

        var spec = new UserSpec
        {
            Role = UserRole.Owner,
            Profile = new UserProfile
            {
                DisplayName = displayName,
                JobTitle = "Job Title",
                Email = displayName
            },
        };

        var entity = new Entity(EntityKind.User)
        {
            Metadata = metadata,
            Spec = spec,
            Relations =
            {
                new Relation
                {
                    Type = Entity.WellKnown.Relations.MemberOf,
                    TargetRef = EntityRef.Parse($"Project:devccnter/project")
                },
            }
        };

        return entity;
    }
}
