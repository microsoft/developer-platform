/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.Developer.Api.Services;
using Microsoft.Identity.Web;

namespace Microsoft.Developer.Api.Controllers;

public abstract class MsDeveloperController : ControllerBase
{
    protected T GetService<T>()
        => (T)HttpContext.RequestServices.GetService(typeof(T))!;

    public UserService UserService
        => GetService<UserService>();

    public string? UserId => User.GetObjectId();

    public string? TenantId => User.GetTenantId();
}