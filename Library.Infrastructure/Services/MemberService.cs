using Library.Application.Dtos.Members;
using Library.Application.Interfaces;
using Library.Infrastructure.Data;
using Library.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Services;

public class MemberService : IMemberService
{
    private readonly LibraryDbContext _db;

    public MemberService(LibraryDbContext db)
    {
        _db = db;
    }

    public async Task<List<MemberDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Members
            .AsNoTracking()
            .OrderBy(m => m.FullName)
            .Select(m => new MemberDto(m.Id, m.FullName, m.Email))
            .ToListAsync(ct);
    }

    public async Task<MemberDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Members
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => new MemberDto(m.Id, m.FullName, m.Email))
            .SingleOrDefaultAsync(ct);
    }

    public async Task<MemberDto> CreateAsync(CreateMemberDto dto, CancellationToken ct = default)
    {
        var member = new Member
        {
            FullName = dto.FullName.Trim(),
            Email = dto.Email.Trim()
        };

        _db.Members.Add(member);
        await _db.SaveChangesAsync(ct);

        return new MemberDto(member.Id, member.FullName, member.Email);
    }

    public async Task<bool> UpdateAsync(int id, UpdateMemberDto dto, CancellationToken ct = default)
    {
        var member = await _db.Members.SingleOrDefaultAsync(m => m.Id == id, ct);
        if (member is null) return false;

        member.FullName = dto.FullName.Trim();
        member.Email = dto.Email.Trim();

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var member = await _db.Members.SingleOrDefaultAsync(m => m.Id == id, ct);
        if (member is null) return false;

        _db.Members.Remove(member);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}