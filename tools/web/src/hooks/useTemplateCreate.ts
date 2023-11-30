// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router';
import { create } from '../API';
import { sleep } from '../Utils';
import { TemplateRequest } from '../model/TemplateRequest';

export const useTemplateCreate = () => {
    // const isAuthenticated = useIsAuthenticated();

    const navigate = useNavigate();

    const queryClient = useQueryClient();

    return useMutation(
        async (templateRequest: TemplateRequest) => {
            await create(templateRequest);
            return;
        },
        {
            onSuccess: async (data: void, variables: TemplateRequest, context: unknown) => {
                const sleepSeconds = variables.provider.startsWith('devcenter') ? 10 : 3;
                navigate(`/new`);
                console.log(`Invalidating entities after ${sleepSeconds} seconds...`);
                await sleep(sleepSeconds * 1000);
                console.log(`Invalidating entities`);
                queryClient.invalidateQueries(['entities'], { type: 'all' });
            },
        }
    ).mutateAsync;
};
