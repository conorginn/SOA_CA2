namespace Library.Api.Entities;

public class Member
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    // Navigation: 1 member -> many loans
    public List<Loan> Loans { get; set; } = new();
}