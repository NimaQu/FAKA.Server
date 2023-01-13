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
    
    public async Task CreateAsync(Order order, Gateway gateway, GatewayResponse gatewayResponse)
    {
        var transaction = new Transaction
        {
            Amount = order.Amount,
            GatewayTradeNumber = gatewayResponse.TradeNumber,
            OrderId = order.Id,
            GatewayId = gateway.Id,
            UserId = order.UserId
        };
        _context.Transaction.Add(transaction);
        await _context.SaveChangesAsync();
    }
}