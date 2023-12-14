// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using DurableTask.AzureStorage;
using DurableTask.DependencyInjection;
using DurableTask.Emulator;
using DurableTask.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Developer.Api;
using Microsoft.Developer.Azure;
using Microsoft.Developer.Data;
using Microsoft.Developer.Entities.Serialization;
using Microsoft.Developer.MSGraph;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IO;

var builder = WebApplication.CreateBuilder(args);

const string AllowLocalhost = nameof(AllowLocalhost);

builder.Configuration
    .AddMsDeveloperConfiguration(builder.Environment);

builder.Services.AddHttpForwarder();
builder.Services
    .AddMemoryCache()
    .AddHttpContextAccessor();

builder.Services.AddSingleton<RecyclableMemoryStreamManager>();

builder.Host.ConfigureTaskHubWorker((context, builder) =>
{
    builder.WithOrchestrationService(sp =>
    {
        if (sp.GetRequiredService<IOptions<AzureStorageOrchestrationServiceSettings>>().Value is { StorageConnectionString: { } } config)
        {
            return new AzureStorageOrchestrationService(config);
        }
        else
        {
            sp.GetRequiredService<ILoggerFactory>().CreateLogger("Program.cs").LogWarning("No storage account found - using local emulator for durable tasks.");
            return new LocalOrchestrationService();
        }
    });

    builder.Services.AddOptions<AzureStorageOrchestrationServiceSettings>()
        .Configure<IHostEnvironment>((options, env) =>
        {
            options.AppName = env.ApplicationName;
            options.StorageConnectionString = context.Configuration["AzureWebJobsStorage"];
        })
        .Bind(context.Configuration.GetSection("DurableTasks"));

    builder.AddClient();
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(jwtOptions => { }, identityOptions =>
    {
        builder.Configuration.GetSection(AzureAdOptions.Section).Bind(identityOptions);
    })
    .EnableTokenAcquisitionToCallDownstreamApi(clientOptions =>
    {
        builder.Configuration.GetSection(AzureAdOptions.Section).Bind(clientOptions);
    })
    .AddDistributedTokenCaches()
    .AddDownstreamApi(string.Empty, builder.Configuration.GetSection("default"));

builder.Services.AddDeveloperPlatform()
    .AddEntitySerialization()
    .AddOpenApi(builder.Configuration, options =>
    {
        options.OneOf.Add(typeof(TemplateSpec));
        options.OneOf.Add(typeof(UserSpec));
    })
    .AddCosmos(builder.Configuration)
    .AddCosmosEntities()
    .AddAzure(builder.Configuration)
    .AddProviders(builder.Configuration.GetSection("Providers"))
    .AddMicrosoftGraph();

if (builder.Environment.IsDesignTime())
{
    builder.Host.ConfigureDesignTime();
}

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
        options.AddPolicy(AllowLocalhost, policy =>
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
}

var app = builder.Build();

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors(AllowLocalhost);
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.UseDeveloperPlatform();

// For container healthchecks
app.MapGet("/", () => Environment.GetEnvironmentVariable("DEVELOPER_API_IMAGE_VERSION") is string version ? version : "OK")
    // We don't need it to be showing up in OpenAPI definitions
    .ExcludeFromDescription();

app.MapDeveloperPlatform();

if (app.Environment.IsDevelopment())
{
    app.MapProviderPassthrough();
}

app.Run();
