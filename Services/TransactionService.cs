using faka.Data;
using faka.Models;
using faka.Payment;

namespace faka.Services;

public class TransactionService
{
    private readonly fakaContext _context;

    public TransactionService(fakaContext context)
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