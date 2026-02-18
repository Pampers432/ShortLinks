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
            // ShortCode уникальный, т.к. это публичный идентификатор ссылки.
            // Если не задать уникальность — возможны коллизии и редиректы на неправильные URL.
            modelBuilder.Entity<ShortLink>()
                .HasIndex(s => s.Code)
                .IsUnique();
        }
    }
}
