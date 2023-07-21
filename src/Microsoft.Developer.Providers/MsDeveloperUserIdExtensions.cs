/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;

using HttpContext = Microsoft.AspNetCore.Http.HttpContext;

namespace Microsoft.Developer.Providers;

public static class MsDeveloperUserIdExtensions
{

    public static MsDeveloperUserId? GetMsDeveloperUserId(this HttpContext context)
        => context.Request.GetMsDeveloperUserId() // check for header first
        ?? context.User.GetMsDeveloperUserId();   // fallback to claims

    public static MsDeveloperUserId? GetMsDeveloperUserId(this HttpRequest request)
        => request.Headers.GetMsDeveloperUserId();

    public static MsDeveloperUserId? GetMsDeveloperUserId(this ClaimsPrincipal principal)
        => principal.GetObjectId() is string userId
        && principal.GetTenantId() is string tenantId
        && (principal.Identity?.IsAuthenticated ?? false)
        ? new MsDeveloperUserId(userId, tenantId)
        : null;

    public static MsDeveloperUserId? GetMsDeveloperUserId(this IHeaderDictionary headers)
        => headers.TryGetValue(MsDeveloperUserId.HEADER, out var headerValue)
        && headerValue.FirstOrDefault()?.ToString() is string header
        && MsDeveloperUserId.Parse(header) is MsDeveloperUserId userId
        ? userId : null;
}