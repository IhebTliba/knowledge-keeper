using KnowledgeKeeper.Application.Abstractions;
using KnowledgeKeeper.Domain.Identity;
using KnowledgeKeeper.Domain.Organization;
using KnowledgeKeeper.Domain.Transfers;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeKeeper.Infrastructure.Persistence;

/// <summary>Données fictives du scénario de démo : Karim part, Sarah reprend son poste, Nadia (RH) supervise.</summary>
public static class DemoDataSeeder
{
    /// <summary>Mot de passe des comptes de démo. Uniquement pour le développement local et les tests.</summary>
    public const string DemoPassword = "Demo123!";

    public static async Task SeedAsync(AppDbContext db, IPasswordHasher hasher, CancellationToken ct = default)
    {
        if (await db.Departments.AnyAsync(ct))
            return; // Idempotent : ne fait rien si les données existent déjà.

        var finance = Department.Create("Finance");
        var supplyChain = Department.Create("Supply Chain");
        var humanResources = Department.Create("Ressources humaines");

        var accountsReceivable = Position.Create("Responsable comptabilité clients", finance.Id, Criticality.High);
        var buyer = Position.Create("Acheteuse", supplyChain.Id, Criticality.Medium);
        var hrManager = Position.Create("Responsable RH", humanResources.Id, Criticality.Medium);

        var karim = Employee.Create("Karim Benali", "karim.benali@example.com", accountsReceivable.Id, new DateOnly(2011, 3, 1));
        karim.AnnounceDeparture(new DateOnly(2026, 12, 31));

        var sarah = Employee.Create("Sarah Haddad", "sarah.haddad@example.com", accountsReceivable.Id, new DateOnly(2026, 11, 2));
        var lina = Employee.Create("Lina Mansour", "lina.mansour@example.com", buyer.Id, new DateOnly(2019, 6, 15));
        var nadia = Employee.Create("Nadia Cherif", "nadia.cherif@example.com", hrManager.Id, new DateOnly(2016, 9, 1));

        var transfer = KnowledgeTransfer.Plan(karim);
        transfer.AssignSuccessor(sarah);

        db.AddRange(finance, supplyChain, humanResources, accountsReceivable, buyer, hrManager, karim, sarah, lina, nadia, transfer);
        db.AddRange(
            UserAccount.Create(karim, hasher.Hash(DemoPassword), Role.Employee),
            UserAccount.Create(sarah, hasher.Hash(DemoPassword), Role.Employee),
            UserAccount.Create(lina, hasher.Hash(DemoPassword), Role.Employee),
            UserAccount.Create(nadia, hasher.Hash(DemoPassword), Role.HumanResources));

        await db.SaveChangesAsync(ct);
    }
}
