using Microsoft.Extensions.Options;

namespace faka.Payment.Gateways;

public class StripeAlipayPaymentGateway : IPaymentGateway
{
    public string Name => "stripe-alipay";

    public StripeAlipayPaymentGateway(IOptionsSnapshot<Dictionary<string, Dictionary<string, object>>> options)
    {
        var gatewayConfig = options.Value[Name];
    }

    public async Task<string> CreatePaymentAsync(decimal amount)
    {
        // 使用 Stripe API 创建支付
        return "https://stripe.com/pay/123456";
    }
}

public class StripeCardPaymentGateway : IPaymentGateway
{
    public string Name => "stripe-card";

    public StripeCardPaymentGateway(IOptionsSnapshot<Dictionary<string, Dictionary<string, object>>> options)
    {
        var gatewayConfig = options.Value[Name];
        Console.WriteLine(gatewayConfig["ApiKey"]);
    }

    public async Task<string> CreatePaymentAsync(decimal amount)
    {
        // 使用 Stripe API 创建支付
        return "card payment";
    }
}