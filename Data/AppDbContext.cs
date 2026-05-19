using Microsoft.EntityFrameworkCore;
using TodoApi.Models.Entities;

namespace TodoApi.Data;

/// <summary>Entity Framework Core database context for the TodoApi application.</summary>
public class AppDbContext : DbContext
{
    /// <summary>Initializes a new <see cref="AppDbContext"/> with the given options.</summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>Gets or sets the Todos table.</summary>
    public DbSet<TodoItem> Todos => Set<TodoItem>();

    /// <summary>Gets or sets the Books table.</summary>
    public DbSet<BookItem> Books => Set<BookItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Create an index on CreatedAt for efficient date-based querying of todo items.
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}
