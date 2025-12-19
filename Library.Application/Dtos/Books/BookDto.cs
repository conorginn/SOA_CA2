namespace Library.Application.Dtos.Books;

public record BookDto(
    int Id,
    string Title,
    string Isbn,
    int AuthorId,
    string AuthorName
);