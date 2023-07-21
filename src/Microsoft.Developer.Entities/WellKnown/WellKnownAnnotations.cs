/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public static class WellKnownAnnotations
{
    public static class Backstage
    {
        const string prefix = "backstage.io";

        public const string ManagedByLocation = $"{prefix}/managed-by-location";

        public const string ManagedByOriginLocation = $"{prefix}/managed-by-origin-location";

        public const string Orphan = $"{prefix}/orphan";

        public const string TechdocsRef = $"{prefix}/techdocs-ref";

        public const string ViewUrl = $"{prefix}/view-url";

        public const string EditUrl = $"{prefix}/edit-url";

        public const string SourceLocation = $"{prefix}/source-location";

        public const string LdapRdn = $"{prefix}/ldap-rdn";

        public const string LdapUuid = $"{prefix}/ldap-uuid";

        public const string LdapDn = $"{prefix}/ldap-dn";

        public const string CodeCoverage = $"{prefix}/code-coverage";
    }

    public static class GitHub
    {
        const string prefix = "github.com";

        public const string ProjectSlug = $"{prefix}/project-slug";

        public const string TeamSlug = $"{prefix}/team-slug";

        public const string UserLogin = $"{prefix}/user-login";
    }

    public static class MSGraph
    {
        const string prefix = "graph.microsoft.com";

        public const string TenantId = $"{prefix}/tenant-id";

        public const string GroupId = $"{prefix}/group-id";

        public const string UserId = $"{prefix}/user-id";
    }
}