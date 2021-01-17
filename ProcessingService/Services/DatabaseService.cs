using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessingService.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IDbContextFactory<UptimeContext> _contextFactory;

        public DatabaseService(IDbContextFactory<UptimeContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void Create(HttpResult result)
        {
            if (Object.Equals(result, null))
                return;

            using var context = _contextFactory.CreateDbContext();
            context.HttpResult.Add(result);
            context.SaveChanges();
        }

        public List<EndPoint> Get()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.EndPoint.ToList();
        }
    }
}
