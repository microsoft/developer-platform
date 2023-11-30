// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Developer.Entities;
using Microsoft.Developer.Requests;
using Microsoft.Developer.Serialization.Json.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Moq;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Microsoft.Developer.Api.Tests;

public class EntitiesApiTests
{
    private const string ProviderBaseUrl = "http://provider.local/";
    private const string Localhost = "http://localhost";

    [Fact]
    public async Task GetEntitySingleProvider()
    {
        // Arrange
        using var host = CreateHostBuilder()
            .Start();

        using var content = JsonContent.Create(new[] { new Entity(EntityKind.Template) }, options: EntitySerializerOptions.Default);
        var downstream = host.Services.GetRequiredService<DownstreamApiTestBuilder>();
        downstream.Add(HttpMethod.Get, "/entities", response => response.Content = content);

        var client = host.GetTestClient();

        // Act
        var result = await client.GetFromJsonAsync<Entity[]>("/entities", EntitySerializerOptions.Default);

        // Assert
        var downstreamRequest = downstream.GetRequestMessage(HttpMethod.Get, "/entities");

        Assert.NotNull(result);
        Assert.Collection(result, entity => Assert.Equal(EntityKind.Template, entity.Kind));
        Assert.NotNull(downstreamRequest);
        Assert.Equal(Localhost, downstreamRequest.Headers.GetValues(HeaderNames.Origin).Single());
        Assert.Equal($"{Localhost}/", downstreamRequest.Headers.Referrer?.ToString());
    }

    [InlineData(ProviderBaseUrl, false)]
    [InlineData($"{ProviderBaseUrl}with-path", false)]
    [InlineData("http://other", true)]
    [Theory]
    public async Task GetEntitySingleProviderExcludeReferrer(string referrer, bool isDownstreamCalled)
    {
        // Arrange
        using var host = CreateHostBuilder()
            .Start();

        using var content = JsonContent.Create(new[] { new Entity(EntityKind.Template) }, options: EntitySerializerOptions.Default);
        var downstream = host.Services.GetRequiredService<DownstreamApiTestBuilder>();
        downstream.Add(HttpMethod.Get, "/entities", response => response.Content = content);

        var client = host.GetTestClient();

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Get, "/entities") { Headers = { Referrer = new(referrer) } };

        using var response = await client.SendAsync(request);
        var result = JsonSerializer.Deserialize<Entity[]>(await response.Content.ReadAsStringAsync(), EntitySerializerOptions.Default)!;

        // Assert
        Assert.NotNull(result);

        var downstreamRequest = downstream.GetRequestMessage(HttpMethod.Get, "/entities");

        if (isDownstreamCalled)
        {
            Assert.Collection(result, r => Assert.Equal(EntityKind.Template, r.Kind));
            Assert.NotNull(downstreamRequest);
            Assert.True(downstreamRequest.Headers.Contains(HeaderNames.Origin));
            Assert.True(downstreamRequest.Headers.Contains(HeaderNames.Referer));
        }
        else
        {
            Assert.Empty(result);
            Assert.Null(downstreamRequest);
        }
    }

    [Fact]
    public async Task GetEntityProviderSendsWWWAuthenticate()
    {
        // Arrange
        using var host = CreateHostBuilder()
            .Start();

        using var content = JsonContent.Create(new[] { new Entity(EntityKind.Template) }, options: EntitySerializerOptions.Default);
        var downstream = host.Services.GetRequiredService<DownstreamApiTestBuilder>();
        downstream.Add(HttpMethod.Get, "/entities", response =>
        {
            response.Content = content;
            response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("bearer"));
        });

        var client = host.GetTestClient();

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Get, "/entities") { Content = content };

        using var response = await client.SendAsync(request);
        var result = JsonSerializer.Deserialize<Entity[]>(await response.Content.ReadAsStringAsync(), EntitySerializerOptions.Default)!;

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Collection(response.Headers,
            h =>
            {
                Assert.Equal(HeaderNames.WWWAuthenticate, h.Key);
                Assert.Collection(h.Value, h => Assert.Equal("bearer", h));
            },
            h =>
            {
                Assert.Equal(HeaderNames.AccessControlExposeHeaders, h.Key);
                Assert.Collection(h.Value, h => Assert.Equal(HeaderNames.WWWAuthenticate, h));
            });
    }

    [Fact(Skip = "Not currently short circuiting orchestrator")]
    public async Task PostEntityReturns200()
    {
        // Arrange
        using var host = CreateHostBuilder()
            .Start();

        var durableResult = new DurableTaskResult(Guid.NewGuid().ToString());
        using var content = JsonContent.Create(durableResult);
        var downstream = host.Services.GetRequiredService<DownstreamApiTestBuilder>();
        downstream.Add(HttpMethod.Post, "/entities", response => response.Content = content);

        var client = host.GetTestClient();

        // Act
        var templateRequest = new TemplateRequest
        {
            Provider = ProviderBaseUrl,
            InputJson = "{}",
            TemplateRef = new EntityRef(EntityKind.Template) { Name = "template" }
        };

        using var requestContent = JsonContent.Create(templateRequest, options: EntitySerializerOptions.Default);
        using var response = await client.PostAsync("/entities", content: requestContent);
        var result = JsonSerializer.Deserialize<TemplateResponse>(await response.Content.ReadAsStringAsync(), EntitySerializerOptions.Default)!;

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
    }

    [InlineData(0)] // TODO: will be removed when PostEntityReturns200 is enabled
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [Theory]
    public async Task PostEntityReturns202(int callCountToCompletion)
    {
        // Arrange
        using var host = CreateHostBuilder()
            .Start();

        var durableResult = new DurableTaskResult(Guid.NewGuid().ToString());
        var downstream = host.Services.GetRequiredService<DownstreamApiTestBuilder>();
        var numberOfCalls = 0;

        downstream.Add(HttpMethod.Post, "/entities", response =>
        {
            if (numberOfCalls < callCountToCompletion)
            {
                response.Content = JsonContent.Create(durableResult);
                response.StatusCode = HttpStatusCode.Accepted;
                numberOfCalls++;
            }
            else
            {
                response.Content = JsonContent.Create(new TemplateResponse());
                response.StatusCode = HttpStatusCode.OK;
            }
        });

        var client = host.GetTestClient();

        // Act
        var templateRequest = new TemplateRequest
        {
            Provider = ProviderBaseUrl,
            InputJson = "{}",
            TemplateRef = new EntityRef(EntityKind.Template) { Name = "template" }
        };

        using var requestContent = JsonContent.Create(templateRequest, options: EntitySerializerOptions.Default);
        using var response = await client.PostAsync("/entities", content: requestContent);
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DurableTaskResult>(responseString, EntitySerializerOptions.Default)!;

        Assert.NotNull(result.Id);

        var finalResult = await GetDurableResult<TemplateResponse>(client, result.Id);
    }

    private static async Task<T?> GetDurableResult<T>(HttpClient client, string id)
    {
        using var cts = Debugger.IsAttached
            ? new CancellationTokenSource()
            : new CancellationTokenSource(TimeSpan.FromSeconds(10));

        while (!cts.IsCancellationRequested)
        {
            using var result = await client.GetAsync($"/status/{id}", cts.Token);

            if (!result.IsSuccessStatusCode)
            {
                throw new InvalidOperationException();
            }

            var resultString = await result.Content.ReadAsStringAsync();

            if (result.StatusCode == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<T>(resultString, EntitySerializerOptions.Default)!;
            }
            else if (result.StatusCode == HttpStatusCode.Accepted)
            {
                var acceptedResult = JsonSerializer.Deserialize<DurableTaskResult>(resultString, EntitySerializerOptions.Default)!;

                Assert.NotNull(acceptedResult);
                Assert.Equal(id, acceptedResult.Id);

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        throw new InvalidOperationException();
    }

    private static IHostBuilder CreateHostBuilder()
    {
        var downstream = new DownstreamApiTestBuilder();
        var userService = new Mock<IUserService>();
        var user = new ClaimsPrincipal(new ClaimsIdentity("Test"));

        userService.Setup(u => u.GetOrCreateUser(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(new Entity(EntityKind.User));

        return Host.CreateDefaultBuilder()
            .ConfigureTestDevPlatform()
            .ConfigureServices(services =>
            {
                services.AddSingleton(user);
                services.AddSingleton(downstream);
                services.AddSingleton<IDownstreamApi>(downstream);
                services.AddSingleton(userService.Object);
                services.AddDeveloperPlatform()
                    .AddProviders(options => options
                        .Configure(options =>
                        {
                            options[ProviderBaseUrl] = new()
                            {
                                Enabled = true,
                                Id = "provider",
                                Name = "provider name",
                                Uri = new Uri(ProviderBaseUrl),
                            };
                        }));
            });
    }

    private class DownstreamApiTestBuilder : IDownstreamApi, IDisposable
    {
        private readonly Dictionary<Endpoint, HttpRequestMessage> requests = [];
        private readonly List<(Endpoint, Action<HttpResponseMessage> Response)> configures = [];

        public HttpRequestMessage? GetRequestMessage(HttpMethod method, string relativeUrl) => requests.TryGetValue(new(method, relativeUrl), out var result) ? result : null;

        Task<HttpResponseMessage> IDownstreamApi.SendAsync(DownstreamApiOptions options, CancellationToken token)
        {
            var request = new HttpRequestMessage();
            options.CustomizeHttpRequestMessage?.Invoke(request);
            requests[new(options.Method, options.RelativeUrl)] = request;

            var message = new HttpResponseMessage();

            if (string.Equals(options.BaseUrl, ProviderBaseUrl, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var ((method, relativeUrl), response) in configures)
                {
                    if (string.Equals(options.RelativeUrl, relativeUrl, StringComparison.OrdinalIgnoreCase) && options.Method == method)
                    {
                        response(message);
                    }
                }
            }

            return Task.FromResult(message);
        }

        public DownstreamApiTestBuilder Add(HttpMethod method, string relativeUrl, Action<HttpResponseMessage> configure)
        {
            configures.Add((new(method, relativeUrl), configure));
            return this;
        }

        public void Dispose()
        {
            foreach (var request in requests.Values)
            {
                request.Dispose();
            }
        }
    }

    private record struct Endpoint(HttpMethod Method, string RelativeUrl);
}