/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Microsoft.Identity.Web;
using Microsoft.Identity.Abstractions;

namespace Microsoft.Developer.Azure;

public interface IUserArmService : IArmService { }

public class UserArmService : ArmService, IUserArmService
{
    private readonly ITokenAcquirerFactory tokenFactory;

    public UserArmService(ITokenAcquirerFactory tokenFactory)
    {
        this.tokenFactory = tokenFactory ?? throw new ArgumentNullException(nameof(tokenFactory));
    }

    public override TokenCredential GetTokenCredential()
        => new TokenAcquirerTokenCredential(tokenFactory.GetTokenAcquirer());
}
