// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { InteractionStatus, InteractionType } from '@azure/msal-browser';
import { AuthenticatedTemplate, MsalAuthenticationResult, useMsal, useMsalAuthentication } from '@azure/msal-react';
import { Box, CssBaseline, LinearProgress, ThemeProvider, createTheme } from '@mui/material';
import React, { useEffect, useMemo, useState } from 'react';
import { Route, Routes } from 'react-router-dom';
import { CreateForm } from './CreateForm';
import { DashboardView } from './DashboardView';
import { EntitiesView } from './EntitiesView';
import MainAppBar from './MainAppBar';
import MainDrawer from './MainDrawer';
import { MainView } from './MainView';
import { SettingsView } from './SettingsView';
import { TemplatesView } from './TemplatesView';

export interface IRootViewProps { }

export const ColorModeContext = React.createContext({ toggleColorMode: () => { } });

export const RootView: React.FC<IRootViewProps> = (props) => {

    const authResult: MsalAuthenticationResult = useMsalAuthentication(InteractionType.Redirect, { scopes: ['openid'], redirectUri: window.location.href });

    const { inProgress } = useMsal();

    useEffect(() => {
        // console.info('inprogress:', inProgress);

        if (authResult.error) {
            console.error('authResult.error:', authResult.error);
        }

        if (authResult.error && inProgress === InteractionStatus.None) {
            console.log('logging in...');
            authResult.login(InteractionType.Redirect, { scopes: ['openid'], redirectUri: window.location.href });
        }
    }, [authResult, inProgress]);


    const [mode, setMode] = useState<'light' | 'dark'>('dark');

    const colorMode = useMemo(() => ({
        toggleColorMode: () => setMode((prevMode) => (prevMode === 'light' ? 'dark' : 'light')),
    }), []);

    const theme = React.useMemo(() =>
        createTheme({
            palette: {
                mode: mode,
                background: {
                    default: mode === 'dark' ? '#1f1f1f' : '#ffffff',
                    paper: mode === 'dark' ? '#181818' : '#f8f8f8',
                }
            },
        }),
        [mode]
    );

    return (
        <ColorModeContext.Provider value={colorMode}>
            <ThemeProvider theme={theme}>
                <AuthenticatedTemplate>
                    <Box sx={{ display: 'flex' }}>
                        <CssBaseline />
                        <MainAppBar />
                        <LinearProgress />
                        <MainDrawer />
                        <Routes>
                            <Route path='/' element={<MainView main><DashboardView {...{}} /></MainView>} />
                            <Route path='/new' element={<MainView main><TemplatesView {...{}} /></MainView>} />
                            <Route path='/settings' element={<MainView main><SettingsView {...{}} /></MainView>} />
                            <Route path='/:kind' element={<MainView main><EntitiesView {...{}} /></MainView>} />
                            <Route path='/new/:namespace/:name' element={<MainView main><CreateForm {...{}} /></MainView>} />
                        </Routes>
                    </Box>
                </AuthenticatedTemplate>
            </ThemeProvider>
        </ColorModeContext.Provider>
    );
};