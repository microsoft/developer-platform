// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export class Entity {
    apiVersion!: string;
    kind!: string;
    metadata!: Metadata;
    spec!: Spec;
    relations?: [string];
    ref!: string;
}

export class Metadata {
    name!: string;
    namespace!: string;
    provider!: string;

    uid?: string;

    title?: string;
    description?: string;

    labels: { [key: string]: string } = {};
    annotations?: { [key: string]: string };
    tags?: [string];
}

export class Spec {
    inputJsonSchema?: any;
    inputUiSchema?: any;
    creates?: [EntityPlan];
}

export class EntityPlan {
    kind!: string;
    namespace!: string;
    labels: { [key: string]: string } = {};
}
