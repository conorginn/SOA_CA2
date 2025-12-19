namespace Library.Application.Dtos.Loans;

public record LoanDto(
    int Id,
    int BookId,
    string BookTitle,
    int MemberId,
    string MemberName,
    DateTime LoanedAtUtc,
    DateTime DueAtUtc,
    DateTime? ReturnedAtUtc
);