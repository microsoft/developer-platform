// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useIsAuthenticated } from '@azure/msal-react';
import { useQuery } from '@tanstack/react-query';
import { getEntity } from '../API';

export const useEntity = (kind: string, name: string, namespace?: string) => {
    const isAuthenticated = useIsAuthenticated();

    return useQuery(
        ['entities', kind, namespace ?? 'default', name],
        async () => {
            console.log(`Fetching ${kind} entity ${kind}:${namespace ?? 'default'}/${name}`);

            const entity = await getEntity(kind, name, namespace);

            console.log(`Found ${kind} entity ${kind}:${namespace ?? 'default'}/${name}`);

            entity.ref = `${entity.kind}:${entity.metadata!.namespace}/${entity.metadata!.name}`.toLowerCase();

            return entity;
        },
        {
            refetchOnMount: false,
            refetchOnWindowFocus: false,
            staleTime: 1000 * 60 * 2, // 2 minutes
            enabled: isAuthenticated,
        }
    );
};
