/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Data;
using System.Text;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Applications.Item.AddPassword;
using Microsoft.Graph.Applications.Item.RemovePassword;
using Microsoft.Graph.Models;

namespace Microsoft.Developer.MSGraph;

public class GraphService : IGraphService
{

    private readonly static string[] userSelect = new string[] { "id", "userPrincipalName", "displayName", "givenName", "surname", "mail", "otherMails", "identities", "deletedDateTime", "companyName", "jobTitle", "preferredLanguage", "userType", "department" };
    private readonly static string[] groupSelect = new string[] { "id", "displayName", "mail", "deletedDateTime" };
    private readonly static string[] memberSelect = new string[] { "id", "displayName", "mail", "deletedDateTime" };
    private readonly static string[] servicePrincipalSelect = new string[] { "id", "displayName", "appId", "appDisplayName", "alternativeNames", "deletedDateTime", "servicePrincipalNames", "servicePrincipalType", "replyUrls" };
    private readonly static string[] applicationSelect = new string[] { "id", "displayName", "appId", "deletedDateTime", "identifierUris", "uniqueName", "passwordCredentials" };

    private const string SECRET_DESCRIPTION = "Managed by Microsoft Developer Platform";
    private readonly GraphServiceClient graphClient;

    // public GraphService(ITeamCloudCredentialOptions teamCloudCredentialOptions = null)
    // {
    //     graphClient = new GraphServiceClient(new TeamCloudCredential(teamCloudCredentialOptions));
    // }

    public GraphService()
    {
        graphClient = new GraphServiceClient(new DefaultAzureCredential());
    }

    private static string? SanitizeIdentifier(string identifier) => identifier?
        .Replace("%3A", ":", StringComparison.OrdinalIgnoreCase)?
        .Replace("%2F", "/", StringComparison.OrdinalIgnoreCase);

    public async Task<string?> GetDisplayNameAsync(string identifier)
    {
        identifier = SanitizeIdentifier(identifier) ?? throw new ArgumentNullException(nameof(identifier));

        // assume user first
        var user = await GetUserInternalAsync(graphClient, identifier)
            .ConfigureAwait(false);

        if (!string.IsNullOrEmpty(user?.DisplayName))
            return user.DisplayName;

        // otherwise try to find a service principal
        var principal = await GetServicePrincipalInternalAsync(graphClient, identifier)
            .ConfigureAwait(false);

        if (!string.IsNullOrEmpty(principal?.DisplayName))
            return principal.DisplayName;

        return null;
    }

    public async Task<string?> GetGroupIdAsync(string identifier)
    {
        if (!identifier.IsGuid())
            return null;

        try
        {
            var group = await graphClient
                .Groups[identifier]
                .GetAsync()
                .ConfigureAwait(false);

            return group?.Id;
        }
        catch (ServiceException)
        {
            return null;
        }
    }

    public async IAsyncEnumerable<string> GetGroupMembersAsync(string identifier, bool resolveAllGroups = false)
    {
        if (identifier is null)
            throw new ArgumentNullException(nameof(identifier));

        var groupId = await GetGroupIdAsync(identifier)
            .ConfigureAwait(false);

        if (groupId is not null)
        {
            DirectoryObjectCollectionResponse? collectionResponse = null;

            var memberList = new List<DirectoryObject>();

            if (resolveAllGroups)
            {
                collectionResponse = await graphClient
                    .Groups[groupId]
                    .TransitiveMembers
                    .GetAsync(config =>
                    {
                        config.Headers.Add("ConsistencyLevel", "eventual");
                        config.QueryParameters.Select = memberSelect;
                    })
                    .ConfigureAwait(false);
            }
            else
            {
                collectionResponse = await graphClient
                    .Groups[groupId]
                    .Members
                    .GetAsync(config =>
                    {
                        config.Headers.Add("ConsistencyLevel", "eventual");
                        config.QueryParameters.Select = memberSelect;
                    })
                    .ConfigureAwait(false);
            }

            if (collectionResponse is null)
                yield break;

            var pageIterator = PageIterator<DirectoryObject, DirectoryObjectCollectionResponse>
                .CreatePageIterator(graphClient, collectionResponse, (member) =>
                {
                    memberList.Add(member);
                    return true;
                });

            await pageIterator.IterateAsync();

            foreach (var memberId in memberList.Where(m => !m.DeletedDateTime.HasValue && !string.IsNullOrEmpty(m.Id)).Select(m => m.Id))
                yield return memberId!;
        }
    }

    public async Task<string?> GetLoginNameAsync(string identifier)
    {
        identifier = SanitizeIdentifier(identifier) ?? throw new ArgumentNullException(nameof(identifier));

        var user = await GetUserInternalAsync(graphClient, identifier)
            .ConfigureAwait(false);

        return user?.Mail;
    }

    public async Task<string?> GetMailAddressAsync(string identifier)
    {
        identifier = SanitizeIdentifier(identifier) ?? throw new ArgumentNullException(nameof(identifier));

        var user = await GetUserInternalAsync(graphClient, identifier)
            .ConfigureAwait(false);

        return user?.Mail;
    }

    public async Task<string?> GetUserIdAsync(string identifier)
    {
        identifier = SanitizeIdentifier(identifier) ?? throw new ArgumentNullException(nameof(identifier));

        // assume user first
        var user = await GetUserInternalAsync(graphClient, identifier)
            .ConfigureAwait(false);

        if (!string.IsNullOrEmpty(user?.Id))
            return user.Id;

        // otherwise try to find a service principal
        var principal = await GetServicePrincipalInternalAsync(graphClient, identifier)
            .ConfigureAwait(false);

        if (!string.IsNullOrEmpty(principal?.Id))
            return principal.Id;

        // not a user name or id, and not a service principal name, appId, or id
        return null;
    }

    public Task<bool> IsGroupAsync(string identifier)
        => GetGroupIdAsync(identifier).ContinueWith(t => t.Result is not null, TaskContinuationOptions.OnlyOnRanToCompletion);

    public Task<bool> IsUserAsync(string identifier)
        => GetUserIdAsync(identifier).ContinueWith(t => t.Result is not null, TaskContinuationOptions.OnlyOnRanToCompletion);

    public async Task<AzureServicePrincipal?> CreateServicePrincipalAsync(string name)
    {
        if (name is null)
            throw new ArgumentNullException(nameof(name));

        try
        {
            name = SanitizeServicePrincipalName(name);

            var application = new Application
            {
                DisplayName = name,
                SignInAudience = "AzureADMyOrg",
                RequiredResourceAccess = new List<RequiredResourceAccess> {
                    new RequiredResourceAccess {
                        ResourceAppId = "00000003-0000-0000-c000-000000000000",
                        ResourceAccess = new List<ResourceAccess> {
                            new ResourceAccess {
                                Id = Guid.Parse("e1fe6dd8-ba31-4d61-89e7-88639da4683d"), // User.Read
                                Type = "Scope"
                            }
                        }
                    }
                },
                IdentifierUris = new List<string>
                {
                    $"api://{name}"
                }
            };

            application = await graphClient
                .Applications
                .PostAsync(application)
                .ConfigureAwait(false);

            if (application is null)
                return null;

            var servicePrincipal = new ServicePrincipal
            {
                AppId = application.AppId
            };

            servicePrincipal = await graphClient.ServicePrincipals
                .PostAsync(servicePrincipal)
                .ConfigureAwait(false);

            if (servicePrincipal is null || string.IsNullOrEmpty(servicePrincipal.Id) || string.IsNullOrEmpty(servicePrincipal.AppId))
                return null;

            var expiresOn = DateTimeOffset.UtcNow.AddYears(1);

            var passwordCredential = new PasswordCredential
            {
                StartDateTime = DateTimeOffset.UtcNow,
                EndDateTime = expiresOn,
                KeyId = Guid.NewGuid(),
                CustomKeyIdentifier = Encoding.UTF8.GetBytes(SECRET_DESCRIPTION)
            };

            passwordCredential = await graphClient
                .Applications[application.Id]
                .AddPassword
                .PostAsync(new AddPasswordPostRequestBody { PasswordCredential = passwordCredential })
                .ConfigureAwait(false);

            var azureServicePrincipal = new AzureServicePrincipal
            {
                Id = Guid.Parse(servicePrincipal.Id),
                AppId = Guid.Parse(servicePrincipal.AppId),
                TenantId = servicePrincipal.AppOwnerOrganizationId.GetValueOrDefault(),
                Name = servicePrincipal.ServicePrincipalNames?.FirstOrDefault() ?? string.Empty,
                Password = passwordCredential?.SecretText ?? string.Empty,
                ExpiresOn = expiresOn
            };

            return azureServicePrincipal;
        }
        catch
        {
            throw;
        }
    }

    public async Task DeleteServicePrincipalAsync(string identifier)
    {
        if (identifier is null)
            throw new ArgumentNullException(nameof(identifier));

        identifier = SanitizeServicePrincipalName(identifier);

        var servicePrincipal = await GetServicePrincipalInternalAsync(graphClient, identifier)
            .ConfigureAwait(false);

        if (servicePrincipal is not null && !string.IsNullOrEmpty(servicePrincipal.AppId))
        {
            var application = await GetApplicationInternalAsync(graphClient, servicePrincipal.AppId)
                .ConfigureAwait(false);

            if (application is not null)
            {
                await graphClient
                    .Applications[application.Id]
                    .DeleteAsync()
                    .ConfigureAwait(false);
            }
        }
    }

    public async Task<AzureServicePrincipal?> GetServicePrincipalAsync(string identifier)
    {
        if (identifier is null)
            throw new ArgumentNullException(nameof(identifier));

        identifier = SanitizeServicePrincipalName(identifier);

        var servicePrincipal = await GetServicePrincipalInternalAsync(graphClient, identifier)
            .ConfigureAwait(false);

        if (servicePrincipal is null || string.IsNullOrEmpty(servicePrincipal.AppId) || string.IsNullOrEmpty(servicePrincipal.Id))
            return null;

        var application = await GetApplicationInternalAsync(graphClient, servicePrincipal.AppId)
            .ConfigureAwait(false);

        if (application is null || application.PasswordCredentials is null || !application.PasswordCredentials.Any())
            return null;

        var customKeyIdentifier = Guid.Parse(servicePrincipal.Id).ToByteArray();

        var expiresOn = application.PasswordCredentials
            .SingleOrDefault(c => c.KeyId.GetValueOrDefault().ToString().Equals(servicePrincipal.Id, StringComparison.Ordinal))?.EndDateTime;

        var azureServicePrincipal = new AzureServicePrincipal
        {
            Id = Guid.Parse(servicePrincipal.Id),
            AppId = Guid.Parse(servicePrincipal.AppId),
            TenantId = servicePrincipal.AppOwnerOrganizationId.GetValueOrDefault(),
            Name = servicePrincipal.ServicePrincipalNames?.FirstOrDefault() ?? string.Empty,
            ExpiresOn = expiresOn
        };

        return azureServicePrincipal;
    }

    public async Task<Dictionary<string, Dictionary<Guid, AccessType>>> GetResourcePermissionsAsync(string identifier)
    {
        if (identifier is null)
            throw new ArgumentNullException(nameof(identifier));

        identifier = SanitizeServicePrincipalName(identifier);

        var application = await GetApplicationInternalAsync(graphClient, identifier)
            .ConfigureAwait(false) ?? throw new ArgumentException($"Could not find application by identifier '{identifier}'", nameof(identifier));

        if (application.RequiredResourceAccess is null)
            return new Dictionary<string, Dictionary<Guid, AccessType>>();

        return application.RequiredResourceAccess
            .Where(rra => rra.ResourceAccess?.Any() ?? false && !string.IsNullOrEmpty(rra.ResourceAppId))
            .GroupBy(rra => rra.ResourceAppId!)
            .ToDictionary(grp => grp.Key,
                          grp => grp.SelectMany(item => item.ResourceAccess!)
                                    .Where(ra => ra.Id.HasValue && !string.IsNullOrEmpty(ra.Type))
                                    .ToDictionary(ra => ra.Id!.Value, ra => Enum.Parse<AccessType>(ra.Type!)));
    }

    public async Task<Dictionary<string, Dictionary<Guid, AccessType>>> SetResourcePermissionsAsync(string identifier, Dictionary<string, Dictionary<Guid, AccessType>> resourcePermissions)
    {
        if (identifier is null)
            throw new ArgumentNullException(nameof(identifier));

        identifier = SanitizeServicePrincipalName(identifier);

        var application = await GetApplicationInternalAsync(graphClient, identifier)
            .ConfigureAwait(false) ?? throw new ArgumentException($"Could not find application by identifier '{identifier}'", nameof(identifier));

        var requiredResourceAccess = Enumerable.Empty<RequiredResourceAccess>();

        if (resourcePermissions?.Any() ?? false)
        {
            requiredResourceAccess = resourcePermissions.Select(rp => new RequiredResourceAccess()
            {
                ResourceAppId = rp.Key,
                ResourceAccess = ((rp.Value?.Any() ?? false)
                    ? rp.Value.Select(ra => new ResourceAccess() { Id = ra.Key, Type = ra.Value.ToString() })
                    : Enumerable.Empty<ResourceAccess>()).ToList()
            });
        }

        var applicationPatch = new Application()
        {
            RequiredResourceAccess = requiredResourceAccess.ToList()
        };

        await graphClient.Applications[application.Id]
            .PatchAsync(applicationPatch)
            .ConfigureAwait(false);

        return await GetResourcePermissionsAsync(identifier)
            .ConfigureAwait(false);
    }

    public async Task<List<string>> GetServicePrincipalRedirectUrlsAsync(string identifier)
    {
        if (identifier is null)
            throw new ArgumentNullException(nameof(identifier));

        identifier = SanitizeServicePrincipalName(identifier);

        var servicePrincipal = await GetServicePrincipalInternalAsync(graphClient, identifier)
            .ConfigureAwait(false);

        return servicePrincipal?.ReplyUrls ?? new List<string>();
    }

    public async Task<AzureServicePrincipal?> RefreshServicePrincipalAsync(string identifier)
    {
        if (identifier is null)
            throw new ArgumentNullException(nameof(identifier));

        identifier = SanitizeServicePrincipalName(identifier);

        var servicePrincipal = await GetServicePrincipalInternalAsync(graphClient, identifier)
            .ConfigureAwait(false);

        if (servicePrincipal is null || string.IsNullOrEmpty(servicePrincipal.Id) || string.IsNullOrEmpty(servicePrincipal.AppId))
            return null;

        var application = await GetApplicationInternalAsync(graphClient, servicePrincipal.AppId)
            .ConfigureAwait(false);

        if (application is null || application.PasswordCredentials is null || !application.PasswordCredentials.Any())
            return null;

        var expiresOn = DateTimeOffset.UtcNow.AddYears(1);

        var passwordCredential = application
            .PasswordCredentials
            .FirstOrDefault(cred => Encoding.UTF8.GetBytes(SECRET_DESCRIPTION).SequenceEqual(cred.CustomKeyIdentifier ?? Enumerable.Empty<byte>()));

        if (passwordCredential is not null && passwordCredential.KeyId.HasValue)
        {
            await graphClient
                .Applications[application.Id]
                .RemovePassword
                .PostAsync(new RemovePasswordPostRequestBody { KeyId = passwordCredential.KeyId.Value })
                .ConfigureAwait(false);
        }

        passwordCredential = new PasswordCredential
        {
            StartDateTime = DateTimeOffset.UtcNow,
            EndDateTime = expiresOn,
            KeyId = Guid.NewGuid(),
            CustomKeyIdentifier = Encoding.UTF8.GetBytes(SECRET_DESCRIPTION)
        };

        passwordCredential = await graphClient
            .Applications[application.Id]
            .AddPassword
            .PostAsync(new AddPasswordPostRequestBody { PasswordCredential = passwordCredential })
            .ConfigureAwait(false);

        var azureServicePrincipal = new AzureServicePrincipal
        {
            Id = Guid.Parse(servicePrincipal.Id),
            AppId = Guid.Parse(servicePrincipal.AppId),
            TenantId = servicePrincipal.AppOwnerOrganizationId.GetValueOrDefault(),
            Name = servicePrincipal.ServicePrincipalNames?.FirstOrDefault() ?? string.Empty,
            Password = passwordCredential?.SecretText ?? string.Empty,
            ExpiresOn = expiresOn
        };

        return azureServicePrincipal;
    }

    public async Task<IEnumerable<string>> SetServicePrincipalRedirectUrlsAsync(string identifier, params string[] redirectUrls)
    {
        if (identifier is null)
            throw new ArgumentNullException(nameof(identifier));

        identifier = SanitizeServicePrincipalName(identifier);

        var application = await GetApplicationInternalAsync(graphClient, identifier)
            .ConfigureAwait(false);

        if (application is null)
            return new List<string>();

        redirectUrls = redirectUrls.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

        var applicationPatch = new Application()
        {
            Web = new WebApplication()
            {
                RedirectUris = redirectUrls.ToList()
            }
        };

        await graphClient
            .Applications[application.Id]
            .PatchAsync(applicationPatch)
            .ConfigureAwait(false);

        var servicePrincipal = await GetServicePrincipalInternalAsync(graphClient, identifier)
            .ConfigureAwait(false);

        if (servicePrincipal is not null)
        {
            var servicePrincipalPath = new ServicePrincipal()
            {
                ReplyUrls = redirectUrls.ToList()
            };

            await graphClient
                .ServicePrincipals[servicePrincipal.Id]
                .PatchAsync(servicePrincipalPath)
                .ConfigureAwait(false);
        }

        return redirectUrls;
    }

    public static async Task<List<Domain>> GetDomainsAsync(GraphServiceClient client)
    {
        try
        {
            var collectionResponse = await client
                .Domains
                .GetAsync()
                .ConfigureAwait(false);

            var domains = new List<Domain>();

            if (collectionResponse is null)
                return domains;

            var pageIterator = PageIterator<Domain, DomainCollectionResponse>
                .CreatePageIterator(client, collectionResponse, (domain) =>
                {
                    domains.Add(domain);
                    return true;
                });

            await pageIterator.IterateAsync();

            return domains;

        }
        // catch (ServiceException exc)
        catch (ServiceException)
        {
            return new List<Domain>();
        }
    }

    private static async Task<User?> GetUserInternalAsync(GraphServiceClient client, string identifier)
    {
        if (identifier.StartsWithHttp())
            return null;

        if (!(identifier.IsGuid() || identifier.IsEMail()))
            return null;

        if (identifier.IsEMail())
        {
            var domains = await GetDomainsAsync(client);

            var hasVerifiedDomain = domains
                .Where(d => d.IsVerified.HasValue && d.IsVerified.Value)
                .Any(d => identifier.EndsWith($"@{d.Id}", StringComparison.OrdinalIgnoreCase));

            if (!hasVerifiedDomain)
            {
                var defaultDomain = domains
                    .First(d => d.IsDefault.HasValue && d.IsDefault.Value);

                identifier = $"{identifier.Replace("@", "_", StringComparison.OrdinalIgnoreCase)}#EXT#@{defaultDomain.Id}";
            }
        }

        try
        {
            return await client
                .Users[identifier]
                .GetAsync(config => config.QueryParameters.Select = userSelect)
                .ConfigureAwait(false);
        }
        catch (ServiceException)
        {
            return null;
        }
    }

    private static async Task<ServicePrincipal?> GetServicePrincipalInternalAsync(GraphServiceClient client, string identifier)
    {
        var filter = $"servicePrincipalNames/any(p:p eq '{identifier}') or servicePrincipalNames/any(p:p eq 'api://{identifier}')";

        if (identifier.IsGuid())
            filter += $" or id eq '{identifier}'";

        try
        {
            var collectionResponse = await client
                .ServicePrincipals
                .GetAsync(config =>
                {
                    config.QueryParameters.Filter = filter;
                    config.QueryParameters.Select = servicePrincipalSelect;
                })
                .ConfigureAwait(false);

            if (collectionResponse is null)
                return null;

            var principal = collectionResponse.Value?.FirstOrDefault();

            if (principal is null)
            {
                var pageIterator = PageIterator<ServicePrincipal, ServicePrincipalCollectionResponse>
                    .CreatePageIterator(client, collectionResponse, (sp) =>
                    {
                        principal = sp;
                        return false;
                    });

                await pageIterator.IterateAsync();
            }

            if (principal is not null)
                principal = await client
                    .ServicePrincipals[principal.Id]
                    .GetAsync()
                    .ConfigureAwait(false);

            return principal;
        }
        catch (ServiceException)
        {
            return null;
        }
    }

    private static async Task<Application?> GetApplicationInternalAsync(GraphServiceClient client, string identifier)
    {
        var filter = $"identifierUris/any(p:p eq 'http://{identifier}') or identifierUris/any(p:p eq 'https://{identifier}') or identifierUris/any(p:p eq 'api://{identifier}')";

        if (identifier.IsGuid())
            filter += $" or id eq '{identifier}' or appId eq '{identifier}'";

        try
        {
            var collectionResponse = await client
                .Applications
                .GetAsync(config =>
                {
                    config.QueryParameters.Filter = filter;
                    config.QueryParameters.Select = applicationSelect;
                })
                .ConfigureAwait(false);

            if (collectionResponse is null)
                return null;

            var application = collectionResponse.Value?.FirstOrDefault();

            if (application is null)
            {
                var pageIterator = PageIterator<Application, ApplicationCollectionResponse>
                    .CreatePageIterator(client, collectionResponse, (app) =>
                    {
                        application = app;
                        return false;
                    });

                await pageIterator.IterateAsync();
            }

            if (application is not null)
                application = await client
                    .Applications[application.Id]
                    .GetAsync()
                    .ConfigureAwait(false);

            return application;
        }
        catch (ServiceException)
        {
            return null;
        }
    }

    private static string SanitizeServicePrincipalName(string name)
    {
        const string ServicePrincipalNamePrefix = "MSDeveloper/";

        if (name.StartsWith(ServicePrincipalNamePrefix, StringComparison.OrdinalIgnoreCase))
            name = ServicePrincipalNamePrefix + name[ServicePrincipalNamePrefix.Length..];
        else
            name = ServicePrincipalNamePrefix + name;

        return name.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase);
    }
}
