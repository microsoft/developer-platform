// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Serialization.Json.Entities;

internal interface IPropertyBagOptions
{
    ICollection<string> IgnoredKeys { get; }
}
