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
public class EntityController : MsDeveloperController
{

    private readonly ILogger<EntityController> logger;
    private readonly IEntitiesRepository repo;

    public EntityController(ILogger<EntityController> logger, IEntitiesRepository repo) : base()
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }


    [HttpGet("/entities/API/{id:alpha}")]
    public Task<API> GetAPI(string id) => repo.GetAsync<API>(TenantId!, id)!;


    [HttpGet("/entities/Component/{id:alpha}")]
    public Task<Component> GetComponent(string id) => repo.GetAsync<Component>(TenantId!, id)!;


    [HttpGet("/entities/Domain/{id:alpha}")]
    public Task<Domain> GetDomain(string id) => repo.GetAsync<Domain>(TenantId!, id)!;


    [HttpGet("/entities/Group/{id:alpha}")]
    public Task<Group> GetGroup(string id) => repo.GetAsync<Group>(TenantId!, id)!;


    [HttpGet("/entities/Location/{id:alpha}")]
    public Task<Location> GetLocation(string id) => repo.GetAsync<Location>(TenantId!, id)!;


    [HttpGet("/entities/Project/{id:alpha}")]
    public Task<Project> GetProject(string id) => repo.GetAsync<Project>(TenantId!, id)!;


    [HttpGet("/entities/Provider/{id:alpha}")]
    public Task<Provider> GetProvider(string id) => repo.GetAsync<Provider>(TenantId!, id)!;


    [HttpGet("/entities/Resource/{id:alpha}")]
    public Task<Resource> GetResource(string id) => repo.GetAsync<Resource>(TenantId!, id)!;


    [HttpGet("/entities/System/{id:alpha}")]
    public Task<Microsoft.Developer.Entities.System> GetSystem(string id) => repo.GetAsync<Microsoft.Developer.Entities.System>(TenantId!, id)!;


    [HttpGet("/entities/Template/{id:alpha}")]
    public Task<Template> GetTemplate(string id) => repo.GetAsync<Template>(TenantId!, id)!;


    [HttpGet("/entities/User/{id:alpha}")]
    public Task<User> GetUser(string id) => repo.GetAsync<User>(TenantId!, id)!;
}