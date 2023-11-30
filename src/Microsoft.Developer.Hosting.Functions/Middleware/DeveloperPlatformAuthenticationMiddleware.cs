// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Microsoft.Developer.Hosting.Middleware;

internal sealed class DeveloperPlatformAuthenticationMiddleware : IFunctionsWorkerMiddleware
{
    // https://github.com/AzureAD/microsoft-identity-web/blob/master/changelog.md?plain=1#L10
    // https://github.com/AzureAD/microsoft-identity-web/blob/master/src/Microsoft.Identity.Web.TokenAcquisition/Constants.cs#L118
    internal const string JwtSecurityTokenUsedToCallWebApi = "JwtSecurityTokenUsedToCallWebAPI";

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var httpContext = context.GetRequiredHttpContext();

        if (await AuthenticateAsync(httpContext) is { } unauthenticatedResult)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsJsonAsync(unauthenticatedResult);
        }
        else
        {
            await next(context);
        }
    }

    private static async Task<ProblemDetails?> AuthenticateAsync(HttpContext httpContext)
    {
        // resolve principal from bearer token and add user/claims to httpContext.User
        // https://github.com/AzureAD/microsoft-identity-web/blob/master/src/Microsoft.Identity.Web/AzureFunctionsAuthenticationHttpContextExtension.cs
        var result = await httpContext.AuthenticateAsync(Constants.Bearer).ConfigureAwait(false);

        if (result.None)
        {
            return null;
        }

        if (result is { Succeeded: true })
        {
            httpContext.User = result.Principal;

            if (httpContext.User.Identity is ClaimsIdentity identity && httpContext.Items.TryGetValue(JwtSecurityTokenUsedToCallWebApi, out var token) && token is SecurityToken)
            {
                // https://github.com/AzureAD/microsoft-identity-web/blob/master/src/Microsoft.Identity.Web.TokenAcquisition/AspNetCore/HttpContextExtensions.cs
                // https://github.com/AzureAD/microsoft-identity-web/blob/master/src/Microsoft.Identity.Web.TokenAcquisition/TokenAcquisition.cs#L666C19-L666C19
                // https://github.com/AzureAD/microsoft-identity-web/blob/master/src/Microsoft.Identity.Web.TokenAcquisition/PrincipalExtensionsForSecurityTokens.cs
                identity.BootstrapContext = token;
                return null;
            }
            else
            {
                return new ProblemDetails
                {
                    Title = "Authentication failed.",
                    Detail = "Unable to get original token",
                };
            }
        }
        else
        {
            return new ProblemDetails
            {
                Title = "Authentication failed.",
                Detail = result?.Failure?.Message,
            };
        }
    }
}
