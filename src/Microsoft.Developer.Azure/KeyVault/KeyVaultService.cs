/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;

namespace Microsoft.Developer.Azure.KeyVault;

public interface IKeyVaultService
{
    CertificateClient Certificates { get; }
    KeyClient Keys { get; }
    SecretClient Secrets { get; }
}

public class KeyVaultService : IKeyVaultService
{
    public CertificateClient Certificates { get; private set; }
    public KeyClient Keys { get; private set; }
    public SecretClient Secrets { get; private set; }

    public KeyVaultService(IAppArmService arm, IOptions<KeyVaultOptions> options)
    {
        if (arm is null)
            throw new ArgumentNullException(nameof(arm));

        if (options?.Value is null)
            throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrEmpty(options?.Value.Endpoint))
            throw new ArgumentException("Endpoint cannot be null or empty.", nameof(options));

        if (!Uri.IsWellFormedUriString(options?.Value.Endpoint, UriKind.Absolute))
            throw new ArgumentException("Endpoint is not a valid URI.", nameof(options));

        var uri = new Uri(options!.Value.Endpoint, UriKind.Absolute);
        var token = arm.GetTokenCredential();

        Certificates = new CertificateClient(uri, token);

        Keys = new KeyClient(uri, token);

        Secrets = new SecretClient(uri, token);
    }
}