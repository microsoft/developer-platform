// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import Box from '@mui/material/Box';
import React from 'react';

export interface ISettingsViewProps { }

export const SettingsView: React.FC<ISettingsViewProps> = (props) => {

    return (
        <Box sx={{ display: 'flex', flexWrap: 'wrap' }}>
            <Box>Settings</Box>
            {/* <TokenForm /> */}
        </Box>
    );
};