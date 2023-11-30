// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security.Claims;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;

namespace Microsoft.Developer.Azure;

// Reworks TokenAcquirerTokenCredential from Microsoft.Identity.Web to allow passing the user in directly
// https://github.com/AzureAD/microsoft-identity-web/blob/master/src/Microsoft.Identity.Web.Azure/TokenAcquirerTokenCredential.cs
public class ClaimsPrincipalTokenCredential(ClaimsPrincipal user, ITokenAcquirer tokenAcquirer) : TokenCredential
{
    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        var tokenAcquisitionOptions = new AcquireTokenOptions
        {
            Tenant = user.GetTenantId(),
        };

        // https://github.com/AzureAD/microsoft-identity-web/blob/master/src/Microsoft.Identity.Web.TokenAcquisition/TokenAcquisition.cs#L233C3-L233C3
        var result = tokenAcquirer.GetTokenForUserAsync(requestContext.Scopes, tokenAcquisitionOptions, user, cancellationToken)
            .GetAwaiter()
            .GetResult();

        return new AccessToken(result.AccessToken!, result.ExpiresOn);
    }

    public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        var tokenAcquisitionOptions = new AcquireTokenOptions
        {
            Tenant = user.GetTenantId()
        };

        // https://github.com/AzureAD/microsoft-identity-web/blob/master/src/Microsoft.Identity.Web.TokenAcquisition/TokenAcquisition.cs#L233C3-L233C3
        var result = await tokenAcquirer.GetTokenForUserAsync(requestContext.Scopes, tokenAcquisitionOptions, user, cancellationToken).ConfigureAwait(false);

        return new AccessToken(result.AccessToken!, result.ExpiresOn);
    }
}
