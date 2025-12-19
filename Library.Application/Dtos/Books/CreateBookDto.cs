namespace Library.Application.Dtos.Books;

public record CreateBookDto(
    string Title,
    string Isbn,
    int AuthorId
);