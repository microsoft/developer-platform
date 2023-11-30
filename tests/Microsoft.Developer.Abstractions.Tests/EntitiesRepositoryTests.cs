// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Data;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Microsoft.Developer.Abstractions.Tests;

public class EntitiesRepositoryTests
{
    [Fact]
    public void RegistrationGetsConfiguredStore()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDeveloperPlatform()
            .AddDocumentRepository<TestRepository1, MyDocument>(options =>
            {
                options.DatabaseName = nameof(TestRepository1);
            });

        var factoryMock = new Mock<IDocumentRepositoryFactory<MyDocument>>();
        services.AddSingleton(factoryMock.Object);

        var defaultRepositoryMock = new Mock<IDocumentRepository<MyDocument>>();
        factoryMock.Setup(s => s.Create(typeof(TestRepository1).FullName!)).Returns(defaultRepositoryMock.Object);

        using var provider = services.BuildServiceProvider();

        // Act
        var testRepository = provider.GetRequiredService<TestRepository1>();

        // Assert
        Assert.Equal(defaultRepositoryMock.Object, testRepository.Repository);
    }

    [Fact]
    public void MultipleRegistrations()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDeveloperPlatform()
            .AddDocumentRepository<TestRepository1, MyDocument>(options =>
            {
                options.DatabaseName = nameof(TestRepository1);
            })
            .AddDocumentRepository<TestRepository2, MyDocument>(options =>
            {
                options.DatabaseName = nameof(TestRepository2);
            });

        var factoryMock = new Mock<IDocumentRepositoryFactory<MyDocument>>();
        services.AddSingleton(factoryMock.Object);

        var testRepository1Mock = new Mock<IDocumentRepository<MyDocument>>();
        factoryMock.Setup(s => s.Create(typeof(TestRepository1).FullName!)).Returns(testRepository1Mock.Object);

        var testRepository2Mock = new Mock<IDocumentRepository<MyDocument>>();
        factoryMock.Setup(s => s.Create(typeof(TestRepository2).FullName!)).Returns(testRepository2Mock.Object);

        using var provider = services.BuildServiceProvider();

        // Act
        var testRepository1 = provider.GetRequiredService<TestRepository1>();
        var testRepository2 = provider.GetRequiredService<TestRepository2>();

        // Assert
        Assert.Equal(testRepository1Mock.Object, testRepository1.Repository);
        Assert.Equal(testRepository2Mock.Object, testRepository2.Repository);
    }

    [Fact]
    public void RegisterByName()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDeveloperPlatform()
            .AddDocumentRepository<MyDocument>("test", options =>
            {
                options.DatabaseName = nameof(TestRepository1);
            })
            .AddDocumentRepository<TestRepository1, MyDocument>("test");

        var factoryMock = new Mock<IDocumentRepositoryFactory<MyDocument>>();
        services.AddSingleton(factoryMock.Object);

        var repositoryMock = new Mock<IDocumentRepository<MyDocument>>();
        factoryMock.Setup(s => s.Create("test")).Returns(repositoryMock.Object);

        using var provider = services.BuildServiceProvider();

        // Act
        var testRepository = provider.GetRequiredService<TestRepository1>();

        // Assert
        Assert.Equal(repositoryMock.Object, testRepository.Repository);
    }

    public class MyDocument
    {
    }

    public class TestRepository1(IDocumentRepository<MyDocument> repository)
    {
        public IDocumentRepository<MyDocument> Repository { get; } = repository;
    }

    public class TestRepository2(IDocumentRepository<MyDocument> repository)
    {
        public IDocumentRepository<MyDocument> Repository { get; } = repository;
    }
}
