// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Entities;
using System.Collections.Concurrent;

namespace Microsoft.Developer.Data;

public sealed class EntitiesRepositoryFactory(IDocumentRepository<Entity> repository) : IEntitiesRepositoryFactory
{
    private readonly ConcurrentDictionary<string, IEntitiesRepository> cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly Func<string, EntitiesRepository> factory = tenantId => new EntitiesRepository(tenantId, repository);

    public IEntitiesRepository Create(string tenantId)
        => cache.GetOrAdd(tenantId, factory);
}
