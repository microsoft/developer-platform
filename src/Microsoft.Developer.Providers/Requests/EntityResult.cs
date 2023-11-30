// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Developer.Entities;
using Microsoft.Developer.Serialization.Json.Entities;

namespace Microsoft.Developer.Providers;

public class EntityResult : ContentResult
{
    public EntityResult(Entity entity)
    {
        Content = JsonSerializer.Serialize(entity, EntitySerializerOptions.Default);
        ContentType = "application/json";
        StatusCode = (int)HttpStatusCode.OK;
    }
}