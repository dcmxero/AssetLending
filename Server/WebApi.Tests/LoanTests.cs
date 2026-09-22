using Domain.Enums;
using Domain.Models.AssetManagement;
using Xunit;

namespace WebApi.Tests;

public class LoanTests
{
    private static readonly DateTime Now = new(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc);

    private static Loan CreateActiveLoan() => new()
    {
        Id = 1,
        AssetId = 1,
        BorrowedById = 1,
        BorrowedAt = Now,
        DueDate = Now.AddDays(7),
        Status = LoanStatus.Active
    };

    [Fact]
    public void MarkReturned_SetsStatusToReturned_WhenLoanIsActive()
    {
        // Arrange
        var loan = CreateActiveLoan();

        // Act
        var result = loan.MarkReturned(Now.AddDays(3));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(LoanStatus.Returned, loan.Status);
        Assert.Equal(Now.AddDays(3), loan.ReturnedAt);
    }

    [Fact]
    public void MarkReturned_ReturnsFailure_WhenLoanIsAlreadyReturned()
    {
        // Arrange
        var loan = CreateActiveLoan();
        loan.MarkReturned(Now.AddDays(3));

        // Act
        var result = loan.MarkReturned(Now.AddDays(4));

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void MarkReturned_LeavesTheEarlierReturnTimestampUntouched()
    {
        // Arrange
        var loan = CreateActiveLoan();
        loan.MarkReturned(Now.AddDays(3));

        // Act
        loan.MarkReturned(Now.AddDays(4));

        // Assert
        Assert.Equal(Now.AddDays(3), loan.ReturnedAt);
    }

    [Fact]
    public void NewLoan_HasActiveStatus_ByDefault()
    {
        // Arrange & Act
        var loan = new Loan
        {
            AssetId = 1,
            BorrowedById = 1,
            DueDate = Now.AddDays(7)
        };

        // Assert
        Assert.Equal(LoanStatus.Active, loan.Status);
        Assert.Null(loan.ReturnedAt);
    }
}
