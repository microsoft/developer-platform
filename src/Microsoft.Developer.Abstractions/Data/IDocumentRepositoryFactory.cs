// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Data;

public interface IDocumentRepositoryFactory<T>
{
    IDocumentRepository<T> Create(string name);
}
