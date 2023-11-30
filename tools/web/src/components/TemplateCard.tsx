// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import GitHubIcon from '@mui/icons-material/GitHub';
import LibraryBooksIcon from '@mui/icons-material/LibraryBooks';
import ManageSearchIcon from '@mui/icons-material/ManageSearch';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import RocketLaunch from '@mui/icons-material/RocketLaunch';
import { Button, Card, CardActions, CardContent, CardHeader, Chip, Divider, IconButton, Menu, MenuItem, Popover, Stack, SvgIcon, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import React from 'react';
import { useNavigate } from 'react-router-dom';
import { ReactComponent as AzureLogo } from '../img/azure.svg';
import { Entity } from '../model';


const getProviderIcon = (provider?: string) => {
    switch (provider?.toLowerCase()) {
        case 'devcenter.azure.com': return <SvgIcon fontSize='inherit' component={AzureLogo} />;
        case 'github.com': return <GitHubIcon fontSize='inherit' />;
    }
    return undefined;
};

export interface ITemplateCardProps {
    template: Entity;
}

export const TemplateCard: React.FC<ITemplateCardProps> = (props) => {

    const { template } = props;

    const theme = useTheme();

    const navigate = useNavigate();

    const [menuAnchorEl, setMenuAnchorEl] = React.useState<HTMLButtonElement | null>(null);
    const [popoverAnchorEl, setPopoverAnchorEl] = React.useState<HTMLButtonElement | null>(null);

    const menuOpen = Boolean(menuAnchorEl);
    const popoverOpen = Boolean(popoverAnchorEl);

    const handlePopoverClose = () => {
        setPopoverAnchorEl(null);
    };

    const handleMenuClick = (event: React.MouseEvent<HTMLButtonElement>) => {
        setMenuAnchorEl(event.currentTarget);
    };

    const handleMenuClose = () => {
        setMenuAnchorEl(null);
    };

    return (
        <Card sx={{ p: 1 }}>
            <CardHeader sx={{ pt: theme.spacing(3) }}
                action={<IconButton onClick={handleMenuClick} aria-label='settings'>
                    <MoreVertIcon />
                </IconButton>}
                title={(
                    <Stack direction='row' pb={theme.spacing(1)} spacing={1} alignItems='center'>
                        <strong>{template.metadata!.title ?? template.metadata!.name}</strong>
                        {getProviderIcon(template.metadata!.provider)}
                    </Stack>
                )}
            />
            <Divider />
            <CardContent sx={{ pt: 2 }}>
                <Typography variant='subtitle2' gutterBottom component='div'
                    minHeight={template.metadata.tags ? '150px' : '230px'}
                    maxHeight={template.metadata.tags ? '150px' : '230px'}>
                    {template.metadata.description}
                </Typography>

                {template.metadata.tags && (
                    <Stack pb='20px'>
                        <Typography variant='overline' display='block' gutterBottom component='div'>
                            Tags
                        </Typography>
                        <Stack direction='row' spacing={1}>
                            {template.metadata.tags.map((tag, index) => (
                                <Chip key={index} label={tag} size='small' color='secondary' variant="outlined" />
                            ))}
                        </Stack>
                    </Stack>
                )}

            </CardContent>
            <CardActions sx={{justifyContent: 'space-between', paddingBottom: theme.spacing(2)}}>
                <Stack pl={1}>
                    <Stack direction='row' spacing={1}>
                        {template.spec.creates ? template.spec.creates?.map((plan, index) => (
                            <Chip key={index} label={plan.kind} color='primary' variant="outlined" />
                        )) : (
                            <Chip label='Automation' color='primary' variant="outlined" />
                        )}
                    </Stack>
                </Stack>
                <Button
                    color='inherit'
                    variant='outlined'
                    startIcon={<RocketLaunch />}
                    onClick={() => navigate(`/new/${template.metadata!.namespace}/${template.metadata!.name}`)}>
                        Create...
                </Button>
                <Menu
                    open={menuOpen}
                    anchorEl={menuAnchorEl}
                    onClose={handleMenuClose}
                    anchorOrigin={{ vertical: 'bottom', horizontal: 'left' }}
                    transformOrigin={{ vertical: 'top', horizontal: 'left' }}>
                    <MenuItem onClick={handleMenuClose}>
                        <LibraryBooksIcon sx={{ marginRight: theme.spacing(1) }} fontSize='medium' color='info' />
                        View docs
                    </MenuItem>
                    <MenuItem onClick={handleMenuClose}>
                        <ManageSearchIcon sx={{ marginRight: theme.spacing(1) }} fontSize='medium' color='info' />
                        See existing
                    </MenuItem>
                </Menu>
                <Popover
                    open={popoverOpen}
                    anchorEl={popoverAnchorEl}
                    onClose={handlePopoverClose}
                    anchorOrigin={{ vertical: 'bottom', horizontal: 'right', }}
                    transformOrigin={{ vertical: 'top', horizontal: 'right', }}>
                    <Button color='inherit' sx={{ padding: theme.spacing(1, 2) }} startIcon={<LibraryBooksIcon color='info' />}>
                        View docs
                    </Button>
                </Popover>
            </CardActions>
        </Card>
    );
};