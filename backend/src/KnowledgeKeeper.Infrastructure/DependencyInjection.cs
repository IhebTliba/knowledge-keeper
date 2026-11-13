using KnowledgeKeeper.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KnowledgeKeeper.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName = "KnowledgeKeeper";
    public const string ReadyTag = "ready";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // La chaîne de connexion est lue à l'exécution, depuis la configuration finale,
        // pour que les tests d'intégration puissent la remplacer.
        services.AddDbContext<AppDbContext>((sp, options) =>
            options.UseSqlServer(GetConnectionString(sp)));

        services.AddHealthChecks()
            .AddSqlServer(GetConnectionString, name: "sqlserver", tags: [ReadyTag]);

        return services;
    }

    private static string GetConnectionString(IServiceProvider sp) =>
        sp.GetRequiredService<IConfiguration>().GetConnectionString(ConnectionStringName)
        ?? throw new InvalidOperationException($"La chaîne de connexion '{ConnectionStringName}' est absente.");
}
