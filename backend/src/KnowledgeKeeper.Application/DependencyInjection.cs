using KnowledgeKeeper.Application.Auth;
using KnowledgeKeeper.Application.Transfers;
using Microsoft.Extensions.DependencyInjection;

namespace KnowledgeKeeper.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<LoginService>();
        services.AddScoped<TransferQueries>();
        return services;
    }
}
