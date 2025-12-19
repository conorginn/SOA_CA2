using Library.Application.Dtos.Books;

namespace Library.Application.Interfaces;

public interface IBookService
{
    Task<List<BookDto>> GetAllAsync(CancellationToken ct = default);
    Task<BookDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<BookDto?> CreateAsync(CreateBookDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, UpdateBookDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    // Relationship endpoint
    Task<List<BookDto>> GetByAuthorIdAsync(int authorId, CancellationToken ct = default);
}