using faka.Data;
using faka.Models;
using faka.Payment;
using Microsoft.EntityFrameworkCore;

namespace faka.Services;

public class OrderService
{
    private readonly fakaContext _context;
    private readonly PaymentGatewayFactory _paymentGatewayFactory;
    private readonly TransactionService _transactionService;

    public OrderService(fakaContext context, PaymentGatewayFactory paymentGatewayFactory, TransactionService transactionService)
    {
        _context = context;
        _paymentGatewayFactory = paymentGatewayFactory;
        _transactionService = transactionService;
    }

    public async Task<GatewayResponse> CreatePaymentAsync(Order order, Gateway gateway)
    {
        var payment = _paymentGatewayFactory.Create(gateway.Name);
        var res = await payment.CreateAsync(order);

        await _transactionService.CreateAsync(order, gateway, res);
        return res;
    }

    public async Task<Order?> GetOrderByCodeAsync(string accessCode)
    {
        return await _context.Order
            .Include(o => o.Product)
            .FirstOrDefaultAsync(o => o.AccessCode == accessCode);
    }
    
    public async Task<Order?> GetOrderAsync(int id)
    {
        return await _context.Order
            .Include(o => o.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task FulfillOrderAsync(string gatewayTradeNumber)
    {
        var transaction = await _context.Transaction.Include(t => t.Order)
            .FirstOrDefaultAsync(t => t.GatewayTradeNumber == gatewayTradeNumber);
        if (transaction is null) throw new Exception("Transaction not found");
        transaction.SetPaid();
        var order = transaction.Order;
        if (order is null) throw new Exception("Order not found");
        // 错误处理
        var result = await AssignKeysAsync(order);
        await _context.SaveChangesAsync();
        //todo 发送邮件/WebHook通知客户
    }

    private async Task<bool> AssignKeysAsync(Order order)
    {
        var keys = await _context.Key.Where(k => k.ProductId == order.ProductId && k.IsUsed == false)
            .Take(order.Quantity).ToListAsync();
        if (keys.Count < order.Quantity)
        {
            //todo 库存不足，写入 log 或者发送提醒邮件
            order.SetPaid();
            await _context.SaveChangesAsync();
            return false;
        }

        foreach (var key in keys)
        {
            key.IsUsed = true;
            _context.Key.Update(key);
            _context.AssignedKey.Add(new AssignedKey
            {
                OrderId = order.Id,
                Content = key.Content
            });
        }
        
        order.SetComplete();
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //todo 写入 log 或者发送提醒邮件
            return false;
        }
        return true;
    }
}