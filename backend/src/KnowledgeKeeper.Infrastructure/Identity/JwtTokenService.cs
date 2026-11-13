using System.Text;
using KnowledgeKeeper.Application.Abstractions;
using KnowledgeKeeper.Domain.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace KnowledgeKeeper.Infrastructure.Identity;

public sealed class JwtTokenService(IOptions<JwtOptions> options, TimeProvider time) : ITokenService
{
    public AccessToken CreateToken(UserAccount account)
    {
        var jwt = options.Value;
        var now = time.GetUtcNow();
        var expiresAt = now.AddMinutes(jwt.ExpiresMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            IssuedAt = now.UtcDateTime,
            NotBefore = now.UtcDateTime,
            Expires = expiresAt.UtcDateTime,
            Claims = new Dictionary<string, object>
            {
                [JwtClaims.Subject] = account.Id.ToString(),
                [JwtClaims.Email] = account.Email,
                [JwtClaims.Role] = account.Role.ToString(),
                [JwtClaims.EmployeeId] = account.EmployeeId.ToString()
            },
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                SecurityAlgorithms.HmacSha256)
        };

        return new AccessToken(new JsonWebTokenHandler().CreateToken(descriptor), expiresAt);
    }
}
