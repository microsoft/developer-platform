// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;

namespace Microsoft.Developer.Data.Cosmos;

internal sealed class CosmosJsonSerializer(JsonSerializerOptions jsonSerializerOptions) : CosmosSerializer
{
    public override T FromStream<T>(Stream stream)
    {
        // CosmosSerializer expects us to dispose of the stream when we're done
        using (stream)
        {
            return JsonSerializer.Deserialize<T>(stream, jsonSerializerOptions)!;
        }
    }

    public override Stream ToStream<T>(T input)
    {
        MemoryStream stream = new();

        JsonSerializer.Serialize(stream, input, jsonSerializerOptions);

        stream.Position = 0;

        return stream;
    }
}
