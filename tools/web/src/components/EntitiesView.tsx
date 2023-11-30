// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Box, CircularProgress, Grid } from '@mui/material';
import React from 'react';
import { useParams } from 'react-router-dom';
import { useEntities } from '../hooks';
import { EntityCard } from './EntityCard';

export interface IEntitiesViewProps { }

export const EntitiesView: React.FC<IEntitiesViewProps> = (props) => {

    const { kind } = useParams();

    const { data: entities, isLoading } = useEntities(kind!);

    if (isLoading)
        return (<Box sx={{ display: 'flex', minHeight: '300px', justifyContent: 'center', alignItems: 'center' }}>
            <CircularProgress />
        </Box>);

    return (
        <Grid container spacing={4} >
            {entities?.map(entity => (
                <Grid item xs={12} sm={12} md={6} lg={4} xl={3} key={entity.ref}>
                    <EntityCard entity={entity} />
                </Grid>
            ))}
        </Grid>
    );
};