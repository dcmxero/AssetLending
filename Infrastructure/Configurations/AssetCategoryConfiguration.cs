using Domain.Models.AssetManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

/// <summary>
/// EF Core configuration for the AssetCategory entity.
/// </summary>
public class AssetCategoryConfiguration
    : IEntityTypeConfiguration<AssetCategory>
{
    /// <summary>
    /// Configures the AssetCategory entity relationships and constraints.
    /// </summary>
    public void Configure(EntityTypeBuilder<AssetCategory> builder)
    {
        builder.HasMany(c => c.Assets)
            .WithOne(a => a.AssetCategory)
            .HasForeignKey(a => a.AssetCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
