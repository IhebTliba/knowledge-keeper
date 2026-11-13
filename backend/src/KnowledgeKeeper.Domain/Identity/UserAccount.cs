using KnowledgeKeeper.Domain.Common;
using KnowledgeKeeper.Domain.Organization;

namespace KnowledgeKeeper.Domain.Identity;

public enum Role
{
    Employee,
    HumanResources,
    Admin
}

/// <summary>Le compte de connexion d'un employé. Le mot de passe n'est jamais stocké en clair.</summary>
public sealed class UserAccount : Entity
{
    private UserAccount() { } // EF Core

    public Guid EmployeeId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public Role Role { get; private set; }

    public static UserAccount Create(Employee employee, string passwordHash, Role role)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Le hash du mot de passe est obligatoire.");

        return new UserAccount
        {
            EmployeeId = employee.Id,
            Email = employee.Email,
            PasswordHash = passwordHash,
            Role = role
        };
    }
}
