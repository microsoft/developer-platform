/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public interface ISystem<TMetadata, TSpec> : IEntity<TMetadata, TSpec>
    where TMetadata : IMetadata, new()
    where TSpec : ISystemSpec, new()
{

}

public interface ISystem<TMetadata, TSpec, TStatus> : IEntity<TMetadata, TSpec, TStatus>
    where TMetadata : IMetadata, new()
    where TSpec : ISystemSpec, new()
    where TStatus : ISystemStatus, new()
{

}
