// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Hosting;
using Azure.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Developer.Data.Cosmos;
using Microsoft.Extensions.Options;

namespace Microsoft.Developer.Data;

internal class CosmosBuilderFactory(IOptions<CosmosOptions> options, IServiceProvider services, IHostEnvironment env)
{
    public CosmosClientBuilder GetBuilder()
    {
        var cosmos = options.Value;

        var builder = string.IsNullOrEmpty(cosmos.ConnectionString)
            ? new CosmosClientBuilder(cosmos.Endpoint, services.GetRequiredService<TokenCredential>())
            : new CosmosClientBuilder(cosmos.ConnectionString);

        // Allows us to use the emulator during development. See https://learn.microsoft.com/azure/cosmos-db/how-to-develop-emulator for details
        if (options.Value.DangerousAcceptAnyServerCertificate)
        {
            if (!env.IsDevelopment())
            {
                throw new InvalidOperationException("Must be in development mode to bypass SSL validation");
            }

            builder
                .WithHttpClientFactory(() =>
                {
                    return new HttpClient(new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    });
                })
                .WithConnectionModeGateway()
                .WithLimitToEndpoint(true);
        }

        return builder;
    }
}
