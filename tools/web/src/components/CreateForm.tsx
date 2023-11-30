// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import GitHubIcon from '@mui/icons-material/GitHub';
import { Box, CircularProgress, Stack, SvgIcon, Typography, useTheme } from '@mui/material';
import { IChangeEvent } from '@rjsf/core';
import { Form } from '@rjsf/mui';
import { RJSFSchema } from '@rjsf/utils';
import validator from '@rjsf/validator-ajv8';
import React, { FormEvent, useEffect, useState } from 'react';
import { useParams } from 'react-router';
import { useEntities, useTemplateCreate } from '../hooks';
import { ReactComponent as AzureLogo } from '../img/azure.svg';
import { Entity, TemplateRequest } from '../model';

export interface ICreateFormProps {}

export const CreateForm: React.FC<ICreateFormProps> = (props) => {

    const { namespace, name } = useParams();

    const { data: templates, isLoading } = useEntities('template');

    const create = useTemplateCreate();

    const [template, setTemplate] = useState<Entity>();

    const [formEnabled, setFormEnabled] = useState<boolean>(true);

    useEffect(() => {
        if (!isLoading && templates && namespace && name) {
            const template = templates.find(t => t.metadata!.namespace === namespace && t.metadata!.name === name);
            if (template)
                setTemplate(template);
        }
    }, [isLoading, templates, namespace, name]);

    const onSubmit = async (
        data: IChangeEvent<any, RJSFSchema, any>,
        event: FormEvent<any>
    ) => {
        console.log('Data submitted: ', data.formData);

        if (template) {
            setFormEnabled(false);

            const templateRequest: TemplateRequest = {
                templateRef: template.ref!,
                // templateRef: {
                //     kind: template.kind!,
                //     namespace: template.metadata!.namespace!,
                //     name: template.metadata!.name!,
                // },
                provider: template.metadata!.provider!,
                inputJson: JSON.stringify(data.formData),
            };

            console.log('TemplateRequest:', templateRequest);

            console.log('Submitting form...');

            await create(templateRequest);

            console.log('Done.');
        }
    };

    const theme = useTheme();

    const getProviderIcon = (provider?: string) => {
        switch (provider?.toLowerCase()) {
            case 'devcenter.azure.com': return <SvgIcon fontSize='inherit' component={AzureLogo} />;
            case 'github.com': return <GitHubIcon fontSize='inherit' />;
        }
        return undefined;
    };

    if (isLoading || !formEnabled)
        return (<Box sx={{ display: 'flex', minHeight: '300px', justifyContent: 'center', alignItems: 'center' }}>
            <CircularProgress />
        </Box>);

    return template ? (
        <Stack spacing={3} pt={1}>
            <Stack direction='row' pb={theme.spacing(1)} spacing={1} alignItems='center'>
                <Typography variant='h3' display='block' gutterBottom component='div'>
                    {/* <strong>{template.metadata!.title ?? template.metadata!.name}</strong> */}
                    {template.metadata!.title ?? template.metadata!.name}
                </Typography>
                <Typography variant='h3' display='block' >
                    {getProviderIcon(template.metadata!.provider)}
                </Typography>
            </Stack>

            <Typography variant='body1' display='block' gutterBottom>
                {template.metadata.description}
            </Typography>

            <Box sx={{ display: 'flex', flexWrap: 'wrap' }}>
                <Form
                    disabled={!formEnabled}
                    validator={validator}
                    onSubmit={onSubmit}
                    schema={template.spec.inputJsonSchema ? JSON.parse(template.spec.inputJsonSchema) : {}}
                    uiSchema={template.spec.inputUiSchema ? JSON.parse(template.spec.inputUiSchema) : {}}>
                </Form>
            </Box>
        </Stack>

    )  : <> </>;
};