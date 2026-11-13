using KnowledgeKeeper.Domain.Identity;

namespace KnowledgeKeeper.Application.Abstractions;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string passwordHash, string password);
}

public sealed record AccessToken(string Token, DateTimeOffset ExpiresAt);

public interface ITokenService
{
    AccessToken CreateToken(UserAccount account);
}

/// <summary>L'utilisateur de la requête en cours, lu depuis son token.</summary>
public interface ICurrentUser
{
    Guid UserId { get; }
    Guid EmployeeId { get; }
    Role Role { get; }
}

/// <summary>Noms des claims du JWT, partagés entre l'émission et la lecture du token.</summary>
public static class JwtClaims
{
    public const string Subject = "sub";
    public const string Email = "email";
    public const string Role = "role";
    public const string EmployeeId = "employee_id";
}
