// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Developer;

public class KeyVaultSecretsManager(SecretClient secrets) : ISecretsManager
{
    public async Task<T?> GetSecretAsync<T>(string name, CancellationToken cancellationToken)
    {
        try
        {
            var secret = await secrets.GetSecretAsync(name, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrEmpty(secret?.Value.Value))
            {
                return default;
            }

            if (IsSimpleType<T>())
            {
                return (T)Convert.ChangeType(secret.Value.Value, typeof(T));
            }

            return JsonSerializer.Deserialize<T>(secret.Value.Value);
        }
        catch (RequestFailedException exc) when (exc.Status == StatusCodes.Status404NotFound)
        {
            return default;
        }
    }

    public async Task SetSecretAsync<T>(string name, T secret, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(secret);

        var value = IsSimpleType<T>() ? secret.ToString() : JsonSerializer.Serialize(secret);

        _ = await secrets.SetSecretAsync(name, value, cancellationToken).ConfigureAwait(false);
    }

    public Task DeleteSecretAsync(string name, CancellationToken cancellationToken = default)
        => secrets.StartDeleteSecretAsync(name, cancellationToken);

    private static bool IsSimpleType<T>() => typeof(T).IsPrimitive || typeof(T).IsValueType || (typeof(T) == typeof(string));
}
