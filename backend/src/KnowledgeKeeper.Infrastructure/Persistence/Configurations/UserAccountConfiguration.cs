using KnowledgeKeeper.Domain.Identity;
using KnowledgeKeeper.Domain.Organization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KnowledgeKeeper.Infrastructure.Persistence.Configurations;

internal sealed class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.ToTable("UserAccounts");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();
        builder.Property(a => a.Email).HasMaxLength(254).IsRequired();
        builder.Property(a => a.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(a => a.Role).HasConversion<string>().HasMaxLength(30);

        builder.HasIndex(a => a.Email).IsUnique();
        builder.HasIndex(a => a.EmployeeId).IsUnique(); // un compte par employé

        builder.HasOne<Employee>().WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
