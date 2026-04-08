using Domain.Enums;
using Domain.Models.AssetManagement;
using Xunit;

namespace WebApi.Tests;

public class LoanTests
{
    private static Loan CreateActiveLoan() => new()
    {
        Id = 1,
        AssetId = 1,
        BorrowedById = 1,
        DueDate = DateTime.UtcNow.AddDays(7),
        Status = LoanStatus.Active
    };

    [Fact]
    public void MarkReturned_SetsStatusToReturned_WhenLoanIsActive()
    {
        // Arrange
        var loan = CreateActiveLoan();

        // Act
        var result = loan.MarkReturned();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(LoanStatus.Returned, loan.Status);
        Assert.NotNull(loan.ReturnedAt);
    }

    [Fact]
    public void MarkReturned_ReturnsFailure_WhenLoanIsAlreadyReturned()
    {
        // Arrange
        var loan = CreateActiveLoan();
        loan.MarkReturned();

        // Act
        var result = loan.MarkReturned();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void NewLoan_HasActiveStatus_ByDefault()
    {
        // Arrange & Act
        var loan = new Loan
        {
            AssetId = 1,
            BorrowedById = 1,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        // Assert
        Assert.Equal(LoanStatus.Active, loan.Status);
        Assert.Null(loan.ReturnedAt);
    }
}
