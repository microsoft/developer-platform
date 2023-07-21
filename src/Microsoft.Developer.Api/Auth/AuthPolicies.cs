/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Api.Auth;

public static class AuthPolicies
{
    public const string Default = "default";

    public const string TenantOwner = nameof(TenantOwner);
    public const string TenantAdmin = nameof(TenantAdmin);
    public const string TenantMember = nameof(TenantMember);
    // public const string TenantRead = nameof(TenantRead);

    public const string ProjectOwner = nameof(ProjectOwner);
    public const string ProjectAdmin = nameof(ProjectAdmin);
    public const string ProjectMember = nameof(ProjectMember);

    public const string TenantUserRead = nameof(TenantUserRead);
    public const string TenantUserWrite = nameof(TenantUserWrite);
    public const string ProjectUserWrite = nameof(ProjectUserWrite);

    public const string ProjectComponentOwner = nameof(ProjectComponentOwner);
}
