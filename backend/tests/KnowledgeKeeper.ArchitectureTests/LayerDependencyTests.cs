using System.Reflection;
using NetArchTest.Rules;

namespace KnowledgeKeeper.ArchitectureTests;

public class LayerDependencyTests
{
    private const string ApplicationNs = "KnowledgeKeeper.Application";
    private const string InfrastructureNs = "KnowledgeKeeper.Infrastructure";
    private const string ApiNs = "KnowledgeKeeper.Api";

    private static readonly Assembly DomainAssembly = typeof(KnowledgeKeeper.Domain.AssemblyReference).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(KnowledgeKeeper.Application.AssemblyReference).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(KnowledgeKeeper.Infrastructure.AssemblyReference).Assembly;

    [Fact]
    public void Domain_ne_depend_d_aucune_autre_couche()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot().HaveDependencyOnAny(ApplicationNs, InfrastructureNs, ApiNs)
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Application_ne_depend_ni_de_Infrastructure_ni_de_Api()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot().HaveDependencyOnAny(InfrastructureNs, ApiNs)
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Infrastructure_ne_depend_pas_de_Api()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot().HaveDependencyOn(ApiNs)
            .GetResult();

        AssertSuccess(result);
    }

    private static void AssertSuccess(TestResult result) =>
        Assert.True(result.IsSuccessful,
            $"Dépendances interdites : {string.Join(", ", result.FailingTypeNames ?? [])}");
}
