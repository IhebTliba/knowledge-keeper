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

        await db.Database.MigrateAsync(ct);
        await DemoDataSeeder.SeedAsync(db, ct);
    }
}
