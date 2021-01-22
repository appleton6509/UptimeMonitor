
using AutoMapper;
using Data;
using Data.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UptimeAPI.Controllers.DTOs;
using UptimeAPI.Controllers.Extensions;
using UptimeAPI.Controllers.Helper;
using UptimeAPI.Controllers.QueryParams;

namespace UptimeAPI.Controllers.Repositories
{
    public class HttpResultRepository : BaseRepository<HttpResult>, IHttpResultRepository
    {
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        public HttpResultRepository(
            UptimeContext context,
            IHttpContextAccessor httpContext,
            IMemoryCache cache,
            IMapper mapper) : base(context, httpContext)
        {
            _cache = cache;
            _mapper = mapper;
        }

        public List<EndPointDetailsDTO> GetAll(PaginationParam page, ResultFilterParam filter)
        {
            var key = $"{nameof(HttpResultRepository)} - {_userId} - {nameof(GetAll)} - {page.RequestedPage} - {filter}";
            var cacheValue = _cache.Get<List<EndPointDetailsDTO>>(key);
            if (cacheValue != null)
                return cacheValue;

            var query =
                from ep in _context.EndPoint
                join ht in _context.HttpResult
                on ep.Id equals ht.EndPointId
                where ep.UserId == _userId
                orderby ht.TimeStamp descending
                select new EndPointDetailsDTO()
                {
                    Ip = ep.Ip,
                    IsReachable = ht.IsReachable,
                    Description = ep.Description,
                    Id = ep.Id,
                    Latency = ht.Latency,
                    Status = ht.StatusMessage,
                    TimeStamp = ht.TimeStamp
                };

            var filteredQuery = new ResultFilterRules(filter, query).ApplyFilters();
            List<EndPointDetailsDTO> results;
            if (page.RequestedPage > 0 & page.MaxPageSize > 0)
                results = PagedList<EndPointDetailsDTO>.ToPagedList(filteredQuery, page);
            else
                results = filteredQuery.ToList();

            _cache.SetCache<List<EndPointDetailsDTO>>(key, results);
            return results;
        }
        public async Task<List<HttpResultLatencyDTO>> GetByEndPoint(Guid id, TimeRangeParam range)
        {
            var key = $"{nameof(HttpResultRepository)} - {_userId} - {nameof(GetByEndPoint)} - {id}";
            var cacheValue = _cache.Get<List<HttpResultLatencyDTO>>(key);
            if (cacheValue != null)
                return cacheValue;


            EndPoint endPoint = _context.EndPoint.Find(id);
            var query = from ep in _context.EndPoint
                        join ht in _context.HttpResult
                        on ep.Id equals ht.EndPointId
                        where
                        ep.UserId == _userId &&
                        ep.Id == endPoint.Id &&
                        ht.TimeStamp >= range.Start &&
                        ht.TimeStamp <= range.End
                        orderby ht.TimeStamp ascending
                        select _mapper.Map<HttpResult, HttpResultLatencyDTO>(ht);

            var results = await query.ToListAsync();
            _cache.SetCache<List<HttpResultLatencyDTO>>(key, results);
            return results;
        }

        public override Task<List<HttpResult>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<int> PostAsync(HttpResult model)
        {
            throw new NotImplementedException();
        }

        public override Task<int> PutAsync(Guid id, HttpResult model)
        {
            throw new NotImplementedException();
        }
    }
}
