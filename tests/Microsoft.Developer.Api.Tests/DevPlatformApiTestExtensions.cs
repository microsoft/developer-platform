using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Developer.Entities.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace Microsoft.Developer.Api.Tests;

internal static class DevPlatformApiTestExtensions
{
    public static IHostBuilder ConfigureTestDevPlatform(this IHostBuilder host) => host
         .ConfigureDesignTime()
         .ConfigureWebHost(builder => builder
                .UseTestServer()
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                    services.AddAuthentication(options =>
                    {
                        options.AddScheme<TestAuthHandler>("Test", "Test");
                    });
                    services.AddAuthorization();
                    services.AddDeveloperPlatform()
                       .AddEntitySerialization();
                })
                .Configure(app =>
                {
                    app.UseRouting();

                    app.UseAuthentication();
                    app.UseAuthorization();
                    app.UseDeveloperPlatform();

                    app.UseEndpoints(endpoints => endpoints.MapDeveloperPlatform());
                }));

    private sealed class TestAuthHandler(ClaimsPrincipal principal) : IAuthenticationHandler
    {
        public Task<AuthenticateResult> AuthenticateAsync()
        {
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, "Test")));
        }

        public Task ChallengeAsync(AuthenticationProperties? properties) => Task.CompletedTask;
        public Task ForbidAsync(AuthenticationProperties? properties) => Task.CompletedTask;
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context) => Task.CompletedTask;
    }
}
