using Domain.Models.AssetManagement;
using DTOs.Asset;

namespace Application.Mappers;

/// <summary>
/// Provides mapping extension methods between <see cref="Reservation"/> domain entities and reservation DTOs.
/// </summary>
public static class ReservationMapper
{
    /// <summary>
    /// Maps a <see cref="Reservation"/> domain entity to a <see cref="ReservationDto"/>.
    /// </summary>
    public static ReservationDto ToDto(this Reservation reservation) => new()
    {
        Id = reservation.Id,
        AssetId = reservation.AssetId,
        AssetName = reservation.Asset?.Name ?? "",
        ReservedById = reservation.ReservedById,
        ReservedByName = reservation.ReservedBy?.FullName ?? "",
        ReservedAt = reservation.ReservedAt,
        ReservedUntil = reservation.ReservedUntil,
        IsCancelled = reservation.IsCancelled,
        IsExpired = reservation.IsExpired
    };
}
