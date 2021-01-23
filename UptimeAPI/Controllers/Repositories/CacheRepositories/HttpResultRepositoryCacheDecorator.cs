﻿using AutoMapper;
using Data;
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
    public class HttpResultRepositoryCacheDecorator : BaseRepository, IHttpResultRepository
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpResultRepository _repo;
        public HttpResultRepositoryCacheDecorator(
            UptimeContext context,
            IHttpContextAccessor httpcontext,
            IMemoryCache cache,
            HttpResultRepository repo,
            IMapper mapper = null) : base(context, httpcontext, mapper)
        {
            _cache = cache;
            _repo = repo;
        }

        public List<EndPointDetailsDTO> GetAll(PaginationParam page, ResultFilterParam filter)
        {
            var key = $"{nameof(HttpResultRepositoryCacheDecorator)}{UserId()}{nameof(GetAll)}{page.RequestedPage}{filter}";

            bool hasCache = _cache.TryGetValue(key, out List<EndPointDetailsDTO> data);
            if (hasCache)
                return data;

            data = _repo.GetAll(page, filter);
            _cache.SetCache(key, data, 120);
            return data;

        }

        public async Task<List<HttpResultLatencyDTO>> GetByEndPointAsync(Guid id, TimeRangeParam range)
        {
            var key = $"{nameof(HttpResultRepositoryCacheDecorator)}{UserId()}{nameof(GetByEndPointAsync)}{id}";
            bool hasCache = _cache.TryGetValue(key, out List<HttpResultLatencyDTO> data);
            if (hasCache)
                return data;

            data = await _repo.GetByEndPointAsync(id, range);
            _cache.SetCache(key, data, 120);
            return data;
        }
    }
}