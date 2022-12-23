using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using faka.Data;
using faka.Filters;
using faka.Models;

namespace faka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomResultFilter]
    public class BoughtsController : ControllerBase
    {
        private readonly fakaContext _context;

        public BoughtsController(fakaContext context)
        {
            _context = context;
        }

        // GET: api/Boughts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bought>>> GetBought()
        {
            if (_context.Bought == null)
            {
                return NotFound();
            }
            return await _context.Bought.ToListAsync();
        }

        // GET: api/Boughts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bought>> GetBought(int id)
        {
            if (_context.Bought == null)
            {
                return NotFound();
            }
            var bought = await _context.Bought.FindAsync(id);

            if (bought == null)
            {
                return NotFound();
            }

            return bought;
        }

        // PUT: api/Boughts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBought(int id, Bought bought)
        {
            if (id != bought.Id)
            {
                return BadRequest();
            }

            _context.Entry(bought).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoughtExists(id))
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

        // POST: api/Boughts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bought>> PostBought(Bought bought)
        {
            if (_context.Bought == null)
            {
                return Problem("Entity set 'fakaContext.Bought'  is null.");
            }
            _context.Bought.Add(bought);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBought", new { id = bought.Id }, bought);
        }

        // DELETE: api/Boughts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBought(int id)
        {
            if (_context.Bought == null)
            {
                return NotFound();
            }
            var bought = await _context.Bought.FindAsync(id);
            if (bought == null)
            {
                return NotFound();
            }

            _context.Bought.Remove(bought);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BoughtExists(int id)
        {
            return (_context.Bought?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
