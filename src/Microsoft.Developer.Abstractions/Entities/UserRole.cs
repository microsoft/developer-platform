// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;

namespace Microsoft.Developer.Entities;

[Description("The role of the user.")]
public enum UserRole
{
    None,
    Member,
    Admin,
    Owner,
    Provider
}
