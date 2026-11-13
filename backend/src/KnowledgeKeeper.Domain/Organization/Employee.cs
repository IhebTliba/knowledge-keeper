using KnowledgeKeeper.Domain.Common;

namespace KnowledgeKeeper.Domain.Organization;

public sealed class Employee : Entity
{
    private Employee() { } // EF Core

    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public Guid PositionId { get; private set; }
    public DateOnly HireDate { get; private set; }
    public DateOnly? DepartureDate { get; private set; }

    public bool IsLeaving => DepartureDate is not null;

    public static Employee Create(string fullName, string email, Guid positionId, DateOnly hireDate)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Le nom de l'employé est obligatoire.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException("L'email de l'employé est invalide.");

        return new Employee
        {
            FullName = fullName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PositionId = positionId,
            HireDate = hireDate
        };
    }

    public void AnnounceDeparture(DateOnly departureDate)
    {
        if (departureDate <= HireDate)
            throw new DomainException("La date de départ doit être postérieure à la date d'embauche.");

        DepartureDate = departureDate;
    }
}
