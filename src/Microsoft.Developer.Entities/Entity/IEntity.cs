/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

[ContainerName("Entities")]
public interface IEntity
{
    string ApiVersion { get; }

    string Kind { get; }

    string Provider { get; }

    [UniqueKey]
    EntityRef Ref { get; }

    IMetadata Metadata { get; set; }

    ISpec? Spec { get; set; }

    IStatus? Status { get; set; }

    List<Relation> Relations { get; set; }

    string Id { get; set; }

    [PartitionKey]
    string Tenant { get; set; }
}

public interface IEntity<TMetadata, TSpec> : IEntity
    where TMetadata : IMetadata, new()
    where TSpec : ISpec, new()
{
    new TMetadata Metadata { get; set; }
    new TSpec? Spec { get; set; }
}

public interface IEntity<TMetadata, TSpec, TStatus> : IEntity<TMetadata, TSpec>
    where TMetadata : IMetadata, new()
    where TSpec : ISpec, new()
    where TStatus : IStatus, new()
{
    new TStatus? Status { get; set; }
}