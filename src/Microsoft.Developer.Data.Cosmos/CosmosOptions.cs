// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Data.CosmosDb;

public class CosmosOptions
{
    public const string Section = "Cosmos";

    public string Endpoint { get; set; } = string.Empty;

    // TODO will be moved to RepositoryOptions
    public string DatabaseName { get; set; } = "MSDevs";

    public string? ConnectionString { get; set; } = null;
}
