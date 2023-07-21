/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class TemplateSpec : Spec, ITemplateSpec
{
    public string? InputParametersJsonSchema { get; set; }

    ITemplateSpecConfig? ITemplateSpec.Config { get; }
}

public class TemplateSpec<TConfig> : TemplateSpec, ITemplateSpec<TConfig>
    where TConfig : class, ITemplateSpecConfig, new()
{
    public TConfig? Config
    {
        get => ((ITemplateSpec)this).Config as TConfig;
        // set => ((ITemplateSpec)this).Config = value;
    }
}