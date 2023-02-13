using FAKA.Server.Data;
using FAKA.Server.Models;
using FAKA.Server.Payment;

namespace FAKA.Server.Services;

public class TransactionService
{
    private readonly ApplicationDbContext _context;

    public TransactionService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task CreateAsync(PaymentRequest request, GatewayResponse gatewayResponse)
    {
        var transaction = new Transaction
        {
            Amount = request.Order.Amount,
            TradeNumber = request.TradeNumber,
            GatewayTradeNumber = gatewayResponse.GatewayTradeNumber,
            OrderId = request.Order.Id,
            GatewayId = request.Gateway.Id,
            UserId = request.Order.UserId
        };
        _context.Transaction.Add(transaction);
        await _context.SaveChangesAsync();
    }
}