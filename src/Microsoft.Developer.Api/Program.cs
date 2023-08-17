/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Microsoft.Developer.Api;
using Microsoft.Developer.Api.Auth;
using Microsoft.Developer.Api.Services;
using Microsoft.Developer.Azure;
using Microsoft.Developer.Configuration;
using Microsoft.Developer.Configuration.Options;
using Microsoft.Developer.Data;
using Microsoft.Developer.Data.CosmosDb;
using Microsoft.Developer.Entities.Serialization;
using Microsoft.Developer.MSGraph;

var builder = WebApplication.CreateBuilder(args);

const string AllowLocalhost = nameof(AllowLocalhost);

builder.Services
    .AddMsDeveloperConfiguration(builder.Configuration)
    .AddMsDeveloperOptions(builder.Configuration);

builder.Services
    .ConfigureHttpJsonOptions(o => o.SerializerOptions.AddEntitySerialization());

builder.Services
    .AddMemoryCache()
    .AddHttpContextAccessor();

builder.Services
    .AddMsDeveloperData()
    .AddSingleton<IUserRepository, CosmosUserRepository>()
    .AddSingleton<IProjectRepository, CosmosProjectRepository>()
    .AddSingleton<IEntitiesRepository, CosmosEntitiesRepository>();

builder.Services
    .AddMsDeveloperAzure(includeUserServices: true)
    .AddMsDeveloperCache(builder.Configuration)
    .AddMsDeveloperMsGraph();


builder.Services
    .AddSingleton<UserService>();


AzureAdOptions? azureAdOptions = null!;

if (!builder.Configuration.TryBind(AzureAdOptions.Section, out azureAdOptions))
    throw new InvalidOperationException($"{AzureAdOptions.Section} is not configured.");

builder.Services
    .AddMsDeveloperAuthentication(builder.Configuration, azureAdOptions)
    .AddMsDeveloperAuthorization();


if (builder.Environment.IsDevelopment())
    builder.Services.AddCors(options =>
        options.AddPolicy(AllowLocalhost, policy =>
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));


builder.Services
    .AddControllers();

builder.Services
    .AddMsDeveloperSwagger(builder.Configuration, azureAdOptions);


var app = builder.Build();

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMsDeveloperSwagger(azureAdOptions!.ClientId);
    app.UseCors(AllowLocalhost);
}


app.UseAuthorization();

app.MapControllers();

app.Run();