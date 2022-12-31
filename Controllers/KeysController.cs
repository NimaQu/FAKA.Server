using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using faka.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using faka.Data;
using faka.Filters;
using faka.Models;
using Microsoft.AspNetCore.Authorization;

namespace faka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeysController : ControllerBase
    {
        private readonly fakaContext _context;

        public KeysController(fakaContext context)
        {
            _context = context;
        }

        // GET: api/Keys
        [HttpGet, Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<IEnumerable<Key>>> GetKey()
        {
            if (_context.Key == null)
            {
                return NotFound();
            }
            return await _context.Key.ToListAsync();
        }

        // GET: api/Keys/5
        [HttpGet("{id}"), Authorize(Roles = Roles.User)]
        public async Task<ActionResult<Key>> GetKey(int id)
        {
            if (_context.Key == null)
            {
                return NotFound();
            }
            var key = await _context.Key.FindAsync(id);

            if (key == null)
            {
                return NotFound();
            }

            return key;
        }

        // PUT: api/Keys/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> PutKey(int id, Key key)
        {
            if (id != key.Id)
            {
                return BadRequest();
            }

            _context.Entry(key).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KeyExists(id))
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

        // POST: api/Keys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<Key>> PostKey(Key key)
        {
            if (_context.Key == null)
            {
                return Problem("Entity set 'fakaContext.Key'  is null.");
            }
            _context.Key.Add(key);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKey", new { id = key.Id }, key);
        }

        // DELETE: api/Keys/5
        [HttpDelete("{id}"), Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteKey(int id)
        {
            if (_context.Key == null)
            {
                return NotFound();
            }
            var key = await _context.Key.FindAsync(id);
            if (key == null)
            {
                return NotFound();
            }

            _context.Key.Remove(key);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool KeyExists(int id)
        {
            return (_context.Key?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
