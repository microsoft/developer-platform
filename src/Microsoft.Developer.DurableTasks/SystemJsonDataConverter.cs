// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using DurableTask.Core.Serializing;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace Microsoft.Developer.DurableTasks;

internal sealed class SystemJsonDataConverter(IOptions<JsonOptions> options) : JsonDataConverter
{
    private readonly JsonSerializerOptions options = options.Value.SerializerOptions;

    public override object Deserialize(string data, Type objectType) => JsonSerializer.Deserialize(data, objectType, options)!;

    public override string Serialize(object value) => JsonSerializer.Serialize(value, options);

    public override string Serialize(object value, bool formatted) => Serialize(value);
}
