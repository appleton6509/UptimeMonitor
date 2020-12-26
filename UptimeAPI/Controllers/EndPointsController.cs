using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;

namespace UptimeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EndPointsController : ControllerBase
    {
        private readonly UptimeContext _context;

        public EndPointsController(UptimeContext context)
        {
            _context = context;
        }

        // GET: api/EndPoints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EndPoint>>> GetEndPoint()
        {
            return await _context.EndPoint.ToListAsync();
        }

        // GET: api/EndPoints/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EndPoint>> GetEndPoint(int id)
        {
            var endPoint = await _context.EndPoint.FindAsync(id);

            if (endPoint == null)
            {
                return NotFound();
            }

            return endPoint;
        }

        // PUT: api/EndPoints/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEndPoint(int id, EndPoint endPoint)
        {
            if (id != endPoint.ID)
            {
                return BadRequest();
            }

            _context.Entry(endPoint).State = EntityState.Modified;

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
        public async Task<ActionResult<EndPoint>> PostEndPoint(EndPoint endPoint)
        {
            _context.EndPoint.Add(endPoint);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEndPoint", new { id = endPoint.ID }, endPoint);
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

        private bool EndPointExists(int id)
        {
            return _context.EndPoint.Any(e => e.ID == id);
        }
    }
}
