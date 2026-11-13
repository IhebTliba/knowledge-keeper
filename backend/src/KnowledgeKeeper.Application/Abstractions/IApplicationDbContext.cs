using KnowledgeKeeper.Domain.Identity;
using KnowledgeKeeper.Domain.Organization;
using KnowledgeKeeper.Domain.Transfers;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeKeeper.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Department> Departments { get; }
    DbSet<Position> Positions { get; }
    DbSet<Employee> Employees { get; }
    DbSet<KnowledgeTransfer> KnowledgeTransfers { get; }
    DbSet<UserAccount> UserAccounts { get; }
}
