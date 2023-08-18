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
    // This is only meant to be a stub for testing foundational concepts like database connections,
    // auth, etc. It is not the decided upon api, and and likely removed


    private readonly ILogger<EntityController> logger;
    private readonly IEntitiesRepository repo;

    public EntityController(ILogger<EntityController> logger, IEntitiesRepository repo) : base()
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    [HttpGet("/entities")]
    public IList<IEntity> GetEntities() => Enumerable.Empty<IEntity>().ToList();

    [HttpGet("/entities/{kind:alpha}/{namespace:alpha}/{name:alpha}")]
    public IList<IEntity> GetEntity(string id) => Enumerable.Empty<IEntity>().ToList();


    [HttpGet("/entities/api/{namespace:alpha}/{name:alpha}")]
    public Task<API> GetAPI(string id) => repo.GetAsync<API>(TenantId!, id)!;


    [HttpGet("/entities/component/{namespace:alpha}/{name:alpha}")]
    public Task<Component> GetComponent(string id) => repo.GetAsync<Component>(TenantId!, id)!;


    [HttpGet("/entities/domain/{namespace:alpha}/{name:alpha}")]
    public Task<Domain> GetDomain(string id) => repo.GetAsync<Domain>(TenantId!, id)!;


    [HttpGet("/entities/group/{namespace:alpha}/{name:alpha}")]
    public Task<Group> GetGroup(string id) => repo.GetAsync<Group>(TenantId!, id)!;


    [HttpGet("/entities/location/{namespace:alpha}/{name:alpha}")]
    public Task<Location> GetLocation(string id) => repo.GetAsync<Location>(TenantId!, id)!;


    [HttpGet("/entities/project/{namespace:alpha}/{name:alpha}")]
    public Task<Project> GetProject(string id) => repo.GetAsync<Project>(TenantId!, id)!;


    [HttpGet("/entities/provider/{namespace:alpha}/{name:alpha}")]
    public Task<Provider> GetProvider(string id) => repo.GetAsync<Provider>(TenantId!, id)!;


    [HttpGet("/entities/resource/{namespace:alpha}/{name:alpha}")]
    public Task<Resource> GetResource(string id) => repo.GetAsync<Resource>(TenantId!, id)!;


    [HttpGet("/entities/system/{namespace:alpha}/{name:alpha}")]
    public Task<Microsoft.Developer.Entities.System> GetSystem(string id) => repo.GetAsync<Microsoft.Developer.Entities.System>(TenantId!, id)!;


    [HttpGet("/entities/template/{namespace:alpha}/{name:alpha}")]
    public Task<Template> GetTemplate(string id) => repo.GetAsync<Template>(TenantId!, id)!;


    [HttpGet("/entities/user/{namespace:alpha}/{name:alpha}")]
    public Task<User> GetUser(string id) => repo.GetAsync<User>(TenantId!, id)!;
}