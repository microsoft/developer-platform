// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { AccessToken, TokenCredential } from '@azure/core-auth';
import { Configuration, InteractionRequiredAuthError, PublicClientApplication } from '@azure/msal-browser';
import { AuthenticationResult } from '@azure/msal-common';
import { AuthenticationProvider, AuthenticationProviderOptions } from '@microsoft/microsoft-graph-client';

export class Auth implements TokenCredential, AuthenticationProvider {
    _getClientId = (): string => {
        // allow for injected value (i.e. string replacement)
        const injected = '__REACT_APP_MSAL_CLIENT_ID__';
        if (injected && injected.length > 0 && !injected.startsWith('__')) return injected;

        if (!process.env.REACT_APP_MSAL_CLIENT_ID) throw new Error('Must set env variable $REACT_APP_MSAL_CLIENT_ID');
        return process.env.REACT_APP_MSAL_CLIENT_ID;
    };

    _getAuthority = (): string => {
        const prefix = 'https://login.microsoftonline.com/';

        // allow for injected value (i.e. string replacement)
        const injected = '__REACT_APP_MSAL_TENANT_ID__';
        if (injected && injected.length > 0 && !injected.startsWith('__')) return prefix + injected;

        if (!process.env.REACT_APP_MSAL_TENANT_ID) throw new Error('Must set env variable $REACT_APP_MSAL_TENANT_ID');
        return prefix + process.env.REACT_APP_MSAL_TENANT_ID;
    };

    _getScope = (): string => {
        // allow for injected value (i.e. string replacement)
        const injected = '__REACT_APP_MSAL_SCOPE__';
        if (injected && injected.length > 0 && !injected.startsWith('__')) return injected;

        if (!process.env.REACT_APP_MSAL_SCOPE) throw new Error('Must set env variable REACT_APP_MSAL_SCOPE');
        return process.env.REACT_APP_MSAL_SCOPE;
    };

    configuration: Configuration = {
        auth: {
            clientId: this._getClientId(),
            authority: this._getAuthority(),
            redirectUri: window.location.origin,
            postLogoutRedirectUri: window.location.origin,
            navigateToLoginRequestUrl: true,
        },
        cache: {
            cacheLocation: 'sessionStorage',
            storeAuthStateInCookie: false,
        },
        // system: { navigationClient }
    };

    clientApplication = new PublicClientApplication(this.configuration);

    getScopes = (scopes: string | string[]): string[] => {
        if (!Array.isArray(scopes)) scopes = [scopes];

        const oidScope = 'openid';
        const apiScope = this._getScope();

        const hostScope = '{$host}/.default';

        // autorest clients add sclope: '{$host}/.default'
        const hostIndex = scopes.indexOf(hostScope);

        if (hostIndex > -1) {
            scopes.splice(hostIndex, 1);

            // ensure we have the oid scope
            if (!scopes.includes(oidScope)) scopes.push(oidScope);

            // ensure we have the platform api scope
            if (!scopes.includes(apiScope)) scopes.push(apiScope);
        }

        return scopes;
    };

    _getAuthResult = async (scopes: string | string[]): Promise<AuthenticationResult | null> => {
        scopes = this.getScopes(scopes);

        const accounts = this.clientApplication.getAllAccounts();

        if (accounts.length <= 0) {
            console.error('nope');
            return null;
        }

        const account = accounts[0];

        try {
            var authResult = await this.clientApplication.acquireTokenSilent({ account: account, scopes: scopes });

            // console.log(authResult);

            return authResult;
        } catch (error) {
            console.warn('Nope');
            console.error(error);

            if (error instanceof InteractionRequiredAuthError) {
                // console.error(error);
                console.error(`errorCode : ${error.errorCode}`);
                console.error(`errorMessage : ${error.errorMessage}`);
                console.error(`message : ${error.message}`);
                console.error(`name : ${error.name}`);
                console.error(`subError : ${error.subError}`);

                try {
                    await this.clientApplication.acquireTokenRedirect({ account: account, scopes: scopes as string[] });
                } catch (err) {
                    if (err instanceof InteractionRequiredAuthError) {
                        console.error(`err.errorCode : ${err.errorCode}`);
                        console.error(`err.errorMessage : ${err.errorMessage}`);
                        console.error(`err.message : ${err.message}`);
                        console.error(`err.name : ${err.name}`);
                        console.error(`err.subError : ${err.subError}`);
                    }
                }
            }

            return null;
        }
    };

    getTokenHeader = async (scopes: string | string[]): Promise<string | null> => {
        const authResult = await this._getAuthResult(scopes);

        if (authResult?.accessToken) {
            if (authResult.tokenType) {
                return `${authResult.tokenType} ${authResult.accessToken}`;
            } else return authResult.accessToken;
        }

        return null;
    };

    getToken = async (scopes: string | string[]): Promise<AccessToken | null> => {
        const authResult = await this._getAuthResult(scopes);

        if (authResult) return { token: authResult.accessToken, expiresOnTimestamp: authResult.expiresOn!.getTime() };
        else return null;
    };

    getAccessToken = async (authenticationProviderOptions?: AuthenticationProviderOptions): Promise<string> => {
        const graphScopes = ['User.Read', 'User.ReadBasic.All', 'Directory.Read.All', 'People.Read']; // An array of graph scopes

        if (authenticationProviderOptions?.scopes) graphScopes.concat(authenticationProviderOptions.scopes);

        const authResult = await this._getAuthResult(graphScopes);

        return authResult?.accessToken ?? Promise.reject('Unable to get token');
    };

    // logout = async (): Promise<void> => this.clientApplication.logout();
}
