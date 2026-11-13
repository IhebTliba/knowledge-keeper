using System.Security.Claims;
using KnowledgeKeeper.Application.Abstractions;
using KnowledgeKeeper.Application.Auth;

namespace KnowledgeKeeper.Api.Endpoints;

public static class AuthEndpoints
{
    public sealed record LoginRequest(string Email, string Password);

    public sealed record MeResponse(Guid UserId, Guid EmployeeId, string? Email, string Role);

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api").WithTags("Auth");

        group.MapPost("/auth/login", async (LoginRequest request, LoginService login, CancellationToken ct) =>
                await login.LoginAsync(request.Email, request.Password, ct) is { } token
                    ? Results.Ok(token)
                    : Results.Unauthorized())
            .AllowAnonymous();

        group.MapGet("/me", (ICurrentUser user, ClaimsPrincipal principal) =>
                Results.Ok(new MeResponse(
                    user.UserId,
                    user.EmployeeId,
                    principal.FindFirstValue(JwtClaims.Email),
                    user.Role.ToString())))
            .RequireAuthorization();

        return app;
    }
}
