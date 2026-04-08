using Application.Mappers;
using Domain.Common;
using Domain.Models.AssetManagement;
using DTOs.Asset;
using Infrastructure.Repositories.AssetManagement;
using Infrastructure.Repositories.Identity;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Service responsible for asset reservation operations including creation and cancellation.
/// </summary>
public sealed class ReservationService(
    IReservationRepository reservationRepository,
    IAssetRepository assetRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<ReservationService> logger)
    : IReservationService
{
    public async Task<Result<ReservationDto>> CreateReservationAsync(CreateReservationDto dto, CancellationToken cancellationToken = default)
    {
        var asset = await assetRepository.GetByIdAsync(dto.AssetId, cancellationToken);
        if (asset is null)
        {
            return Result.Failure<ReservationDto>($"Asset with ID {dto.AssetId} not found.");
        }

        var user = await userRepository.GetByIdAsync(dto.ReservedById, cancellationToken);
        if (user is null)
        {
            return Result.Failure<ReservationDto>($"User with ID {dto.ReservedById} not found.");
        }

        // Check for expired reservation and auto-cancel if needed
        if (asset.Status == Domain.Enums.AssetStatus.Reserved)
        {
            var existingReservation = await reservationRepository.GetActiveByAssetIdAsync(asset.Id, cancellationToken);
            if (existingReservation is not null && existingReservation.IsExpired)
            {
                existingReservation.Cancel();
                asset.CancelReservation();
                logger.LogInformation("Auto-cancelled expired reservation {ReservationId} for asset {AssetId}", existingReservation.Id, asset.Id);
            }
        }

        var reserveResult = asset.Reserve();
        if (!reserveResult.IsSuccess)
        {
            logger.LogWarning("Failed to reserve asset {AssetId}: {Error}", dto.AssetId, reserveResult.Error);
            return Result.Failure<ReservationDto>(reserveResult.Error!);
        }

        var reservation = new Reservation
        {
            AssetId = asset.Id,
            ReservedById = user.Id,
            ReservedUntil = dto.ReservedUntil
        };

        await reservationRepository.AddAsync(reservation, cancellationToken);

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            logger.LogWarning("Concurrency conflict creating reservation for asset {AssetId}", dto.AssetId);
            return Result.Failure<ReservationDto>("The asset was modified by another user. Please try again.");
        }

        reservation.Asset = asset;
        reservation.ReservedBy = user;

        logger.LogInformation("Created reservation {ReservationId} for asset {AssetId} by user {UserId}", reservation.Id, asset.Id, user.Id);

        return Result.Success(reservation.ToDto());
    }

    public async Task<Result<ReservationDto>> CancelReservationAsync(int reservationId, CancellationToken cancellationToken = default)
    {
        var reservation = await reservationRepository.GetByIdAsync(reservationId, cancellationToken);
        if (reservation is null)
        {
            return Result.Failure<ReservationDto>($"Reservation with ID {reservationId} not found.");
        }

        var cancelResult = reservation.Cancel();
        if (!cancelResult.IsSuccess)
        {
            logger.LogWarning("Failed to cancel reservation {ReservationId}: {Error}", reservationId, cancelResult.Error);
            return Result.Failure<ReservationDto>(cancelResult.Error!);
        }

        var assetResult = reservation.Asset.CancelReservation();
        if (!assetResult.IsSuccess)
        {
            logger.LogWarning("Failed to cancel reservation asset {AssetId}: {Error}", reservation.AssetId, assetResult.Error);
            return Result.Failure<ReservationDto>(assetResult.Error!);
        }

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            logger.LogWarning("Concurrency conflict cancelling reservation {ReservationId}", reservationId);
            return Result.Failure<ReservationDto>("The asset was modified by another user. Please try again.");
        }

        logger.LogInformation("Cancelled reservation {ReservationId} for asset {AssetId}", reservation.Id, reservation.AssetId);

        return Result.Success(reservation.ToDto());
    }
}
