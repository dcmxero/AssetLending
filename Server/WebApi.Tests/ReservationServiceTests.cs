using Application.Abstractions.Persistence;
using Application.Abstractions.Queries;
using Application.Services;
using Domain.Common;
using Domain.Enums;
using Domain.Models.AssetManagement;
using Domain.Models.Identity;
using DTOs.Asset;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;

namespace WebApi.Tests;

/// <summary>
/// Covers reservation creation and cancellation, including the expiry decision
/// that depends on the clock.
/// </summary>
public class ReservationServiceTests
{
    private static readonly DateTime Now = new(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc);

    private readonly Mock<IReservationRepository> reservations = new();
    private readonly Mock<IAssetRepository> assets = new();
    private readonly Mock<IUserRepository> users = new();
    private readonly Mock<IUnitOfWork> unitOfWork = new();
    private readonly FakeTimeProvider clock = new(Now);

    private readonly Asset asset = new() { Id = 1, Name = "ThinkPad", AssetCategoryId = 1 };
    private readonly User holder = new() { Id = 2, FirstName = "Jana", LastName = "Novakova", Email = "jana@example.com" };

    private ReservationService CreateService() => new(
        reservations.Object,
        Mock.Of<IReservationQueries>(),
        assets.Object,
        users.Object,
        unitOfWork.Object,
        clock,
        NullLogger<ReservationService>.Instance);

    private CreateReservationDto Request() => new()
    {
        AssetId = asset.Id,
        ReservedById = holder.Id,
        ReservedUntil = Now.AddDays(3)
    };

    private void GivenAssetAndHolderExist()
    {
        assets.Setup(r => r.GetByIdAsync(asset.Id, It.IsAny<CancellationToken>())).ReturnsAsync(asset);
        users.Setup(r => r.GetByIdAsync(holder.Id, It.IsAny<CancellationToken>())).ReturnsAsync(holder);
    }

    [Fact]
    public async Task CreateReservation_StampsReservedAtFromTheClock()
    {
        GivenAssetAndHolderExist();
        Reservation? saved = null;
        reservations.Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .Callback<Reservation, CancellationToken>((r, _) => saved = r);

        var result = await CreateService().CreateReservationAsync(Request());

        Assert.True(result.IsSuccess);
        Assert.Equal(Now, saved!.ReservedAt);
        Assert.Equal(AssetStatus.Reserved, asset.Status);
        Assert.False(result.Value!.IsExpired);
    }

    [Fact]
    public async Task CreateReservation_ReportsNotFound_WhenTheAssetIsMissing()
    {
        users.Setup(r => r.GetByIdAsync(holder.Id, It.IsAny<CancellationToken>())).ReturnsAsync(holder);

        var result = await CreateService().CreateReservationAsync(Request());

        Assert.Equal(ResultErrorKind.NotFound, result.ErrorKind);
    }

    [Fact]
    public async Task CreateReservation_ReplacesAReservationThatHasRunOut()
    {
        GivenAssetAndHolderExist();
        asset.Status = AssetStatus.Reserved;
        var stale = new Reservation { Id = 9, AssetId = asset.Id, ReservedById = 99, ReservedAt = Now, ReservedUntil = Now.AddDays(1) };
        reservations.Setup(r => r.GetActiveByAssetIdAsync(asset.Id, It.IsAny<CancellationToken>())).ReturnsAsync(stale);

        clock.Advance(TimeSpan.FromDays(2));

        var result = await CreateService().CreateReservationAsync(Request());

        Assert.True(result.IsSuccess);
        Assert.True(stale.IsCancelled);
    }

    [Fact]
    public async Task CreateReservation_RefusesWhileAnotherReservationStillHolds()
    {
        GivenAssetAndHolderExist();
        asset.Status = AssetStatus.Reserved;
        var held = new Reservation { Id = 9, AssetId = asset.Id, ReservedById = 99, ReservedAt = Now, ReservedUntil = Now.AddDays(5) };
        reservations.Setup(r => r.GetActiveByAssetIdAsync(asset.Id, It.IsAny<CancellationToken>())).ReturnsAsync(held);

        var result = await CreateService().CreateReservationAsync(Request());

        Assert.Equal(ResultErrorKind.RuleViolation, result.ErrorKind);
        Assert.False(held.IsCancelled);
    }

    [Fact]
    public async Task CancelReservation_ReportsNotFound_WhenItDoesNotExist()
    {
        var result = await CreateService().CancelReservationAsync(404);

        Assert.Equal(ResultErrorKind.NotFound, result.ErrorKind);
    }

    [Fact]
    public async Task CancelReservation_ReleasesTheAsset()
    {
        asset.Status = AssetStatus.Reserved;
        var reservation = new Reservation { Id = 9, AssetId = asset.Id, ReservedById = holder.Id, Asset = asset, ReservedBy = holder, ReservedAt = Now, ReservedUntil = Now.AddDays(3) };
        reservations.Setup(r => r.GetByIdAsync(reservation.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reservation);

        var result = await CreateService().CancelReservationAsync(reservation.Id);

        Assert.True(result.IsSuccess);
        Assert.True(reservation.IsCancelled);
        Assert.Equal(AssetStatus.Available, asset.Status);
    }
}
