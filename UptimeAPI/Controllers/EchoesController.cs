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
    public class EchoesController : ControllerBase
    {
        private readonly UptimeContext _context;

        public EchoesController(UptimeContext context)
        {
            _context = context;
        }

        // GET: api/Echoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Echo>>> GetEcho()
        {
            return await _context.Echo.ToListAsync();
        }

        // GET: api/Echoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Echo>> GetEcho(Guid id)
        {
            var echo = await _context.Echo.FindAsync(id);

            if (echo == null)
            {
                return NotFound();
            }

            return echo;
        }

        // PUT: api/Echoes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEcho(Guid id, Echo echo)
        {
            if (id != echo.Id)
            {
                return BadRequest();
            }

            _context.Entry(echo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EchoExists(id))
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

        // POST: api/Echoes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Echo>> PostEcho(Echo echo)
        {
            _context.Echo.Add(echo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEcho", new { id = echo.Id }, echo);
        }

        // DELETE: api/Echoes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Echo>> DeleteEcho(Guid id)
        {
            var echo = await _context.Echo.FindAsync(id);
            if (echo == null)
            {
                return NotFound();
            }

            _context.Echo.Remove(echo);
            await _context.SaveChangesAsync();

            return echo;
        }

        private bool EchoExists(Guid id)
        {
            return _context.Echo.Any(e => e.Id == id);
        }
    }
}
