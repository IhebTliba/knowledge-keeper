using KnowledgeKeeper.Application.Abstractions;
using KnowledgeKeeper.Infrastructure.Identity;
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
        // Persistance : la chaîne de connexion est lue à l'exécution (surchargeable par les tests).
        services.AddDbContext<AppDbContext>((sp, options) =>
            options.UseSqlServer(GetConnectionString(sp)));
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        // Sécurité : la configuration JWT est validée au démarrage, l'API refuse de démarrer si elle est invalide.
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        services.AddHealthChecks()
            .AddSqlServer(GetConnectionString, name: "sqlserver", tags: [ReadyTag]);

        return services;
    }

    private static string GetConnectionString(IServiceProvider sp) =>
        sp.GetRequiredService<IConfiguration>().GetConnectionString(ConnectionStringName)
        ?? throw new InvalidOperationException($"La chaîne de connexion '{ConnectionStringName}' est absente.");
}
