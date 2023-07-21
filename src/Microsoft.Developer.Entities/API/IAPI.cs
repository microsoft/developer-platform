/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public interface IAPI<TMetadata, TSpec> : IEntity<TMetadata, TSpec>
    where TMetadata : IMetadata, new()
    where TSpec : IAPISpec, new()
{

}

public interface IAPI<TMetadata, TSpec, TStatus> : IEntity<TMetadata, TSpec, TStatus>
    where TMetadata : IMetadata, new()
    where TSpec : IAPISpec, new()
    where TStatus : IAPIStatus, new()
{

}
