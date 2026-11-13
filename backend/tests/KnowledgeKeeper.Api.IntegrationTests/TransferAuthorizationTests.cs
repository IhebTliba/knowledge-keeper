using System.Net;
using System.Net.Http.Json;
using KnowledgeKeeper.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KnowledgeKeeper.Api.IntegrationTests;

[Collection(nameof(ApiCollection))]
public sealed class TransferAuthorizationTests(ApiFactory factory)
{
    [Fact]
    public async Task Le_successeur_peut_voir_la_transmission_qui_le_concerne()
    {
        var client = await factory.CreateClientForAsync("sarah.haddad@example.com");

        var response = await client.GetAsync($"/api/transfers/{await KarimTransferIdAsync()}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Karim Benali", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Un_employe_non_concerne_recoit_403()
    {
        var client = await factory.CreateClientForAsync("lina.mansour@example.com");

        var response = await client.GetAsync($"/api/transfers/{await KarimTransferIdAsync()}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task La_liste_complete_est_reservee_aux_RH()
    {
        var lina = await factory.CreateClientForAsync("lina.mansour@example.com");
        var nadia = await factory.CreateClientForAsync("nadia.cherif@example.com");

        Assert.Equal(HttpStatusCode.Forbidden, (await lina.GetAsync("/api/transfers")).StatusCode);

        var transfers = await nadia.GetFromJsonAsync<List<object>>("/api/transfers");
        Assert.Single(transfers!);
    }

    [Fact]
    public async Task Une_transmission_inexistante_renvoie_404()
    {
        var client = await factory.CreateClientForAsync("nadia.cherif@example.com");

        var response = await client.GetAsync($"/api/transfers/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<Guid> KarimTransferIdAsync()
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var karim = await db.Employees.SingleAsync(e => e.Email == "karim.benali@example.com");
        return (await db.KnowledgeTransfers.SingleAsync(t => t.DepartingEmployeeId == karim.Id)).Id;
    }
}
