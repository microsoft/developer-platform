// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export interface ProviderAuth {
    authorizationUri: string;
    realm: string;
    scopes: [string];
}

const getWwwAuthParam = (param: string): { key: string; value: string } | undefined => {
    const parts = param
        .trim()
        .split('=')
        .map((p) => p.trim());

    if (parts.length !== 2) {
        console.error(`could not split param into key/value: ${param}`);
        return undefined;
    }

    const keyPart = parts[0];
    var valuePart = parts[1];

    if (!keyPart || !valuePart) {
        console.error(`could not split param into key/value: ${param}`);
        return undefined;
    }

    if (valuePart.startsWith('"')) valuePart = valuePart.substring(1, valuePart.length);

    if (valuePart.endsWith('"')) valuePart = valuePart.substring(0, valuePart.length - 1);

    if (!valuePart) {
        console.error(`could not split param into key/value: ${param} - value is empty`);
        return undefined;
    }

    return {
        key: keyPart,
        value: valuePart,
    };
};

const parseWwwAuth = (header: string): ProviderAuth | undefined => {
    // remove all double spaces
    while (header.includes('  ')) {
        header.replace('  ', ' ');
    }

    const parts = header.split(' ');
    if (parts.length < 2) return undefined;

    const scheme = parts[0].trim();

    if (scheme.toLowerCase() !== 'bearer') {
        console.error(`scheme does not start with Bearer`);
        return undefined;
    }
    const params = header
        .replace(scheme, '')
        .trim()
        .split(',')
        .map(getWwwAuthParam)
        .filter((x) => x !== undefined) as { key: string; value: string }[];

    const realm = params.find((x) => x.key.toLowerCase() === 'realm')?.value;

    if (!realm) {
        console.error(`WWW-Authenticate header value does not have realm`);
        return undefined;
    }

    const uri = params.find((x) => x.key.toLowerCase() === 'authorization_uri')?.value;

    if (!uri) {
        console.error(`WWW-Authenticate header value does not have authorization_uri`);
        return undefined;
    }

    const scopes = params.find((x) => x.key.toLowerCase() === 'scopes')?.value;

    if (!scopes) {
        console.error(`WWW-Authenticate header value does not have scopes`);
        return undefined;
    }

    // console.log(`scheme: ${scheme}`);
    console.log(`realm: ${realm}`);
    console.log(`uri: ${uri}`);
    console.log(`scopes: ${scopes}`);

    return {
        authorizationUri: uri,
        realm: realm,
        scopes: [scopes],
    };
};

export const getProviderAuths = (headers: Headers): ProviderAuth[] => {
    console.log(`Get provider auth`);

    const wwwAuthenticate = headers.get('WWW-Authenticate');

    if (wwwAuthenticate) {
        // currently only support one provider
        const auth = parseWwwAuth(wwwAuthenticate);

        if (auth) return [auth];
    }

    return [];
};
