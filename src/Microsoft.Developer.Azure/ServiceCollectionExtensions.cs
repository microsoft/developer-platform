// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Configuration.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;

namespace Microsoft.Developer.Azure;

public static class ServiceCollectionExtensions
{
    public static IDeveloperPlatformBuilder AddAzure(this IDeveloperPlatformBuilder builder, IConfiguration configuration)
    {
        builder.Services
            .AddAzureClients(clientBuilder =>
            {
                clientBuilder.AddSecretClient(configuration.GetSection(KeyVaultOptions.Section));
                clientBuilder.AddKeyClient(configuration.GetSection(KeyVaultOptions.Section));
                clientBuilder.AddCertificateClient(configuration.GetSection(KeyVaultOptions.Section));

                clientBuilder.UseCredential(AppTokenCredential.GetTokenCredential());
            });

        builder.Services
            .AddScoped<IUserTokenCredentialFactory, UserTokenCredentialFactory>()
            .AddSingleton<ISecretsManager, KeyVaultSecretsManager>()
            .Configure<AzureAdOptions>(configuration.GetSection(AzureAdOptions.Section));

        return builder;
    }
}
