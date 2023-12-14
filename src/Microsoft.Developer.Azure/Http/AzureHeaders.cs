// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Azure;

public static class AzureHeaders
{
    public const string CorrelationRequestId = "x-ms-correlation-request-id";

    public const string RatelimitRemainingTenantResourceRequests = "x-ms-ratelimit-remaining-tenant-resource-requests";

    public const string UserQuotaRemaining = "x-ms-user-quota-remaining";

    public const string UserQuotaResetsAfter = "x-ms-user-quota-resets-after";

    public const string ResourceGraphRequestDuration = "x-ms-resource-graph-request-duration";

    public const string RatelimitRemainingTenantReads = "x-ms-ratelimit-remaining-tenant-reads";

    public const string RequestId = "x-ms-request-id";

    public const string RoutingRequestId = "x-ms-routing-request-id";
}

