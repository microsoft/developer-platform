// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities;

public interface IEntityRef<T> where T : IEntityRef<T>
{
    static abstract EntityKind DefaultKind { get; }

    static abstract T Create(EntityName name, EntityNamespace @namespace = default);
}

