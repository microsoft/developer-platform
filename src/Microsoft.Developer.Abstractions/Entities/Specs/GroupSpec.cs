// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities;

public class GroupSpec : Spec
{
    public string Type
    {
        get => Get<string>()!;
        set => Set(value);
    }

    //public JsonNode? Profile { get; set; }
    public EntityRef? Parent
    {
        get => Get<EntityRef>();
        set => Set(value);
    }

    public IList<EntityRef> Children
    {
        get => GetOrCreate(Factory<EntityRef>.List);
        set => Set(value);
    }

    public IList<EntityRef>? Members
    {
        get => GetOrCreate(Factory<EntityRef>.List);
        set => Set(value);
    }
}
