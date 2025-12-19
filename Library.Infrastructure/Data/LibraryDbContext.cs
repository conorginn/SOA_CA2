using Library.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Library.Infrastructure.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<User> Users => Set<User>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Author
        modelBuilder.Entity<Author>()
            .Property(a => a.Name)
            .HasMaxLength(200);

        // Book
        modelBuilder.Entity<Book>()
            .Property(b => b.Title)
            .HasMaxLength(300);

        modelBuilder.Entity<Book>()
            .Property(b => b.Isbn)
            .HasMaxLength(20);

        modelBuilder.Entity<Book>()
            .HasIndex(b => b.Isbn)
            .IsUnique();

        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Member
        modelBuilder.Entity<Member>()
            .Property(m => m.FullName)
            .HasMaxLength(200);

        modelBuilder.Entity<Member>()
            .Property(m => m.Email)
            .HasMaxLength(320);

        modelBuilder.Entity<Member>()
            .HasIndex(m => m.Email)
            .IsUnique();

        // Loan
        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Member)
            .WithMany(m => m.Loans)
            .HasForeignKey(l => l.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Book)
            .WithMany(b => b.Loans)
            .HasForeignKey(l => l.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
    }
}