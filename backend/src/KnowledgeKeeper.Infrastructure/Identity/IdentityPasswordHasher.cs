using KnowledgeKeeper.Application.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace KnowledgeKeeper.Infrastructure.Identity;

/// <summary>Hachage PBKDF2 salé, l'algorithme d'ASP.NET Core Identity.</summary>
public sealed class IdentityPasswordHasher : IPasswordHasher
{
    private static readonly object User = new();
    private readonly PasswordHasher<object> _inner = new();

    public string Hash(string password) => _inner.HashPassword(User, password);

    public bool Verify(string passwordHash, string password) =>
        _inner.VerifyHashedPassword(User, passwordHash, password) != PasswordVerificationResult.Failed;
}
