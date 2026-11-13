using System.Text.Json.Serialization;
using KnowledgeKeeper.Api.Auth;
using KnowledgeKeeper.Api.Endpoints;
using KnowledgeKeeper.Application;
using KnowledgeKeeper.Infrastructure;
using KnowledgeKeeper.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddJwtSecurity();
builder.Services.AddProblemDetails();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // En local uniquement : migrations + données de démo au démarrage.
    await DatabaseInitializer.InitializeAsync(app.Services);
}

app.UseAuthentication();
app.UseAuthorization();

// Liveness : le processus répond-il ? Aucune dépendance vérifiée.
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

// Readiness : l'API peut-elle servir des requêtes ? Vérifie SQL Server.
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
        Predicate = check => check.Tags.Contains(KnowledgeKeeper.Infrastructure.DependencyInjection.ReadyTag)
});

app.MapAuthEndpoints();
app.MapTransferEndpoints();

app.Run();

// Rend Program visible pour WebApplicationFactory dans les tests d'intégration.
public partial class Program
{
}
