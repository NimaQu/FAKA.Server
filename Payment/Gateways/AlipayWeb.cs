using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;

namespace FAKA.Server.Payment.Gateways;

public class AlipayWeb : IPaymentGateway
{
    public string Name { get; } = "AlipayWeb";
    public string ConfigSection { get; } = "Alipay";
    private readonly IConfiguration _configuration;

    public AlipayWeb(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<GatewayResponse> CreateAsync(PaymentRequest paymentRequest)
    {
        var order = paymentRequest.Order;
        var product = order.Product;
        if (product == null)
        {
            throw new Exception("Product not found");
        }
        var client = GetClient();
        var alipayRequest = new AlipayTradePagePayRequest();
        alipayRequest.SetNotifyUrl(_configuration["BaseUrls:Server"] + "/api/callback/alipay");
        alipayRequest.SetReturnUrl(paymentRequest.ReturnUrl);
        var model = new AlipayTradePagePayModel
        {
            OutTradeNo = paymentRequest.TradeNumber,
            TotalAmount = order.Amount.ToString("g2"),
            Subject = product.Name,
            ProductCode = "FAST_INSTANT_TRADE_PAY"
        };
        alipayRequest.SetBizModel(model);
        var response = client.pageExecute(alipayRequest);
        return new GatewayResponse
        {
            Status = GatewayStatus.Success,
            GatewayTradeNumber = paymentRequest.TradeNumber, //因为这里没有返回支付宝的交易号，所以这里直接用我们自己的交易号
            Data = response.Body,
            Gateway = Name
        };
    }

    private IAopClient GetClient()
    {
        var aliPayConfig = _configuration.GetSection($"PaymentGateways:Alipay")
            .GetChildren()
            .ToDictionary(x => x.Key, x => x.Value);
        IAopClient client;
        try
        {
            client = new DefaultAopClient(
                serverUrl: "https://openapi.alipay.com/gateway.do",
                appId: aliPayConfig["AppId"],
                privateKeyPem: aliPayConfig["MerchantPrivateKey"],
                alipayPulicKey: aliPayConfig["AlipayPublicKey"],
                format: "json",
                version: "1.0",
                signType: "RSA2",
                charset: "utf-8");
        }
        catch (NullReferenceException e)
        {
            throw new Exception("支付宝配置不完整:", e);
        }
        catch (Exception e)
        {
            throw new Exception("支付宝初始化失败:", e);
        }

        return client;
    }
}