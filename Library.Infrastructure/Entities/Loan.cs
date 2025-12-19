namespace Library.Infrastructure.Entities;

public class Loan
{
    public int Id { get; set; }

    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public DateTime LoanedAtUtc { get; set; }

    public DateTime DueAtUtc { get; set; }

    public DateTime? ReturnedAtUtc { get; set; }
}