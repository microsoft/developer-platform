// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useIsAuthenticated } from '@azure/msal-react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { getEntities } from '../API';

export const useEntities = (kind: string) => {
    const isAuthenticated = useIsAuthenticated();
    const queryClient = useQueryClient();

    return useQuery(
        ['entities', kind],
        async () => {
            console.log(`Fetching ${kind} entities`);

            const { entities, providerAuth } = await getEntities(kind);

            console.log(`Found ${entities.length} ${kind} entities`);
            console.log(`Found ${providerAuth.length} provider auths`);

            for (const entity of entities) {
                entity.ref = `${entity.kind}:${entity.metadata!.namespace}/${entity.metadata!.name}`.toLowerCase();
            }

            if (providerAuth.length > 0) {
                queryClient.setQueryData(['providers', 'auth'], providerAuth);
            }

            return entities;
        },
        {
            refetchOnMount: false,
            refetchOnWindowFocus: false,
            staleTime: 1000 * 60 * 2, // 2 minutes
            enabled: isAuthenticated,
        }
    );
};
