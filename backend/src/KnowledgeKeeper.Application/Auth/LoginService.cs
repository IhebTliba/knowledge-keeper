using KnowledgeKeeper.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeKeeper.Application.Auth;

public sealed class LoginService(IApplicationDbContext db, IPasswordHasher hasher, ITokenService tokens)
{
    /// <returns>Un token si les identifiants sont valides, sinon null.</returns>
    public async Task<AccessToken?> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var account = await db.UserAccounts.SingleOrDefaultAsync(a => a.Email == normalizedEmail, ct);

        // Même réponse pour un email inconnu ou un mauvais mot de passe : on ne révèle pas quels comptes existent.
        if (account is null || !hasher.Verify(account.PasswordHash, password))
            return null;

        return tokens.CreateToken(account);
    }
}
