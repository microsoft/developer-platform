// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useIsAuthenticated } from '@azure/msal-react';
import { useQuery } from '@tanstack/react-query';
import { ProviderAuth } from '../model';

export const useProviderAuth = () => {
    const isAuthenticated = useIsAuthenticated();

    return useQuery(['providers', 'auth'], () => [] as ProviderAuth[], {
        refetchOnMount: false,
        refetchOnWindowFocus: false,
        staleTime: Infinity,
        enabled: isAuthenticated,
    });
};
