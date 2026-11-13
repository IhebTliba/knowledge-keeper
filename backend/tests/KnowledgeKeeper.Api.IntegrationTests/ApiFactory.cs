using System.Net.Http.Headers;
using System.Net.Http.Json;
using KnowledgeKeeper.Application.Abstractions;
using KnowledgeKeeper.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace KnowledgeKeeper.Api.IntegrationTests;

/// <summary>Toutes les classes de cette collection partagent le même SQL Server.</summary>
[CollectionDefinition(nameof(ApiCollection))]
public sealed class ApiCollection : ICollectionFixture<ApiFactory>
{
}

public static class TestSettings
{
    public const string JwtSigningKey = "integration-tests-signing-key-at-least-32-chars";
}

/// <summary>Démarre l'API en mémoire, branchée sur un vrai SQL Server 2025 jetable, migré et peuplé.</summary>
public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2025-latest")
        .Build();

    private string _connectionString = string.Empty;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) =>
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:KnowledgeKeeper"] = _connectionString,
                ["Jwt:SigningKey"] = TestSettings.JwtSigningKey
            }));
    }

    public async Task InitializeAsync()
    {
        await _sqlServer.StartAsync();

        // Une base dédiée plutôt que "master", la base par défaut du conteneur.
        _connectionString = new SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
        {
            InitialCatalog = "KnowledgeKeeper"
        }.ConnectionString;

        await using var scope = Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await DemoDataSeeder.SeedAsync(db, scope.ServiceProvider.GetRequiredService<IPasswordHasher>());
    }

    /// <summary>Un client HTTP déjà connecté avec le compte de démo indiqué.</summary>
    public async Task<HttpClient> CreateClientForAsync(string email)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { email, password = DemoDataSeeder.DemoPassword });
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.Token);
        return client;
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await DisposeAsync();
        await _sqlServer.DisposeAsync();
    }

    private sealed record TokenResponse(string Token, DateTimeOffset ExpiresAt);
}

/// <summary>Démarre l'API avec une base injoignable, pour tester les cas de panne.</summary>
public sealed class UnreachableDatabaseApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) =>
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:KnowledgeKeeper"] =
                    "Server=localhost,1;User Id=sa;Password=unused;TrustServerCertificate=True;Connect Timeout=2",
                ["Jwt:SigningKey"] = TestSettings.JwtSigningKey
            }));
    }
}
