// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities;

public class SystemSpec : Spec
{
    public EntityRef? Owner
    {
        get => Get<EntityRef>();
        set => Set(value);
    }

    public EntityRef? Domain
    {
        get => Get<EntityRef>();
        set => Set(value);
    }
}
