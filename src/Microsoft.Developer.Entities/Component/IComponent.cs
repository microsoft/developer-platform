/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public interface IComponent<TMetadata, TSpec> : IEntity<TMetadata, TSpec>
    where TMetadata : IMetadata, new()
    where TSpec : IComponentSpec, new()
{

}

public interface IComponent<TMetadata, TSpec, TStatus> : IEntity<TMetadata, TSpec, TStatus>
    where TMetadata : IMetadata, new()
    where TSpec : IComponentSpec, new()
    where TStatus : IComponentStatus, new()
{

}
