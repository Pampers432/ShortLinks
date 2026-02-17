using Microsoft.EntityFrameworkCore;
using ShortLinks.Models;

namespace ShortLinks.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<ShortLink> ShortLinks => Set<ShortLink>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortLink>()
                .HasIndex(s => s.Code)
                .IsUnique();
        }
    }
}
