/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Data;


public interface IProjectRepository : IEntityRepository<Project>
{
    Task<string?> ResolveIdAsync(string tenantId, string identifier);

    IAsyncEnumerable<Project> ListAsync(string partitionId, IEnumerable<string> entityIds, CancellationToken cancellationToken = default);
}