// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Developer.Entities;
using Microsoft.Developer.Features;
using Microsoft.Identity.Web;

namespace Microsoft.Developer.Hosting.Middleware;

internal sealed class DeveloperPlatformUserMiddleware : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var httpContext = context.GetRequiredHttpContext();

        if (context.Features.Get<IDeveloperPlatformUserFeature>() is null)
        {
            var devUser = GetUser(httpContext.User);

            if (devUser is not null)
            {
                var platformFeature = new DevPlatformFeature(devUser);

                context.Features.Set<IDeveloperPlatformUserFeature>(platformFeature);
            }
        }

        await next(context);
    }

    private sealed record DevPlatformFeature(Entity Entity) : IDeveloperPlatformUserFeature;

    // TODO: move this to IUserService implementation
    private static Entity? GetUser(ClaimsPrincipal user)
    {
        if (user.GetObjectId() is { } objectId && user.GetTenantId() is { } tenantId && user.GetDisplayName() is { } displayName)
        {
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
                }
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
                }
            }
            };

            return entity;
        }

        return null;
    }
}
