using Library.Application.Dtos.Loans;

namespace Library.Application.Interfaces;

public interface ILoanService
{
    Task<List<LoanDto>> GetAllAsync(bool? activeOnly = null, CancellationToken ct = default);
    Task<LoanDto?> GetByIdAsync(int id, CancellationToken ct = default);

    /// Creates a loan. Returns null if Member/Book not found. Throws InvalidOperationException if book already on loan.
    Task<LoanDto?> CreateAsync(CreateLoanDto dto, CancellationToken ct = default);

    /// Returns true if returned, false if not found. Throws InvalidOperationException if already returned.
    Task<bool> ReturnAsync(int id, DateTime? returnedAtUtc, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}