/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Developer.Api.Auth;
using Microsoft.Developer.Data;
using Microsoft.Developer.Entities.Serialization;

namespace Microsoft.Developer.Api.Controllers;

[Authorize]
[ApiController]
public class UserController : MsDeveloperController
{
    // This is only meant to be a stub for testing foundational concepts like database connections,
    // auth, etc. It is not the decided upon api, and and likely removed

    private readonly ILogger<UserController> logger;
    private readonly IUserRepository userRepo;

    public UserController(ILogger<UserController> logger, IUserRepository userRepo) : base()
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
    }

    [HttpGet("me")]
    [Authorize(Policy = AuthPolicies.TenantUserRead)]
    public async Task<IResult> GetMe()
    {
        var user = await userRepo
            .GetAsync(TenantId!, UserId!)
            .ConfigureAwait(false);

        if (user is null)
            return Results.NotFound();

        return user is null
            ? Results.NotFound()
            : Results.Json(user, EntitySerializerOptions.API);
    }

    [HttpGet("users")]
    [Authorize(Policy = AuthPolicies.TenantAdmin)]
    public async IAsyncEnumerable<User> Get()
    {
        var users = userRepo
            .ListAsync(TenantId!)
            .ConfigureAwait(false);


        await foreach (var user in users)
            yield return user;
    }

    [HttpGet("users/{userId:alpha}")]
    [Authorize(Policy = AuthPolicies.TenantUserRead)]
    public async Task<IResult> Get(string userId)
    {
        var user = await userRepo
            .GetAsync(TenantId!, userId!)
            .ConfigureAwait(false);

        return user is null
            ? Results.NotFound()
            : Results.Json(user, EntitySerializerOptions.API);

    }
}