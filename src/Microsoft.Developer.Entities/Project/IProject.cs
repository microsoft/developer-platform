/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public interface IProject<TMetadata, TSpec> : IEntity<TMetadata, TSpec>
    where TMetadata : IMetadata, new()
    where TSpec : IProjectSpec, new()
{

}

public interface IProject<TMetadata, TSpec, TStatus> : IEntity<TMetadata, TSpec, TStatus>
    where TMetadata : IMetadata, new()
    where TSpec : IProjectSpec, new()
    where TStatus : IProjectStatus, new()
{

}
