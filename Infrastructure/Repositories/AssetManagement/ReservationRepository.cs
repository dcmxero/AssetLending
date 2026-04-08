using Domain.Models.AssetManagement;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository for reservation-specific data access operations.
/// </summary>
public sealed class ReservationRepository(ApplicationDbContext context)
    : GenericRepository<Reservation>(context), IReservationRepository
{
    public override async Task<Reservation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Reservations
            .Include(r => r.Asset)
            .Include(r => r.ReservedBy)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Reservation?> GetActiveByAssetIdAsync(int assetId, CancellationToken cancellationToken = default)
    {
        return await Context.Reservations
            .FirstOrDefaultAsync(r => r.AssetId == assetId && !r.IsCancelled, cancellationToken);
    }
}
