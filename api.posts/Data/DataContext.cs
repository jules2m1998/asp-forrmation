using api.posts.Models;
using Microsoft.EntityFrameworkCore;

namespace api.posts.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; init; }
    public DbSet<Post> Posts { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity => { entity.HasIndex(e => e.Email).IsUnique(); });
        modelBuilder.Entity<Post>()
            .HasOne(e => e.Parent)
            .WithMany(e => e.Posts)
            .OnDelete(DeleteBehavior.Cascade);
    }
}