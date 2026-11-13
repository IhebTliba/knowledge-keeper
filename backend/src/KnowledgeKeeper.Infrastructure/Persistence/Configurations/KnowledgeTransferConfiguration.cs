using KnowledgeKeeper.Domain.Organization;
using KnowledgeKeeper.Domain.Transfers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KnowledgeKeeper.Infrastructure.Persistence.Configurations;

internal sealed class KnowledgeTransferConfiguration : IEntityTypeConfiguration<KnowledgeTransfer>
{
    public void Configure(EntityTypeBuilder<KnowledgeTransfer> builder)
    {
        builder.ToTable("KnowledgeTransfers");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);

        // Une seule transmission par employé sortant, garanti aussi par la base.
        builder.HasIndex(t => t.DepartingEmployeeId).IsUnique();

        builder.HasOne<Employee>().WithMany()
            .HasForeignKey(t => t.DepartingEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Employee>().WithMany()
            .HasForeignKey(t => t.SuccessorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Position>().WithMany()
            .HasForeignKey(t => t.PositionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
