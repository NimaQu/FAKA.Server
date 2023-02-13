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

namespace FAKA.Server.Controllers;

[Route("api/v1/service/[controller]")]
[ApiController]
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

    // GET: api/v1/public/Orders/5
    // for guest
    [HttpGet("{code}")]
    public async Task<ActionResult<OrderOutDto>> GetOrderGuest(string code)
    {
        var order = await _orderService.GetOrderAsync(code);
        if (order == null) return NotFound();
        var gateways = await _context.Gateway.ToListAsync();    
        var orderDto = _mapper.Map<OrderOutDto>(order);
        orderDto.Gateways = _mapper.Map<List<GatewayOutDto>>(gateways);
        return Ok(orderDto);
    }

    // POST: api/v1/public/Orders/code/pay
    // Guest pay for order
    [HttpPost("{code}/pay")]
    public async Task<ActionResult> PayOrder(string code, OrderPayDto orderPayDto)
    {
        //get payment gateway form request
        var order = await _orderService.GetOrderAsync(code);
        if (order == null) return NotFound("订单不存在");
        if (order.Status == OrderStatus.Completed) return BadRequest("订单已完成");
        
        if (await _orderService.GetAvailableKeyCountAsync(order.ProductId) < order.Quantity)
            return BadRequest("库存不足");

        var gateway = await _context.Gateway.FindAsync(orderPayDto.GatewayId);
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
    
    // POST: api/v1/public/Orders/submit
    [HttpPost("submit")]
    public async Task<ActionResult> SubmitOrder(OrderSubmitDto orderSubmitDto)
    {
        var order = _mapper.Map<Order>(orderSubmitDto);
        if (orderSubmitDto.Email == null) return BadRequest("邮箱不能为空");
        order.GenerateAccessCode();
        var product = await _context.Product.FindAsync(orderSubmitDto.ProductId);
        if (product == null) return BadRequest("商品不存在");
        order.Product = product;
        order.Amount = product.Price * order.Quantity;
        _context.Order.Add(order);
        await _context.SaveChangesAsync();
        var orderOutDto = _mapper.Map<OrderOutDto>(order);
        orderOutDto.Gateways = _mapper.Map<List<GatewayOutDto>>(await _context.Gateway.ToListAsync());
        return Ok(orderOutDto);
    }
}