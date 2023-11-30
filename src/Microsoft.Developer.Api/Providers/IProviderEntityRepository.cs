// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Requests;

namespace Microsoft.Developer.Api.Providers;

public interface IProviderEntityRepository
{
    Task<IEnumerable<Entity>> ListAsync(DownstreamProviderOptions options, CancellationToken cancellationToken);

    Task<IEnumerable<Entity>> ListAsync(EntityKind kind, DownstreamProviderOptions options, CancellationToken cancellationToken);

    ValueTask<Entity?> GetAsync(EntityRef entityRef, DownstreamProviderOptions options, CancellationToken cancellationToken);

    Task<PostedResponse<TemplateResponse>> CheckStatus(ProviderStatusCheck status, DownstreamProviderOptions options, CancellationToken token);

    Task<PostedResponse<TemplateResponse>> PostAsync(TemplateRequest request, DownstreamProviderOptions options, CancellationToken cancellationToken);
}
