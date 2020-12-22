using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace UptimeAPI
{
    public class UptimeContext : DbContext
    {
        public UptimeContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Echo> Echo { get; set; }
        public DbSet<EndPoint> EndPoint { get; set; }
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) { }
    }
}
