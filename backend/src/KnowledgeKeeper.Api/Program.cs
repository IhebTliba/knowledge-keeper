using KnowledgeKeeper.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using KnowledgeKeeper.Infrastructure.Persistence;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // En local uniquement : migrations + données de démo au démarrage.
    await DatabaseInitializer.InitializeAsync(app.Services);
}

// Liveness : le processus répond-il ? Aucune dépendance vérifiée.
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

// Readiness : l'API peut-elle servir des requêtes ? Vérifie SQL Server.
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains(DependencyInjection.ReadyTag)
});

app.Run();

// Rend Program visible pour WebApplicationFactory dans les tests d'intégration.
public partial class Program
{
}
