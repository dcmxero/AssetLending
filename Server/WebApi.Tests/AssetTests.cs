using Domain.Enums;
using Domain.Models.AssetManagement;
using Xunit;

namespace WebApi.Tests;

public class AssetTests
{
    private static Asset CreateAvailableAsset() => new()
    {
        Id = 1,
        Name = "ThinkPad T14s",
        Description = "Test laptop",
        SerialNumber = "SN-001",
        Status = AssetStatus.Available
    };

    #region Checkout

    [Fact]
    public void Checkout_SetsStatusToLoaned_WhenAssetIsAvailable()
    {
        // Arrange
        var asset = CreateAvailableAsset();

        // Act
        var result = asset.Checkout();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(AssetStatus.Loaned, asset.Status);
    }

    [Fact]
    public void Checkout_ReturnsFailure_WhenAssetIsAlreadyLoaned()
    {
        // Arrange
        var asset = CreateAvailableAsset();
        asset.Checkout();

        // Act
        var result = asset.Checkout();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Checkout_ReturnsFailure_WhenAssetIsReserved()
    {
        // Arrange
        var asset = CreateAvailableAsset();
        asset.Reserve();

        // Act
        var result = asset.Checkout();

        // Assert
        Assert.False(result.IsSuccess);
    }

    #endregion

    #region Return

    [Fact]
    public void Return_SetsStatusToAvailable_WhenAssetIsLoaned()
    {
        // Arrange
        var asset = CreateAvailableAsset();
        asset.Checkout();

        // Act
        var result = asset.Return();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(AssetStatus.Available, asset.Status);
    }

    [Fact]
    public void Return_ReturnsFailure_WhenAssetIsNotLoaned()
    {
        // Arrange
        var asset = CreateAvailableAsset();

        // Act
        var result = asset.Return();

        // Assert
        Assert.False(result.IsSuccess);
    }

    #endregion

    #region Reserve

    [Fact]
    public void Reserve_SetsStatusToReserved_WhenAssetIsAvailable()
    {
        // Arrange
        var asset = CreateAvailableAsset();

        // Act
        var result = asset.Reserve();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(AssetStatus.Reserved, asset.Status);
    }

    [Fact]
    public void Reserve_ReturnsFailure_WhenAssetIsLoaned()
    {
        // Arrange
        var asset = CreateAvailableAsset();
        asset.Checkout();

        // Act
        var result = asset.Reserve();

        // Assert
        Assert.False(result.IsSuccess);
    }

    #endregion

    #region CancelReservation

    [Fact]
    public void CancelReservation_SetsStatusToAvailable_WhenAssetIsReserved()
    {
        // Arrange
        var asset = CreateAvailableAsset();
        asset.Reserve();

        // Act
        var result = asset.CancelReservation();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(AssetStatus.Available, asset.Status);
    }

    [Fact]
    public void CancelReservation_ReturnsFailure_WhenAssetIsNotReserved()
    {
        // Arrange
        var asset = CreateAvailableAsset();

        // Act
        var result = asset.CancelReservation();

        // Assert
        Assert.False(result.IsSuccess);
    }

    #endregion

    #region CheckoutFromReservation

    [Fact]
    public void CheckoutFromReservation_SetsStatusToLoaned_WhenReservedBySameUser()
    {
        // Arrange
        var asset = CreateAvailableAsset();
        asset.Reserve();

        // Act
        var result = asset.CheckoutFromReservation(reservedByUserId: 1, requestingUserId: 1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(AssetStatus.Loaned, asset.Status);
    }

    [Fact]
    public void CheckoutFromReservation_ReturnsFailure_WhenReservedByDifferentUser()
    {
        // Arrange
        var asset = CreateAvailableAsset();
        asset.Reserve();

        // Act
        var result = asset.CheckoutFromReservation(reservedByUserId: 1, requestingUserId: 2);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(AssetStatus.Reserved, asset.Status);
    }

    [Fact]
    public void CheckoutFromReservation_ReturnsFailure_WhenAssetIsNotReserved()
    {
        // Arrange
        var asset = CreateAvailableAsset();

        // Act
        var result = asset.CheckoutFromReservation(reservedByUserId: 1, requestingUserId: 1);

        // Assert
        Assert.False(result.IsSuccess);
    }

    #endregion

    #region Activate / Deactivate

    [Fact]
    public void Deactivate_SetsIsActiveToFalse_WhenAssetIsAvailable()
    {
        // Arrange
        var asset = CreateAvailableAsset();

        // Act
        var result = asset.Deactivate();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(asset.IsActive);
    }

    [Fact]
    public void Deactivate_ReturnsFailure_WhenAssetIsLoaned()
    {
        // Arrange
        var asset = CreateAvailableAsset();
        asset.Checkout();

        // Act
        var result = asset.Deactivate();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(asset.IsActive);
    }

    [Fact]
    public void Activate_SetsIsActiveToTrue_WhenAssetIsInactive()
    {
        // Arrange
        var asset = CreateAvailableAsset();
        asset.Deactivate();

        // Act
        var result = asset.Activate();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(asset.IsActive);
    }

    [Fact]
    public void Activate_ReturnsFailure_WhenAssetIsAlreadyActive()
    {
        // Arrange
        var asset = CreateAvailableAsset();

        // Act
        var result = asset.Activate();

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Checkout_ReturnsFailure_WhenAssetIsInactive()
    {
        // Arrange
        var asset = CreateAvailableAsset();
        asset.Deactivate();

        // Act
        var result = asset.Checkout();

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Reserve_ReturnsFailure_WhenAssetIsInactive()
    {
        // Arrange
        var asset = CreateAvailableAsset();
        asset.Deactivate();

        // Act
        var result = asset.Reserve();

        // Assert
        Assert.False(result.IsSuccess);
    }

    #endregion
}
