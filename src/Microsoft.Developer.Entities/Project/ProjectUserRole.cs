/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProjectUserRole
{
    None,
    Member,
    Admin,
    Owner,
    Provider
}
