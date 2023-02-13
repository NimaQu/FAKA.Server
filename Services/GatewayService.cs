using AutoMapper;
using FAKA.Server.Data;
using FAKA.Server.Payment;
using Microsoft.EntityFrameworkCore;

namespace FAKA.Server.Services;

public class GatewayService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly PaymentGatewayFactory _paymentGatewayFactory;
    
    public GatewayService(ApplicationDbContext context, IMapper mapper, PaymentGatewayFactory paymentGatewayFactory)
    {
        _context = context;
        _mapper = mapper;
        _paymentGatewayFactory = paymentGatewayFactory;
    }
    
    public bool IsGatewayAvailable(string gatewayName)
    {
        var gatewayAvailable = _paymentGatewayFactory.GetAvailableGateways();
        return gatewayAvailable.Any(x => x == gatewayName);
    }
    
    public async Task<bool> IsGatewayExistAsync(string gatewayName)
    {
        var gatewayExist = _context.Gateway.AnyAsync(x => x.Name == gatewayName);
        return await gatewayExist;
    }
}