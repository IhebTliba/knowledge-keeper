using KnowledgeKeeper.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KnowledgeKeeper.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    /// <summary>Applique les migrations et insère les données de démo. Réservé au développement local.</summary>
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        // 1. Crée la base si besoin et applique les migrations manquantes.
        await db.Database.MigrateAsync(ct);

        // 2. Insère les données de démo si la base est vide.
        await DemoDataSeeder.SeedAsync(db, hasher, ct);
    }
}
