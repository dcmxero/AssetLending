using Domain.Models.AssetManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class AssetConfiguration
    : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasIndex(a => a.SerialNumber)
            .IsUnique()
            .HasFilter("[SerialNumber] IS NOT NULL");

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(a => a.IsActive)
            .HasDefaultValue(true);

        builder.Property(a => a.RowVersion)
            .IsRowVersion();
    }
}
