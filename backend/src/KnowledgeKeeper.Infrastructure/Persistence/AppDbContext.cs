using KnowledgeKeeper.Application.Abstractions;
using KnowledgeKeeper.Domain.Identity;
using KnowledgeKeeper.Domain.Organization;
using KnowledgeKeeper.Domain.Transfers;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeKeeper.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<KnowledgeTransfer> KnowledgeTransfers => Set<KnowledgeTransfer>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
