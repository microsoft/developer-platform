// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export const getVersion = () => {
    let value = process.env.REACT_APP_VERSION || '__REACT_APP_VERSION__';
    if (value.startsWith('__')) value = '0.0.0';
    return value;
};

export const sleep = (ms: number) => new Promise((r) => setTimeout(r, ms));

export const matchesLowerCase = (id: string, param: string) =>
    id && param ? id.toLowerCase() === param.toLowerCase() : id === param;

export const matchesAnyLowerCase = (id: string, ...params: string[]) => params.some((p) => matchesLowerCase(id, p));

export const matchesRouteParam = (obj: { id: string; slug: string }, param: string) =>
    matchesLowerCase(obj.id, param) || matchesLowerCase(obj.slug, param);

export const endsWithLowerCase = (path: string, check: string) =>
    path && check ? path.toLowerCase().endsWith(check.toLowerCase()) : false;

export const endsWithAnyLowerCase = (path: string, ...checks: string[]) =>
    checks.some((c) => endsWithLowerCase(path, c));

export const includesLowerCase = (path: string, check: string) =>
    path && check ? path.toLowerCase().includes(check.toLowerCase()) : false;

export const matchesParent = (list: [{ id: string }], parent: { id: string }) =>
    list && !list.some((i) => i.id !== parent.id);

export const undefinedOrWrongParent = (list: [{ id: string }], parent: { id: string }) =>
    list === undefined || list.some((i) => i.id !== parent.id);

export const prettyPrintCamlCaseString = (value: string) =>
    value.charAt(0).toUpperCase() + value.slice(1).replace(/\B[A-Z]/g, ' $&');
