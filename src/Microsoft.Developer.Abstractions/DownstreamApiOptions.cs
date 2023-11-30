// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security.Claims;

namespace Microsoft.Developer;

public class DownstreamApiOptions(string baseUrl, string relativeUrl, HttpMethod method)
{
    public string BaseUrl => baseUrl;

    public string RelativeUrl => relativeUrl;

    public HttpMethod Method => method;

    public required ClaimsPrincipal User { get; init; }

    public IEnumerable<string> Scopes { get; init; } = Enumerable.Empty<string>();

    public HttpContent? Content { get; init; }

    public Action<HttpRequestMessage>? CustomizeHttpRequestMessage { get; set; }
}
