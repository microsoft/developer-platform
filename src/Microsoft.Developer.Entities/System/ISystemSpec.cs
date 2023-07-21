/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public interface ISystemSpec : ISpec
{
    EntityRef Owner { get; set; }

    DomainEntityRef? Domain { get; set; }
}