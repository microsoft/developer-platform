// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities;

[Example("[template:][{namespace}/]{name}")]
public class TemplateRef : EntityRef, IEntityRef<TemplateRef>
{
    public TemplateRef()
        : base(DefaultKind)
    {
    }

    public static EntityKind DefaultKind => EntityKind.Template;

    public static TemplateRef Create(EntityName name, EntityNamespace @namespace = default) => new() { Name = name, Namespace = @namespace };
}

