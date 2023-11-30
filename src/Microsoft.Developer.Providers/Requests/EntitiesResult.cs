// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Developer.Entities;
using Microsoft.Developer.Serialization.Json.Entities;

namespace Microsoft.Developer.Providers;

public class EntitiesResult : ContentResult
{
    private static readonly IEnumerable<Entity> empty = Enumerable.Empty<Entity>();

    public EntitiesResult(IEnumerable<Entity>? entities)
    {
        Content = JsonSerializer.Serialize(entities?.OrderBy(e => e.GetEntityRef()) ?? empty, EntitySerializerOptions.Default);
        ContentType = "application/json";
        StatusCode = (int)HttpStatusCode.OK;
    }

    public EntitiesResult(IEnumerable<IEnumerable<Entity>> entities) : this(entities?.SelectMany(e => e))
    { }

    public static EntitiesResult Empty => new(empty);
}