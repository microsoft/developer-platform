/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class Relation
{
    public required string Type { get; set; }

    public required EntityRef TargetRef { get; set; }

    public RelationTarget Target => new(TargetRef);


    public class RelationTarget
    {
        public RelationTarget(EntityRef entityRef)
        {
            Kind = entityRef.Kind;
            Provider = entityRef.Provider;
            Name = entityRef.Name;
            Namespace = entityRef.Namespace ?? EntityDefaults.Namespace;
        }

        public string Kind { get; set; }

        public string Provider { get; set; }

        public string Name { get; set; }

        public string Namespace { get; set; } = EntityDefaults.Namespace;
    }
}