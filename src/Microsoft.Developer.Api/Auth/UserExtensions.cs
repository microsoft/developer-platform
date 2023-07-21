/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using User = Microsoft.Developer.Entities.User;

namespace Microsoft.Developer.Api.Auth;

public static class UserExtensions
{
    public static string AuthPolicy(this User user)
        => user?.Spec?.Role.AuthPolicy() ?? throw new ArgumentNullException(nameof(user));

    // public static string AuthPolicy(this User user, string projectId)
    //     => user?.RoleFor(projectId).AuthPolicy() ?? throw new ArgumentNullException(nameof(user));

    // TODO: This is a temporary workaround until we can get the user's role from the relations
    public static string AuthPolicy(this User user, string projectId)
        => user?.Spec?.Role.AuthPolicy() ?? throw new ArgumentNullException(nameof(user));
}

public static class UserRoleExtensions
{
    public static string AuthPolicy(this UserRole role)
        => $"Tenant{role}";


    public static string AuthPolicy(this ProjectUserRole role)
        => $"Project{role}";
}

public static class UserRolePolicies
{
    public static string UserReadPolicy
        => $"User_Read";

    public static string UserWritePolicy
        => $"User_ReadWrite";

    public static string ComponentWritePolicy
        => $"Component_ReadWrite";
}
