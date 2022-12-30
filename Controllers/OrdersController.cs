using System.Security.Claims;
using AutoMapper;
using faka.Auth;
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

        // GET: api/Orders
        [HttpGet, Authorize(Roles = Roles.User)]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
             if (User.IsInRole(Roles.Admin))
             {
                 return await _context.Order.ToListAsync();
             }
            var orders = await _context.Order.Where(b => b.UserId == userId).ToListAsync();
            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return Ok(orderDtos);
        }

        // GET: api/Orders/5
        [HttpGet("{id}"), Authorize(Roles = Roles.User)]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine(User.IsInRole(Roles.User));
            var order = await _context.Order.FindAsync(id);
            if (User.IsInRole("Admin"))
            {
                return Ok(order);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (order.UserId != userId)
            {
                return NotFound();
            }
            var orderDto = _mapper.Map<OrderDto>(order);
            return Ok(orderDto);
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
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
                if (!OrderExists(id))
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

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'fakaContext.Order'  is null.");
            }

            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (_context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}