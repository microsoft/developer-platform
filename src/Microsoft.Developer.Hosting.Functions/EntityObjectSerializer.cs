// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core.Serialization;
using Microsoft.Developer.Serialization.Json.Entities;

namespace Microsoft.Developer.Entities.Serialization;

public class EntityObjectSerializer(JsonSerializerOptions options) : JsonObjectSerializer(options)
{
    public static new readonly EntityObjectSerializer Default = new(EntitySerializerOptions.Default);
}