/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

[JsonConverter(typeof(EntityRefJsonConverter))]
public class EntityRef
{
    const string DefaultNamespace = "default";

    public EntityRef(string id, string? defaultKind = null)
    {
        var idParts = id.Split(':');

        var useDefaultKind = idParts.Length == 1 && !string.IsNullOrWhiteSpace(defaultKind);

        if (!useDefaultKind && idParts.Length != 2)
            throw new ArgumentException("Id must be in the format of Kind:Provider/[Namespace/]Name");

        Kind = useDefaultKind ? defaultKind! : idParts[0];

        var nameParts = (useDefaultKind ? idParts[0] : idParts[1]).Split('/');

        var useDefaultNamespace = nameParts.Length == 2;

        if (!useDefaultNamespace && nameParts.Length != 3)
            throw new ArgumentException("Id must be in the format of Kind:Provider/[Namespace/]Name");

        Provider = nameParts[0];
        Namespace = useDefaultNamespace ? DefaultNamespace : nameParts[1];
        Name = useDefaultNamespace ? nameParts[1] : nameParts[2];
    }

    public EntityRef(string kind, string provider, string name, string? @namespace = null)
    {
        Kind = kind;
        Provider = provider;
        Name = name;
        Namespace = @namespace ?? DefaultNamespace;
    }

    public string Kind { get; } = default!;

    public string Provider { get; } = default!;

    public string Name { get; } = default!;

    public string Namespace { get; } = DefaultNamespace;

    public string Id
        => Namespace.Equals(DefaultNamespace, StringComparison.OrdinalIgnoreCase)
           ? $"{Kind}:{Provider}/{Name}"
           : $"{Kind}:{Provider}/{Namespace}/{Name}";

    public static implicit operator EntityRef(string id) => new(id);

    public static implicit operator string(EntityRef entityRef) => entityRef.Id;
}

[JsonConverter(typeof(ApiEntityRefJsonConverter))]
public class ApiEntityRef : EntityRef
{
    public ApiEntityRef(string id) : base(id, nameof(API)) { }
}

[JsonConverter(typeof(SystemEntityRefJsonConverter))]
public class SystemEntityRef : EntityRef
{
    public SystemEntityRef(string id) : base(id, nameof(System)) { }
}

[JsonConverter(typeof(ComponentEntityRefJsonConverter))]
public class ComponentEntityRef : EntityRef
{
    public ComponentEntityRef(string id) : base(id, nameof(Component)) { }
}

[JsonConverter(typeof(GroupEntityRefJsonConverter))]
public class GroupEntityRef : EntityRef
{
    public GroupEntityRef(string id) : base(id, nameof(Group)) { }
}

[JsonConverter(typeof(UserEntityRefJsonConverter))]
public class UserEntityRef : EntityRef
{
    public UserEntityRef(string id) : base(id, nameof(User)) { }
}

[JsonConverter(typeof(DomainEntityRefJsonConverter))]
public class DomainEntityRef : EntityRef
{
    public DomainEntityRef(string id) : base(id, nameof(Domain)) { }
}

[JsonConverter(typeof(ResourceEntityRefJsonConverter))]
public class ResourceEntityRef : EntityRef
{
    public ResourceEntityRef(string id) : base(id, nameof(Resource)) { }
}

[JsonConverter(typeof(LocationEntityRefJsonConverter))]
public class LocationEntityRef : EntityRef
{
    public LocationEntityRef(string id) : base(id, nameof(Location)) { }
}