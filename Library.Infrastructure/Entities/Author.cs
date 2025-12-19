namespace Library.Infrastructure.Entities;

public class Author
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    // Navigation: 1 author -> many books
    public List<Book> Books { get; set; } = new();
}