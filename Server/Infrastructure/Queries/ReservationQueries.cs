using System.Linq.Expressions;
using Application.Abstractions.Queries;
using Domain.Models.AssetManagement;
using DTOs.Asset;
using DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Queries;

/// <summary>
/// Reads reservation data and projects it into DTOs within the database query.
/// </summary>
public sealed class ReservationQueries(ApplicationDbContext context, TimeProvider timeProvider)
    : IReservationQueries
{
    public async Task<List<ReservationDto>> GetActiveReservationsAsync(CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        return await context.Reservations
            .Where(r => !r.IsCancelled && r.ReservedUntil >= now)
            .OrderBy(r => r.ReservedUntil)
            .Select(ToDto(now))
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedList<ReservationDto>> GetAllReservationsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var totalCount = await context.Reservations.CountAsync(cancellationToken);

        var items = await context.Reservations
            .OrderByDescending(r => r.ReservedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToDto(now))
            .ToListAsync(cancellationToken);

        return new PaginatedList<ReservationDto>
        {
            Data = items,
            TotalCount = totalCount,
            PageIndex = page,
            PageSize = pageSize
        };
    }

    public async Task<ReservationDto?> GetReservationByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        return await context.Reservations
            .Where(r => r.Id == id)
            .Select(ToDto(now))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static Expression<Func<Reservation, ReservationDto>> ToDto(DateTime utcNow) => reservation => new ReservationDto
    {
        Id = reservation.Id,
        AssetId = reservation.AssetId,
        AssetName = reservation.Asset.Name,
        ReservedById = reservation.ReservedById,
        ReservedByName = reservation.ReservedBy.FirstName + " " + reservation.ReservedBy.LastName,
        ReservedAt = reservation.ReservedAt,
        ReservedUntil = reservation.ReservedUntil,
        IsCancelled = reservation.IsCancelled,
        IsExpired = !reservation.IsCancelled && reservation.ReservedUntil < utcNow
    };
}
