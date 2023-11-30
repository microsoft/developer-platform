// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities;

public partial class Entity
{
    public static partial class WellKnown
    {
        public static class Relations
        {
            public const string OwnedBy = "ownedBy";
            public const string OwnerOf = "ownerOf";

            public const string ProvidesApi = "providesApi";
            public const string ConsumesApi = "consumesApi";

            public const string DependsOn = "dependsOn";
            public const string DependencyOf = "dependencyOf";

            public const string ParentOf = "parentOf";
            public const string ChildOf = "childOf";

            public const string MemberOf = "memberOf";
            public const string HasMember = "hasMember";

            public const string PartOf = "partOf";
            public const string HasPart = "hasPart";
        }
    }
}
