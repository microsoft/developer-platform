// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Box, Toolbar, Typography } from '@mui/material';
import React from 'react';

export interface IMainViewProps extends React.PropsWithChildren {
    main?: boolean;
    title?: string;
}

export const MainView: React.FC<IMainViewProps> = (props) => {

    const { main, title, children } = props;

    return main ? (
        <Box component='main' sx={{ flexGrow: 1, px: 6, pb: 6, pt: 4 }}>
            <Toolbar />
            <Toolbar />
            {title && <Typography variant='h4' fontWeight='600' component='h1' mb='26px' gutterBottom>{title}</Typography>}
            {children}
        </Box>
    ) : (
        <Box>
            <Toolbar />
            <Toolbar />
            {title && <Typography variant='h4' fontWeight='600' component='h1' mb='26px' gutterBottom>{title}</Typography>}
            {children}
        </Box>
    );
};