using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<List<EndPoint>> FindNewEndpoints()
        {
            using var context = _contextFactory.CreateDbContext();
            var newEndpoint = await context.EndPoint
                .Select(x => x.Id)
                .Except(context.HttpResult.Select(y => y.EndPointId))
                .ToListAsync();
            List<EndPoint> endPoints = new List<EndPoint>();
            foreach(Guid id in newEndpoint)
                endPoints.Add(Get(id));

            return endPoints;

        }
        public List<EndPoint> Get()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.EndPoint.ToList();
        }
        public EndPoint Get(Guid id)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.EndPoint.Find(id);
        }
    }
}

