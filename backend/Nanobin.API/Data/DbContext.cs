using Microsoft.EntityFrameworkCore;
using Nanobin.API.Model;

namespace Nanobin.API.Data;

public sealed class NanobinDbContext(DbContextOptions<NanobinDbContext> options) : DbContext(options)
{
    public DbSet<Paste> Pastes => Set<Paste>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Paste>()
            .HasIndex(p => p.ExpiresAtUtc);
    }
}