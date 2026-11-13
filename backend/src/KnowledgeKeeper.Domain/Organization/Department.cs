using KnowledgeKeeper.Domain.Common;

namespace KnowledgeKeeper.Domain.Organization;

public sealed class Department : Entity
{
    private Department() { } // EF Core

    public string Name { get; private set; } = string.Empty;

    public static Department Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Le nom du service est obligatoire.");

        return new Department { Name = name.Trim() };
    }
}
