// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Api.Providers;
using Microsoft.Developer.Data;
using Microsoft.Developer.DurableTasks;
using Microsoft.Developer.Requests;
using Microsoft.Developer.Serialization.Json.Entities;

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

    public static IDeveloperPlatformBuilder AddCosmosEntities(this IDeveloperPlatformBuilder builder)
    {
        builder.AddDocumentRepository<IEntitiesRepositoryFactory, EntitiesRepositoryFactory, Entity>(nameof(Entity), options =>
        {
            options.DatabaseName = "Entities";
            options.SerializerOptions = EntitySerializerOptions.Database;
        });

        return builder;
    }
}
