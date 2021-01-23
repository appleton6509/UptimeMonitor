
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
    public class HttpResultRepository : BaseRepository, IHttpResultRepository
    {
        public HttpResultRepository(UptimeContext context, IHttpContextAccessor httpcontext, IMapper mapper = null) : base(context, httpcontext, mapper)
        {
        }

        public List<EndPointDetailsDTO> GetAll(PaginationParam page, ResultFilterParam filter)
        {
            var query =
                from ep in _context.EndPoint
                join ht in _context.HttpResult
                on ep.Id equals ht.EndPointId
                where ep.UserId == UserId()
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
            return results;
        }
        public async Task<List<HttpResultLatencyDTO>> GetByEndPointAsync(Guid id, TimeRangeParam range)
        {
            EndPoint endPoint = _context.EndPoint.Find(id);
            var query = from ep in _context.EndPoint
                        join ht in _context.HttpResult
                        on ep.Id equals ht.EndPointId
                        where
                        ep.UserId == UserId() &&
                        ep.Id == endPoint.Id &&
                        ht.TimeStamp >= range.Start &&
                        ht.TimeStamp <= range.End
                        orderby ht.TimeStamp ascending
                        select _mapper.Map<HttpResult, HttpResultLatencyDTO>(ht);

            return await query.ToListAsync(); ;
        }

        public List<HttpResult> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<int> PostAsync(HttpResult model)
        {
            throw new NotImplementedException();
        }

        public Task<int> PutAsync(Guid id, HttpResult model)
        {
            throw new NotImplementedException();
        }
    }
}
