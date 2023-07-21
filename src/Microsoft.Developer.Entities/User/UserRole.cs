/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Text.Json.Serialization;

namespace Microsoft.Developer.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    None,
    Member,
    Admin,
    Owner,
    Provider
}
