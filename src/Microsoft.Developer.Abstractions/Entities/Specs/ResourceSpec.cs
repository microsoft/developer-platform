// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities;

public class ResourceSpec : Spec
{
    public string? Type
    {
        get => Get<string>();
        set => Set(value);
    }

    public EntityRef? Owner
    {
        get => Get<EntityRef>();
        set => Set(value);
    }

    public EntityRef? System
    {
        get => Get<EntityRef>();
        set => Set(value);
    }

    public string? Lifecycle
    {
        get => Get<string>();
        set => Set(value);
    }

    public IList<EntityRef> DependsOn
    {
        get => GetOrCreate(Factory<EntityRef>.List);
        set => Set(value);
    }

    public IList<EntityRef> DependencyOf
    {
        get => GetOrCreate(Factory<EntityRef>.List);
        set => Set(value);
    }
}
