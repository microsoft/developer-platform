// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security.Claims;

namespace Microsoft.Developer.Api.Providers;

public class DownstreamProviderOptions(ClaimsPrincipal user)
{
    public ClaimsPrincipal User => user;

    /// <summary>
    /// Gets the providers that should be queried for the downstream call.
    /// </summary>
    public IEnumerable<ProviderDefinition> Providers { get; init; } = Enumerable.Empty<ProviderDefinition>();

    /// <summary>
    /// Gets or sets a callback that is invoked when preparing the <see cref="HttpRequestMessage"/> for the downstream call. 
    /// </summary>
    public Action<HttpRequestMessage>? CustomizeHttpRequestMessage { get; set; }

    /// <summary>
    /// Gets or sets a callback that will be invoked iff in an ASP.NET Core request
    /// </summary>
    public Action<HttpContext>? CustomizeHttpContext { get; set; }
}
