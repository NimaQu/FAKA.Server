using System.Security.Claims;
using AutoMapper;
using faka.Auth;
using faka.Data;
using faka.Models;
using faka.Models.Dtos;
using faka.Payment;
using faka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace faka.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly fakaContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly OrderService _orderService;

    public OrdersController(fakaContext context, UserManager<IdentityUser> userManager, IMapper mapper,
        OrderService orderService)
    {
        // 依赖注入
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
        _orderService = orderService;
    }

    // GET: api/Orders
    [HttpGet]
    [Authorize(Roles = Roles.User)]
    public async Task<ActionResult<IEnumerable<OrderOutDto>>> GetOrder()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (User.IsInRole(Roles.Admin)) return Ok(await _context.Order.ToListAsync());
        var orders = await _context.Order.Where(b => b.UserId == userId).Include(o => o.Product).ToListAsync();
        var orderDtos = _mapper.Map<IEnumerable<OrderOutDto>>(orders);
        return Ok(orderDtos);
    }

    // GET: api/Orders/5
    [HttpGet("{id}")]
    [Authorize(Roles = Roles.User)]
    public async Task<ActionResult<OrderOutDto>> GetOrder(int id)
    {
        var order = await _orderService.GetOrderAsync(id);
        if (order == null) return NotFound();
        if (User.IsInRole(Roles.Admin)) return Ok(order);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (order.UserId != userId) return NotFound();
        var gateways = await _context.Gateway.ToListAsync();
        var orderDto = _mapper.Map<OrderOutDto>(order);
        orderDto.Gateways = _mapper.Map<List<GatewayOutDto>>(gateways);
        return Ok(orderDto);
    }

    // POST: api/Orders/id/pay
    // User pay for order
    [HttpPost("{id}/pay")]
    [Authorize(Roles = Roles.User)]
    public async Task<ActionResult> PayOrder(int id, OrderPayDto orderPayDto)
    {
        var order = await _orderService.GetOrderAsync(id);
        if (order == null) return NotFound("订单不存在");
        if (order.Status == OrderStatus.Completed) return BadRequest("订单已完成");

        var gateways = await _context.Gateway.ToListAsync();
        var gateway = gateways.FirstOrDefault(g => g.Id == orderPayDto.GatewayId);
        if (gateway == null) return NotFound("支付方式不可用");
        GatewayResponse res;
        try
        {
            res = await _orderService.CreatePaymentAsync(order, gateway);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }

        return Ok(res);
    }

    // GET: api/guest/Orders/5
    // for guest
    [HttpGet("guest/{code}")]
    public async Task<ActionResult<OrderOutDto>> GetOrderGuest(string code)
    {
        var order = await _orderService.GetOrderByCodeAsync(code);
        if (order == null) return NotFound();
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
        var order = await _orderService.GetOrderByCodeAsync(code);
        if (order == null) return NotFound("订单不存在");
        if (order.Status == OrderStatus.Completed) return BadRequest("订单已完成");

        var gateway = await _context.Gateway.FindAsync(orderPayDto.GatewayId);
        if (gateway == null) return NotFound("支付方式不可用");
        GatewayResponse res;
        try
        {
            res = await _orderService.CreatePaymentAsync(order, gateway);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }

        return Ok(res);
    }

    // PUT: api/Orders/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> PutOrder(int id, OrderInDto orderInDto)
    {
        var order = await _context.Order.FindAsync(id);
        if (order == null) return NotFound();
        _mapper.Map(orderInDto, order);
        _context.Entry(order).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrderExists(id)) return BadRequest("订单不存在");
            throw;
        }

        return Ok();
    }

    // POST: api/Orders
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> PostOrder(OrderInDto orderInDto)
    {
        if (orderInDto.UserId != null)
        {
            var user = await _userManager.FindByIdAsync(orderInDto.UserId);
            if (user == null) return BadRequest("用户不存在");
        }

        var product = await _context.Product.FindAsync(orderInDto.ProductId);
        if (product == null) return BadRequest("商品不存在");
        var order = _mapper.Map<Order>(orderInDto);
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("submit")]
    public async Task<ActionResult> SubmitOrder(OrderSubmitDto orderSubmitDto)
    {
        var order = _mapper.Map<Order>(orderSubmitDto);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (userId == null)
        {
            if (orderSubmitDto.Email == null) return BadRequest("邮箱不能为空");
            order.GenerateAccessCode();
        }
        else
            order.Email = User.FindFirstValue(ClaimTypes.Email) ?? throw new InvalidOperationException("用户的邮箱不存在");
        var product = await _context.Product.FindAsync(orderSubmitDto.ProductId);
        if (product == null) return BadRequest("商品不存在");
        order.Product = product;
        order.Amount = product.Price * order.Quantity;
        order.UserId = userId;
        _context.Order.Add(order);
        await _context.SaveChangesAsync();
        var orderOutDto = _mapper.Map<OrderOutDto>(order);
        orderOutDto.Gateways = _mapper.Map<List<GatewayOutDto>>(await _context.Gateway.ToListAsync());
        return Ok(orderOutDto);
    }

    // DELETE: api/Orders/5
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> DeleteOrder(int id)
    {
        var order = await _context.Order.FindAsync(id);
        if (order == null) return NotFound("订单不存在");

        _context.Order.Remove(order);
        await _context.SaveChangesAsync();

        return Ok();
    }

    private bool OrderExists(int id)
    {
        return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}