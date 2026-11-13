using KnowledgeKeeper.Domain.Organization;
using KnowledgeKeeper.Domain.Transfers;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeKeeper.Infrastructure.Persistence;

/// <summary>Données fictives du scénario de démo : Karim part, Sarah reprend son poste.</summary>
public static class DemoDataSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        if (await db.Departments.AnyAsync(ct))
            return; // Idempotent : ne fait rien si les données existent déjà.

        var finance = Department.Create("Finance");
        var supplyChain = Department.Create("Supply Chain");

        var accountsReceivable = Position.Create("Responsable comptabilité clients", finance.Id, Criticality.High);
        var buyer = Position.Create("Acheteuse", supplyChain.Id, Criticality.Medium);

        var karim = Employee.Create("Karim Benali", "karim.benali@example.com", accountsReceivable.Id, new DateOnly(2011, 3, 1));
        karim.AnnounceDeparture(new DateOnly(2026, 12, 31));

        var sarah = Employee.Create("Sarah Haddad", "sarah.haddad@example.com", accountsReceivable.Id, new DateOnly(2026, 11, 2));
        var lina = Employee.Create("Lina Mansour", "lina.mansour@example.com", buyer.Id, new DateOnly(2019, 6, 15));

        var transfer = KnowledgeTransfer.Plan(karim);
        transfer.AssignSuccessor(sarah);

        db.AddRange(finance, supplyChain, accountsReceivable, buyer, karim, sarah, lina, transfer);
        await db.SaveChangesAsync(ct);
    }
}
