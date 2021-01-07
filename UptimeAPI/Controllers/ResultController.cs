using AutoMapper;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UptimeAPI.Controllers.QueryParams;

namespace UptimeAPI.Controllers
{
    [Route("/api/Result")]
    [Authorize]
    [ApiController]
    public class ResultController : Controller
    {
        private readonly IMapper _mapper;
        private readonly UptimeContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ResultController(
             IMapper mapper
            , UptimeContext context
            , UserManager<IdentityUser> userManager)
        {
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        [Route("Logs")]
        public async Task<ActionResult<object>> GetWebRequestLogs([FromQuery] LogFilter filter)
        {
            var query =
                from ep in _context.EndPoint
                join ht in _context.HttpResult
                on ep.Id equals ht.EndPointId
                orderby ht.TimeStamp descending
                select new
                {
                    Ip = ep.Ip,
                    IsReachable = ht.IsReachable,
                    Description = ep.Description,
                    Id = ep.Id,
                    Latency = ht.Latency,
                    Status = ht.StatusMessage,
                    TimeStamp = ht.TimeStamp
                };

            if (filter.Reachable.HasValue)
                query = query.Where(x => x.IsReachable == filter.Reachable.Value);

            if (filter.OrderBy == OrderBy.Descending)
            {
                switch (filter.SortBy)
                {
                    case SortBy.Description:
                        query = query.OrderByDescending(x => x.Description);
                        break;
                    case SortBy.Latency:
                        query = query.OrderByDescending(x => x.Latency);
                        break;
                    case SortBy.Reachable:
                        query = query.OrderByDescending(x => x.IsReachable);
                        break;
                    case SortBy.Site:
                        query = query.OrderByDescending(x => x.Ip);
                        break;
                    case SortBy.Timestamp:
                        query = query.OrderByDescending(x => x.TimeStamp);
                        break;
                    default:
                        query = query.OrderByDescending(x => x.TimeStamp);
                        break;
                }
            }
            if (filter.OrderBy == OrderBy.Ascending)
            {
                switch (filter.SortBy)
                {
                    case SortBy.Description:
                        query = query.OrderBy(x => x.Description);
                        break;
                    case SortBy.Latency:
                        query = query.OrderBy(x => x.Latency);
                        break;
                    case SortBy.Reachable:
                        query = query.OrderBy(x => x.IsReachable);
                        break;
                    case SortBy.Site:
                        query = query.OrderBy(x => x.Ip);
                        break;
                    case SortBy.Timestamp:
                        query = query.OrderBy(x => x.TimeStamp);
                        break;
                    default:
                        query = query.OrderBy(x => x.TimeStamp);
                        break;
                }
            }
            var value = await query.ToListAsync();
            return value;
        }
    }
}
