using Domain.Models.AssetManagement;
using Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

/// <summary>
/// Application database context for Entity Framework Core.
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<AssetCategory> AssetCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
