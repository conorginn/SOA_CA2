using Library.Application.Dtos.Authors;

namespace Library.Application.Interfaces;

public interface IAuthorService
{
    Task<List<AuthorDto>> GetAllAsync(CancellationToken ct = default);
    Task<AuthorDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<AuthorDto> CreateAsync(CreateAuthorDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, UpdateAuthorDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}