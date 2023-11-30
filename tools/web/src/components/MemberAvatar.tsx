// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import Avatar from '@mui/material/Avatar';
import { SxProps, Theme } from '@mui/material/styles';
import React from 'react';
import { GraphUser } from '../model';

export interface IMemberAvatarProps {
    user?: GraphUser;
    sx?: SxProps<Theme>;
}

const memberInitials = (user?: GraphUser) => {
    if (!user) return undefined;
    return user.givenName && user.surname ? user.givenName.substring(0, 1) + user.surname.substring(0, 1) : user.displayName?.substring(0, 2);
};

export const MemberAvatar: React.FC<IMemberAvatarProps> = (props) => {

    const { user, sx } = props;

    return (
        <Avatar alt={user?.displayName} src={user?.imageUrl} sx={sx}>
            {memberInitials(user)}
        </Avatar>
    );
};