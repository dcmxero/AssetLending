using Application.Abstractions.Persistence;
using Application.Abstractions.Queries;
using Application.Mappers;
using Domain.Common;
using Domain.Models.AssetManagement;
using DTOs.Asset;
using DTOs.Common;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Service responsible for asset loan operations including checkout and return.
/// </summary>
public sealed class LoanService(
    ILoanRepository loanRepository,
    ILoanQueries loanQueries,
    IAssetRepository assetRepository,
    IUserRepository userRepository,
    IReservationRepository reservationRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ILogger<LoanService> logger)
    : ILoanService
{
    public async Task<LoanDto?> GetLoanByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await loanQueries.GetLoanByIdAsync(id, cancellationToken);
    }

    public async Task<List<LoanDto>> GetActiveLoansAsync(CancellationToken cancellationToken = default)
    {
        return await loanQueries.GetActiveLoansAsync(cancellationToken);
    }

    public async Task<PaginatedList<LoanDto>> GetAllLoansAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await loanQueries.GetAllLoansAsync(page, pageSize, cancellationToken);
    }

    public async Task<List<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        return await loanQueries.GetOverdueLoansAsync(cancellationToken);
    }

    public async Task<Result<LoanDto>> CreateLoanAsync(CreateLoanDto dto, CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var asset = await assetRepository.GetByIdAsync(dto.AssetId, cancellationToken);
        if (asset is null)
        {
            return Result.NotFound<LoanDto>($"Asset with ID {dto.AssetId} not found.");
        }

        var user = await userRepository.GetByIdAsync(dto.BorrowedById, cancellationToken);
        if (user is null)
        {
            return Result.NotFound<LoanDto>($"User with ID {dto.BorrowedById} not found.");
        }

        bool checkedOut = false;

        // Check for expired reservation and auto-cancel, or allow checkout from active reservation by the same user
        if (asset.Status == Domain.Enums.AssetStatus.Reserved)
        {
            var reservation = await reservationRepository.GetActiveByAssetIdAsync(asset.Id, cancellationToken);
            if (reservation is not null)
            {
                if (reservation.IsExpiredAt(now))
                {
                    reservation.Cancel();
                    asset.CancelReservation();
                    logger.LogInformation("Auto-cancelled expired reservation {ReservationId} for asset {AssetId}", reservation.Id, asset.Id);
                }
                else if (reservation.ReservedById == dto.BorrowedById)
                {
                    var fromReservationResult = asset.CheckoutFromReservation(reservation.ReservedById, dto.BorrowedById);
                    if (!fromReservationResult.IsSuccess)
                    {
                        logger.LogWarning("Failed to checkout from reservation for asset {AssetId}: {Error}", dto.AssetId, fromReservationResult.Error);
                        return Result.Failure<LoanDto>(fromReservationResult);
                    }
                    reservation.Cancel();
                    checkedOut = true;
                    logger.LogInformation("Checked out asset {AssetId} from reservation {ReservationId}", asset.Id, reservation.Id);
                }
            }
        }

        if (!checkedOut)
        {
            var checkoutResult = asset.Checkout();
            if (!checkoutResult.IsSuccess)
            {
                logger.LogWarning("Failed to checkout asset {AssetId}: {Error}", dto.AssetId, checkoutResult.Error);
                return Result.Failure<LoanDto>(checkoutResult);
            }
        }

        var loan = new Loan
        {
            AssetId = asset.Id,
            BorrowedById = user.Id,
            BorrowedAt = now,
            DueDate = dto.DueDate
        };

        await loanRepository.AddAsync(loan, cancellationToken);

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (ConcurrencyConflictException)
        {
            logger.LogWarning("Concurrency conflict creating loan for asset {AssetId}", dto.AssetId);
            return Result.ConcurrencyConflict<LoanDto>();
        }

        loan.Asset = asset;
        loan.BorrowedBy = user;

        logger.LogInformation("Created loan {LoanId} for asset {AssetId} by user {UserId}", loan.Id, asset.Id, user.Id);

        return Result.Success(loan.ToDto());
    }

    public async Task<Result<LoanDto>> ReturnAssetAsync(int loanId, CancellationToken cancellationToken = default)
    {
        var loan = await loanRepository.GetByIdAsync(loanId, cancellationToken);
        if (loan is null)
        {
            return Result.NotFound<LoanDto>($"Loan with ID {loanId} not found.");
        }

        var returnResult = loan.MarkReturned(timeProvider.GetUtcNow().UtcDateTime);
        if (!returnResult.IsSuccess)
        {
            logger.LogWarning("Failed to return loan {LoanId}: {Error}", loanId, returnResult.Error);
            return Result.Failure<LoanDto>(returnResult);
        }

        var assetResult = loan.Asset.Return();
        if (!assetResult.IsSuccess)
        {
            logger.LogWarning("Failed to return asset for loan {LoanId}: {Error}", loanId, assetResult.Error);
            return Result.Failure<LoanDto>(assetResult);
        }

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (ConcurrencyConflictException)
        {
            logger.LogWarning("Concurrency conflict returning loan {LoanId}", loanId);
            return Result.ConcurrencyConflict<LoanDto>();
        }

        logger.LogInformation("Returned loan {LoanId} for asset {AssetId}", loan.Id, loan.AssetId);

        return Result.Success(loan.ToDto());
    }

    public async Task<PaginatedList<LoanDto>> GetLoansByAssetIdAsync(int assetId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await loanQueries.GetLoansByAssetIdAsync(assetId, page, pageSize, cancellationToken);
    }
}
