using Application.Mappers;
using Domain.Common;
using Domain.Models.AssetManagement;
using DTOs.Asset;
using DTOs.Common;
using Infrastructure.Repositories.AssetManagement;
using Infrastructure.Repositories.Identity;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Service responsible for asset loan operations including checkout and return.
/// </summary>
public sealed class LoanService(
    ILoanRepository loanRepository,
    IAssetRepository assetRepository,
    IUserRepository userRepository,
    IReservationRepository reservationRepository,
    IUnitOfWork unitOfWork,
    ILogger<LoanService> logger)
    : ILoanService
{
    public async Task<List<LoanDto>> GetActiveLoansAsync(CancellationToken cancellationToken = default)
    {
        var loans = await loanRepository.GetActiveLoansAsync(cancellationToken);
        return [.. loans.Select(l => l.ToDto())];
    }

    public async Task<PaginatedList<LoanDto>> GetAllLoansAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await loanRepository.GetAllLoansAsync(page, pageSize, cancellationToken);
        return new PaginatedList<LoanDto>
        {
            Data = [.. items.Select(l => l.ToDto())],
            TotalCount = totalCount,
            PageIndex = page,
            PageSize = pageSize
        };
    }

    public async Task<List<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        var loans = await loanRepository.GetOverdueLoansAsync(cancellationToken);
        return [.. loans.Select(l => l.ToDto())];
    }

    public async Task<Result<LoanDto>> CreateLoanAsync(CreateLoanDto dto, CancellationToken cancellationToken = default)
    {
        var asset = await assetRepository.GetByIdAsync(dto.AssetId, cancellationToken);
        if (asset is null)
        {
            return Result.Failure<LoanDto>($"Asset with ID {dto.AssetId} not found.");
        }

        var user = await userRepository.GetByIdAsync(dto.BorrowedById, cancellationToken);
        if (user is null)
        {
            return Result.Failure<LoanDto>($"User with ID {dto.BorrowedById} not found.");
        }

        bool checkedOut = false;

        // Check for expired reservation and auto-cancel, or allow checkout from active reservation by the same user
        if (asset.Status == Domain.Enums.AssetStatus.Reserved)
        {
            var reservation = await reservationRepository.GetActiveByAssetIdAsync(asset.Id, cancellationToken);
            if (reservation is not null)
            {
                if (reservation.IsExpired)
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
                        return Result.Failure<LoanDto>(fromReservationResult.Error!);
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
                return Result.Failure<LoanDto>(checkoutResult.Error!);
            }
        }

        var loan = new Loan
        {
            AssetId = asset.Id,
            BorrowedById = user.Id,
            DueDate = dto.DueDate
        };

        await loanRepository.AddAsync(loan, cancellationToken);

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            logger.LogWarning("Concurrency conflict creating loan for asset {AssetId}", dto.AssetId);
            return Result.Failure<LoanDto>("The asset was modified by another user. Please try again.");
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
            return Result.Failure<LoanDto>($"Loan with ID {loanId} not found.");
        }

        var returnResult = loan.MarkReturned();
        if (!returnResult.IsSuccess)
        {
            logger.LogWarning("Failed to return loan {LoanId}: {Error}", loanId, returnResult.Error);
            return Result.Failure<LoanDto>(returnResult.Error!);
        }

        var assetResult = loan.Asset.Return();
        if (!assetResult.IsSuccess)
        {
            logger.LogWarning("Failed to return asset for loan {LoanId}: {Error}", loanId, assetResult.Error);
            return Result.Failure<LoanDto>(assetResult.Error!);
        }

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            logger.LogWarning("Concurrency conflict returning loan {LoanId}", loanId);
            return Result.Failure<LoanDto>("The asset was modified by another user. Please try again.");
        }

        logger.LogInformation("Returned loan {LoanId} for asset {AssetId}", loan.Id, loan.AssetId);

        return Result.Success(loan.ToDto());
    }

    public async Task<PaginatedList<LoanDto>> GetLoansByAssetIdAsync(int assetId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await loanRepository.GetLoansByAssetIdAsync(assetId, page, pageSize, cancellationToken);
        return new PaginatedList<LoanDto>
        {
            Data = [.. items.Select(l => l.ToDto())],
            TotalCount = totalCount,
            PageIndex = page,
            PageSize = pageSize
        };
    }
}
