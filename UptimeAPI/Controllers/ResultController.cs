using AutoMapper;
using Data;
using Data.Models;
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
using UptimeAPI.Messaging;
using UptimeAPI.Services;

namespace UptimeAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ResultController : ApiBaseController
    {
        private readonly IMapper _mapper;
        private readonly UptimeContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public ResultController(
             IMapper mapper
            , UptimeContext context
            , UserManager<IdentityUser> userManager
            , IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }
        [HttpGet]
        [Route("Logs")]
        public async Task<ActionResult<List<EndPointDetailsDTO>>> GetWebRequestLogs([FromQuery] ResultFilterParam filter, [FromQuery] PaginationParam page)
        {
            Guid userId = UserId();
            var query =
                from ep in _context.EndPoint
                join ht in _context.HttpResult
                on ep.Id equals ht.EndPointId
                where ep.UserId == userId
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
                PagedList<EndPointDetailsDTO> pagedList = PagedList<EndPointDetailsDTO>.ToPagedList(filteredQuery, page);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pagedList.Pagination));
                return  pagedList;
            }
            else
                return await filteredQuery.ToListAsync();

        }
        [HttpGet]
        [Route("LogsByTime/{id}")]
        public async Task<ActionResult<List<HttpResultLatencyDTO>>> GetHttpResultByTime(string id, [FromQuery]TimeRangeParam range)
        {
            Guid userId = UserId();

            var endPoint = Guid.TryParse(id, out _) ? _context.EndPoint.Find(Guid.Parse(id)) : null;
            var auth = await _authorizationService.AuthorizeAsync(User, endPoint, Operations.Read);
            if (!auth.Succeeded)
                return BadRequest(Error.Auth[AuthErrors.NoResourceAccess]);

            var query = from ep in _context.EndPoint
                        join ht in _context.HttpResult
                        on ep.Id equals ht.EndPointId
                        where
                        ep.UserId == userId &&
                        ep.Id == endPoint.Id &&
                        ht.TimeStamp >= range.Start &&
                        ht.TimeStamp <= range.End
                        orderby ht.TimeStamp ascending
                        select _mapper.Map<HttpResult, HttpResultLatencyDTO>(ht);
         
            return await query.ToListAsync();
        }
    }
}
