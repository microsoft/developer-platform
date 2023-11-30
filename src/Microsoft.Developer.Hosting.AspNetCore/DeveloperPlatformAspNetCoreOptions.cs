// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Microsoft.Developer.Features;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.Developer.Hosting;

public class DeveloperPlatformAspNetCoreOptions
{
    [Required]
    public Func<HttpContext, IDeveloperPlatformRepositoryFeature> RepositoryFactory { get; set; } = null!;
}
