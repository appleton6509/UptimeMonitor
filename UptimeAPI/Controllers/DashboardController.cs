using AutoMapper;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UptimeAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly UptimeContext _context;

        public DashboardController(
            UserManager<IdentityUser> userManager
            , IMapper mapper
            , UptimeContext context)
        {
            _mapper = mapper;
            _userManager = userManager;
            _context = context;
        }
        [HttpGet]
        [Route("ConnectionStatus")]
        // GET: api/Dashboard/ConnectionStatus
        //create a list containing all of the endpoints latest webrequest results.
        public async Task<ActionResult<object>> CurrentOnlineOffline()
        {
            var query =
                from ep in _context.EndPoint
                join ht in _context.HttpResult
                on ep.Id equals ht.EndPointId
                orderby ht.TimeStamp descending
                select new
                {
                    Ip = ep.Ip,
                    TimeStamp = ht.TimeStamp,
                    IsReachable = ht.IsReachable
                };
            var totalEndPoints = _context.EndPoint.Select(x => x.Ip).Distinct().Count();
            return query.Take(totalEndPoints).ToList();
        }

        [HttpGet]
        [Route("Offline")]
        public async Task<ActionResult<object>> Offline()
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
                    Id = ep.Id
                };
            var totalEndPoints = _context.EndPoint.Select(x => x.Ip).Distinct().Count();
            return query.Take(totalEndPoints).ToList().Where(x => !x.IsReachable).Distinct().ToList();
        }
    }
}
