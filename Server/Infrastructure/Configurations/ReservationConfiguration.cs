using Domain.Models.AssetManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ReservationConfiguration
    : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasOne(r => r.Asset)
            .WithMany()
            .HasForeignKey(r => r.AssetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ReservedBy)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.ReservedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.AssetId, r.IsCancelled });

        builder.Ignore(r => r.IsExpired);
    }
}
