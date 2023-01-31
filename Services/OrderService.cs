using faka.Data;
using faka.Models;
using faka.Models.Dtos;
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
    
    /// <summary>
    /// 使用指定网关创建支付请求
    /// </summary>
    /// <param name="order"></param>
    /// <param name="gateway"></param>
    /// <param name="orderPayDto"></param>
    /// <returns></returns>

    public async Task<GatewayResponse> CreatePaymentAsync(Order order, Gateway gateway, OrderPayDto orderPayDto)
    {
        var request = new PaymentRequest
        {
            Order = order,
            Gateway = gateway,
            ReturnUrl = orderPayDto.ReturnUrl,
            TradeNumber = GenTradeNumber()
        };
        var payment = _paymentGatewayFactory.Create(gateway.Name);
        var res = await payment.CreateAsync(request);

        await _transactionService.CreateAsync(request, res);
        return res;
    }
    
    public async Task<int> GetAvailableKeyCountAsync(int productId)
    {
        return await _context.Key.CountAsync(k => k.ProductId == productId && k.IsUsed == false);
    }

    /// <summary>
    /// 使用 AccessCode 获取订单
    /// </summary>
    /// <param name="accessCode">订单访问代码</param>
    /// <returns></returns>
    public async Task<Order?> GetOrderAsync(string accessCode)
    {
        return await _context.Order
            .Include(o => o.Product)
            .Include(o => o.AssignedKeys)
            .FirstOrDefaultAsync(o => o.AccessCode == accessCode);
    }
    
    /// <summary>
    /// 使用订单 ID 获取订单
    /// </summary>
    /// <param name="id">订单 ID</param>
    /// <returns></returns>
    public async Task<Order?> GetOrderAsync(int id)
    {
        return await _context.Order
            .Include(o => o.Product)
            .Include(o => o.AssignedKeys)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    /// <summary>
    /// 获取指定用户的所有订单, 使用参数 perPage 和 page 分页
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="perPage">每页的数量</param>
    /// <param name="page">页数</param>
    /// <returns>List of Order</returns>
    /// <exception cref="Exception"></exception>
    public async Task<List<Order>> GetOrdersAsync (string userId, int perPage, int page)
    {
        if (perPage < 1 || page < 1) throw new Exception("Invalid perPage or page");
        var orders = await _context.Order
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.Id)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync();

        return orders;
    }
    
    /// <summary>
    /// 获取所有订单, 使用参数 perPage 和 page 分页
    /// </summary>
    /// <param name="perPage">每页的数量</param>
    /// <param name="page">页数</param>
    /// <returns>List of Order</returns>
    /// <exception cref="Exception"></exception>
    public async Task<List<Order>> GetOrdersAsync (int perPage, int page)
    {
        //获取所有订单，使用参数 perPage 和 page 分页
        if (perPage < 1 || page < 1) throw new Exception("Invalid perPage or page");
        var orders = await _context.Order
            .OrderByDescending(o => o.Id)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync();
        
        return orders;
    }
    
    /// <summary>
    /// 分配激活码，更新库存，更新订单状态
    /// </summary>
    /// <param name="gatewayTradeNumber">网关交易号</param>
    /// <exception cref="Exception"></exception>

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
        if (result)
        {
            order.SetComplete();
            await UpdateProductStockAsync(order.ProductId, order.Quantity);
        }
        else
        {
            //todo 库存不足，写入 log 或者发送提醒邮件
            order.SetPaid();
        }
        await _context.SaveChangesAsync();
        //todo 发送邮件/WebHook通知客户
    }
    
    private async Task UpdateProductStockAsync(int productId, int quantity)
    {
        var product = await _context.Product.FindAsync(productId);
        if (product is null) throw new Exception("Product not found");
        product.Stock -= quantity;
        await _context.SaveChangesAsync();
    }

    private async Task<bool> AssignKeysAsync(Order order)
    {
        var keys = await _context.Key.Where(k => k.ProductId == order.ProductId && k.IsUsed == false)
            .Take(order.Quantity).ToListAsync();
        if (keys.Count < order.Quantity)
        {
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
    private static string GenTradeNumber()
    {
        return $"{DateTime.Now:yyyyMMddHHmmssfff}{new Random().Next(1000, 9999)}";
    }
}