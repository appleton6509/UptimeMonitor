using Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class UptimeContext : IdentityDbContext
    {
        public UptimeContext(DbContextOptions options) : base(options) { }
        public DbSet<ResultData> ResultData { get; set; }
        public DbSet<EndPoint> EndPoint { get; set; }
        public DbSet<WebUser> WebUser { get; set; }

    }
}
