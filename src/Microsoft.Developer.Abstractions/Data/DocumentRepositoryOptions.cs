// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Data;

public class DocumentRepositoryOptions<TDocument>
{
    public const string DefaultDatabaseName = "MSDevs";

    public const string DefaultContainerName = "Default";

    public const string DefaultPartitionKey = "/tenant";

    public string DatabaseName { get; set; } = DefaultDatabaseName;

    public string ContainerName { get; set; } = DefaultContainerName;

    public string PartitionKey { get; set; } = DefaultPartitionKey;

    public JsonSerializerOptions? SerializerOptions { get; set; }

    public ICollection<string> UniqueKeys { get; } = new HashSet<string>();

    public bool IsSoftDelete { get; set; }
}


