// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Azure;

public static class ResponseExtensions
{
    public static Guid? CorrelationRequestId<T>(this Response<T> response)
        => response.GetRawResponse().Headers.CorrelationRequestId();

    public static Guid? CorrelationRequestId(this ResponseHeaders headers)
        => headers.TryGetValue(AzureHeaders.CorrelationRequestId, out var value)
            && Guid.TryParse(value, out var guid) ? guid : null;


    public static int? RatelimitRemainingTenantResourceRequests<T>(this Response<T> response)
        => response.GetRawResponse().Headers.RatelimitRemainingTenantResourceRequests();

    public static int? RatelimitRemainingTenantResourceRequests(this ResponseHeaders headers)
        => headers.TryGetValue(AzureHeaders.RatelimitRemainingTenantResourceRequests, out var value)
            && int.TryParse(value, out var num) ? num : null;


    public static int? UserQuotaRemaining<T>(this Response<T> response)
        => response.GetRawResponse().Headers.UserQuotaRemaining();

    public static int? UserQuotaRemaining(this ResponseHeaders headers)
        => headers.TryGetValue(AzureHeaders.UserQuotaRemaining, out var value)
            && int.TryParse(value, out var num) ? num : null;


    public static DateTimeOffset? UserQuotaResetsAfter<T>(this Response<T> response)
        => response.GetRawResponse().Headers.UserQuotaResetsAfter();

    public static DateTimeOffset? UserQuotaResetsAfter(this ResponseHeaders headers)
        => headers.TryGetValue(AzureHeaders.UserQuotaResetsAfter, out var value)
            && TimeSpan.TryParse(value, out var span) ? headers.Date?.Add(span) : null;


    public static TimeSpan? ResourceGraphRequestDuration<T>(this Response<T> response)
        => response.GetRawResponse().Headers.ResourceGraphRequestDuration();

    public static TimeSpan? ResourceGraphRequestDuration(this ResponseHeaders headers)
        => headers.TryGetValue(AzureHeaders.ResourceGraphRequestDuration, out var value)
            && TimeSpan.TryParse(value, out var span) ? span : null;


    public static int? RatelimitRemainingTenantReads<T>(this Response<T> response)
        => response.GetRawResponse().Headers.RatelimitRemainingTenantReads();

    public static int? RatelimitRemainingTenantReads(this ResponseHeaders headers)
        => headers.TryGetValue(AzureHeaders.RatelimitRemainingTenantReads, out var value)
            && int.TryParse(value, out var num) ? num : null;


    public static string? RoutingRequestId<T>(this Response<T> response)
        => response.GetRawResponse().Headers.RoutingRequestId();

    public static string? RoutingRequestId(this ResponseHeaders headers)
        => headers.TryGetValue(AzureHeaders.RoutingRequestId, out var value)
            ? value : null;
}
