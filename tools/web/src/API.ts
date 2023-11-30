// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Auth } from './Auth';
import { Entity, ProviderAuth, ProviderLogin, TemplateRequest, getProviderAuths } from './model';

const _getApiUrl = () => {
    // allow for injected value (i.e. string replacement)
    const injected = '__REACT_APP_API_URL__';
    if (injected && injected.length > 0 && !injected.startsWith('__')) return injected;

    if (!process.env.REACT_APP_API_URL) throw new Error('Must set env variable $REACT_APP_API_URL');

    return process.env.REACT_APP_API_URL;
};

export const apiUrl = _getApiUrl();

export const auth = new Auth();

export const callApi = async (path: string): Promise<Response> => {
    const url = `${apiUrl}/${path}`;

    try {
        const token = await auth.getTokenHeader('{$host}/.default');

        if (!token) throw new Error('Failed to get token.');

        const response = await fetch(url, {
            method: 'GET',
            headers: {
                Authorization: token,
            },
        });

        if (!response.ok) {
            console.error(response);
        }

        return response;
    } catch (error) {
        console.error(error);
        throw error;
    }
};

export const getEntity = async (kind: string, name: string, namespace?: string): Promise<Entity> => {
    const response = await callApi(`entities/${kind}/${namespace ?? 'default'}/${name}`);

    const entity = (await response.json()) as Entity;

    return entity;
};

export const getEntities = async (kind: string): Promise<{ entities: Entity[]; providerAuth: ProviderAuth[] }> => {
    const response = await callApi(`entities/${kind}`);

    const entities = (await response.json()) as Entity[];

    const providerAuth = getProviderAuths(response.headers);

    return { entities, providerAuth };
};

export const create = async (templateRequest: TemplateRequest): Promise<void> => {
    const url = `${apiUrl}/entities`;

    try {
        const token = await auth.getTokenHeader('{$host}/.default');

        if (!token) throw new Error('Failed to get token.');

        const response = await fetch(url, {
            method: 'POST',
            headers: {
                Authorization: token,
                Accept: 'application/json',
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(templateRequest),
        });

        if (!response.ok) {
            console.error(response);
        }
    } catch (error) {
        console.error(error);
        throw error;
    }
};

export const getProviderLogin = async (providerAuth: ProviderAuth, redirectUri: string): Promise<ProviderLogin> => {
    const url = `${providerAuth.authorizationUri}?redirect_uri=${redirectUri}`;

    console.log(`Get provider login: ${url}`);

    try {
        const token = await auth.getTokenHeader(providerAuth.scopes);

        if (!token) throw new Error('Failed to get token.');

        const response = await fetch(url, {
            method: 'GET',
            headers: {
                Authorization: token,
            },
        });

        if (!response.ok) {
            console.error(response);
        }

        return (await response.json()) as ProviderLogin;
    } catch (error) {
        console.error(error);
        throw error;
    }
};
