using KnowledgeKeeper.Domain.Common;
using KnowledgeKeeper.Domain.Organization;

namespace KnowledgeKeeper.Domain.Transfers;

public enum TransferStatus
{
    Planned,
    InProgress,
    Completed
}

/// <summary>La transmission du savoir d'un employé sortant vers son successeur.</summary>
public sealed class KnowledgeTransfer : Entity
{
    private KnowledgeTransfer() { } // EF Core

    public Guid DepartingEmployeeId { get; private set; }
    public Guid PositionId { get; private set; }
    public Guid? SuccessorId { get; private set; }
    public DateOnly DueDate { get; private set; }
    public TransferStatus Status { get; private set; }

    public static KnowledgeTransfer Plan(Employee departingEmployee)
    {
        if (departingEmployee.DepartureDate is not { } departureDate)
            throw new DomainException("Impossible de planifier une transmission pour un employé sans date de départ.");

        return new KnowledgeTransfer
        {
            DepartingEmployeeId = departingEmployee.Id,
            PositionId = departingEmployee.PositionId,
            DueDate = departureDate,
            Status = TransferStatus.Planned
        };
    }

    public void AssignSuccessor(Employee successor)
    {
        if (successor.Id == DepartingEmployeeId)
            throw new DomainException("Un employé ne peut pas être son propre successeur.");
        if (Status == TransferStatus.Completed)
            throw new DomainException("La transmission est déjà terminée.");

        SuccessorId = successor.Id;
    }

    public void Start()
    {
        if (Status != TransferStatus.Planned)
            throw new DomainException("Seule une transmission planifiée peut démarrer.");

        Status = TransferStatus.InProgress;
    }

    public void Complete()
    {
        if (Status != TransferStatus.InProgress)
            throw new DomainException("Seule une transmission en cours peut être terminée.");

        Status = TransferStatus.Completed;
    }
}
