// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Developer.Entities;
using Microsoft.Developer.Requests;
using Microsoft.Developer.Serialization.Json.Entities;

namespace Microsoft.Developer.Providers;

public class TemplateResult : ContentResult
{
    public TemplateResult()
    {
        Content = JsonSerializer.Serialize(new TemplateResponse(), EntitySerializerOptions.Default);
        ContentType = "application/json";
        StatusCode = (int)HttpStatusCode.OK;
    }

    public TemplateResult(params EntityRef[] entities)
    {
        Content = JsonSerializer.Serialize(new TemplateResponse { Entities = [.. entities] }, EntitySerializerOptions.Default);
        ContentType = "application/json";
        StatusCode = (int)HttpStatusCode.OK;
    }
}