/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class Location : Entity<LocationMetadata, LocationSpec, LocationStatus>, ILocation<LocationMetadata, LocationSpec, LocationStatus>
{
    public override string Kind => nameof(Location);
}
