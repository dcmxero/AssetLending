using Domain.Enums;
using Domain.Models.AssetManagement;
using Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Seeds;

/// <summary>
/// Provides initial seed data for the application database.
/// </summary>
public static class DataSeed
{
    /// <summary>
    /// Seeds the database with initial users and assets if the database is empty.
    /// </summary>
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (context.Users.Any())
        {
            return;
        }

        var users = new List<User>
        {
            new() { FirstName = "Ján", LastName = "Novák", Email = "jan.novak@m2ms.sk" },
            new() { FirstName = "Mária", LastName = "Horváthová", Email = "maria.horvathova@m2ms.sk" },
            new() { FirstName = "Peter", LastName = "Kováč", Email = "peter.kovac@m2ms.sk" }
        };

        context.Users.AddRange(users);
        context.SaveChanges();

        var categories = new List<AssetCategory>
        {
            new() { Name = "Electronics", Description = "Laptops, monitors, projectors and other electronic devices" },
            new() { Name = "Tools", Description = "Power tools, hand tools and workshop equipment" },
            new() { Name = "Office Equipment", Description = "Office furniture, supplies and accessories" }
        };

        context.AssetCategories.AddRange(categories);
        context.SaveChanges();

        var electronics = categories[0];
        var tools = categories[1];

        var assets = new List<Asset>
        {
            new() { Name = "ThinkPad T14s", Description = "Lenovo ThinkPad T14s Gen 4, 16GB RAM", SerialNumber = "SN-TP-001", Status = AssetStatus.Available, AssetCategoryId = electronics.Id },
            new() { Name = "MacBook Pro 14\"", Description = "Apple MacBook Pro M3, 16GB RAM", SerialNumber = "SN-MB-002", Status = AssetStatus.Available, AssetCategoryId = electronics.Id },
            new() { Name = "Dell Monitor U2723QE", Description = "27\" 4K USB-C Monitor", SerialNumber = "SN-MN-003", Status = AssetStatus.Available, AssetCategoryId = electronics.Id },
            new() { Name = "Bosch Drill GSR 18V", Description = "Cordless drill/driver", SerialNumber = "SN-DR-004", Status = AssetStatus.Available, AssetCategoryId = tools.Id },
            new() { Name = "Projector Epson EB-W51", Description = "WXGA 3LCD projector", SerialNumber = "SN-PJ-005", Status = AssetStatus.Available, AssetCategoryId = electronics.Id }
        };

        context.Assets.AddRange(assets);
        context.SaveChanges();
    }
}
