/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class Resource : Entity<ResourceMetadata, ResourceSpec, ResourceStatus>, IResource<ResourceMetadata, ResourceSpec, ResourceStatus>
{
    public override string Kind => nameof(Resource);
}
