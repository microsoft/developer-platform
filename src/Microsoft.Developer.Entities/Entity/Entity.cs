/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public abstract class Entity<TMetadata, TSpec> : IEntity<TMetadata, TSpec>
    where TMetadata : class, IMetadata, new()
    where TSpec : class, ISpec, new()
{
    [JsonRequired]
    [JsonPropertyOrder(1)]
    public virtual string ApiVersion { get; set; } = EntityDefaults.ApiVersion;


    [JsonRequired]
    [JsonPropertyOrder(2)]
    public virtual string Kind => throw new NotImplementedException();


    [JsonRequired]
    [JsonPropertyOrder(3)]
    public virtual string Provider { get; set; } = EntityDefaults.Platform;


    [ApiIgnore]
    [JsonPropertyOrder(4)]
    public virtual EntityRef Ref => new(Kind, Provider, Metadata.Name, Metadata.Namespace);


    [JsonRequired]
    [JsonPropertyOrder(5)]
    public TMetadata Metadata
    {
        get => ((IEntity)this).Metadata as TMetadata ?? default!;
        set => ((IEntity)this).Metadata = value;
    }

    IMetadata IEntity.Metadata { get; set; } = default!;


    [JsonPropertyOrder(6)]
    public TSpec? Spec
    {
        get => ((IEntity)this).Spec as TSpec;
        set => ((IEntity)this).Spec = value;
    }


    ISpec? IEntity.Spec { get; set; }


    IStatus? IEntity.Status { get; set; }


    [JsonPropertyOrder(8)]
    public List<Relation> Relations { get; set; } = new();


    [ApiIgnore]
    [JsonPropertyOrder(8)]
    public virtual string Id
    {
        get => Metadata.Uid;
        set => Metadata.Uid = value;
    }


    [ApiIgnore]
    [JsonRequired]
    [JsonPropertyOrder(9)]
    [PartitionKey]
    public virtual string Tenant
    {
        get => Metadata.Tenant;
        set => Metadata.Tenant = value;
    }

}

public abstract class Entity<TMetadata, TSpec, TStatus> : Entity<TMetadata, TSpec>, IEntity<TMetadata, TSpec, TStatus>
    where TMetadata : class, IMetadata, new()
    where TSpec : class, ISpec, new()
    where TStatus : class, IStatus, new()
{
    [JsonPropertyOrder(7)]
    public TStatus? Status
    {
        get => ((IEntity)this).Status as TStatus;
        set => ((IEntity)this).Status = value;
    }
}