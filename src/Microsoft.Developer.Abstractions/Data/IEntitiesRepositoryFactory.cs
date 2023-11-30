// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Data;

public interface IEntitiesRepositoryFactory
{
    IEntitiesRepository Create(string tenantId);
}
