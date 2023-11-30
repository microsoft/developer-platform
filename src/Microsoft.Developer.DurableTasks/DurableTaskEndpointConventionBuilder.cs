// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Routing;

public sealed class DurableTaskEndpointConventionBuilder<TResult>(IEndpointRouteBuilder builder, RouteHandlerBuilder post) : IEndpointConventionBuilder, IEndpointRouteBuilder
{
    public IServiceProvider ServiceProvider => builder.ServiceProvider;

    public ICollection<EndpointDataSource> DataSources => builder.DataSources;

    public void Add(Action<EndpointBuilder> convention)
    {
        post.Add(convention);
    }

    public IApplicationBuilder CreateApplicationBuilder() => builder.CreateApplicationBuilder();

    public void Finally(Action<EndpointBuilder> finallyConvention)
    {
        post.Finally(finallyConvention);
    }
}
