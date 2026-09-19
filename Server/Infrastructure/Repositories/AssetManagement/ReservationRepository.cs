using Domain.Models.AssetManagement;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository for reservation write operations.
/// </summary>
public sealed class ReservationRepository(ApplicationDbContext context)
    : IReservationRepository
{
    public async Task<Reservation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Reservations
            .Include(r => r.Asset)
            .Include(r => r.ReservedBy)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Reservation?> GetActiveByAssetIdAsync(int assetId, CancellationToken cancellationToken = default)
    {
        return await context.Reservations
            .FirstOrDefaultAsync(r => r.AssetId == assetId && !r.IsCancelled, cancellationToken);
    }

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await context.Reservations.AddAsync(reservation, cancellationToken);
    }
}
