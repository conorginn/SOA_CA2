using FluentAssertions;
using Library.Application.Dtos.Loans;
using Library.Infrastructure.Entities;
using Library.Infrastructure.Services;
using Library.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Library.Tests;

public class LoanServiceTests
{
    private static async Task<(int bookId, int memberId)> SeedBookAndMemberAsync(SqliteInMemoryDb db)
    {
        var author = new Author { Name = "Test Author" };
        db.DbContext.Authors.Add(author);
        await db.DbContext.SaveChangesAsync();

        var book = new Book { Title = "Test Book", Isbn = "TEST-ISBN-1", AuthorId = author.Id };
        var member = new Member { FullName = "Test Member", Email = "member@test.com" };

        db.DbContext.Books.Add(book);
        db.DbContext.Members.Add(member);
        await db.DbContext.SaveChangesAsync();

        return (book.Id, member.Id);
    }

    [Fact]
    public async Task CreateAsync_ReturnsNull_WhenBookOrMemberDoesNotExist()
    {
        using var db = new SqliteInMemoryDb();
        var service = new LoanService(db.DbContext);

        var created = await service.CreateAsync(new CreateLoanDto(BookId: 9999, MemberId: 9999, LoanDays: 14));

        created.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenBookAlreadyOnActiveLoan()
    {
        using var db = new SqliteInMemoryDb();
        var (bookId, memberId) = await SeedBookAndMemberAsync(db);

        var service = new LoanService(db.DbContext);

        var dto = new CreateLoanDto(BookId: bookId, MemberId: memberId, LoanDays: 14);

        var first = await service.CreateAsync(dto);
        first.Should().NotBeNull();

        Func<Task> act = async () => await service.CreateAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("That book is already on loan.");
    }

    [Fact]
    public async Task ReturnAsync_ReturnsFalse_WhenLoanNotFound()
    {
        using var db = new SqliteInMemoryDb();
        var service = new LoanService(db.DbContext);

        var ok = await service.ReturnAsync(id: 12345, returnedAtUtc: DateTime.UtcNow);

        ok.Should().BeFalse();
    }

    [Fact]
    public async Task ReturnAsync_Throws_WhenAlreadyReturned()
    {
        using var db = new SqliteInMemoryDb();
        var (bookId, memberId) = await SeedBookAndMemberAsync(db);

        var service = new LoanService(db.DbContext);

        var created = await service.CreateAsync(new CreateLoanDto(bookId, memberId, 14));
        created.Should().NotBeNull();

        (await service.ReturnAsync(created!.Id, DateTime.UtcNow)).Should().BeTrue();

        Func<Task> act = async () => await service.ReturnAsync(created.Id, DateTime.UtcNow);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Loan is already returned.");
    }

    [Fact]
    public async Task ReturnAsync_AllowsRelending_AfterReturn()
    {
        using var db = new SqliteInMemoryDb();
        var (bookId, memberId) = await SeedBookAndMemberAsync(db);

        var service = new LoanService(db.DbContext);

        var created = await service.CreateAsync(new CreateLoanDto(bookId, memberId, 14));
        created.Should().NotBeNull();

        (await service.ReturnAsync(created!.Id, DateTime.UtcNow)).Should().BeTrue();

        var second = await service.CreateAsync(new CreateLoanDto(bookId, memberId, 7));
        second.Should().NotBeNull();
        second!.Id.Should().NotBe(created.Id);

        (await db.DbContext.Loans.CountAsync(l => l.BookId == bookId)).Should().Be(2);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenLoanNotFound()
    {
        using var db = new SqliteInMemoryDb();
        var service = new LoanService(db.DbContext);

        (await service.DeleteAsync(9999)).Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_DeletesLoan_WhenExists()
    {
        using var db = new SqliteInMemoryDb();
        var (bookId, memberId) = await SeedBookAndMemberAsync(db);

        var service = new LoanService(db.DbContext);

        var created = await service.CreateAsync(new CreateLoanDto(bookId, memberId, 14));
        created.Should().NotBeNull();

        (await service.DeleteAsync(created!.Id)).Should().BeTrue();

        (await db.DbContext.Loans.AnyAsync(l => l.Id == created.Id)).Should().BeFalse();
    }
}