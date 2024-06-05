using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeWorkDelivery.API.Models
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {

        public DbSet<HomeWork> HomeWorks { get; set; }
        public DbSet<HomeWorkStep> HomeWorkSteps { get; set; }
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
