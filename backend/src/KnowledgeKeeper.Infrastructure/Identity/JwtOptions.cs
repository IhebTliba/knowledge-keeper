using System.ComponentModel.DataAnnotations;

namespace KnowledgeKeeper.Infrastructure.Identity;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required] public string Issuer { get; init; } = string.Empty;
    [Required] public string Audience { get; init; } = string.Empty;

    /// <summary>Clé HMAC-SHA256 : 32 caractères minimum. Jamais dans appsettings.json.</summary>
    [Required, MinLength(32)] public string SigningKey { get; init; } = string.Empty;

    [Range(5, 1440)] public int ExpiresMinutes { get; init; } = 60;
}
