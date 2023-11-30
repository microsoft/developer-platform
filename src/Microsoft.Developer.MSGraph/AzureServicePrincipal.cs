// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security.Claims;

namespace Microsoft.Developer.MSGraph;

public sealed class AzureServicePrincipal
{
    public Guid Id { get; internal set; }
    public Guid AppId { get; internal set; }
    public Guid TenantId { get; internal set; }
    public string Name { get; internal set; } = null!;
    public string Password { get; internal set; } = null!;
    public DateTimeOffset? ExpiresOn { get; internal set; }

    public ClaimsIdentity ToClaimsIdentity(string? authenticationType = null)
    {
        const string ObjectIdClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        const string TenantIdClaimType = "http://schemas.microsoft.com/identity/claims/tenantid";

        var claims = new List<Claim>()
        {
            new(ObjectIdClaimType, Id.ToString()),
            new(TenantIdClaimType, TenantId.ToString()),
            new(ClaimTypes.Name, Name)
        };

        return new ClaimsIdentity(claims, authenticationType);
    }
}
