/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class UserSpec : Spec, IUserSpec
{
    public UserRole Role { get; set; }

    public UserProfile? Profile { get; set; }

    public List<GroupEntityRef> MemberOf { get; set; } = new();
}
