namespace Library.Application.Dtos.Books;

public record UpdateBookDto(
    string Title,
    string Isbn,
    int AuthorId
);