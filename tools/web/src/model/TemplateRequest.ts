// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { EntityRef } from './EntityRef';

export interface TemplateRequest {
    templateRef: EntityRef | string;
    provider: string;
    inputJson: string;
}
