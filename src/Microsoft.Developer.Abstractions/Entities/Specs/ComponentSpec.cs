// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities;

public class ComponentSpec : Spec
{
    public string? Type
    {
        get => Get<string>();
        set => Set(value);
    }

    public string? Lifecycle
    {
        get => Get<string>();
        set => Set(value);
    }

    public EntityRef? Owner
    {
        get => Get<EntityRef>();
        set => Set(value);
    }

    public IList<EntityRef> DependsOn
    {
        get => GetOrCreate(Factory<EntityRef>.List);
        set => Set(value);
    }
}
