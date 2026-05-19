using Microsoft.EntityFrameworkCore;
using TodoApi.Models.Entities;

namespace TodoApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Schema definitions
    public DbSet<TodoItem> Todos => Set<TodoItem>();
    public DbSet<BookItem> Books => Set<BookItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}
