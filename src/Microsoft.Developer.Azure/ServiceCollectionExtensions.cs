// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Azure.KeyVault;
using Microsoft.Developer.Configuration.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Developer.Azure;

public static class ServiceCollectionExtensions
{
    public static IDeveloperPlatformBuilder AddAzure(this IDeveloperPlatformBuilder builder, IConfiguration configuration)
    {
        builder.Services
            .AddScoped<IUserArmService, UserArmService>()
            .AddSingleton<IArmService, ArmService>()
            .AddSingleton<IKeyVaultService, KeyVaultService>()
            .AddSingleton<ISecretsManager, KeyVaultSecretsManager>()
            .Configure<AzureAdOptions>(configuration.GetSection(AzureAdOptions.Section))
            .Configure<KeyVaultOptions>(configuration.GetSection(KeyVaultOptions.Section));

        return builder;
    }
}
