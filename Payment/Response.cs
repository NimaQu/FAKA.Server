namespace faka.Payment;

public class GatewayResponse
{
    public string Status { get; set; }
    public string TradeNumber { get; set; }
    public string PaymentUrl { get; set; }
}