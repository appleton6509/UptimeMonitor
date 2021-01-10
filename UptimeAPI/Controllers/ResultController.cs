using AutoMapper;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UptimeAPI.Controllers.DTOs;
using UptimeAPI.Controllers.Helper;
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
        public async Task<ActionResult<object>> GetWebRequestLogs([FromQuery] ResultFilterParam filter, [FromQuery] PaginationParam page)
        {
            var query =
                from ep in _context.EndPoint
                join ht in _context.HttpResult
                on ep.Id equals ht.EndPointId
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

            if (page.RequestedPage > 0 & page.MaxPageSize > 0)
            {
                var pagedList = PagedList<EndPointDetailsDTO>.ToPagedList(filteredQuery, page);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pagedList.Pagination));
                return pagedList;
            }
            else
                return await filteredQuery.ToListAsync();

        }
    }
}
