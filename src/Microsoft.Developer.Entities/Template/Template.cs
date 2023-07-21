/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class Template : Entity<TemplateMetadata, TemplateSpec, TemplateStatus>, ITemplate<TemplateMetadata, TemplateSpec, TemplateStatus>
{
    public override string Kind => nameof(Template);
}

public class Template<TConfig> : Entity<TemplateMetadata, TemplateSpec<TConfig>, TemplateStatus>, ITemplate<TemplateMetadata, TemplateSpec<TConfig>, TemplateStatus, TConfig>
    where TConfig : class, ITemplateSpecConfig, new()
{
    public override string Kind => nameof(Template);
}