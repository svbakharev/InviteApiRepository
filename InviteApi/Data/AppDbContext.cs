using Microsoft.EntityFrameworkCore;

namespace InviteApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Invite> Invites { get; set; }
    }
}
