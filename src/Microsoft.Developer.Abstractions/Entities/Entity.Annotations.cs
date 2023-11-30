// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities;

public partial class Entity
{
    public static partial class WellKnown
    {
        public static class Annotations
        {
            public static class Backstage
            {
                private const string Prefix = "backstage.io";

                public const string ManagedByLocation = $"{Prefix}/managed-by-location";

                public const string ManagedByOriginLocation = $"{Prefix}/managed-by-origin-location";

                public const string Orphan = $"{Prefix}/orphan";

                public const string TechdocsRef = $"{Prefix}/techdocs-ref";

                public const string ViewUrl = $"{Prefix}/view-url";

                public const string EditUrl = $"{Prefix}/edit-url";

                public const string SourceLocation = $"{Prefix}/source-location";

                public const string LdapRdn = $"{Prefix}/ldap-rdn";

                public const string LdapUuid = $"{Prefix}/ldap-uuid";

                public const string LdapDn = $"{Prefix}/ldap-dn";

                public const string CodeCoverage = $"{Prefix}/code-coverage";
            }

            public static class GitHub
            {
                private const string Prefix = "github.com";

                public const string ProjectSlug = $"{Prefix}/project-slug";

                public const string TeamSlug = $"{Prefix}/team-slug";

                public const string UserLogin = $"{Prefix}/user-login";
            }

            public static class MSGraph
            {
                private const string Prefix = "graph.microsoft.com";

                public const string TenantId = $"{Prefix}/tenant-id";

                public const string GroupId = $"{Prefix}/group-id";

                public const string UserId = $"{Prefix}/user-id";
            }
        }
    }
}
