using Application.Abstractions.Persistence;
using Application.Abstractions.Queries;
using Application.Services;
using Domain.Models.AssetManagement;
using DTOs.Asset;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace WebApi.Tests;

/// <summary>
/// Covers asset category creation and the reads that go straight to the query side.
/// </summary>
public class AssetCategoryServiceTests
{
    private readonly Mock<IAssetCategoryRepository> categories = new();
    private readonly Mock<IAssetCategoryQueries> queries = new();
    private readonly Mock<IUnitOfWork> unitOfWork = new();

    private AssetCategoryService CreateService() => new(
        categories.Object,
        queries.Object,
        unitOfWork.Object,
        NullLogger<AssetCategoryService>.Instance);

    [Fact]
    public async Task CreateCategory_SavesAndAnswersWithTheCategory()
    {
        AssetCategory? saved = null;
        categories.Setup(r => r.AddAsync(It.IsAny<AssetCategory>(), It.IsAny<CancellationToken>()))
            .Callback<AssetCategory, CancellationToken>((c, _) => saved = c);

        var result = await CreateService().CreateCategoryAsync(new CreateAssetCategoryDto { Name = "Tools", Description = "Workshop" });

        Assert.Equal("Tools", saved!.Name);
        Assert.Equal("Tools", result.Name);
        unitOfWork.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCategoryById_ReturnsNull_WhenTheQuerySideFindsNothing()
    {
        Assert.Null(await CreateService().GetCategoryByIdAsync(404));
    }

    [Fact]
    public async Task GetAllCategories_PassesThroughToTheQuerySide()
    {
        queries
            .Setup(q => q.GetAllCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new AssetCategoryDto { Id = 1, Name = "Electronics" }]);

        var result = await CreateService().GetAllCategoriesAsync();

        Assert.Equal("Electronics", Assert.Single(result).Name);
    }
}
