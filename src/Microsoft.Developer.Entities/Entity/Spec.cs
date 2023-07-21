/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public abstract class Spec : ISpec
{
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}