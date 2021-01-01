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
using UptimeAPI.Messaging;

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
        // GET: api/EndPoints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EndPoint>>> GetAllEndPoints()
        {
            Guid userId = UserId();
            List<EndPoint> endPoint = userId != null ?
                await _context.EndPoint.Where(x => x.UserId.Equals(userId)).ToListAsync() : new List<EndPoint>() { new EndPoint() };
            return endPoint;
        }

        // PUT: api/EndPoints/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEndPoint(string id, WebEndPointDTO endPoint)
        {
            if (!IsMatching(id, endPoint.Id))
                return BadRequest(Error.HttpRequest[AuthErrors.KeysNoMatch]);
            Guid.TryParse(endPoint.Id, out Guid result);
            EndPoint ep = result != null ? _context.EndPoint.Find(result) : null;

            if (!OwnsModel(ep.UserId))
                return BadRequest(Error.HttpRequest[AuthErrors.NoResourceAccess]);

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

        // DELETE: api/EndPoints/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<EndPoint>> DeleteEndPoint(string id)
        {
            Guid.TryParse(id, out Guid result);
            var endPoint = await _context.EndPoint.FindAsync(result);
            if (Object.Equals(endPoint,null))
                return NotFound();
            else if (!OwnsModel(endPoint.UserId))
                return BadRequest(Error.HttpRequest[AuthErrors.NoResourceAccess]);

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
