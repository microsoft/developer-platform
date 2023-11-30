// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import SearchIcon from '@mui/icons-material/Search';
import { Alert, AppBar, Box, IconButton, InputBase, MenuItem, Popover, Select, SelectChangeEvent, Toolbar } from '@mui/material';
import { alpha, styled, useTheme } from '@mui/material/styles';
import React, { useState } from 'react';
import { useLocation } from 'react-router-dom';
import { useGraphUser } from '../hooks';
import { MemberAvatar } from './MemberAvatar';
import { MemberDetails } from './MemberDetails';

const drawerWidth = 240;

const Search = styled('div')(({ theme }) => ({
    position: 'relative',
    borderRadius: theme.shape.borderRadius,
    backgroundColor: alpha(theme.palette.common.white, 0.15),
    '&:hover': {
        backgroundColor: alpha(theme.palette.common.white, 0.25),
    },
    marginRight: theme.spacing(2),
    marginLeft: 0,
    width: '100%',
    [theme.breakpoints.up('sm')]: {
        marginLeft: theme.spacing(3),
        width: 'auto',
    },
}));

const SearchIconWrapper = styled('div')(({ theme }) => ({
    padding: theme.spacing(0, 2),
    height: '100%',
    position: 'absolute',
    pointerEvents: 'none',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
}));

const StyledInputBase = styled(InputBase)(({ theme }) => ({
    color: 'inherit',
    lineHeight: 1,
    width: '100%',
    '& .MuiInputBase-input': {
        padding: theme.spacing(1, 1, 1, 0),
        // vertical padding + font size from searchIcon
        paddingLeft: `calc(1em + ${theme.spacing(4)})`,
        transition: theme.transitions.create('width'),
        width: '100%',
        // [theme.breakpoints.up('md')]: {
        //     width: '20ch',
        // },
    },
}));


export interface IMainAppBarProps { }

export const MainAppBar: React.FC<IMainAppBarProps> = (props) => {

    const { pathname } = useLocation();

    const theme = useTheme();

    const { data: user } = useGraphUser();
    // const { data: githubUser } = useGitHubUser();

    const [org, setOrg] = useState('contoso');

    const [popoverAnchorEl, setPopoverAnchorEl] = useState<HTMLButtonElement | null>(null);

    const popoverOpen = Boolean(popoverAnchorEl);

    const handleOrgChange = (event: SelectChangeEvent) => {
        setOrg(event.target.value as string);
    };

    const handlePopoverClick = (event: React.MouseEvent<HTMLButtonElement>) => {
        setPopoverAnchorEl(event.currentTarget);
    };

    const handlePopoverClose = () => {
        setPopoverAnchorEl(null);
    };

    // const getSizeBox = () => {
    //     if (process.env.NODE_ENV !== 'production') {
    //         return (<>
    //             <Box display={{ xs: 'block', sm: 'none' }}>xs</Box>
    //             <Box display={{ xs: 'none', sm: 'block', md: 'none' }}>sm</Box>
    //             <Box display={{ xs: 'none', md: 'block', lg: 'none' }}>md</Box>
    //             <Box display={{ xs: 'none', lg: 'block', xl: 'none' }}>lg</Box>
    //             <Box display={{ xs: 'none', xl: 'block' }}>xl</Box>
    //         </>);
    //     }
    //     return <></>;
    // };

    return (
        // <AppBar position='fixed' sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <AppBar
            position="fixed"
            sx={{ width: `calc(100% - ${drawerWidth}px)`, ml: `${drawerWidth}px` }}>
            <Alert variant="filled" severity="error">
                This is a mock UI used for testing the API during development
            </Alert>
            <Toolbar disableGutters sx={{ pl: '14px', pr: '14px' }}>

                {/* {getSizeBox()} */}

                {/* <ContosoLogo height='36px' width='172px' style={{ width: 'inherit', height: 'inherit' }} /> */}

                {/* <Typography>
                    Dev Boxes
                </Typography> */}


                <Box sx={{ flexGrow: 1 }} />
                {pathname !== '/orgs/contoso' && (

                    <Search sx={{ flexGrow: 1, mx: 2 }}>
                        <SearchIconWrapper>
                            <SearchIcon />
                        </SearchIconWrapper>
                        <StyledInputBase
                            placeholder='Search…'
                            inputProps={{ 'aria-label': 'search' }} />
                    </Search>
                )}
                <Box sx={{ flexGrow: 1 }} />

                <Box minWidth='116px' textAlign='end'>

                    <Select
                        value={org}
                        sx={{
                            color: theme.palette.common.white,
                            '& fieldset': { border: 'none' },
                            '& .MuiSelect-icon': {
                                color: theme.palette.common.white,
                            }
                        }}
                        // SelectDisplayProps={{ style: { border: 'none' } }}
                        MenuProps={{ style: { border: 'none' } }}
                        onChange={handleOrgChange}
                        IconComponent={KeyboardArrowDownIcon}>

                        <MenuItem value={'contoso'}>Contoso</MenuItem>
                        <MenuItem value={'fabrikam'}>Fabrikam</MenuItem>

                    </Select>
                </Box>

                <Box sx={{ paddingLeft: '12px', flexGrow: 0 }}>
                    <IconButton onClick={handlePopoverClick} sx={{ p: 0 }}>
                        <MemberAvatar user={user} />
                    </IconButton>
                    <Popover
                        open={popoverOpen}
                        anchorEl={popoverAnchorEl}
                        onClose={handlePopoverClose}
                        anchorOrigin={{ vertical: 'bottom', horizontal: 'right', }}
                        transformOrigin={{ vertical: 'top', horizontal: 'right', }}>
                        {user && <MemberDetails user={user} signout />}
                    </Popover>
                </Box>
            </Toolbar>
        </AppBar>
    );
};

export default MainAppBar;