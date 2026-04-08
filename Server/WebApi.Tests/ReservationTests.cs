using Domain.Models.AssetManagement;
using Xunit;

namespace WebApi.Tests;

public class ReservationTests
{
    private static Reservation CreateActiveReservation() => new()
    {
        Id = 1,
        AssetId = 1,
        ReservedById = 1,
        ReservedUntil = DateTime.UtcNow.AddDays(3),
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
}
