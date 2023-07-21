/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public interface ITemplate<TMetadata, TSpec> : IEntity<TMetadata, TSpec>
    where TMetadata : IMetadata, new()
    where TSpec : ITemplateSpec, new()
{

}

public interface ITemplate<TMetadata, TSpec, TStatus> : IEntity<TMetadata, TSpec, TStatus>
    where TMetadata : IMetadata, new()
    where TSpec : ITemplateSpec, new()
    where TStatus : ITemplateStatus, new()
{

}

public interface ITemplate<TMetadata, TSpec, TStatus, TConfig> : ITemplate<TMetadata, TSpec, TStatus>
    where TMetadata : IMetadata, new()
    where TSpec : ITemplateSpec<TConfig>, new()
    where TStatus : ITemplateStatus, new()
    where TConfig : ITemplateSpecConfig, new()
{

}
