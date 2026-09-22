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
/// Covers the checkout and return decisions, including the reservation cases that
/// only the integration tests reached before.
/// </summary>
public class LoanServiceTests
{
    private static readonly DateTime Now = new(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc);

    private readonly Mock<ILoanRepository> loans = new();
    private readonly Mock<IAssetRepository> assets = new();
    private readonly Mock<IUserRepository> users = new();
    private readonly Mock<IReservationRepository> reservations = new();
    private readonly Mock<IUnitOfWork> unitOfWork = new();
    private readonly FakeTimeProvider clock = new(Now);

    private readonly Asset asset = new() { Id = 1, Name = "ThinkPad", AssetCategoryId = 1 };
    private readonly User borrower = new() { Id = 2, FirstName = "Jana", LastName = "Novakova", Email = "jana@example.com" };

    private LoanService CreateService() => new(
        loans.Object,
        Mock.Of<ILoanQueries>(),
        assets.Object,
        users.Object,
        reservations.Object,
        unitOfWork.Object,
        clock,
        NullLogger<LoanService>.Instance);

    private void GivenAssetAndBorrowerExist()
    {
        assets.Setup(r => r.GetByIdAsync(asset.Id, It.IsAny<CancellationToken>())).ReturnsAsync(asset);
        users.Setup(r => r.GetByIdAsync(borrower.Id, It.IsAny<CancellationToken>())).ReturnsAsync(borrower);
    }

    private CreateLoanDto Request() => new()
    {
        AssetId = asset.Id,
        BorrowedById = borrower.Id,
        DueDate = Now.AddDays(7)
    };

    [Fact]
    public async Task CreateLoan_StampsBorrowedAtFromTheClock()
    {
        GivenAssetAndBorrowerExist();
        Loan? saved = null;
        loans.Setup(r => r.AddAsync(It.IsAny<Loan>(), It.IsAny<CancellationToken>()))
            .Callback<Loan, CancellationToken>((l, _) => saved = l);

        var result = await CreateService().CreateLoanAsync(Request());

        Assert.True(result.IsSuccess);
        Assert.Equal(Now, saved!.BorrowedAt);
        Assert.Equal(AssetStatus.Loaned, asset.Status);
        unitOfWork.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateLoan_ReportsNotFound_WhenTheAssetIsMissing()
    {
        users.Setup(r => r.GetByIdAsync(borrower.Id, It.IsAny<CancellationToken>())).ReturnsAsync(borrower);

        var result = await CreateService().CreateLoanAsync(Request());

        Assert.Equal(ResultErrorKind.NotFound, result.ErrorKind);
        unitOfWork.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateLoan_ReportsNotFound_WhenTheBorrowerIsMissing()
    {
        assets.Setup(r => r.GetByIdAsync(asset.Id, It.IsAny<CancellationToken>())).ReturnsAsync(asset);

        var result = await CreateService().CreateLoanAsync(Request());

        Assert.Equal(ResultErrorKind.NotFound, result.ErrorKind);
    }

    [Fact]
    public async Task CreateLoan_ReportsRuleViolation_WhenTheAssetIsAlreadyLoaned()
    {
        GivenAssetAndBorrowerExist();
        asset.Status = AssetStatus.Loaned;

        var result = await CreateService().CreateLoanAsync(Request());

        Assert.Equal(ResultErrorKind.RuleViolation, result.ErrorKind);
        unitOfWork.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateLoan_CheckoutsOutFromTheBorrowersOwnReservation()
    {
        GivenAssetAndBorrowerExist();
        asset.Status = AssetStatus.Reserved;
        var reservation = GivenReservation(heldBy: borrower.Id, until: Now.AddDays(2));

        var result = await CreateService().CreateLoanAsync(Request());

        Assert.True(result.IsSuccess);
        Assert.True(reservation.IsCancelled);
        Assert.Equal(AssetStatus.Loaned, asset.Status);
    }

    [Fact]
    public async Task CreateLoan_RefusesAReservationHeldBySomebodyElse()
    {
        GivenAssetAndBorrowerExist();
        asset.Status = AssetStatus.Reserved;
        var reservation = GivenReservation(heldBy: 99, until: Now.AddDays(2));

        var result = await CreateService().CreateLoanAsync(Request());

        Assert.Equal(ResultErrorKind.RuleViolation, result.ErrorKind);
        Assert.False(reservation.IsCancelled);
    }

    [Fact]
    public async Task CreateLoan_ClearsAReservationThatHasRunOut()
    {
        GivenAssetAndBorrowerExist();
        asset.Status = AssetStatus.Reserved;
        var reservation = GivenReservation(heldBy: 99, until: Now.AddDays(2));

        clock.Advance(TimeSpan.FromDays(3));

        var result = await CreateService().CreateLoanAsync(Request());

        Assert.True(result.IsSuccess);
        Assert.True(reservation.IsCancelled);
        Assert.Equal(AssetStatus.Loaned, asset.Status);
    }

    [Fact]
    public async Task ReturnAsset_StampsReturnedAtFromTheClock()
    {
        var loan = new Loan { Id = 5, AssetId = asset.Id, BorrowedById = borrower.Id, Asset = asset, BorrowedBy = borrower, BorrowedAt = Now, DueDate = Now.AddDays(7) };
        asset.Status = AssetStatus.Loaned;
        loans.Setup(r => r.GetByIdAsync(loan.Id, It.IsAny<CancellationToken>())).ReturnsAsync(loan);

        clock.Advance(TimeSpan.FromDays(4));

        var result = await CreateService().ReturnAssetAsync(loan.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(Now.AddDays(4), loan.ReturnedAt);
        Assert.Equal(AssetStatus.Available, asset.Status);
    }

    [Fact]
    public async Task ReturnAsset_ReportsConcurrencyConflict_WhenTheSaveLosesTheRace()
    {
        var loan = new Loan { Id = 5, AssetId = asset.Id, BorrowedById = borrower.Id, Asset = asset, BorrowedBy = borrower, BorrowedAt = Now, DueDate = Now.AddDays(7) };
        asset.Status = AssetStatus.Loaned;
        loans.Setup(r => r.GetByIdAsync(loan.Id, It.IsAny<CancellationToken>())).ReturnsAsync(loan);
        unitOfWork.Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConcurrencyConflictException(new InvalidOperationException()));

        var result = await CreateService().ReturnAssetAsync(loan.Id);

        Assert.Equal(ResultErrorKind.ConcurrencyConflict, result.ErrorKind);
    }

    private Reservation GivenReservation(int heldBy, DateTime until)
    {
        var reservation = new Reservation
        {
            Id = 3,
            AssetId = asset.Id,
            ReservedById = heldBy,
            ReservedAt = Now,
            ReservedUntil = until
        };

        reservations
            .Setup(r => r.GetActiveByAssetIdAsync(asset.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);

        return reservation;
    }
}
