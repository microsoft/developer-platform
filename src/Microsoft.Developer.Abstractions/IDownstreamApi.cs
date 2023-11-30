// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer;

/// <summary>
/// A simplified API to handle calling to downstream APIs that is more easily mockable, testable, and extensible.
/// </summary>
public interface IDownstreamApi
{
    Task<HttpResponseMessage> SendAsync(DownstreamApiOptions options, CancellationToken token);
}
