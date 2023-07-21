/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Text.Json;
using Azure.Core.Serialization;
using Microsoft.Azure.Cosmos;
using Microsoft.Developer.Entities;

namespace Microsoft.Developer.Data.CosmosDb.Serialization;

public class CosmosDbSerializer : CosmosSerializer
{
    private readonly JsonSerializerOptions options;
    private readonly JsonObjectSerializer serializer;

    public CosmosDbSerializer(JsonSerializerOptions jsonSerializerOptions)
    {
        options = jsonSerializerOptions;
        serializer = new JsonObjectSerializer(jsonSerializerOptions);
    }

    public override T FromStream<T>(Stream stream)
    {
        using (stream)
        {
            if (stream.CanSeek && stream.Length == 0)
                return default!;

            if (typeof(Stream).IsAssignableFrom(typeof(T)))
                return (T)(object)stream;

            if (typeof(IEntity).IsAssignableFrom(typeof(T)))
                return (T?)serializer.Deserialize(stream, typeof(IEntity), default) ?? default!;

            return (T?)serializer.Deserialize(stream, typeof(T), default) ?? default!;
        }
    }

    public override Stream ToStream<T>(T input)
    {
        MemoryStream stream = new();

        if (typeof(IEntity).IsAssignableFrom(input.GetType()))
            serializer.Serialize(stream, input, typeof(IEntity), default);
        else
            serializer.Serialize(stream, input, input.GetType(), default);

        stream.Position = 0;

        return stream;
    }
}