using Microsoft.EntityFrameworkCore;
using MvcLibrary.Models;

namespace MvcLibrary.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; } = default!;
    public DbSet<Volume> Volumes { get; set; } = default!;
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Borrowing> Borrowings { get; set; } = default!;
}
