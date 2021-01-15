using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using UptimeAPI.Messaging;
using UptimeAPI.Controllers.QueryParams;
using UptimeAPI.Controllers.DTOs;
using Microsoft.AspNetCore.Authorization;
using UptimeAPI.Services;

namespace UptimeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EndPointsController : ApiBaseController
    {
        #region Properties  / Constructor
        private readonly IMapper _mapper;
        private readonly UptimeContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public EndPointsController(
             IMapper mapper
            , IAuthorizationService authorizationService
            , UptimeContext context
            , UserManager<IdentityUser> userManager)
        {
            _authorizationService = authorizationService;
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
        }
        #endregion

        #region Basic CRUD
        // GET: api/EndPoints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EndPoint>>> GetAllEndPoints()
        {
            Guid userId = UserId();
            List<EndPoint> endPoint = await _context.EndPoint.Where(x => x.UserId.Equals(userId)).ToListAsync();
            return endPoint;
        }

        // PUT: api/EndPoints/5DFBBC20-D61E-4506-58DE-08D8B0516C01
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEndPoint(string id, WebEndPointDTO endPoint)
        {
            bool isValid = Guid.TryParse(endPoint.Id, out Guid result);
            EndPoint ep = isValid? _context.EndPoint.Find(result) : null;

            AuthorizationResult auth = await _authorizationService.AuthorizeAsync(User, ep, Operations.Update);
            if (!auth.Succeeded)
                return BadRequest(Error.Auth[AuthErrors.NoResourceAccess]);


            ep.Description = endPoint.Description;
            ep.Ip = endPoint.Ip;
            _context.Entry(ep).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EndPointExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/EndPoints
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<EndPoint>> PostEndPoint(WebEndPointDTO webEndPoint)
        {
            EndPoint endPoint = _mapper.Map<WebEndPointDTO, EndPoint>(webEndPoint);
            endPoint.UserId = UserId();
            _context.EndPoint.Add(endPoint);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEndPoint", new { id = endPoint.Id }, endPoint);
        }

        // DELETE: api/EndPoints/5DFBBC20-D61E-4506-58DE-08D8B0516C01
        [HttpDelete("{id}")]
        public async Task<ActionResult<EndPoint>> DeleteEndPoint(string id)
        {
            Guid.TryParse(id, out Guid result);
            var endPoint = await _context.EndPoint.FindAsync(result);

            AuthorizationResult auth = await _authorizationService.AuthorizeAsync(User, endPoint, Operations.Delete);
            if (!auth.Succeeded)
                return BadRequest(Error.Auth[AuthErrors.NoResourceAccess]);


            _context.EndPoint.Remove(endPoint);
            await _context.SaveChangesAsync();

            return endPoint;
        }
        #endregion Basic CRUD

        #region Custom GET

        // GET: api/EndPoints/Offline
        [HttpGet("Offline")]
        public async Task<ActionResult<List<EndPointOfflineDTO>>> Offline()
        {
            Guid userId = UserId();
            var query =
                from ep in _context.EndPoint
                join ht in _context.HttpResult
                on ep.Id equals ht.EndPointId
                where ep.UserId == userId
                orderby ht.TimeStamp descending
                select new EndPointOfflineDTO()
                {
                    Ip = ep.Ip,
                    IsReachable = ht.IsReachable,
                    Description = ep.Description,
                    Id = ep.Id
                };
            var totalEndPoints = _context.EndPoint.Where(x => x.UserId == userId).Select(x => x.Ip).Distinct().Count();
            return await query.Take(totalEndPoints).Where(x => !x.IsReachable).Distinct().ToListAsync();
        }

        // GET: api/EndPoints/ConnectionStatus
        [HttpGet("ConnectionStatus")]
        public async Task<ActionResult<List<EndPointOfflineOnlineDTO>>> GetConnectionStatus()
        {
            Guid userId = UserId();
            var query =
                from ep in _context.EndPoint
                join ht in _context.HttpResult
                on ep.Id equals ht.EndPointId
                where ep.UserId == userId
                orderby ht.TimeStamp descending
                select new EndPointOfflineOnlineDTO()
                {
                   Ip = ep.Ip,
                   TimeStamp = ht.TimeStamp,
                   IsReachable = ht.IsReachable
                };
            var totalEndPoints = _context.EndPoint.Where(x => x.UserId == userId).Select(x => x.Ip).Distinct().Count();
            return await query.Take(totalEndPoints).ToListAsync();
        }

        //api/EndPoints/Statistics
        [HttpGet("Statistics")]
        public ActionResult<List<EndPointStatisticsDTO>> GetAllEndPointStatistics()
        {
            Guid userId = UserId();
            var endPoints = (from ep in _context.EndPoint
                             where ep.UserId == userId
                             select ep).ToList();
            List<EndPointStatisticsDTO> stats = new List<EndPointStatisticsDTO>();

            endPoints.ForEach(x =>
           {
               var epstat = GetEndPointStatistics(x);
               stats.Add(epstat);
           });
            return stats;
        }

        //api/EndPoints/Statistics/5DFBBC20-D61E-4506-58DE-08D8B0516C01
        [HttpGet("Statistics/{id}")]
        public async Task<ActionResult<EndPointStatisticsDTO>> GetEndPointStatistics(string id)
        {
            Guid userId = UserId();
            Guid.TryParse(id, out Guid result);
            var endPoint = await _context.EndPoint.FindAsync(result);

            AuthorizationResult auth = await _authorizationService.AuthorizeAsync(User, endPoint, Operations.Update);
            if (!auth.Succeeded)
                return BadRequest(Error.Auth[AuthErrors.NoResourceAccess]);

            return GetEndPointStatistics(endPoint);
        }

        #endregion Custom GET

        #region Helper Methods
        private EndPointStatisticsDTO GetEndPointStatistics(EndPoint endPoint)
        {
            var query = from ep in _context.EndPoint
                        join ht in _context.HttpResult
                        on ep.Id equals ht.EndPointId
                        where ep.Id == endPoint.Id
                        orderby ht.TimeStamp descending
                        select ht;
            if (!query.Any())
                return new EndPointStatisticsDTO();

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
                Id = endPoint.Id
            };
            return data;
        }

        private bool EndPointExists(string id)
        {
            return _context.EndPoint.Any(e => e.Id.ToString() == id);
        }

        #endregion Helper Methods
    }
}
