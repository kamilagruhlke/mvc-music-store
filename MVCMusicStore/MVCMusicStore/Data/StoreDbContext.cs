using Microsoft.EntityFrameworkCore;
using MVCMusicStore.Models;

namespace MVCMusicStore.Data
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Artist> Artists { get; set; }
    }

}
