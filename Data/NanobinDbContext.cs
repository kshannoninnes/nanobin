using Microsoft.EntityFrameworkCore;
using Nanobin.Components.Models;

namespace Nanobin.Data;

public class NanobinDbContext(DbContextOptions<NanobinDbContext> options) : DbContext(options)
{
    public DbSet<Paste> Pastes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Paste>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(18).ValueGeneratedNever();
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });
    }
}
