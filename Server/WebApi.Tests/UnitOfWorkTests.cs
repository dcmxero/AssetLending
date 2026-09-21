using Application.Abstractions.Persistence;
using Domain.Models.AssetManagement;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace WebApi.Tests;

/// <summary>
/// Covers the persistence boundary: the unit of work must not let provider-specific
/// exceptions reach the application layer.
/// </summary>
public class UnitOfWorkTests
{
    [Fact]
    public async Task CompleteAsync_TranslatesProviderConcurrencyFailures()
    {
        var databaseName = "UnitOfWorkTests_" + Guid.NewGuid();

        using var writer = CreateContext(databaseName);
        var category = new AssetCategory { Name = "Electronics" };
        writer.AssetCategories.Add(category);
        await writer.SaveChangesAsync();

        var asset = new Asset { Name = "ThinkPad", AssetCategoryId = category.Id };
        writer.Assets.Add(asset);
        await writer.SaveChangesAsync();

        asset.Name = "ThinkPad X1";

        using (var competitor = CreateContext(databaseName))
        {
            competitor.Assets.Remove(competitor.Assets.Single(a => a.Id == asset.Id));
            await competitor.SaveChangesAsync();
        }

        var unitOfWork = new Infrastructure.UnitOfWork.UnitOfWork(writer);

        var exception = await Assert.ThrowsAsync<ConcurrencyConflictException>(() => unitOfWork.CompleteAsync());
        Assert.IsType<DbUpdateConcurrencyException>(exception.InnerException);
    }

    [Fact]
    public async Task CompleteAsync_PersistsPendingChanges()
    {
        using var context = CreateContext("UnitOfWorkTests_" + Guid.NewGuid());
        context.AssetCategories.Add(new AssetCategory { Name = "Tools" });

        await new Infrastructure.UnitOfWork.UnitOfWork(context).CompleteAsync();

        Assert.Single(context.AssetCategories);
    }

    private static ApplicationDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new ApplicationDbContext(options);
    }
}
