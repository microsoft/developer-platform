/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Api;

public static class Extensions
{
    public static bool RequestPathStartsWithSegments(this HttpContext httpContext, PathString other, bool ignoreCase = true)
        => httpContext.Request.Path.StartsWithSegments(other, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

    public static bool RequestPathEndsWith(this HttpContext httpContext, string value, bool ignoreCase = true)
        => httpContext.Request.Path.Value?.EndsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) ?? false;

    public static string? RouteValueOrDefault(this HttpContext httpContext, string key, bool ignoreCase = true)
        => httpContext.GetRouteData().ValueOrDefault(key, ignoreCase);

    public static string? ValueOrDefault(this RouteData routeData, string key, bool ignoreCase = true)
        => routeData.Values.GetValueOrDefault(key, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)?.ToString();

    internal static bool IsGuid(this string value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        return Guid.TryParse(value, out var _);
    }
}
