using System.Security.Claims;
using AutoMapper;
using FAKA.Server.Auth;
using FAKA.Server.Data;
using FAKA.Server.Models;
using FAKA.Server.Models.Dtos;
using FAKA.Server.Payment;
using FAKA.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FAKA.Server.Controllers.User;

[Route("api/v1/user/[controller]")]
[ApiController]
[Authorize(Roles = Roles.User)]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly OrderService _orderService;

    public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper,
        OrderService orderService)
    {
        // 依赖注入
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
        _orderService = orderService;
    }

    // GET: api/v1/user/Orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderOutDto>>> GetOrder(int perPage = 10, int page = 1)
    {
        if (perPage < 1 || page < 1) return BadRequest("分页参数错误");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orders = await _orderService.GetOrdersAsync(userId, perPage, page);
        var orderDtos = _mapper.Map<IEnumerable<OrderOutDto>>(orders);
        return Ok(orderDtos);
    }

    // GET: api/v1/user/Orders/5
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderOutDto>> GetOrderById(int id)
    {
        var order = await _orderService.GetOrderAsync(id);
        if (order == null) return NotFound();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (order.UserId != userId) return NotFound();
        var gateways = await _context.Gateway.ToListAsync();
        var orderDto = _mapper.Map<OrderOutDto>(order);
        orderDto.Gateways = _mapper.Map<List<GatewayOutDto>>(gateways);
        return Ok(orderDto);
    }

    // POST: api/v1/user/Orders/id/pay
    [HttpPost("{id}/pay")]
    public async Task<ActionResult> PayOrder(int id, OrderPayDto orderPayDto)
    {
        var order = await _orderService.GetOrderAsync(id);
        if (order == null) return NotFound("订单不存在");
        if (order.Status == OrderStatus.Completed) return BadRequest("订单已完成");
        
        if (await _orderService.GetAvailableKeyCountAsync(order.ProductId) < order.Quantity)
            return BadRequest("库存不足");
        
        var gateways = await _context.Gateway.ToListAsync();
        var gateway = gateways.FirstOrDefault(g => g.Id == orderPayDto.GatewayId);
        if (gateway == null) return NotFound("支付方式不可用");
        
        GatewayResponse res;
        try
        {
            res = await _orderService.CreatePaymentAsync(order, gateway, orderPayDto);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }

        return Ok(res);
    }
    
    // POST: api/v1/user/Orders/submit
    [HttpPost("submit")]
    public async Task<ActionResult> SubmitOrder(OrderSubmitDto orderSubmitDto)
    {
        var order = _mapper.Map<Order>(orderSubmitDto);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
}