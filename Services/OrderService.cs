using faka.Data;
using faka.Models;
using Microsoft.EntityFrameworkCore;

namespace faka.Services;

public class OrderService
{
    private readonly fakaContext _context;

    public OrderService(fakaContext context)
    {
        _context = context;
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
        var keys = await _context.Key.Where(k => k.ProductId == order.ProductId && k.IsUsed == false).Take(order.Quantity).ToListAsync();
        if (keys.Count < order.Quantity)
        {
            //todo 库存不足，写入 log 或者发送提醒邮件
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
        try { await _context.SaveChangesAsync(); }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //todo 写入 log 或者发送提醒邮件
            return false;
        }
        order.Status = OrderStatus.Completed;
        return true;
    }
}