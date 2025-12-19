using Library.Application.Dtos.Books;
using Library.Application.Interfaces;
using Library.Infrastructure.Data;
using Library.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Services;

public class BookService : IBookService
{
    private readonly LibraryDbContext _db;

    public BookService(LibraryDbContext db)
    {
        _db = db;
    }

    public async Task<List<BookDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Books
            .AsNoTracking()
            .Include(b => b.Author)
            .OrderBy(b => b.Title)
            .Select(b => new BookDto(
                b.Id,
                b.Title,
                b.Isbn,
                b.AuthorId,
                b.Author.Name
            ))
            .ToListAsync(ct);
    }

    public async Task<BookDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Books
            .AsNoTracking()
            .Include(b => b.Author)
            .Where(b => b.Id == id)
            .Select(b => new BookDto(
                b.Id,
                b.Title,
                b.Isbn,
                b.AuthorId,
                b.Author.Name
            ))
            .SingleOrDefaultAsync(ct);
    }

    public async Task<List<BookDto>> GetByAuthorIdAsync(int authorId, CancellationToken ct = default)
    {
        return await _db.Books
            .AsNoTracking()
            .Include(b => b.Author)
            .Where(b => b.AuthorId == authorId)
            .OrderBy(b => b.Title)
            .Select(b => new BookDto(
                b.Id,
                b.Title,
                b.Isbn,
                b.AuthorId,
                b.Author.Name
            ))
            .ToListAsync(ct);
    }

    public async Task<BookDto?> CreateAsync(CreateBookDto dto, CancellationToken ct = default)
    {
        // Validate Author exists
        var authorExists = await _db.Authors.AnyAsync(a => a.Id == dto.AuthorId, ct);
        if (!authorExists) return null;

        var book = new Book
        {
            Title = dto.Title.Trim(),
            Isbn = dto.Isbn.Trim(),
            AuthorId = dto.AuthorId
        };

        _db.Books.Add(book);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // Likely ISBN unique constraint violation
            throw;
        }

        // Return the created dto including AuthorName
        var created = await GetByIdAsync(book.Id, ct);
        return created;
    }

    public async Task<bool> UpdateAsync(int id, UpdateBookDto dto, CancellationToken ct = default)
    {
        var book = await _db.Books.SingleOrDefaultAsync(b => b.Id == id, ct);
        if (book is null) return false;

        var authorExists = await _db.Authors.AnyAsync(a => a.Id == dto.AuthorId, ct);
        if (!authorExists) return false;

        book.Title = dto.Title.Trim();
        book.Isbn = dto.Isbn.Trim();
        book.AuthorId = dto.AuthorId;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var book = await _db.Books.SingleOrDefaultAsync(b => b.Id == id, ct);
        if (book is null) return false;

        _db.Books.Remove(book);
        await _db.SaveChangesAsync(ct);

        return true;
    }
}