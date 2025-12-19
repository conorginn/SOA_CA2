namespace Library.Application.Dtos.Loans;

public record CreateLoanDto(
    int BookId,
    int MemberId,
    int? LoanDays
);