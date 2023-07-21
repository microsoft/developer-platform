/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class System : Entity<SystemMetadata, SystemSpec, SystemStatus>, ISystem<SystemMetadata, SystemSpec, SystemStatus>
{
    public override string Kind => nameof(System);
}
