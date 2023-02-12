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

namespace FAKA.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GatewayController : ControllerBase
{
    private readonly FakaContext _context;
    private readonly IMapper _mapper;
    private readonly PaymentGatewayFactory _paymentGatewayFactory;
    private readonly GatewayService _gatewayService;
    
    public GatewayController(FakaContext context, IMapper mapper, PaymentGatewayFactory paymentGatewayFactory, GatewayService gatewayService)
    {
        _context = context;
        _mapper = mapper;
        _paymentGatewayFactory = paymentGatewayFactory;
        _gatewayService = gatewayService;
    }
    
    [HttpGet, Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<List<Gateway>>> GetGateway()
    {
        var gateways = await _context.Gateway.ToListAsync();
        return Ok(gateways);
    }
    
    [HttpGet("{id}"), Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Gateway>> GetGateway(int id)
    {
        var gateway = await _context.Gateway.FindAsync(id);
        return Ok(gateway);
    }
    
    [HttpPost, Authorize(Roles = Roles.Admin)]
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
    
    [HttpPut("{id}"), Authorize(Roles = Roles.Admin)]
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
    
    [HttpDelete("{id}"), Authorize(Roles = Roles.Admin)]
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
    
    [HttpGet("available"), Authorize(Roles = Roles.Admin)]
    public ActionResult<List<string>> GetAvailableGateway()
    {
        var gateways = _paymentGatewayFactory.GetAvailableGateways();
        return Ok(gateways);
    }
}