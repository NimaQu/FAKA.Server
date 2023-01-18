namespace faka.Payment;

public class GatewayResponse
{
    public GatewayStatus Status { get; set; }
    // 网关交易号
    public string TradeNumber { get; set; }
    // 网关名字
    public string Gateway { get; set; }
    // 支付跳转连接
    public string PaymentUrl { get; set; }
}

public enum GatewayStatus
{
    Success,
    Failed
}