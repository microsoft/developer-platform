/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class UserProfile : IAdditionalProperties
{
    public string? DisplayName { get; set; }

    public string? JobTitle { get; set; }

    public string? Email { get; set; }

    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}