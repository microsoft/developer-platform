// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Features;

public interface IDeveloperPlatformRepositoryFeature
{
    IAsyncEnumerable<Entity> GetAsync(EntityKind kind);

    ValueTask<Entity?> GetAsync(EntityRef entityRef);

    IAsyncEnumerable<Entity> GetAsync();
}
