using AutoMapper;
using FAKA.Server.Auth;
using FAKA.Server.Data;
using FAKA.Server.Models;
using FAKA.Server.Models.Dtos;
using FAKA.Server.Payment;
using FAKA.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FAKA.Server.Controllers.Admin;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = Roles.Admin)]
public class GatewayController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly PaymentGatewayFactory _paymentGatewayFactory;
    private readonly GatewayService _gatewayService;
    
    public GatewayController(ApplicationDbContext context, IMapper mapper, PaymentGatewayFactory paymentGatewayFactory, GatewayService gatewayService)
    {
        _context = context;
        _mapper = mapper;
        _paymentGatewayFactory = paymentGatewayFactory;
        _gatewayService = gatewayService;
    }
    
    // GET: api/v1/admin/Gateway
    [HttpGet]
    public async Task<ActionResult<List<Gateway>>> GetGateway()
    {
        var gateways = await _context.Gateway.ToListAsync();
        return Ok(gateways);
    }
    
    // GET: api/v1/admin/Gateway/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Gateway>> GetGateway(int id)
    {
        var gateway = await _context.Gateway.FindAsync(id);
        return Ok(gateway);
    }
    
    // POST: api/v1/admin/Gateway
    [HttpPost]
    public async Task<ActionResult> PostGateway(GatewayInDto gatewayInDto)
    {
        var gateway = _mapper.Map<Gateway>(gatewayInDto);
        if (!_gatewayService.IsGatewayAvailable(gateway.Name))
        {
            return BadRequest("目标支付网关不可用, 请检查名称是否正确");
        }
        if (await _gatewayService.IsGatewayExistAsync(gateway.Name))
        {
            return BadRequest("目标支付网关已存在");
        }
        _context.Gateway.Add(gateway);
        await _context.SaveChangesAsync();
        return Ok();
    }
    
    // PUT: api/v1/admin/Gateway/5
    [HttpPut("{id}")]
    public async Task<ActionResult> PutGateway(int id, GatewayInDto gatewayInDto)
    {
        var gateway = await _context.Gateway.FindAsync(id);
        if (gateway == null)
        {
            return NotFound("网关不存在");
        }
        if (!_gatewayService.IsGatewayAvailable(gateway.Name))
        {
            return BadRequest("目标支付网关不可用, 请检查名称是否正确");
        }
        _mapper.Map(gatewayInDto, gateway);
        await _context.SaveChangesAsync();
        return Ok();
    }
    
    // DELETE: api/v1/admin/Gateway/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteGateway(int id)
    {
        var gateway = await _context.Gateway.FindAsync(id);
        if (gateway == null)
        {
            return NotFound("网关不存在");
        }
        //todo 解决删除时外键约束问题
        _context.Gateway.Remove(gateway);
        await _context.SaveChangesAsync();
        return Ok();
    }
    
    // GET: api/v1/admin/Gateway/available
    [HttpGet("available")]
    public ActionResult<List<string>> GetAvailableGateway()
    {
        var gateways = _paymentGatewayFactory.GetAvailableGateways();
        return Ok(gateways);
    }
}