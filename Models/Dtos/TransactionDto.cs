namespace faka.Models.Dtos;

public class TransactionInDto
{
    public string? PaymentMethod { get; set; } = "";
    public string? Gateway { get; set; } = "";
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; } = false;
    public string? GatewayTradeNumber { get; set; }
    public string TradeNumber { get; set; } = "";
    public string? Description { get; set; }

    public int? OrderId { get; set; }
    
    public string? UserId { get; set; }
}