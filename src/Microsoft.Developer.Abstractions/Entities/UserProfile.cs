// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;

namespace Microsoft.Developer.Entities;

[Description("The profile of the user.")]
public class UserProfile : PropertiesBase<UserProfile>
{
    [Example("John Doe")]
    [Description("A simple display name to present to users.")]
    public string? DisplayName
    {
        get => Get<string>();
        set => Set(value);
    }

    [Description("The job title of the user.")]
    public string? JobTitle
    {
        get => Get<string>();
        set => Set(value);
    }

    [Description("The email address of the user.")]
    [Example("john.doe@email.com")]
    public string? Email
    {
        get => Get<string>();
        set => Set(value);
    }
}
