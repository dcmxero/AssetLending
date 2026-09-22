using Domain.Enums;
using Domain.Models.AssetManagement;
using Domain.Models.Identity;
using Infrastructure;
using Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace WebApi.Tests;

/// <summary>
/// Covers the read side: filtering, ordering, paging and the shape of the projected DTOs.
/// </summary>
public class QueryTests
    : IDisposable
{
    private readonly ApplicationDbContext context;
    private readonly FakeTimeProvider clock = new(new DateTimeOffset(2026, 5, 1, 12, 0, 0, TimeSpan.Zero));

    public QueryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("QueryTests_" + Guid.NewGuid())
            .Options;

        context = new ApplicationDbContext(options);
        Seed();
    }

    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }

    #region Assets

    [Fact]
    public async Task GetAssets_ExcludesInactiveAssets()
    {
        var page = await new AssetQueries(context).GetAssetsAsync(null, null, 1, 10);

        Assert.Equal(3, page.TotalCount);
        Assert.DoesNotContain(page.Data, a => a.Name == "Retired Monitor");
    }

    [Fact]
    public async Task GetAssets_FiltersByStatusAndCategory()
    {
        var electronics = context.AssetCategories.Single(c => c.Name == "Electronics");

        var page = await new AssetQueries(context).GetAssetsAsync(AssetStatus.Available, electronics.Id, 1, 10);

        Assert.Single(page.Data);
        Assert.Equal("Docking Station", page.Data[0].Name);
    }

    [Fact]
    public async Task GetAssets_ProjectsCategoryNameAndStatus()
    {
        var page = await new AssetQueries(context).GetAssetsAsync(null, null, 1, 10);
        var asset = page.Data.Single(a => a.Name == "ThinkPad");

        Assert.Equal("Electronics", asset.AssetCategoryName);
        Assert.Equal(nameof(AssetStatus.Loaned), asset.Status);
    }

    [Fact]
    public async Task GetAssets_OrdersByNameAcrossPages()
    {
        var queries = new AssetQueries(context);

        var first = await queries.GetAssetsAsync(null, null, 1, 2);
        var second = await queries.GetAssetsAsync(null, null, 2, 2);

        Assert.Equal(["Docking Station", "Impact Drill"], first.Data.Select(a => a.Name));
        Assert.Equal(["ThinkPad"], second.Data.Select(a => a.Name));
        Assert.Equal(3, first.TotalCount);
        Assert.True(first.HasNextPage);
        Assert.False(second.HasNextPage);
    }

    [Fact]
    public async Task GetAssetById_ReturnsNullWhenMissing()
    {
        Assert.Null(await new AssetQueries(context).GetAssetByIdAsync(9999));
    }

    #endregion

    #region Loans

    [Fact]
    public async Task GetActiveLoans_ProjectsAssetAndBorrowerNames()
    {
        var loans = await new LoanQueries(context, clock).GetActiveLoansAsync();

        var loan = Assert.Single(loans);
        Assert.Equal("ThinkPad", loan.AssetName);
        Assert.Equal("Jana Novakova", loan.BorrowedByName);
        Assert.Equal(nameof(LoanStatus.Active), loan.Status);
    }

    [Fact]
    public async Task GetOverdueLoans_ReturnsOnlyActiveLoansPastTheirDueDate()
    {
        Assert.Empty(await new LoanQueries(context, clock).GetOverdueLoansAsync());

        clock.Advance(TimeSpan.FromDays(10));

        Assert.Single(await new LoanQueries(context, clock).GetOverdueLoansAsync());
    }

    [Fact]
    public async Task GetAllLoans_OrdersNewestFirst()
    {
        var page = await new LoanQueries(context, clock).GetAllLoansAsync(1, 10);

        Assert.Equal(2, page.TotalCount);
        Assert.True(page.Data[0].BorrowedAt >= page.Data[1].BorrowedAt);
    }

    [Fact]
    public async Task GetLoansByAssetId_ReturnsOnlyThatAssetsLoans()
    {
        var thinkPad = context.Assets.Single(a => a.Name == "ThinkPad");

        var page = await new LoanQueries(context, clock).GetLoansByAssetIdAsync(thinkPad.Id, 1, 10);

        Assert.Equal(2, page.TotalCount);
        Assert.All(page.Data, l => Assert.Equal(thinkPad.Id, l.AssetId));
    }

    #endregion

    #region Users and categories

    [Fact]
    public async Task GetUsers_OrdersByLastNameThenFirstName()
    {
        var page = await new UserQueries(context).GetUsersAsync(1, 10);

        Assert.Equal(["Jana Novakova", "Adam Novy"], page.Data.Select(u => $"{u.FirstName} {u.LastName}"));
    }

    [Fact]
    public async Task GetAllCategories_OrdersByName()
    {
        var categories = await new AssetCategoryQueries(context).GetAllCategoriesAsync();

        Assert.Equal(["Electronics", "Tools"], categories.Select(c => c.Name));
    }

    #endregion

    [Fact]
    public async Task Reads_DoNotEnterTheChangeTracker()
    {
        context.ChangeTracker.Clear();

        await new AssetQueries(context).GetAssetsAsync(null, null, 1, 10);
        await new LoanQueries(context, clock).GetAllLoansAsync(1, 10);
        await new UserQueries(context).GetUsersAsync(1, 10);
        await new AssetCategoryQueries(context).GetAllCategoriesAsync();

        Assert.Empty(context.ChangeTracker.Entries());
    }

    private void Seed()
    {
        var now = clock.GetUtcNow().UtcDateTime;

        var electronics = new AssetCategory { Name = "Electronics" };
        var tools = new AssetCategory { Name = "Tools" };
        context.AssetCategories.AddRange(electronics, tools);

        var jana = new User { FirstName = "Jana", LastName = "Novakova", Email = "jana@example.com" };
        var adam = new User { FirstName = "Adam", LastName = "Novy", Email = "adam@example.com" };
        context.Users.AddRange(jana, adam);

        var thinkPad = new Asset { Name = "ThinkPad", AssetCategory = electronics, Status = AssetStatus.Loaned };
        var dock = new Asset { Name = "Docking Station", AssetCategory = electronics };
        var drill = new Asset { Name = "Impact Drill", AssetCategory = tools };
        var retired = new Asset { Name = "Retired Monitor", AssetCategory = electronics, IsActive = false };
        context.Assets.AddRange(thinkPad, dock, drill, retired);

        context.Loans.AddRange(
            new Loan
            {
                Asset = thinkPad,
                BorrowedBy = jana,
                BorrowedAt = now.AddDays(-2),
                DueDate = now.AddDays(5)
            },
            new Loan
            {
                Asset = thinkPad,
                BorrowedBy = adam,
                BorrowedAt = now.AddDays(-9),
                DueDate = now.AddDays(-2),
                ReturnedAt = now.AddDays(-3),
                Status = LoanStatus.Returned
            });

        context.SaveChanges();
        context.ChangeTracker.Clear();
    }
}
