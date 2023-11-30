// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities;

public class APISpec : Spec
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

    public EntityRef? System
    {
        get => Get<EntityRef>();
        set => Set(value);
    }

    public string? Definition
    {
        get => Get<string>();
        set => Set(value);
    }
}
