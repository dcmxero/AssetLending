using Domain.Models.AssetManagement;
using Xunit;

namespace WebApi.Tests;

public class ReservationTests
{
    private static readonly DateTime Now = new(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc);

    private static Reservation CreateActiveReservation() => new()
    {
        Id = 1,
        AssetId = 1,
        ReservedById = 1,
        ReservedAt = Now,
        ReservedUntil = Now.AddDays(3),
        IsCancelled = false
    };

    [Fact]
    public void Cancel_SetsCancelledToTrue_WhenReservationIsActive()
    {
        // Arrange
        var reservation = CreateActiveReservation();

        // Act
        var result = reservation.Cancel();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(reservation.IsCancelled);
    }

    [Fact]
    public void Cancel_ReturnsFailure_WhenAlreadyCancelled()
    {
        // Arrange
        var reservation = CreateActiveReservation();
        reservation.Cancel();

        // Act
        var result = reservation.Cancel();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void IsExpiredAt_IsFalse_BeforeTheReservationRunsOut()
    {
        Assert.False(CreateActiveReservation().IsExpiredAt(Now.AddDays(2)));
    }

    [Fact]
    public void IsExpiredAt_IsTrue_OnceTheReservationHasRunOut()
    {
        Assert.True(CreateActiveReservation().IsExpiredAt(Now.AddDays(4)));
    }

    [Fact]
    public void IsExpiredAt_IsFalse_ForACancelledReservation()
    {
        var reservation = CreateActiveReservation();
        reservation.Cancel();

        Assert.False(reservation.IsExpiredAt(Now.AddDays(4)));
    }
}
