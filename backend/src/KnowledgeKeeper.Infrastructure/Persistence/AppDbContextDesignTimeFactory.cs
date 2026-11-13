using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KnowledgeKeeper.Infrastructure.Persistence;

public sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Aucune connexion n'est ouverte pour générer une migration : cette chaîne ne contient pas de secret.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=localhost,14330;Database=KnowledgeKeeper;TrustServerCertificate=True")
            .Options;

        return new AppDbContext(options);
    }
}
