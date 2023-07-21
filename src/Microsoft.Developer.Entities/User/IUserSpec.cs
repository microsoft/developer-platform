/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Text.Json.Nodes;

namespace Microsoft.Developer.Entities;

public interface IUserSpec : ISpec
{
    UserRole Role { get; set; }

    UserProfile? Profile { get; set; }

    List<GroupEntityRef> MemberOf { get; set; }
}