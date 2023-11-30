// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer;

public interface ISecretsManager
{
    Task<T?> GetSecretAsync<T>(string name, CancellationToken cancellationToken);

    Task SetSecretAsync<T>(string name, T secret, CancellationToken cancellationToken);

    Task DeleteSecretAsync(string name, CancellationToken cancellationToken);
}
