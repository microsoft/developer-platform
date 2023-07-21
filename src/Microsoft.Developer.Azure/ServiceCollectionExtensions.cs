/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Microsoft.Developer.Azure.KeyVault;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Developer.Azure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMsDeveloperAzure(this IServiceCollection services, bool includeUserServices = false)
    {
        services
            .AddSingleton<IAppArmService, AppArmService>()
            .AddSingleton<IKeyVaultService, KeyVaultService>();

        if (includeUserServices)
            services
                .AddScoped<IUserArmService, UserArmService>();

        return services;
    }
}