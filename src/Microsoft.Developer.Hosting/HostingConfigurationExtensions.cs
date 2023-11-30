// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Microsoft.Developer;

public static class HostingConfigurationExtensions
{
    public static void AddMsDeveloperConfiguration(this ConfigurationManager configuration, IHostEnvironment env)
        => configuration.AddMsDeveloperConfiguration(GetAppConfigEndpoint(configuration), env);

    public static void AddMsDeveloperConfiguration(this IConfigurationBuilder builder, IConfigurationRoot configuration, IHostEnvironment env)
        => builder.AddMsDeveloperConfiguration(GetAppConfigEndpoint(configuration), env);

    internal static string? GetAppConfigEndpoint(IConfigurationRoot configuration)
        => configuration[$"{AppConfigOptions.Section}:{nameof(AppConfigOptions.Endpoint)}"];

    internal static void AddMsDeveloperConfiguration(this IConfigurationBuilder builder, string? endpoint, IHostEnvironment env)
    {
        if (endpoint is null)
        {
            return;
        }

        // TODO we only need to do this for functions (they don't load this), but for now we'll just load it twice elsewhere
        if (env.IsDevelopment() && Assembly.GetEntryAssembly() is { } assembly)
        {
            builder.AddUserSecrets(assembly, optional: true, reloadOnChange: true);
        }

        // ApplicationName is sometimes in the format:
        // Microsoft.Developer.Api, Version=0.0.1.0, Culture=neutral, PublicKeyToken=null
        env.ApplicationName = env.ApplicationName.Split(',')[0];
        env.ApplicationName = env.ApplicationName[(env.ApplicationName.LastIndexOf('.') + 1)..];

        var azureEnvironment = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IDENTITY_ENDPOINT"));

        builder.AddAzureAppConfiguration(options =>
        {
            options.Connect(new Uri(endpoint), azureEnvironment ? new ManagedIdentityCredential() : new AzureCliCredential())
                .Select(KeyFilter.Any, LabelFilter.Null) // Load configuration values with no label
                .Select(KeyFilter.Any, env.ApplicationName)
                .Select(KeyFilter.Any, env.EnvironmentName)
                .Select(KeyFilter.Any, $"{env.ApplicationName}-{env.EnvironmentName}");
        });
    }

    internal class AppConfigOptions
    {
        public const string Section = "AppConfig";

        public string Endpoint { get; set; } = string.Empty;
    }
}
