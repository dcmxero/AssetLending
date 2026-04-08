using Domain.Models.AssetManagement;
using DTOs.Asset;

namespace Application.Mappers;

/// <summary>
/// Provides mapping extension methods between <see cref="Loan"/> domain entities and loan DTOs.
/// </summary>
public static class LoanMapper
{
    /// <summary>
    /// Maps a <see cref="Loan"/> domain entity to a <see cref="LoanDto"/>.
    /// </summary>
    public static LoanDto ToDto(this Loan loan) => new()
    {
        Id = loan.Id,
        AssetId = loan.AssetId,
        AssetName = loan.Asset?.Name ?? "",
        BorrowedById = loan.BorrowedById,
        BorrowedByName = loan.BorrowedBy?.FullName ?? "",
        BorrowedAt = loan.BorrowedAt,
        DueDate = loan.DueDate,
        ReturnedAt = loan.ReturnedAt,
        Status = loan.Status.ToString()
    };
}
