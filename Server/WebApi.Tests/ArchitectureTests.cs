using Application.Abstractions.Persistence;
using Application.Services;
using Infrastructure;
using Infrastructure.Queries;
using NetArchTest.Rules;
using Xunit;

namespace WebApi.Tests;

/// <summary>
/// Guards the layer boundaries that the read and write split depends on.
/// </summary>
public class ArchitectureTests
{
    private static readonly System.Reflection.Assembly ApplicationAssembly = typeof(LoanService).Assembly;
    private static readonly System.Reflection.Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;

    [Fact]
    public void Application_does_not_depend_on_infrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Application_does_not_depend_on_entity_framework()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Queries_do_not_depend_on_the_write_side()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .That()
            .ResideInNamespace(typeof(LoanQueries).Namespace)
            .ShouldNot()
            .HaveDependencyOnAny("Infrastructure.Repositories", typeof(IUnitOfWork).FullName)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Repositories_do_not_depend_on_the_read_side()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .That()
            .ResideInNamespaceStartingWith("Infrastructure.Repositories")
            .ShouldNot()
            .HaveDependencyOnAny("Infrastructure.Queries", "Application.Abstractions.Queries", "DTOs")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    private static string Describe(TestResult result)
    {
        return result.IsSuccessful
            ? string.Empty
            : "Offending types: " + string.Join(", ", result.FailingTypeNames ?? []);
    }
}
