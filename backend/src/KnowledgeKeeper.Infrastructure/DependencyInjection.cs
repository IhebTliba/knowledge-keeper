using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KnowledgeKeeper.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName = "KnowledgeKeeper";
    public const string ReadyTag = "ready";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // La chaîne de connexion est lue au moment du check, depuis la configuration finale.
        // Les tests d'intégration peuvent ainsi la remplacer par celle de leur conteneur.
        services.AddHealthChecks()
            .AddSqlServer(
                sp => sp.GetRequiredService<IConfiguration>().GetConnectionString(ConnectionStringName)
                      ?? throw new InvalidOperationException(
                          $"La chaîne de connexion '{ConnectionStringName}' est absente."),
                name: "sqlserver",
                tags: [ReadyTag]);

        return services;
    }
}
