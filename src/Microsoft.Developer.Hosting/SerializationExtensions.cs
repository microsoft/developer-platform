// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Serialization.Json.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace Microsoft.Developer.Entities.Serialization;

public static class SerializationExtensions
{
    public static IDeveloperPlatformBuilder AddEntitySerialization(this IDeveloperPlatformBuilder builder)
    {
        builder.Services.ConfigureHttpJsonOptions(o =>
        {
            o.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            EntitySerializerOptions.AddEntitySerialization(o.SerializerOptions);
        });

        return builder;
    }
}
