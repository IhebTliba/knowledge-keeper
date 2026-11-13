using KnowledgeKeeper.Domain.Transfers;
using KnowledgeKeeper.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KnowledgeKeeper.Api.IntegrationTests;

[Collection(nameof(ApiCollection))]
public sealed class PersistenceTests(ApiFactory factory)
{
    [Fact]
    public async Task Les_donnees_de_demo_ne_sont_inserees_qu_une_seule_fois()
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await DemoDataSeeder.SeedAsync(db);
        await DemoDataSeeder.SeedAsync(db);

        Assert.Equal(2, await db.Departments.CountAsync());
        Assert.Equal(3, await db.Employees.CountAsync());
        Assert.Equal(1, await db.KnowledgeTransfers.CountAsync());
    }

    [Fact]
    public async Task La_transmission_de_Karim_est_relue_avec_son_successeur_et_son_statut()
    {
        await using (var seedScope = factory.Services.CreateAsyncScope())
        {
            await DemoDataSeeder.SeedAsync(seedScope.ServiceProvider.GetRequiredService<AppDbContext>());
        }

        // Nouveau scope = nouveau DbContext : on relit vraiment depuis SQL Server, pas depuis le cache.
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var karim = await db.Employees.SingleAsync(e => e.Email == "karim.benali@example.com");
        var sarah = await db.Employees.SingleAsync(e => e.Email == "sarah.haddad@example.com");
        var transfer = await db.KnowledgeTransfers.SingleAsync(t => t.DepartingEmployeeId == karim.Id);

        Assert.Equal(TransferStatus.Planned, transfer.Status);
        Assert.Equal(sarah.Id, transfer.SuccessorId);
        Assert.Equal(new DateOnly(2026, 12, 31), transfer.DueDate);
    }
}
