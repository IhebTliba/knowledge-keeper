using KnowledgeKeeper.Domain.Organization;
using KnowledgeKeeper.Domain.Transfers;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeKeeper.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<KnowledgeTransfer> KnowledgeTransfers => Set<KnowledgeTransfer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
