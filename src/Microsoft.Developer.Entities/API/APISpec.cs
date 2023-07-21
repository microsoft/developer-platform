/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class APISpec : Spec, IAPISpec
{
    public string Type { get; set; } = default!;

    public string Lifecycle { get; set; } = default!;

    public EntityRef Owner { get; set; } = default!;

    public SystemEntityRef? System { get; set; }

    public string Definition { get; set; } = default!;

}
