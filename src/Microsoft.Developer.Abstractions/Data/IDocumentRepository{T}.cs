// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Data;

public interface IDocumentRepository<TDocument>
{
    Task<TDocument> AddAsync(string partitionId, TDocument entity, CancellationToken cancellationToken = default);

    Task<TDocument?> GetAsync(string partitionId, string entityId, CancellationToken cancellationToken = default);

    IAsyncEnumerable<TDocument> QueryAsync(string? partitionId, Func<IQueryable<TDocument>, IQueryable<TDocument>> filter, CancellationToken cancellationToken = default);

    IAsyncEnumerable<TDocument> QueryAsync(string? partitionId, string query, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(string partitionId, string id, CancellationToken cancellationToken = default);

    Task<TDocument> SetAsync(string partitionId, TDocument entity, CancellationToken cancellationToken = default);
}
