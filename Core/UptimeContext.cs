using Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class UptimeContext : IdentityDbContext
    {
        public UptimeContext(DbContextOptions options) : base(options) { }
        public DbSet<Echo> Echo { get; set; }
        public DbSet<EndPoint> EndPoint { get; set; }
        public DbSet<User> User { get; set; }
    }
}
