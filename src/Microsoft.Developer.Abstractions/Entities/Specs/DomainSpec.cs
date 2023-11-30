// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities;

public class DomainSpec : Spec
{
    public EntityRef Owner
    {
        get => Get<EntityRef>()!;
        set => Set(value);
    }
}
