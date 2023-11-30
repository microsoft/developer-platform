// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Features;

public interface IDeveloperPlatformUserFeature
{
    Entity Entity { get; }

    MsDeveloperUserId User => new(Entity.Metadata.Uid, Entity.Metadata.Tenant);
}
