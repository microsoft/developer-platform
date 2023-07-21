/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public interface ITemplateSpec : ISpec
{
    ITemplateSpecConfig? Config { get; }
}

public interface ITemplateSpec<TConfig> : ITemplateSpec
    where TConfig : ITemplateSpecConfig, new()
{
    new TConfig? Config { get; }
}