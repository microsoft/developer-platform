// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using DurableTask.DependencyInjection;
using Microsoft.Developer.DurableTasks;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Microsoft.Developer.Api;

internal static class MicrosoftIdentityWebDurableTaskExtensions
{
    private const string UserJWT = "UserJWT";

    public static void UseMicrosoftIdentityWebClaimsPrincipal(this ITaskHubWorkerBuilder builder)
    {
        builder.UseActivityMiddleware((context, next) =>
        {
            if (context.GetUser() is { Identity: ClaimsIdentity identity } && context.GetTags().TryGetValue(UserJWT, out var token))
            {
                // Microsoft.Identity.Web expects this to be set
                identity.BootstrapContext = new JsonWebToken(token);
            }

            return next();
        });
    }

    public static DurableTaskEndpointConventionBuilder<TResult> WithMicrosoftWebIdentityTags<TResult>(this DurableTaskEndpointConventionBuilder<TResult> builder)
        => builder.WithOrchestrationTag(UserJWT, context =>
        {
            const string JwtSecurityTokenUsedToCallWebApi = "JwtSecurityTokenUsedToCallWebAPI";
            if (context.Items.TryGetValue(JwtSecurityTokenUsedToCallWebApi, out var result) && result is SecurityToken token)
            {
                return token.UnsafeToString();
            }

            return null;
        });
}
