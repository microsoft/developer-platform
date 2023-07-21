/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public interface IAPISpec : ISpec
{
    string Type { get; set; }

    string Lifecycle { get; set; }

    EntityRef Owner { get; set; }

    SystemEntityRef? System { get; set; }

    string Definition { get; set; }
}