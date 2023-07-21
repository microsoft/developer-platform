/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Azure.Identity;
using Microsoft.Developer.Configuration.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Developer.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMsDeveloperConfiguration(this IServiceCollection services, ConfigurationManager configuration)
    {
        if (configuration[$"{AppConfigOptions.Section}:{nameof(AppConfigOptions.Endpoint)}"] is not string endpoint)
            throw new InvalidOperationException($"Configuration {AppConfigOptions.Section}:{nameof(AppConfigOptions.Endpoint)} is not set");

        configuration.AddAzureAppConfiguration(options =>
        {
            var isAzure = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IDENTITY_ENDPOINT"));
            options.Connect(new Uri(endpoint), isAzure ? new ManagedIdentityCredential() : new AzureCliCredential());
        });

        return services;
    }
}