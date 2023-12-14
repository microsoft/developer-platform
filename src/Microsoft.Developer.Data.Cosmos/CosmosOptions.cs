// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Data.Cosmos;

public class CosmosOptions
{
    public bool DangerousAcceptAnyServerCertificate { get; set; }

    public const string Section = "Cosmos";

    public string? Endpoint { get; set; }

    // TODO will be moved to RepositoryOptions
    public string DatabaseName { get; set; } = "MSDevs";

    public string? ConnectionString { get; set; } = null;
}
