using System.Net;
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
using faka.Payment;

namespace faka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly fakaContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly PaymentGatewayFactory  _paymentGatewayFactory;

        public OrdersController(fakaContext context, UserManager<IdentityUser> userManager, IMapper mapper, PaymentGatewayFactory paymentGatewayFactory)
        {
            // 依赖注入
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _paymentGatewayFactory = paymentGatewayFactory;
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
            var order = await _context.Order.Include(o => o.Product).FirstOrDefaultAsync(o => o.Id == id);
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
            var gateways = await _context.Gateway.ToListAsync();
            var orderDto = _mapper.Map<OrderOutDto>(order);
            orderDto.Gateways = _mapper.Map<List<GatewayOutDto>>(gateways);
            return Ok(orderDto);
        }
        
        // POST: api/Orders/id/pay
        // User pay for order
        [HttpPost("{id}/pay"), Authorize(Roles = Roles.User)]
        public async Task<ActionResult> PayOrder(int id, OrderPayDto orderPayDto)
        {
            //var order = await _context.Order.FindAsync(id);
            var order = await _context.Order
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound("订单不存在");
            }

            var gateways = await _context.Gateway.ToListAsync();
            var gateway = gateways.FirstOrDefault(g => g.Id == orderPayDto.GatewayId);
            if (gateway == null)
            {
                return NotFound("支付方式不可用");
            }

            var payment = _paymentGatewayFactory.Create(gateway.Name);
            var res = await payment.CreatePaymentAsync(order);
            return Ok(res);
        }

        // GET: api/guest/Orders/5
        // for guest
        [HttpGet("guest/{code}")]
        public async Task<ActionResult<OrderOutDto>> GetOrderGuest(string code)
        {
            var order = await _context.Order.Include(o => o.Product).FirstOrDefaultAsync(o => o.AccessCode == code);
            if (order == null)
            {
                return NotFound();
            }
            var gateways = await _context.Gateway.ToListAsync();
            var orderDto = _mapper.Map<OrderOutDto>(order);
            orderDto.Gateways = _mapper.Map<List<GatewayOutDto>>(gateways);
            return Ok(orderDto);
        }
        
        // POST: api/Orders/code/pay
        // Guest pay for order
        [HttpPost("guest/{code}/pay")]
        public async Task<ActionResult> PayOrder(string code, OrderPayDto orderPayDto)
        {
            //get payment gateway form request
            var order = await _context.Order
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.AccessCode == code);
            if (order == null)
            {
                return NotFound("订单不存在");
            }

            var gateways = await _context.Gateway.ToListAsync();
            var gateway = gateways.FirstOrDefault(g => g.Id == orderPayDto.GatewayId);
            if (gateway == null)
            {
                return NotFound("支付方式不可用");
            }

            var payment = _paymentGatewayFactory.Create(gateway.Name);
            var res = await payment.CreatePaymentAsync(order);
            
            var transaction = new Transaction().Create(order, gateway, res);
            try {
                _context.Transaction.Add(transaction);
                await _context.SaveChangesAsync();
            } catch (Exception e) {
                return StatusCode(500, e.Message);
            }
            return Ok(res);
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
            if (orderSubmitDto.Email == null)
            {
                return BadRequest("邮箱不能为空");
            }
            var order = _mapper.Map<Order>(orderSubmitDto);
            order.Price = product.Price * order.Quantity;

            if (userId == null)
            {
                order.GenerateAccessCode();
            }
            else
            {
                order.Email = User.FindFirstValue(ClaimTypes.Email);
            }
            order.UserId = userId;
            _context.Order.Add(order);
            await _context.SaveChangesAsync();
            return Ok(order.AccessCode);
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