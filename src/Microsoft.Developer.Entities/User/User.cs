/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class User : Entity<UserMetadata, UserSpec, UserStatus>, IUser<UserMetadata, UserSpec, UserStatus>
{
    public override string Kind => nameof(User);
}
