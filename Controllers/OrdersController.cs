using System.Security.Claims;
using AutoMapper;
using faka.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using faka.Data;
using faka.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using faka.Models.Dtos;

namespace faka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ActionResult<IEnumerable<OrderOutDto>>> GetOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
             if (User.IsInRole(Roles.Admin))
             {
                 return Ok(await _context.Order.ToListAsync());
             }
            var orders = await _context.Order.Where(b => b.UserId == userId).Include(o => o.Product).ToListAsync();
            var orderDtos = _mapper.Map<IEnumerable<OrderOutDto>>(orders);
            return Ok(orderDtos);
        }

        // GET: api/Orders/5
        [HttpGet("{id}"), Authorize(Roles = Roles.User)]
        public async Task<ActionResult<OrderOutDto>> GetOrder(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            if (User.IsInRole(Roles.Admin))
            {
                return Ok(order);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (order.UserId != userId)
            {
                return NotFound();
            }
            var orderDto = _mapper.Map<OrderOutDto>(order);
            return Ok(orderDto);
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> PutOrder(int id, OrderInDto orderInDto)
        {
            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            _mapper.Map(orderInDto, order);
            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return BadRequest("订单不存在");
                }
                throw;
            }

            return Ok();
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> PostOrder(OrderInDto orderInDto)
        {
            if (orderInDto.UserId != null)
            {
                var user = await _userManager.FindByIdAsync(orderInDto.UserId);
                if (user == null)
                {
                    return BadRequest("用户不存在");
                }
            }
            var product = await _context.Product.FindAsync(orderInDto.ProductId);
            if (product == null)
            {
                return BadRequest("商品不存在");
            }
            var order = _mapper.Map<Order>(orderInDto);
            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            return Ok();
        }
        
        [HttpPost("submit")]
        public async Task<ActionResult> SubmitOrder(OrderSubmitDto orderSubmitDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _context.Product.FindAsync(orderSubmitDto.ProductId);
            if (product == null)
            {
                return BadRequest("商品不存在");
            }
            var order = _mapper.Map<Order>(orderSubmitDto);
            order.UserId = userId;
            _context.Order.Add(order);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}"), Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> DeleteOrder(int id)
        {

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound("订单不存在");
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool OrderExists(int id)
        {
            return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}