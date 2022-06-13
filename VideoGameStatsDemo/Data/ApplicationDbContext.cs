using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VideoGameStatsDemo.Models;

namespace VideoGameStatsDemo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<VideoGameStatsDemo.Models.Game>? Game { get; set; }
        public DbSet<VideoGameStatsDemo.Models.GameConsole>? GameConsole { get; set; }
    }
}