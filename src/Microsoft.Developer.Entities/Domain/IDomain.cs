/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public interface IDomain<TMetadata, TSpec> : IEntity<TMetadata, TSpec>
    where TMetadata : IMetadata, new()
    where TSpec : IDomainSpec, new()
{

}

public interface IDomain<TMetadata, TSpec, TStatus> : IEntity<TMetadata, TSpec, TStatus>
    where TMetadata : IMetadata, new()
    where TSpec : IDomainSpec, new()
    where TStatus : IDomainStatus, new()
{

}
