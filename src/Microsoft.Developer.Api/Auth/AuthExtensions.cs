/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Diagnostics;
using System.Security.Claims;
using Microsoft.Developer.Api.Services;
using Microsoft.Developer.Data;
using Microsoft.Identity.Web;
using Microsoft.Developer.Configuration;
using Microsoft.Developer.Configuration.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Abstractions;

namespace Microsoft.Developer.Api.Auth;

internal static class AuthExtensions
{
    internal static IServiceCollection AddMsDeveloperAuthentication(this IServiceCollection services, IConfiguration config, AzureAdOptions? azureAdOptions = null)
    {
        azureAdOptions ??= config.TryBind(AzureAdOptions.Section, out azureAdOptions)
            ? azureAdOptions
            : throw new InvalidOperationException($"{AzureAdOptions.Section} is not configured.");

        services
            .AddTokenAcquisition()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(jwtOptions =>
            {
                jwtOptions.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var claims = await context.HttpContext
                            .ResolveClaimsAsync(context.Principal!)
                            .ConfigureAwait(false);

                        if (claims.Any())
                            context.Principal!.AddIdentity(new ClaimsIdentity(claims));
                    }
                };
            }, identityOptions =>
            {
                identityOptions.ClientId = azureAdOptions!.ClientId;
                identityOptions.Instance = azureAdOptions.Instance;
                identityOptions.TenantId = azureAdOptions.TenantId;
                identityOptions.ClientSecret = azureAdOptions.ClientSecret;
                identityOptions.ClientCredentials = azureAdOptions.ClientCredentials
                    .Select(c => new CredentialDescription { SourceType = CredentialSource.ClientSecret, ClientSecret = c.ClientSecret })
                    .ToList();

            }).EnableTokenAcquisitionToCallDownstreamApi(clientOptions =>
            {
                // clientOptions.ClientId = azureAdOptions.ClientId;
            })
            // .EnableTokenAcquisitionToCallDownstreamApi()
            // .AddDownstreamApi("ARM", config.GetSection("ARM"))
            // .AddDownstreamApi("DevCenter", config.GetSection("DevCenter"))
            .AddDistributedTokenCaches();

        return services;
    }

    internal static IServiceCollection AddMsDeveloperAuthorization(this IServiceCollection services)
    {
        services
            .AddAuthorization(options =>
            {
                options.AddPolicy(AuthPolicies.Default, policy =>
                {
                    policy.RequireAuthenticatedUser();
                });


                options.AddPolicy(AuthPolicies.TenantOwner, policy =>
                {
                    policy.RequireRole(UserRole.Owner.AuthPolicy());
                });

                options.AddPolicy(AuthPolicies.TenantAdmin, policy =>
                {
                    policy.RequireRole(UserRole.Owner.AuthPolicy(),
                                       UserRole.Admin.AuthPolicy());
                });

                options.AddPolicy(AuthPolicies.TenantMember, policy =>
                {
                    policy.RequireRole(UserRole.Owner.AuthPolicy(),
                                       UserRole.Admin.AuthPolicy(),
                                       UserRole.Member.AuthPolicy());
                });

                // options.AddPolicy(AuthPolicies.TenantRead, policy =>
                // {
                //     policy.RequireRole(UserRole.Owner.AuthPolicy(),
                //                        UserRole.Admin.AuthPolicy(),
                //                        UserRole.Member.AuthPolicy(),
                //                        UserRole.None.AuthPolicy());
                // });


                options.AddPolicy(AuthPolicies.ProjectOwner, policy =>
                {
                    policy.RequireRole(UserRole.Owner.AuthPolicy(),
                                       UserRole.Admin.AuthPolicy(),
                                       ProjectUserRole.Owner.AuthPolicy());
                });

                options.AddPolicy(AuthPolicies.ProjectAdmin, policy =>
                {
                    policy.RequireRole(UserRole.Owner.AuthPolicy(),
                                       UserRole.Admin.AuthPolicy(),
                                       ProjectUserRole.Owner.AuthPolicy(),
                                       ProjectUserRole.Admin.AuthPolicy());
                });

                options.AddPolicy(AuthPolicies.ProjectMember, policy =>
                {
                    policy.RequireRole(UserRole.Owner.AuthPolicy(),
                                       UserRole.Admin.AuthPolicy(),
                                       ProjectUserRole.Owner.AuthPolicy(),
                                       ProjectUserRole.Admin.AuthPolicy(),
                                       ProjectUserRole.Member.AuthPolicy());
                });

                options.AddPolicy(AuthPolicies.TenantUserRead, policy =>
                {
                    policy.RequireRole(UserRole.Owner.AuthPolicy(),
                                       UserRole.Admin.AuthPolicy(),
                                       UserRolePolicies.UserWritePolicy,
                                       UserRolePolicies.UserReadPolicy);
                });


                options.AddPolicy(AuthPolicies.TenantUserWrite, policy =>
                {
                    policy.RequireRole(UserRole.Owner.AuthPolicy(),
                                       UserRole.Admin.AuthPolicy(),
                                       UserRolePolicies.UserWritePolicy);
                });

                options.AddPolicy(AuthPolicies.ProjectUserWrite, policy =>
                {
                    policy.RequireRole(UserRole.Owner.AuthPolicy(),
                                       UserRole.Admin.AuthPolicy(),
                                       ProjectUserRole.Owner.AuthPolicy(),
                                       ProjectUserRole.Admin.AuthPolicy(),
                                       UserRolePolicies.UserWritePolicy);
                });


                options.AddPolicy(AuthPolicies.ProjectComponentOwner, policy =>
                {
                    policy.RequireRole(UserRole.Owner.AuthPolicy(),
                                       UserRole.Admin.AuthPolicy(),
                                       ProjectUserRole.Owner.AuthPolicy(),
                                       ProjectUserRole.Admin.AuthPolicy(),
                                       UserRolePolicies.ComponentWritePolicy);
                });
            });

        return services;
    }

    internal static async Task<IEnumerable<Claim>> ResolveClaimsAsync(this HttpContext httpContext, ClaimsPrincipal principal)
    {
        var claims = new List<Claim>();

        var tenantId = principal.GetTenantId() ?? throw new InvalidOperationException();
        var userId = principal.GetObjectId() ?? throw new InvalidOperationException();

        var users = httpContext.RequestServices
            .GetRequiredService<IUserRepository>();

        var user = await users.GetAsync(tenantId, userId).ConfigureAwait(false)
                ?? await users.AddAsync(principal.GetNewUser()).ConfigureAwait(false);

        // var users = httpContext.RequestServices
        //     .GetRequiredService<IEntitiesRepository>();

        // var user = await users.GetAsync<User>(tenantId, userId).ConfigureAwait(false)
        //         ?? await users.AddAsync(principal.GetNewUser()).ConfigureAwait(false);


        if (user is null || user.Spec?.Role == UserRole.None)
            return claims;

        claims.Add(new Claim(ClaimTypes.Role, user.AuthPolicy()));

        if (httpContext.RequestPathStartsWithSegments($"/projects"))
        {
            claims.AddRange(await httpContext.ResolveProjectClaimsAsync(user));
        }
        else if (httpContext.RequestPathStartsWithSegments($"/users")
              || httpContext.RequestPathStartsWithSegments($"/me"))
        {
            claims.AddRange(await httpContext.ResolveUserClaimsAsync(user));
        }

        Debug.WriteLine($"User {userId} => {string.Join(", ", claims.Select(c => c.Value))}");

        return claims;
    }

    private static async Task<IEnumerable<Claim>> ResolveProjectClaimsAsync(this HttpContext httpContext, User user)
    {
        var claims = new List<Claim>();

        var project = httpContext.RouteValueOrDefault("projectId");

        if (string.IsNullOrEmpty(project))
            return claims;

        var projectId = await ResolveProjectIdFromRouteAsync(httpContext, user);

        if (!string.IsNullOrEmpty(projectId))
        {
            claims.Add(new Claim(ClaimTypes.Role, user.AuthPolicy(projectId)));

            if (httpContext.RequestPathStartsWithSegments($"/projects/{project}/users"))
                claims.AddRange(await httpContext.ResolveUserClaimsAsync(user));

            // if (httpContext.RequestPathStartsWithSegments($"{orgPath}/projects/{project}/components"))
            //     claims.AddRange(await httpContext.ResolveComponentClaimsAsync(projectId, user));
        }

        return claims;
    }

    private static async Task<IEnumerable<Claim>> ResolveUserClaimsAsync(this HttpContext httpContext, User user)
    {
        var claims = new List<Claim>();

        string? userId;

        if (httpContext.RequestPathEndsWith("/me"))
        {
            userId = user.Metadata.Uid;
        }
        else
        {
            userId = await httpContext.ResolveUserIdFromRouteAsync();
        }

        if (!string.IsNullOrEmpty(userId) && userId.Equals(user.Metadata.Uid, StringComparison.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(ClaimTypes.Role, UserRolePolicies.UserReadPolicy));
            claims.Add(new Claim(ClaimTypes.Role, UserRolePolicies.UserWritePolicy));
        }

        return claims;
    }

    private static Task<string?> ResolveProjectIdFromRouteAsync(this HttpContext httpContext, User user)
    {
        var projectId = httpContext.RouteValueOrDefault("projectId");

        if (string.IsNullOrEmpty(projectId) || projectId.IsGuid())
            return Task.FromResult(projectId);

        var projectsRepository = httpContext.RequestServices
            .GetRequiredService<IProjectRepository>();

        return projectsRepository
            .ResolveIdAsync(user.Tenant, projectId);
    }

    private static Task<string?> ResolveUserIdFromRouteAsync(this HttpContext httpContext)
    {
        var userId = httpContext.RouteValueOrDefault("userId");

        if (string.IsNullOrEmpty(userId) || userId.IsGuid())
            return Task.FromResult(userId);

        var userService = httpContext.RequestServices
            .GetRequiredService<UserService>();

        return userService.GetUserIdAsync(userId);
    }

    public static User GetNewUser(this ClaimsPrincipal principal)
    {
        var objectId = principal.GetObjectId() ?? throw new InvalidOperationException();
        var tenantId = principal.GetTenantId() ?? throw new InvalidOperationException();
        var displayName = principal.GetDisplayName() ?? throw new InvalidOperationException();

        var metadata = new UserMetadata
        {
            Name = displayName,
            Uid = objectId,
            Tenant = tenantId,
            Title = displayName,
            Namespace = EntityDefaults.Namespace,
        };

        metadata.Annotations ??= new Dictionary<ProviderKey, string>();

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

        var entity = new User
        {
            ApiVersion = EntityDefaults.ApiVersion,
            Provider = EntityDefaults.Platform,
            Metadata = metadata,
            Spec = spec,
            Relations = new List<Relation>() {
                new Relation {
                    Type = WellKnownRelations.MemberOf,
                    TargetRef = new EntityRef($"Project:{EntityDefaults.Platform}/devccnter/project")
                },
            }
        };

        return entity;
    }
}
