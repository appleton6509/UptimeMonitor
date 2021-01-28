using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UptimeAPI.Controllers.DTOs;
using UptimeAPI.Controllers.Repositories;

namespace Data.Repositories
{
    public class EndPointRepository : BaseRepository, IEndPointRepository
    {

        public EndPointRepository(UptimeContext context, IHttpContextAccessor httpcontext) : base(context, httpcontext) { }


        #region CRUD 
        public  EndPoint Get(Guid id)
        {
            return _context.EndPoint.Find(id); ;
        }

        public List<EndPoint> GetAll()
        {
            return  _context.EndPoint.Where(x => x.UserId.Equals(UserId())).ToList();
        }

        public Task<int> PostAsync(EndPoint model)
        {
            model.UserId = UserId();
            _context.EndPoint.Add(model);
            return _context.SaveChangesAsync();
        }

        public Task<int> PutAsync(Guid id, EndPoint model)
        {
            var ep = _context.EndPoint.Find(id);
            if (ep is null || id != model.Id)
            {
                throw new BadHttpRequestException("Site id not found");
            }
            _context.Entry(ep).CurrentValues.SetValues(model);
            return _context.SaveChangesAsync();
        }
        #endregion

        public  List<EndPointOfflineDTO> GetOfflineEndPoints()
        {
            var query =
                from ep in _context.EndPoint
                join ht in _context.ResultData
                on ep.Id equals ht.EndPointId
                where ep.UserId == UserId()
                orderby ht.TimeStamp descending
                select new EndPointOfflineDTO()
                {
                    Ip = ep.Ip,
                    IsReachable = ht.IsReachable,
                    Description = ep.Description,
                    Id = ep.Id
                };
            var totalEndPoints = _context.EndPoint.Where(x => x.UserId == UserId()).Select(x => x.Id).Distinct().Count();
            return  query.Take(totalEndPoints).Where(x => !x.IsReachable).Distinct().ToList();
        }
        public List<EndPointOfflineOnlineDTO> GetEndPointsStatus()
        {
            var query =
                from ep in _context.EndPoint
                join ht in _context.ResultData
                on ep.Id equals ht.EndPointId
                where ep.UserId == UserId()
                orderby ht.TimeStamp descending
                select new EndPointOfflineOnlineDTO()
                {
                    Ip = ep.Ip,
                    TimeStamp = ht.TimeStamp,
                    IsReachable = ht.IsReachable
                };
            var totalEndPoints = _context.EndPoint.Where(x => x.UserId == UserId()).Select(x => x.Id).Distinct().Count();
            return query.Take(totalEndPoints).ToList();
        }
        public EndPointStatisticsDTO GetEndPointStatistics(EndPoint endPoint)
        {
            var query = from ep in _context.EndPoint
                        join ht in _context.ResultData
                        on ep.Id equals ht.EndPointId
                        where ep.Id == endPoint.Id
                        orderby ht.TimeStamp descending
                        select ht;
            if (!query.Any())
            {
                var endpoint = _context.EndPoint.Find(endPoint.Id);
                if (!Object.Equals(endpoint,null))
                {
                    return new EndPointStatisticsDTO()
                    {
                        AverageLatency = 0,
                        LastDownTime = null,
                        LastSeen = null,
                        Ip = endPoint.Ip,
                        IsReachable = false,
                        Description = endPoint.Description,
                        Id = endPoint.Id,
                        Protocol = endPoint.Protocol
                    };
                }
            }

            DateTime timeNow = DateTime.UtcNow.AddMinutes(-15);
            double? avgLatency = null;
            bool hasRecentLatency = query.Any(x => x.TimeStamp > timeNow);
            if (hasRecentLatency)
                avgLatency = query.Where(x => x.TimeStamp > timeNow).Select(x => x.Latency).Average();
            bool? isOnline = query.FirstOrDefaultAsync(x => x.IsReachable == true)?.Result?.IsReachable;

            DateTime? lastDownTime = query.FirstOrDefault(x => x.IsReachable == false)?.TimeStamp;
            DateTime? lastSeen = query.FirstOrDefault(x => x.IsReachable == true)?.TimeStamp;
            EndPointStatisticsDTO data = new EndPointStatisticsDTO()
            {
                AverageLatency = avgLatency != null ? Convert.ToInt32(Math.Round((double)avgLatency, 0)) : 0,
                LastDownTime = lastDownTime,
                LastSeen = lastSeen,
                Ip = endPoint.Ip,
                IsReachable = isOnline ?? false,
                Description = endPoint.Description,
                Id = endPoint.Id,
                Protocol = endPoint.Protocol
            };
            return data;
        }
        public List<EndPointStatisticsDTO> GetEndPointStatistics()
        {
            var endPoints = (from ep in _context.EndPoint
                             where ep.UserId == UserId()
                             select ep).ToList();
            List<EndPointStatisticsDTO> stats = new List<EndPointStatisticsDTO>();

            endPoints.ForEach(x =>
            {
                var epstat = GetEndPointStatistics(x);
                stats.Add(epstat);
            });
            return stats;
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            var model = _context.EndPoint.Find(id);
            _context.Remove(model);
            return await _context.SaveChangesAsync();
        }

        public bool Exists(Guid id)
        {
            return _context.EndPoint.Any(x => x.Id == id);
        }
    }
}
