using Domain.Models.AssetManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class LoanConfiguration
    : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.Property(l => l.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(l => l.Asset)
            .WithMany()
            .HasForeignKey(l => l.AssetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.BorrowedBy)
            .WithMany(u => u.Loans)
            .HasForeignKey(l => l.BorrowedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => new { l.AssetId, l.Status });
    }
}
