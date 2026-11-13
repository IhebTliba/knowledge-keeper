using KnowledgeKeeper.Domain.Common;

namespace KnowledgeKeeper.Domain.Organization;

public enum Criticality
{
    Low,
    Medium,
    High
}

public sealed class Position : Entity
{
    private Position() { } // EF Core

    public string Title { get; private set; } = string.Empty;
    public Guid DepartmentId { get; private set; }
    public Criticality Criticality { get; private set; }

    public static Position Create(string title, Guid departmentId, Criticality criticality)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("L'intitulé du poste est obligatoire.");

        return new Position
        {
            Title = title.Trim(),
            DepartmentId = departmentId,
            Criticality = criticality
        };
    }
}
