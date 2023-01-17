using AutoMapper;
using faka.Data;
using faka.Payment;
using Microsoft.EntityFrameworkCore;

namespace faka.Services;

public class GatewayService
{
    private readonly fakaContext _context;
    private readonly IMapper _mapper;
    private readonly PaymentGatewayFactory _paymentGatewayFactory;
    
    public GatewayService(fakaContext context, IMapper mapper, PaymentGatewayFactory paymentGatewayFactory)
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