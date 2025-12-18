namespace Library.Api.Entities;

public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Isbn { get; set; } = null!;

    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;

    // Navigation: 1 book -> many loans (over time)
    public List<Loan> Loans { get; set; } = new();
}