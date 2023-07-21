/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Runtime.CompilerServices;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Developer.Azure.KeyVault;

public static class SecretClientExtensions
{
    public static async Task<T?> SetSecretAsync<T>(this SecretClient client, string name, T value, JsonSerializerOptions? options = null)
        where T : class, new()
    {
        if (value is null)
        {
            await client.StartDeleteSecretAsync(name)
                .ConfigureAwait(false);

            return value;
        }
        else
        {
            var secret = await client.SetSecretAsync(name, JsonSerializer.Serialize(value))
                .ConfigureAwait(false);

            return string.IsNullOrEmpty(secret?.Value.Value)
                ? default
                : JsonSerializer.Deserialize<T>(secret.Value.Value, options);
        }
    }

    public static async Task<T?> GetSecretAsync<T>(this SecretClient client, string name, JsonSerializerOptions? options = null)
        where T : class, new()
    {
        try
        {
            var secret = await client.GetSecretAsync(name)
                .ConfigureAwait(false);

            return string.IsNullOrEmpty(secret?.Value.Value)
                ? default
                : JsonSerializer.Deserialize<T>(secret.Value.Value, options);
        }
        catch (RequestFailedException exc) when (exc.Status == StatusCodes.Status404NotFound)
        {
            return null;
        }
    }

    public static async IAsyncEnumerable<KeyValuePair<string, string>> GetSecretsAsync(this SecretClient client, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in client.GetPropertiesOfSecretsAsync(cancellationToken))
        {
            var secret = await client.GetSecretAsync(item.Name, null, cancellationToken)
                .ConfigureAwait(false);

            yield return new KeyValuePair<string, string>(secret.Value.Name, secret.Value.Value);
        }
    }
}
