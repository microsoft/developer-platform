// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Features;

public interface IDeveloperPlatformRequestFeature
{
    EntityKind Kind { get; }

    EntityNamespace Namespace { get; }

    EntityName Name { get; }
}
