/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Developer.Api.Auth;
using Microsoft.Developer.Data;

namespace Microsoft.Developer.Api.Controllers;

[Authorize]
[ApiController]
public class ProjectController : MsDeveloperController
{

    // This is only meant to be a stub for testing foundational concepts like database connections,
    // auth, etc. It is not the decided upon api, and and likely removed


    private readonly ILogger<ProjectController> logger;
    private readonly IProjectRepository projectRepo;

    public ProjectController(ILogger<ProjectController> logger, IProjectRepository projectRepo) : base()
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.projectRepo = projectRepo ?? throw new ArgumentNullException(nameof(projectRepo));
    }

    [HttpGet("projects")]
    [Authorize(Policy = AuthPolicies.TenantMember)]
    public async IAsyncEnumerable<Project> Get()
    {
        var projects = projectRepo
            .ListAsync(TenantId!)
            .ConfigureAwait(false);

        await foreach (var project in projects)
            yield return project;
    }

    [HttpGet("{projectId:alpha}")]
    public async Task<IActionResult> Get(string projectId)
    {
        var project = await projectRepo
            .GetAsync(TenantId!, projectId!)
            .ConfigureAwait(false);

        return project is null ? NotFound() : Ok(project);
    }
}