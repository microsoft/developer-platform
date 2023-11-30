// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities;

public class LocationSpec : Spec
{
    public string? Type
    {
        get => Get<string>();
        set => Set(value);
    }

    public string? Target
    {
        get => Get<string>();
        set => Set(value);
    }

    public IList<string>? Targets
    {
        get => GetOrCreate(Factory<string>.List);
        set => Set(value);
    }

    public string Presence
    {
        get => GetOrCreate(defaultValue: "required");
        set => Set(value);
    }
}
