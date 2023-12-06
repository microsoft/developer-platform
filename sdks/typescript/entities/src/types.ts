import type { components } from './openapi';

export declare type DurableTaskResult = components['schemas']['DurableTaskResult'];

export declare type Entity = components['schemas']['Entity'] & {
  ref: string;
};

export declare type EntityPlan = components['schemas']['EntityPlan'];

export declare type EntityRef = components['schemas']['EntityRef'];

export declare type Link = components['schemas']['Link'];

export declare type Metadata = components['schemas']['Metadata'];

export declare type Relation = components['schemas']['Relation'];

export declare type Spec = components['schemas']['Spec'];

export declare type Status = components['schemas']['Status'];

export declare type TemplateRequest = components['schemas']['TemplateRequest'];

export declare type TemplateResponse = components['schemas']['TemplateResponse'];

export declare type TemplateSpec = components['schemas']['TemplateSpec'];

export declare type UserProfile = components['schemas']['UserProfile'];

export declare type UserRole = components['schemas']['UserRole'];

export declare type UserSpec = components['schemas']['UserSpec'];

interface ITemplate extends Omit<components['schemas']['Entity'], 'spec'> {
  ref: string;
  spec: TemplateSpec;
}

export declare type Template = ITemplate;

interface IUser extends Omit<components['schemas']['Entity'], 'spec'> {
  ref: string;
  spec: UserSpec;
}

export declare type User = IUser;

export declare type ProviderAuth = {
  authorizationUri: string;
  realm: string;
  scopes: [string];
};

export declare type ProviderLogin = {
  uri: string;
};
