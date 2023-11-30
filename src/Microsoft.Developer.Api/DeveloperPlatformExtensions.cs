// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Api.Providers;
using Microsoft.Developer.DurableTasks;
using Microsoft.Developer.Requests;

namespace Microsoft.Developer.Api;

public static class DeveloperPlatformExtensions
{
    public static void MapDeveloperPlatform(this IEndpointRouteBuilder builder)
    {
        builder.MapGroup("/entities")
            .MapEntities()
            .RequireAuthorization()
            .WithOpenApi()
            .AddYamlContentType();

        var templateOrchestration = builder.MapDurableTask<TemplateOrchestration, TemplateRequest, TemplateResponse>("entities", "/status")
            .WithMicrosoftWebIdentityTags()
            .WithUserTag()
            .WithHostUrl()
            .WithName("Create")
            .WithOpenApi();

        templateOrchestration
            .MapStatus("/status")
            .WithName("GetStatus")
            .WithOpenApi();
    }
}
