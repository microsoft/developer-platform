// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.MSGraph;

internal class DeveloperApiDownstreamApi(Identity.Abstractions.IDownstreamApi api) : IDownstreamApi
{
    public Task<HttpResponseMessage> SendAsync(DownstreamApiOptions options, CancellationToken token)
        => api.CallApiForUserAsync(null, opt =>
        {
            opt.RelativePath = options.RelativeUrl;
            opt.HttpMethod = options.Method.Method;
            opt.Scopes = options.Scopes;
            opt.BaseUrl = options.BaseUrl;
            opt.CustomizeHttpRequestMessage += options.CustomizeHttpRequestMessage;
        }, options.User, options.Content, cancellationToken: token);
}
