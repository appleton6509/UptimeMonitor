using Data;
using Microsoft.EntityFrameworkCore;
using ProcessingService.Models;
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
        List<EndPointExtended> GetAll();
        EndPointExtended Get(Guid id);
        List<EndPointExtended> FindNewEndpoints();
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
        public List<EndPointExtended> FindNewEndpoints()
        {
            using var context = _contextFactory.CreateDbContext();
            var newEndpoint =  context.EndPoint
                .Select(x => x.Id)
                .Except(context.ResultData.Select(y => y.EndPointId))
                .ToList();
            List<EndPointExtended> endPoints = new List<EndPointExtended>();
            foreach(Guid id in newEndpoint)
                endPoints.Add(Get(id));

            return endPoints;

        }
        public List<EndPointExtended> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            var endpoint = from ep in context.EndPoint
                      join ht in context.WebUser
                      on ep.UserId equals ht.Id
                      select new EndPointExtended
                      {
                          Id = ep.Id,
                          Description = ep.Description,
                          Ip = ep.Ip,
                          NotifyOnFailure = ep.NotifyOnFailure,
                          Protocol = ep.Protocol,
                          UserId = ep.UserId,
                          Email = ht.UserName
                      };
            return endpoint.ToList();
        }
        public EndPointExtended Get(Guid id)
        {
            using var context = _contextFactory.CreateDbContext();
            var endpoint = from ep in context.EndPoint
                           join ht in context.WebUser
                           on ep.UserId equals ht.Id
                           where ep.Id == id
                           select new EndPointExtended
                           {
                               Id = ep.Id,
                               Description = ep.Description,
                               Ip = ep.Ip,
                               NotifyOnFailure = ep.NotifyOnFailure,
                               Protocol = ep.Protocol,
                               UserId = ep.UserId,
                               Email = ht.UserName
                           };
            return endpoint.FirstOrDefault();
        }
    }
}

