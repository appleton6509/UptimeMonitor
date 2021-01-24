using AutoMapper;
using Data;
using Data.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UptimeAPI.Controllers.DTOs;
using UptimeAPI.Controllers.Extensions;

namespace UptimeAPI.Controllers.Repositories.CacheRepositories
{
    public class EndPointRepositoryCacheDecorator : BaseRepository, IEndPointRepository
    {
        private readonly IMemoryCache _cache;
        private readonly IEndPointRepository _repo;

        public EndPointRepositoryCacheDecorator(
            UptimeContext context,
            IHttpContextAccessor httpcontext,
            EndPointRepository repo,
            IMemoryCache cache,
            IMapper mapper = null) : base(context, httpcontext, mapper)
        {
            _repo = repo;
            _cache = cache;
        }

        public void Delete(Guid id)
        {
            string key = $"{nameof(EndPointRepositoryCacheDecorator)}{nameof(GetAll)}";
            _cache.Remove(key);
            _repo.Delete(id);
        }

        public bool Exists(Guid id)
        {
            return _repo.Exists(id);
        }

        public EndPoint Get(Guid id)
        {
            return _repo.Get(id);
        }

        public List<EndPoint> GetAll()
        {
            string key = $"{nameof(EndPointRepositoryCacheDecorator)}{nameof(GetAll)}";
            return _cache.GetOrCreate(key, x => { return _repo.GetAll(); });
        }

        public List<EndPointOfflineOnlineDTO> GetEndPointsStatus()
        {
            string key = $"{nameof(EndPointRepositoryCacheDecorator)}{nameof(GetEndPointsStatus)}";
            return _cache.GetOrCreate(key, x => { return _repo.GetEndPointsStatus(); });
        }

        public EndPointStatisticsDTO GetEndPointStatistics(EndPoint endPoint)
        {
            string key = $"{nameof(EndPointRepositoryCacheDecorator)}{nameof(GetEndPointStatistics)}{endPoint.Id}";
            return _cache.GetOrCreate(key, x => { return _repo.GetEndPointStatistics(endPoint); });
        }

        public List<EndPointStatisticsDTO> GetEndPointStatistics()
        {
            string key = $"{nameof(EndPointRepositoryCacheDecorator)}{nameof(GetEndPointStatistics)}";
            return _cache.GetOrCreate(key, x => { return _repo.GetEndPointStatistics(); });
        }

        public List<EndPointOfflineDTO> GetOfflineEndPoints()
        {
            string key = $"{nameof(EndPointRepositoryCacheDecorator)}{nameof(GetOfflineEndPoints)}";
            return _cache.GetOrCreate(key, x => { return _repo.GetOfflineEndPoints(); });
        }

        public Task<int> PostAsync(EndPoint model)
        {
            string key = $"{nameof(EndPointRepositoryCacheDecorator)}{nameof(GetAll)}";
            _cache.Remove(key);
            return _repo.PostAsync(model);
        }

        public Task<int> PutAsync(Guid id, EndPoint model)
        {
            string key = $"{nameof(EndPointRepositoryCacheDecorator)}{nameof(GetAll)}";
            _cache.Remove(key);
            return _repo.PutAsync(id, model);
        }

    }
}
