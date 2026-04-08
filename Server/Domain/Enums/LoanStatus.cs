namespace Domain.Enums;

/// <summary>
/// Represents the status of a loan.
/// </summary>
public enum LoanStatus
{
    /// <summary>
    /// Loan is currently active — asset has not been returned yet.
    /// </summary>
    Active,

    /// <summary>
    /// Loan has been completed — asset was returned.
    /// </summary>
    Returned
}
