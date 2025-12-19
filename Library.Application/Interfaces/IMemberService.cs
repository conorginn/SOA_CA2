using Library.Application.Dtos.Members;

namespace Library.Application.Interfaces;

public interface IMemberService
{
    Task<List<MemberDto>> GetAllAsync(CancellationToken ct = default);
    Task<MemberDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<MemberDto> CreateAsync(CreateMemberDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, UpdateMemberDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}