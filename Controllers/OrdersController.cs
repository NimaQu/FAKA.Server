using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using faka.Data;
using faka.Filters;
using faka.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using faka.Models.DTO;

namespace faka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomResultFilter]
    public class OrdersController : ControllerBase
    {
        private readonly fakaContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;

        public OrdersController(fakaContext context, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            // 依赖注入
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: api/Boughts
        [HttpGet, Authorize(Roles = "User")]
        public async Task<ActionResult<IEnumerable<Order>>> GetBought()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var user = await _userManager.GetUserAsync(User);
            //var roles = await _userManager.GetRolesAsync(user);
            // if (User.IsInRole("Admin"))
            // {
            //     return await _context.Bought.ToListAsync();
            // }
            System.Console.WriteLine(userId);
            var orders = await _context.Bought.Where(b => b.UserId == userId).ToListAsync();
            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return Ok(orderDtos);
        }

        // GET: api/Boughts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetBought(int id)
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
        public async Task<IActionResult> PutBought(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

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
        public async Task<ActionResult<Order>> PostBought(Order order)
        {
            if (_context.Bought == null)
            {
                return Problem("Entity set 'fakaContext.Bought'  is null.");
            }

            _context.Bought.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBought", new { id = order.Id }, order);
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