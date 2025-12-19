using Library.Application.Dtos.Authors;
using Library.Application.Interfaces;
using Library.Infrastructure.Data;
using Library.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Services;

public class AuthorService : IAuthorService
{
    private readonly LibraryDbContext _db;

    public AuthorService(LibraryDbContext db)
    {
        _db = db;
    }

    public async Task<List<AuthorDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Authors
            .AsNoTracking()
            .OrderBy(a => a.Name)
            .Select(a => new AuthorDto(a.Id, a.Name))
            .ToListAsync(ct);
    }

    public async Task<AuthorDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Authors
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AuthorDto(a.Id, a.Name))
            .SingleOrDefaultAsync(ct);
    }

    public async Task<AuthorDto> CreateAsync(CreateAuthorDto dto, CancellationToken ct = default)
    {
        var author = new Author
        {
            Name = dto.Name.Trim()
        };

        _db.Authors.Add(author);
        await _db.SaveChangesAsync(ct);

        return new AuthorDto(author.Id, author.Name);
    }

    public async Task<bool> UpdateAsync(int id, UpdateAuthorDto dto, CancellationToken ct = default)
    {
        var author = await _db.Authors.SingleOrDefaultAsync(a => a.Id == id, ct);
        if (author is null) return false;

        author.Name = dto.Name.Trim();
        await _db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var author = await _db.Authors.SingleOrDefaultAsync(a => a.Id == id, ct);
        if (author is null) return false;

        _db.Authors.Remove(author);
        await _db.SaveChangesAsync(ct);

        return true;
    }
}