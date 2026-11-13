using KnowledgeKeeper.Application.Abstractions;
using KnowledgeKeeper.Domain.Identity;
using KnowledgeKeeper.Domain.Transfers;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeKeeper.Application.Transfers;

public sealed record TransferDetails(
    Guid Id,
    string DepartingEmployee,
    string Position,
    string? Successor,
    DateOnly DueDate,
    TransferStatus Status);

public enum LookupOutcome
{
    Found,
    NotFound,
    Forbidden
}

public sealed record TransferLookup(LookupOutcome Outcome, TransferDetails? Transfer = null);

public sealed class TransferQueries(IApplicationDbContext db, ICurrentUser currentUser)
{
    public async Task<IReadOnlyList<TransferDetails>> ListAllAsync(CancellationToken ct = default) =>
        await Project(db.KnowledgeTransfers).ToListAsync(ct);

    public async Task<TransferLookup> GetAsync(Guid id, CancellationToken ct = default)
    {
        var transfer = await db.KnowledgeTransfers.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id, ct);

        if (transfer is null)
            return new TransferLookup(LookupOutcome.NotFound);
        if (!CanView(transfer))
            return new TransferLookup(LookupOutcome.Forbidden);

        var details = await Project(db.KnowledgeTransfers.Where(t => t.Id == id)).SingleAsync(ct);
        return new TransferLookup(LookupOutcome.Found, details);
    }

    // La règle d'accès vit ici, côté serveur : jamais dans le frontend, jamais dans un prompt de LLM.
    private bool CanView(KnowledgeTransfer transfer) =>
        currentUser.Role is Role.HumanResources or Role.Admin
        || transfer.Involves(currentUser.EmployeeId);

    private IQueryable<TransferDetails> Project(IQueryable<KnowledgeTransfer> transfers) =>
        from t in transfers
        join departing in db.Employees on t.DepartingEmployeeId equals departing.Id
        join position in db.Positions on t.PositionId equals position.Id
        join s in db.Employees on t.SuccessorId equals (Guid?)s.Id into successors
        from successor in successors.DefaultIfEmpty()
        select new TransferDetails(
            t.Id,
            departing.FullName,
            position.Title,
            successor == null ? null : successor.FullName,
            t.DueDate,
            t.Status);
}
