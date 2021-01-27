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
    public interface IDatabaseService
    {
        void Create(ResultData result);
        List<EndPoint> GetAll();
        EndPoint Get(Guid id);
        List<EndPoint> FindNewEndpoints();
    }
    public class DatabaseService : IDatabaseService
    {
        private readonly IDbContextFactory<UptimeContext> _contextFactory;

        public DatabaseService(IDbContextFactory<UptimeContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void Create(ResultData result)
        {
            if (Object.Equals(result, null))
                return;

            using var context = _contextFactory.CreateDbContext();
            context.ResultData.Add(result);
            context.SaveChanges();
        }
        public List<EndPoint> FindNewEndpoints()
        {
            using var context = _contextFactory.CreateDbContext();
            var newEndpoint =  context.EndPoint
                .Select(x => x.Id)
                .Except(context.ResultData.Select(y => y.EndPointId))
                .ToList();
            List<EndPoint> endPoints = new List<EndPoint>();
            foreach(Guid id in newEndpoint)
                endPoints.Add(Get(id));

            return endPoints;

        }
        public List<EndPoint> GetAll()
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

