using AutoMapper;
using Data;
using Data.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UptimeAPI.Controllers.DTOs;
using UptimeAPI.Controllers.Extensions;
using UptimeAPI.Controllers.QueryParams;

namespace UptimeAPI.Controllers.Repositories.CacheRepositories
{
    [Obsolete("This class is no longer in use as project required REAL TIME updates")]
    public class ResultDataRepositoryCacheDecorator : BaseRepository, IResultDataRepository
    {
        private readonly IMemoryCache _cache;
        private readonly IResultDataRepository _repo;
        public ResultDataRepositoryCacheDecorator(
            UptimeContext context,
            IHttpContextAccessor httpcontext,
            IMemoryCache cache,
            ResultDataRepository repo,
            IMapper mapper = null) : base(context, httpcontext, mapper)
        {
            _cache = cache;
            _repo = repo;
        }

        public Task<int> DeleteAsync(Guid id)
        {
            return _repo.DeleteAsync(id);
        }

        public bool Exists(Guid id)
        {
            return _repo.Exists(id);
        }

        public ResultData Get(Guid id)
        {
            return _repo.Get(id);
        }

        public List<ResultDataDetailsDTO> GetAll(PaginationParam page, ResultFilterParam filter)
        {
            var key = $"{nameof(ResultDataRepositoryCacheDecorator)}{UserId()}{nameof(GetAll)}{page.RequestedPage}";
            bool hasCache = _cache.TryGetValue(key, out List<ResultDataDetailsDTO> data);
            if (hasCache)
                return data;
            
            data =  _repo.GetAll(page, filter);
            _cache.SetCache(key, data);
            return data;
        }

        public List<ResultData> GetAll()
        {
            return _repo.GetAll();
        }

        public async Task<List<ResultDataLatencyDTO>> GetByEndPointAsync(Guid id, TimeRangeParam range)
        {
            var key = $"{nameof(ResultDataRepositoryCacheDecorator)}{UserId()}{nameof(GetByEndPointAsync)}{id}";
            bool hasCache = _cache.TryGetValue(key, out List<ResultDataLatencyDTO> data);
            if (hasCache)
                return data;

            data = await _repo.GetByEndPointAsync(id, range);
            _cache.SetCache(key, data);
            return data;
        }

        public Task<int> PostAsync(ResultData model)
        {
            return _repo.PostAsync(model);
        }

        public Task<int> PutAsync(Guid id, ResultData model)
        {
            return _repo.PutAsync(id, model);
        }
    }
}
