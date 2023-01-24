# 支付网关集成
1. 在 Payment/Gateways 目录下创建一个新的网关类，继承自 IPaymentGateway 接口

```csharp
public class YourGateway : IPaymentGateway{

}
```

2. 在 Program.cs 中向 IPaymentGateway 注册你的网关

```csharp
builder.Services.AddTransient<IPaymentGateway, YourPaymentGateway>();
```

3. 在 appsettings.json 中加入你的网关配置信息

```json
{
  "Payment": {
    "Gateways": {
        "OtherGateway": {
          "apikey": "OtherGateway"
        },
      "YourGatewayConfig": {
        "ApiKey": "114514",
        "WebhookSecret": "1919810"
      }
    }
  }
}
```

4. 实现 IPaymentGateway 接口, 目前需要实现的方法只有一个，即创建支付请求, 并给支付接口命名和配置文件 section 命名, 别忘了方法一定要是异步的

```csharp
public class YourGateway : IPaymentGateway {
    public string Name { get; } = "YourGateway";
    public string ConfigSection { get; } = "YourGatewayConfig";
    private readonly IConfiguration _configuration; // 依赖注入的配置文件
    
    // 构造函数, 依赖注入配置文件
    public YourGateway(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    
    public Task<GatewayResponse> CreateAsync(PaymentRequest paymentRequest)
    {
        //你的支付逻辑
        //返回一个 GatewayResponse 对象，其中包含支付状态和前端需要处理的数据等等
        return new GatewayResponse
        {
            Status = GatewayResponseStatus.Success,
            GatewayTradeNumber = "你网关返回的交易号码，如果没有可以直接用内部的交易号，然后把这个号码从 Webhook 中传回来",
            Gateway = Name,
            Data = "你的网关数据，可以为任意对象，将会被 json 序列化后传给前端"
        };
    }
    
    // 读取配置文件示例
    public string GetApiKey()
    {
        return _configuration[$"PaymentGateways:{ConfigSection}:ApiKey"];
    }
}
```

5. 在控制器中实现 Webhook 逻辑，如果你对 Asp.Net Core 不熟悉，可以参考 [Asp.Net Core 创建 API 控制器](https://learn.microsoft.com/zh-cn/aspnet/core/tutorials/first-web-api?view=aspnetcore-7.0&tabs=visual-studio#routing-and-url-paths)，也可以照葫芦画瓢，其他接口有例子

找到 WebhookController.cs， 添加一个新的方法
```csharp
// POST /api/Webhook/YourGateway
[HttpPost("YourGateway")]
public async Task<IActionResult> YourGateway()
{
    //这里是你的 Webhook 逻辑，验签，错误处理等等
    //一定要在验证成功后调用 _orderService.FulfillOrderAsync() 传入网关的交易号码
    if (success) {
        await _orderService.FulfillOrderAsync(GatewayTradeNumber);
    }
    return Ok();
}
```