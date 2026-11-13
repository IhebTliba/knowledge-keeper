using KnowledgeKeeper.Api.Auth;
using KnowledgeKeeper.Application.Transfers;

namespace KnowledgeKeeper.Api.Endpoints;

public static class TransferEndpoints
{
    public static IEndpointRouteBuilder MapTransferEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/transfers")
            .WithTags("Transfers")
            .RequireAuthorization();

        group.MapGet("/", async (TransferQueries queries, CancellationToken ct) =>
                Results.Ok(await queries.ListAllAsync(ct)))
            .RequireAuthorization(Policies.HumanResources);

        group.MapGet("/{id:guid}", async (Guid id, TransferQueries queries, CancellationToken ct) =>
        {
            var lookup = await queries.GetAsync(id, ct);
            return lookup.Outcome switch
            {
                LookupOutcome.Found => Results.Ok(lookup.Transfer),
                LookupOutcome.Forbidden => Results.Forbid(),
                _ => Results.NotFound()
            };
        });

        return app;
    }
}
