using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Data
{
    public class UptimeContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public UptimeContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ResultData> ResultData { get; set; }
        public DbSet<EndPoint> EndPoint { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }



    }
}
