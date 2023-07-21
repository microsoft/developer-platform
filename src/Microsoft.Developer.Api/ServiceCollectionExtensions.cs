/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Microsoft.Developer.Configuration;
using Microsoft.Developer.Configuration.Options;
using Microsoft.OpenApi.Models;

namespace Microsoft.Developer.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMsDeveloperOptions(this IServiceCollection services, IConfiguration config)
    {
        services
            .Configure<AzureAdOptions>(config.GetSection(AzureAdOptions.Section))
            .Configure<KeyVaultOptions>(config.GetSection(KeyVaultOptions.Section))
            .Configure<CosmosOptions>(config.GetSection(CosmosOptions.Section));

        return services;
    }

    private static string[] SwaggerScopes(string clientId) => new string[] { "openid", "profile", "offline_access", $"api://{clientId}/.default" };

    public static IServiceCollection AddMsDeveloperSwagger(this IServiceCollection services, IConfiguration config, AzureAdOptions? azureAdOptions = null)
    {
        azureAdOptions ??= config.TryBind(AzureAdOptions.Section, out azureAdOptions)
            ? azureAdOptions
            : throw new InvalidOperationException($"{AzureAdOptions.Section} is not configured.");

        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.OpenIdConnect,
            OpenIdConnectUrl = new Uri($"{azureAdOptions!.Audiance}/v2.0/.well-known/openid-configuration"),
            Scheme = "Bearer",
            In = ParameterLocation.Header,
        };

        var securityReq = new OpenApiSecurityRequirement() {
            {
                new OpenApiSecurityScheme {
                    Flows = new OpenApiOAuthFlows {
                        AuthorizationCode = new OpenApiOAuthFlow {
                            AuthorizationUrl = new Uri($"{azureAdOptions.Audiance}/oauth2/v2.0/authorize"),
                            TokenUrl = new Uri($"{azureAdOptions.Audiance}/oauth2/v2.0/token"),
                        }
                    },
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "OpenId",
                    }
                },
                SwaggerScopes(azureAdOptions.ClientId)
            }
        };

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // o.SwaggerDoc("v1", info);
            options.AddSecurityDefinition("OpenId", securityScheme);
            options.AddSecurityRequirement(securityReq);
        });

        return services;
    }

    public static IApplicationBuilder UseMsDeveloperSwagger(this WebApplication app, string clientId)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var webClientId = app.Configuration[$"Website:{nameof(AzureAdOptions.ClientId)}"];

            options.OAuthClientId(webClientId);
            options.OAuthUsePkce();
            options.OAuthScopes(SwaggerScopes(clientId));
        });

        return app;
    }
}