// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Developer.Azure;
using Microsoft.Developer.Entities.Serialization;
using Microsoft.Developer.MSGraph;
using Microsoft.Developer.Providers.TestProvider;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddMsDeveloperConfiguration(builder.Environment);

builder.Services
    .AddTokenAcquisition()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(jwtOptions => { }, identityOptions =>
    {
        builder.Configuration.GetSection(AzureAdOptions.Section).Bind(identityOptions);
    })
    .EnableTokenAcquisitionToCallDownstreamApi(options =>
    {
        builder.Configuration.Bind(AzureAdOptions.Section, options);
    })
    .AddDistributedTokenCaches();

builder.Services.AddAuthorization();

builder.Services.AddDeveloperPlatform()
    .AddEntitySerialization()
    .AddTestProvider()
    .AddMicrosoftGraph();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseDeveloperPlatform();

var entities = app.MapGroup("/entities")
    .RequireAuthorization();

app.MapGroup("/entities")
    .RequireAuthorization()
    .MapEntities();

app.Run();
