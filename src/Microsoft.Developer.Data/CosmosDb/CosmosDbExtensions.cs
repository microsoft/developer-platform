/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Runtime.CompilerServices;
using Microsoft.Azure.Cosmos;

namespace Microsoft.Developer.Data.CosmosDb;

public static class CosmosDbExtensions
{
    public static async IAsyncEnumerable<T> ReadAllAsync<T>(this FeedIterator<T> feedIterator, Func<T, Task<T>>? processor = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (feedIterator is null)
            throw new ArgumentNullException(nameof(feedIterator));

        // we use a queue to keep the order of the returned
        // documents instead of a list with the usual WaitAny
        // approach that would result in a random result stream

        var processorQueue = new Queue<Task<T>>();

        while (feedIterator.HasMoreResults)
        {
            var queryResponse = await feedIterator
                .ReadNextAsync(cancellationToken)
                .ConfigureAwait(false);

            foreach (var queryResult in queryResponse)
            {
                if (processor is null)
                {
                    yield return queryResult;
                }
                else
                {
                    processorQueue.Enqueue(Task.Run(() => processor(queryResult), cancellationToken));
                }
            }
        }

        while (processorQueue.TryDequeue(out var task))
            yield return await task.ConfigureAwait(false);
    }
}