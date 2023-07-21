/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities.Serialization;

public class EntityTypeResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var typeInfo = base.GetTypeInfo(type, options);

        if (typeInfo.Type == typeof(IEntity))
        {
            typeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                IgnoreUnrecognizedTypeDiscriminators = true,
                TypeDiscriminatorPropertyName = EntitySerialization.TypeDiscriminatorPropertyName,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
                DerivedTypes = {
                    new JsonDerivedType(typeof(API), nameof(API)),
                    new JsonDerivedType(typeof(Component), nameof(Component)),
                    new JsonDerivedType(typeof(Domain), nameof(Domain)),
                    new JsonDerivedType(typeof(Group), nameof(Group)),
                    new JsonDerivedType(typeof(Project), nameof(Project)),
                    new JsonDerivedType(typeof(Provider), nameof(Provider)),
                    new JsonDerivedType(typeof(Resource), nameof(Resource)),
                    new JsonDerivedType(typeof(System), nameof(System)),
                    new JsonDerivedType(typeof(Template), nameof(Template)),
                    new JsonDerivedType(typeof(User), nameof(User))
                }
            };
        }

        return typeInfo;
    }
}