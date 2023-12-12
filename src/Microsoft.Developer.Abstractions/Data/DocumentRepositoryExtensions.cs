// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class DocumentRepositoryExtensions
{
    public static IDeveloperPlatformBuilder AddDocumentRepository<TDocument>(this IDeveloperPlatformBuilder builder, string name, Action<DocumentRepositoryOptions<TDocument>> configure)
    {
        builder.Services.AddOptions<DocumentRepositoryOptions<TDocument>>(name)
            .Configure(configure)
            .ValidateDataAnnotations();

        return builder;
    }

    public static IDeveloperPlatformBuilder AddDocumentRepository<TImplementation, TDocument>(this IDeveloperPlatformBuilder builder, string name)
        where TImplementation : class
    {
        builder.Services.TryAddTransient(typeof(TypedDocumentRepositoryFactory<,>));
        builder.Services.AddSingleton(sp => sp.GetRequiredService<TypedDocumentRepositoryFactory<TImplementation, TDocument>>().Create(name));

        return builder;
    }

    public static IDeveloperPlatformBuilder AddDocumentRepository<TImplementation, TDocument>(this IDeveloperPlatformBuilder builder, Action<DocumentRepositoryOptions<TDocument>> configure)
        where TImplementation : class
    {
        var name = typeof(TImplementation).FullName ?? throw new InvalidOperationException($"Type {typeof(TImplementation)} does not have a full name");

        builder.AddDocumentRepository(name, configure);
        builder.AddDocumentRepository<TImplementation, TDocument>(name);

        return builder;
    }

    public static IDeveloperPlatformBuilder AddDocumentRepository<TService, TImplementation, TDocument>(this IDeveloperPlatformBuilder builder, string name)
        where TService : class
        where TImplementation : class, TService
    {
        builder.AddDocumentRepository<TImplementation, TDocument>(name);
        builder.Services.AddSingleton<TService>(sp => sp.GetRequiredService<TImplementation>());

        return builder;
    }

    public static IDeveloperPlatformBuilder AddDocumentRepository<TService, TImplementation, TDocument>(this IDeveloperPlatformBuilder builder, string name, Action<DocumentRepositoryOptions<TDocument>> configure)
        where TService : class
        where TImplementation : class, TService
    {
        builder.AddDocumentRepository(name, configure);
        builder.AddDocumentRepository<TImplementation, TDocument>(name);
        builder.Services.AddSingleton<TService>(sp => sp.GetRequiredService<TImplementation>());

        return builder;
    }

    public static IDeveloperPlatformBuilder AddDocumentRepository<TService, TImplementation, TDocument>(this IDeveloperPlatformBuilder builder, Action<DocumentRepositoryOptions<TDocument>> configure)
        where TService : class
        where TImplementation : class, TService
    {
        builder.AddDocumentRepository<TImplementation, TDocument>(configure);
        builder.Services.AddSingleton<TService>(sp => sp.GetRequiredService<TImplementation>());

        return builder;
    }

    private sealed class TypedDocumentRepositoryFactory<TRepository, TDocument>(IServiceProvider services, IDocumentRepositoryFactory<TDocument> factory)
    {
        private readonly ObjectFactory documentRepositoryFactory = ActivatorUtilities.CreateFactory(typeof(TRepository), [typeof(IDocumentRepository<TDocument>)]);

        public TRepository Create(string name)
        {
            var repository = factory.Create(name);
            return (TRepository)documentRepositoryFactory(services, new[] { repository });
        }
    }
}
