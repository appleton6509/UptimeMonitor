using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Data.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace UptimeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EndPointsController : ApiBaseController
    {
        private readonly IMapper _mapper;
        private readonly UptimeContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EndPointsController(
             IMapper mapper
            , UptimeContext context
            , UserManager<IdentityUser> userManager)
        {
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public Guid UserId()
        {
            Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId);
            return userId;
        }
        // GET: api/EndPoints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EndPoint>>> GetEndPoint()
        {
            Guid userId = UserId();
            List<EndPoint> endPoint = userId != null ?
                await _context.EndPoint.Where(x => x.UserId.Equals(userId)).ToListAsync() : new List<EndPoint>() { new EndPoint() };
            return endPoint;
        }

        // GET: api/EndPoints/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EndPoint>> GetEndPoint(Guid id)
        {
            //string identityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var userId =  _context.WebUser.Where(x => x.IdentityId.Equals(identityId, StringComparison.OrdinalIgnoreCase)).FirstAsync().Result.Id;
            //var endPoint = await _context.EndPoint.Where(x => x.UserID == Convert.ToInt32(userId)).ToListAsync();

            //if (endPoint == null)
            //{
            //    return NotFound();
            //}

            return null;
        }

        // PUT: api/EndPoints/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEndPoint(string id, WebEndPointDTO endPoint)
        {
            if (String.Compare(id, endPoint.Id) != 0) 
            {
                return BadRequest();
            }
            Guid.TryParse(endPoint.Id, out Guid result);
            EndPoint ep = _context.EndPoint.Find(result);
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
            var userId = UserId();
            if (Object.Equals(userId,null))
                return BadRequest("user linked to endpoint does not exist");

            EndPoint endPoint = _mapper.Map<WebEndPointDTO, EndPoint>(webEndPoint);
            endPoint.UserId = userId;
            _context.EndPoint.Add(endPoint);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEndPoint", new { id = endPoint.Id }, endPoint);
        }

        // DELETE: api/EndPoints/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<EndPoint>> DeleteEndPoint(int id)
        {
            var endPoint = await _context.EndPoint.FindAsync(id);
            if (endPoint == null)
            {
                return NotFound();
            }

            _context.EndPoint.Remove(endPoint);
            await _context.SaveChangesAsync();

            return endPoint;
        }

        private bool EndPointExists(string id)
        {
            return _context.EndPoint.Any(e => e.Id.ToString() == id);
        }
    }
}
