using System.Net;
using System.Net.Http.Json;

namespace KnowledgeKeeper.Api.IntegrationTests;

[Collection(nameof(ApiCollection))]
public sealed class AuthTests(ApiFactory factory)
{
    [Fact]
    public async Task Login_avec_les_bons_identifiants_renvoie_un_token()
    {
        var response = await factory.CreateClient().PostAsJsonAsync("/api/auth/login",
            new { email = "karim.benali@example.com", password = "Demo123!" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("token", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Login_avec_un_mauvais_mot_de_passe_renvoie_401()
    {
        var response = await factory.CreateClient().PostAsJsonAsync("/api/auth/login",
            new { email = "karim.benali@example.com", password = "mauvais" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_sans_token_renvoie_401()
    {
        var response = await factory.CreateClient().GetAsync("/api/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_avec_token_renvoie_le_role_de_l_utilisateur()
    {
        var client = await factory.CreateClientForAsync("nadia.cherif@example.com");

        var me = await client.GetFromJsonAsync<Me>("/api/me");

        Assert.Equal("HumanResources", me!.Role);
        Assert.Equal("nadia.cherif@example.com", me.Email);
    }

    private sealed record Me(Guid UserId, Guid EmployeeId, string Email, string Role);
}
