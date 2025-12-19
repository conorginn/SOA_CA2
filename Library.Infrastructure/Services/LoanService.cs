using Library.Application.Dtos.Loans;
using Library.Application.Interfaces;
using Library.Infrastructure.Data;
using Library.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Services;

public class LoanService : ILoanService
{
    private readonly LibraryDbContext _db;

    public LoanService(LibraryDbContext db)
    {
        _db = db;
    }

    public async Task<List<LoanDto>> GetAllAsync(bool? activeOnly = null, CancellationToken ct = default)
    {
        var q = _db.Loans
            .AsNoTracking()
            .Include(l => l.Book)
            .Include(l => l.Member)
            .AsQueryable();

        if (activeOnly == true)
            q = q.Where(l => l.ReturnedAtUtc == null);
        else if (activeOnly == false)
            q = q.Where(l => l.ReturnedAtUtc != null);

        return await q
            .OrderByDescending(l => l.LoanedAtUtc)
            .Select(l => new LoanDto(
                l.Id,
                l.BookId,
                l.Book.Title,
                l.MemberId,
                l.Member.FullName,
                l.LoanedAtUtc,
                l.DueAtUtc,
                l.ReturnedAtUtc
            ))
            .ToListAsync(ct);
    }

    public async Task<LoanDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Loans
            .AsNoTracking()
            .Include(l => l.Book)
            .Include(l => l.Member)
            .Where(l => l.Id == id)
            .Select(l => new LoanDto(
                l.Id,
                l.BookId,
                l.Book.Title,
                l.MemberId,
                l.Member.FullName,
                l.LoanedAtUtc,
                l.DueAtUtc,
                l.ReturnedAtUtc
            ))
            .SingleOrDefaultAsync(ct);
    }

    public async Task<LoanDto?> CreateAsync(CreateLoanDto dto, CancellationToken ct = default)
    {
        var memberExists = await _db.Members.AnyAsync(m => m.Id == dto.MemberId, ct);
        var bookExists = await _db.Books.AnyAsync(b => b.Id == dto.BookId, ct);
        if (!memberExists || !bookExists) return null;

        var alreadyOnLoan = await _db.Loans.AnyAsync(
            l => l.BookId == dto.BookId && l.ReturnedAtUtc == null,
            ct);

        if (alreadyOnLoan)
            throw new InvalidOperationException("That book is already on loan.");

        var now = DateTime.UtcNow;

        var days = dto.LoanDays.GetValueOrDefault(14);
        if (days <= 0 || days > 60) days = 14;

        var loan = new Loan
        {
            BookId = dto.BookId,
            MemberId = dto.MemberId,
            LoanedAtUtc = now,
            DueAtUtc = now.AddDays(days),
            ReturnedAtUtc = null
        };

        _db.Loans.Add(loan);
        await _db.SaveChangesAsync(ct);

        return await GetByIdAsync(loan.Id, ct);
    }

    public async Task<bool> ReturnAsync(int id, DateTime? returnedAtUtc, CancellationToken ct = default)
    {
        var loan = await _db.Loans.SingleOrDefaultAsync(l => l.Id == id, ct);
        if (loan is null) return false;

        if (loan.ReturnedAtUtc is not null)
            throw new InvalidOperationException("Loan is already returned.");

        loan.ReturnedAtUtc = returnedAtUtc ?? DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var loan = await _db.Loans.SingleOrDefaultAsync(l => l.Id == id, ct);
        if (loan is null) return false;

        _db.Loans.Remove(loan);
        await _db.SaveChangesAsync(ct);

        return true;
    }
}