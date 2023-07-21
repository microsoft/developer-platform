/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Microsoft.Developer.MSGraph;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Web;

namespace Microsoft.Developer.Api.Services;

public class UserService
{
    private readonly ILogger<UserService> logger;
    private readonly IGraphService graph;
    private readonly IMemoryCache cache;
    private readonly IHttpContextAccessor http;

    public UserService(ILogger<UserService> logger, IHttpContextAccessor http, IMemoryCache cache, IGraphService graph)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.http = http ?? throw new ArgumentNullException(nameof(http));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.graph = graph ?? throw new ArgumentNullException(nameof(graph));
    }

    public string? CurrentUserId => http.HttpContext?.User.GetObjectId();

    public string? CurrentUserTenant => http.HttpContext?.User.GetTenantId();

    public async Task<string?> GetUserIdAsync(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentNullException(nameof(identifier));

        string key = $"{nameof(UserService)}_{nameof(GetUserIdAsync)}_{identifier}";

        if (!cache.TryGetValue(key, out string? val))
        {
            var guid = await graph
                .GetUserIdAsync(identifier)
                .ConfigureAwait(false);

            val = guid?.ToString();

            if (!string.IsNullOrEmpty(val))
                cache.Set(key, val, TimeSpan.FromMinutes(5));
        }

        return val;
    }
}