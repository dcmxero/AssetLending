using System.Net;
using System.Net.Http.Json;
using DTOs.Asset;
using DTOs.User;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace WebApi.Tests;

/// <summary>
/// Integration tests that run the full API pipeline with an in-memory database.
/// </summary>
public class IntegrationTests(IntegrationTests.TestFactory factory)
    : IClassFixture<IntegrationTests.TestFactory>
{
    private readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task FullLoanWorkflow_CreateAsset_Checkout_Return()
    {
        // 1. Create category
        var categoryResponse = await client.PostAsJsonAsync("/api/assetcategories", new { name = "Test Category" });
        categoryResponse.EnsureSuccessStatusCode();
        var category = await categoryResponse.Content.ReadFromJsonAsync<AssetCategoryDto>();

        // 2. Create user
        var userResponse = await client.PostAsJsonAsync("/api/users", new
        {
            firstName = "Test",
            lastName = "User",
            email = "test@example.com"
        });
        userResponse.EnsureSuccessStatusCode();
        var user = await userResponse.Content.ReadFromJsonAsync<UserDto>();

        // 3. Create asset
        var assetResponse = await client.PostAsJsonAsync("/api/assets", new
        {
            name = "Test Laptop",
            description = "Integration test asset",
            assetCategoryId = category!.Id
        });
        assetResponse.EnsureSuccessStatusCode();
        var asset = await assetResponse.Content.ReadFromJsonAsync<AssetDto>();
        Assert.Equal("Available", asset!.Status);

        // 4. Checkout (create loan)
        var loanResponse = await client.PostAsJsonAsync("/api/loans", new
        {
            assetId = asset.Id,
            borrowedById = user!.Id,
            dueDate = DateTime.UtcNow.AddDays(7).ToString("o")
        });
        loanResponse.EnsureSuccessStatusCode();
        var loan = await loanResponse.Content.ReadFromJsonAsync<LoanDto>();
        Assert.Equal("Active", loan!.Status);

        // 5. Verify asset is now Loaned
        var assetCheck = await client.GetFromJsonAsync<AssetDto>($"/api/assets/{asset.Id}");
        Assert.Equal("Loaned", assetCheck!.Status);

        // 6. Verify active loans contains our loan
        var activeLoans = await client.GetFromJsonAsync<List<LoanDto>>("/api/loans/active");
        Assert.Contains(activeLoans!, l => l.Id == loan.Id);

        // 7. Return the asset
        var returnResponse = await client.PutAsync($"/api/loans/{loan.Id}/return", null);
        returnResponse.EnsureSuccessStatusCode();
        var returnedLoan = await returnResponse.Content.ReadFromJsonAsync<LoanDto>();
        Assert.Equal("Returned", returnedLoan!.Status);

        // 8. Verify asset is Available again
        var assetAfterReturn = await client.GetFromJsonAsync<AssetDto>($"/api/assets/{asset.Id}");
        Assert.Equal("Available", assetAfterReturn!.Status);

        // 9. Active loans should be empty
        var activeLoansAfter = await client.GetFromJsonAsync<List<LoanDto>>("/api/loans/active");
        Assert.DoesNotContain(activeLoansAfter!, l => l.Id == loan.Id);
    }

    [Fact]
    public async Task ReservationWorkflow_Reserve_CheckoutFromReservation()
    {
        // Setup
        var category = await CreateCategory("Reserve Category");
        var user = await CreateUser("Reserve", "Tester", "reserve@example.com");
        var asset = await CreateAsset("Reserve Laptop", category.Id);

        // 1. Reserve the asset
        var reserveResponse = await client.PostAsJsonAsync("/api/reservations", new
        {
            assetId = asset.Id,
            reservedById = user.Id,
            reservedUntil = DateTime.UtcNow.AddDays(3).ToString("o")
        });
        reserveResponse.EnsureSuccessStatusCode();
        var reservation = await reserveResponse.Content.ReadFromJsonAsync<ReservationDto>();
        Assert.False(reservation!.IsCancelled);

        // 2. Verify asset is Reserved
        var reservedAsset = await client.GetFromJsonAsync<AssetDto>($"/api/assets/{asset.Id}");
        Assert.Equal("Reserved", reservedAsset!.Status);

        // 3. Same user checks out from reservation
        var loanResponse = await client.PostAsJsonAsync("/api/loans", new
        {
            assetId = asset.Id,
            borrowedById = user.Id,
            dueDate = DateTime.UtcNow.AddDays(7).ToString("o")
        });
        loanResponse.EnsureSuccessStatusCode();
        var loan = await loanResponse.Content.ReadFromJsonAsync<LoanDto>();
        Assert.Equal("Active", loan!.Status);

        // 4. Asset should now be Loaned
        var loanedAsset = await client.GetFromJsonAsync<AssetDto>($"/api/assets/{asset.Id}");
        Assert.Equal("Loaned", loanedAsset!.Status);
    }

    [Fact]
    public async Task CannotCheckout_AlreadyLoanedAsset()
    {
        // Setup
        var category = await CreateCategory("Conflict Category");
        var user1 = await CreateUser("User", "One", "user1@example.com");
        var user2 = await CreateUser("User", "Two", "user2@example.com");
        var asset = await CreateAsset("Conflict Laptop", category.Id);

        // User1 checks out
        var loan1 = await client.PostAsJsonAsync("/api/loans", new
        {
            assetId = asset.Id,
            borrowedById = user1.Id,
            dueDate = DateTime.UtcNow.AddDays(7).ToString("o")
        });
        loan1.EnsureSuccessStatusCode();

        // User2 tries to checkout the same asset
        var loan2 = await client.PostAsJsonAsync("/api/loans", new
        {
            assetId = asset.Id,
            borrowedById = user2.Id,
            dueDate = DateTime.UtcNow.AddDays(7).ToString("o")
        });

        Assert.Equal(HttpStatusCode.Conflict, loan2.StatusCode);
    }

    [Fact]
    public async Task DuplicateEmail_ReturnsConflict()
    {
        await CreateUser("Dup", "User", "dup@example.com");

        var response = await client.PostAsJsonAsync("/api/users", new
        {
            firstName = "Another",
            lastName = "User",
            email = "dup@example.com"
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Statistics_ReturnsValidData()
    {
        var response = await client.GetAsync("/api/statistics");
        response.EnsureSuccessStatusCode();
    }

    // Helper methods

    private async Task<AssetCategoryDto> CreateCategory(string name)
    {
        var response = await client.PostAsJsonAsync("/api/assetcategories", new { name });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AssetCategoryDto>())!;
    }

    private async Task<UserDto> CreateUser(string firstName, string lastName, string email)
    {
        var response = await client.PostAsJsonAsync("/api/users", new { firstName, lastName, email });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<UserDto>())!;
    }

    private async Task<AssetDto> CreateAsset(string name, int categoryId)
    {
        var response = await client.PostAsJsonAsync("/api/assets", new { name, assetCategoryId = categoryId });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AssetDto>())!;
    }

    /// <summary>
    /// Custom factory that replaces SQL Server with an in-memory database for testing.
    /// </summary>
    public class TestFactory
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove all DbContext-related registrations
                var descriptors = services
                    .Where(d => d.ServiceType.FullName?.Contains("DbContext") == true
                             || d.ServiceType.FullName?.Contains("EntityFramework") == true
                             || d.ServiceType.FullName?.Contains("SqlServer") == true)
                    .ToList();

                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database
                var dbName = "TestDb_" + Guid.NewGuid();
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                    options.ConfigureWarnings(w => w.Ignore(
                        Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning));
                });
            });

            builder.UseEnvironment("Development");
        }
    }
}
