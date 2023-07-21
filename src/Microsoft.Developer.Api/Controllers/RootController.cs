/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Developer.Api.Controllers;

[Authorize]
[ApiController]
[Route("/")]
public class RootController : ControllerBase
{

    private readonly ILogger<RootController> logger;

    public RootController(ILogger<RootController> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get()
    {
        // TODO: Return some useful config information
        return Ok();
    }
}