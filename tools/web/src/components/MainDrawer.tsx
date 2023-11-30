// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import Account from '@mui/icons-material/AccountTree';
import AddCircle from '@mui/icons-material/AddCircleOutline';
import Cable from '@mui/icons-material/Cable';
import Cloud from '@mui/icons-material/Cloud';
import Code from '@mui/icons-material/Code';
import Computer from '@mui/icons-material/Computer';
import Dashboard from '@mui/icons-material/Dashboard';
import Extension from '@mui/icons-material/Extension';
import GitHub from '@mui/icons-material/GitHub';
import Group from '@mui/icons-material/Group';
import LibraryBooks from '@mui/icons-material/LibraryBooks';
import Login from '@mui/icons-material/Login';
import Settings from '@mui/icons-material/Settings';
import { Box, Divider, Drawer, List, ListItemButton, ListItemIcon, ListItemText, Stack, Toolbar } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import React from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { getProviderLogin } from '../API';
import { useProviderAuth } from '../hooks';
import { ReactComponent as ContosoLogo } from '../img/contoso_logo.svg';
import { ReactComponent as ContosoLogoDark } from '../img/contoso_logo_dark.svg';

const drawerWidth = 240;

const sections = [
    [
        { label: 'New...', icon: <AddCircle />, href: '/new' }
    ],
    [
        { label: 'Dashboard', icon: <Dashboard />, href: '/' }
    ],
    [
        { label: 'Services', icon: <Cable />, href: '/services' }
    ],
    [
        { label: 'Projects', icon: <Account />, href: '/project' },
        { label: 'My teams', icon: <Group />, href: '/teams' }
    ],
    [
        { label: 'Dev boxes', icon: <Computer />, href: '/devbox' },
        { label: 'Source code', icon: <Code />, href: '/repo' },
        { label: 'Environments', icon: <Cloud />, href: '/environment' }
    ],
    [
        { label: 'Docs', icon: <LibraryBooks />, href: '/docs' },
        { label: 'APIs', icon: <Extension />, href: '/apis' }
    ]
];

export interface IMainDrawerProps { }


export const MainDrawer: React.FC<IMainDrawerProps> = (props) => {

    const theme = useTheme();

    const navigate = useNavigate();

    const { pathname } = useLocation();

    const { data: providerAuths } = useProviderAuth();

    return (
        <Drawer
            anchor='left'
            variant='permanent'
            sx={{
                width: drawerWidth,
                flexShrink: 0,
                [`& .MuiDrawer-paper`]: { width: drawerWidth, boxSizing: 'border-box' },
            }}>
            <Toolbar disableGutters sx={{ px: '14px' }}>
                {theme.palette.mode === 'dark' ?
                    <ContosoLogo height='36px' width='172px' style={{ width: 'inherit', height: 'inherit' }} /> :
                    <ContosoLogoDark height='36px' width='172px' style={{ width: 'inherit', height: 'inherit' }} />
                }
            </Toolbar>
            <Stack sx={{ flex: '1 0 auto', justifyContent: 'space-between' }}>
                <Box sx={{ overflow: 'auto', pt: theme.spacing(2) }} >
                    {sections.map((section, index) => (
                        <div key={index}>
                            <List>
                                {section.map((item, _) => (
                                    <ListItemButton sx={{ pl: theme.spacing(3) }} key={item.label} onClick={() => navigate(item.href)} selected={pathname === item.href}>
                                        <ListItemIcon sx={{ minWidth: '42px' }}>{item.icon}</ListItemIcon>
                                        <ListItemText primary={item.label} />
                                    </ListItemButton>
                                ))}
                            </List>
                            {index < sections.length - 1 && <Divider />}
                        </div>
                    ))}
                </Box>
                <Box sx={{ overflow: 'auto', pt: theme.spacing(2) }} >
                    {providerAuths && (
                        <List>
                            {providerAuths.map(auth => (
                                <ListItemButton color='primary' sx={{ pl: theme.spacing(3) }} key={auth.authorizationUri} onClick={async () => {

                                    const path = pathname === '/' ? '' : pathname;
                                    const login = await getProviderLogin(auth, `${window.location.origin}${path}`);

                                    window.open(login.uri, '_self');
                                }}>
                                    <ListItemIcon sx={{ minWidth: '42px' }}>{auth.realm.toLowerCase().includes('github') ? <GitHub/> : <Login/>}</ListItemIcon>
                                    <ListItemText primary={`Login to ${auth.realm}`} />
                                </ListItemButton>
                            ))}
                        </List>
                    )}
                    <Divider />
                    <List>
                        <ListItemButton sx={{ pl: theme.spacing(3) }} onClick={() => navigate(`/settings`)} selected={pathname === '/settings'}>
                            <ListItemIcon sx={{ minWidth: '42px' }}>
                                <Settings />
                            </ListItemIcon>
                            <ListItemText primary='Settings' />
                        </ListItemButton>
                    </List>
                </Box>
            </Stack>
        </Drawer >
    );
};

export default MainDrawer;