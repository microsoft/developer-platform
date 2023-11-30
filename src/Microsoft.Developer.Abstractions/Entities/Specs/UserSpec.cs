// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;

namespace Microsoft.Developer.Entities;

[Description("The specification data describing the user.")]
public class UserSpec : Spec
{
    public UserRole Role
    {
        get => Get<UserRole>();
        set => Set(value);
    }

    public UserProfile? Profile
    {
        get => Get<UserProfile>();
        set => Set(value);
    }

    public IList<EntityRef> MemberOf
    {
        get => GetOrCreate(Factory<EntityRef>.List);
        set => Set(value);
    }
}
