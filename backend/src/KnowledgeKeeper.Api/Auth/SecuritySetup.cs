using System.Security.Claims;
using System.Text;
using KnowledgeKeeper.Application.Abstractions;
using KnowledgeKeeper.Domain.Identity;
using KnowledgeKeeper.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace KnowledgeKeeper.Api.Auth;

public static class Policies
{
    public const string HumanResources = nameof(HumanResources);
}

public static class SecuritySetup
{
    public static IServiceCollection AddJwtSecurity(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        // Configuré à partir de JwtOptions au moment de l'utilisation, pour que les tests puissent fournir leur clé.
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;
                bearer.MapInboundClaims = false; // garde les noms de claims tels qu'émis ("role", "sub")
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                    NameClaimType = JwtClaims.Subject,
                    RoleClaimType = JwtClaims.Role,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.HumanResources, policy =>
                policy.RequireRole(nameof(Role.HumanResources), nameof(Role.Admin)));

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, HttpCurrentUser>();

        return services;
    }

    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services) =>
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Colle le token renvoyé par POST /api/auth/login"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                }] = Array.Empty<string>()
            });
        });
}

/// <summary>Lit l'utilisateur courant depuis les claims du token de la requête.</summary>
public sealed class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal Principal =>
        accessor.HttpContext?.User ?? throw new InvalidOperationException("Aucune requête HTTP en cours.");

    public Guid UserId => Guid.Parse(Principal.FindFirstValue(JwtClaims.Subject)!);
    public Guid EmployeeId => Guid.Parse(Principal.FindFirstValue(JwtClaims.EmployeeId)!);
    public Role Role => Enum.Parse<Role>(Principal.FindFirstValue(JwtClaims.Role)!);
}
