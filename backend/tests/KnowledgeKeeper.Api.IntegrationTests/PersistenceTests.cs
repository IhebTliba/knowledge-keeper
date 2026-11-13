using KnowledgeKeeper.Application.Abstractions;
using KnowledgeKeeper.Domain.Transfers;
using KnowledgeKeeper.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KnowledgeKeeper.Api.IntegrationTests;

[Collection(nameof(ApiCollection))]
public sealed class PersistenceTests(ApiFactory factory)
{
    [Fact]
    public async Task Relancer_le_seed_ne_duplique_pas_les_donnees()
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await DemoDataSeeder.SeedAsync(db, scope.ServiceProvider.GetRequiredService<IPasswordHasher>());

        Assert.Equal(3, await db.Departments.CountAsync());
        Assert.Equal(4, await db.Employees.CountAsync());
        Assert.Equal(4, await db.UserAccounts.CountAsync());
        Assert.Equal(1, await db.KnowledgeTransfers.CountAsync());
    }

    [Fact]
    public async Task La_transmission_de_Karim_est_relue_avec_son_successeur_et_son_statut()
    {
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
