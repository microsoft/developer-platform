/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Configuration.Options;

public class AzureAdOptions
{
    public const string Section = "AzureAd";

    public string Instance { get; set; } = "https://login.microsoftonline.com/";

    public string ClientId { get; set; } = string.Empty;

    public string Domain { get; set; } = string.Empty;

    public string TenantId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string Audiance => $"{Instance}{TenantId}";

    public List<ClientCredential> ClientCredentials { get; set; } = new();

    public class ClientCredential
    {
        public string SourceType { get; set; } = "ClientSecret";

        public string ClientSecret { get; set; } = string.Empty;
    }
}