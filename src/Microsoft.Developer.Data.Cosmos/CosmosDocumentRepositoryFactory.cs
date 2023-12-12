// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Microsoft.Developer.Data.Cosmos;

public class CosmosDocumentRepositoryFactory<T>(IOptions<CosmosOptions> cosmosOptions, IOptionsMonitor<DocumentRepositoryOptions<T>> repositoryOptions) : IDocumentRepositoryFactory<T>
{
    public IDocumentRepository<T> Create(string name)
        => new CosmosRepository<T>(GetContainerFactory(repositoryOptions.Get(name)));

    private Func<CancellationToken, ValueTask<Container>> GetContainerFactory(DocumentRepositoryOptions<T> options)
    {
        var cts = new CancellationTokenSource();
        var factory = new Lazy<Task<Container>>(async () =>
        {
            try
            {
                return await GetContainerAsync(cts.Token);
            }
            finally
            {
                cts.Dispose();
            }
        }, LazyThreadSafetyMode.ExecutionAndPublication);

        async Task<Container> GetContainerAsync(CancellationToken token)
        {
            var serializerOptions = options.SerializerOptions ?? JsonSerializerOptions.Default;

            var client = new CosmosClient(cosmosOptions.Value.ConnectionString, new CosmosClientOptions()
            {
                Serializer = new CosmosJsonSerializer(serializerOptions)
            });

            var database = client.GetDatabase(options.DatabaseName);
            var response = await client
                .CreateDatabaseIfNotExistsAsync(options.DatabaseName, cancellationToken: token)
                .ConfigureAwait(false);

            var containerBuilder = database.DefineContainer(options.ContainerName, options.PartitionKey);

            foreach (var containerUniqueKey in options.UniqueKeys)
            {
                containerBuilder = containerBuilder
                    .WithUniqueKey()
                    .Path(containerUniqueKey)
                    .Attach();
            }

            if (options.IsSoftDelete)
            {
                containerBuilder = containerBuilder
                    .WithDefaultTimeToLive(-1);
            }

            var containerResponse = await containerBuilder
                .CreateIfNotExistsAsync(cancellationToken: token);

            return containerResponse.Container;
        }

        return async token =>
        {
            if (factory is { IsValueCreated: true, Value: { IsCompleted: true } created })
            {
                return created.Result;
            }

            using (token.Register(cts.Cancel))
            {
                return await factory.Value;
            }
        };
    }
}
