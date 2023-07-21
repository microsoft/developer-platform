/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class Component : Entity<ComponentMetadata, ComponentSpec, ComponentStatus>, IComponent<ComponentMetadata, ComponentSpec, ComponentStatus>
{
    public override string Kind => nameof(Component);
}
