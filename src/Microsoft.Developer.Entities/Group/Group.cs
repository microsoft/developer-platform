/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class Group : Entity<GroupMetadata, GroupSpec, GroupStatus>, IGroup<GroupMetadata, GroupSpec, GroupStatus>
{
    public override string Kind => nameof(Group);
}
