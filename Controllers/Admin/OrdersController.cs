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

namespace FAKA.Server.Controllers.Admin;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = Roles.Admin)]
public class OrdersController : ControllerBase
{
    private readonly FakaContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly OrderService _orderService;

    public OrdersController(FakaContext context, UserManager<IdentityUser> userManager, IMapper mapper,
        OrderService orderService)
    {
        // 依赖注入
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
        _orderService = orderService;
    }

    // GET: api/v1/admin/Orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrder(int perPage = 10, int page = 1)
    {
        if (perPage < 1 || page < 1) return BadRequest("分页参数错误");
        return Ok(await _orderService.GetOrdersAsync(perPage, page));
    }

    // GET: api/v1/admin/Orders/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrderById(int id)
    {
        var order = await _orderService.GetOrderAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    // PUT: api/v1/admin/Orders/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
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

    // POST: api/v1/admin/Orders
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
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

    // DELETE: api/v1/admin/Orders/5
    [HttpDelete("{id}")]
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