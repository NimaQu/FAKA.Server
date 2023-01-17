namespace faka.Payment;

public class PaymentGatewayFactory
{
    private readonly IServiceProvider _serviceProvider;


    public PaymentGatewayFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPaymentGateway Create(string paymentGatewayName)
    {
        var paymentGateways = _serviceProvider.GetServices<IPaymentGateway>();
        var gateway = paymentGateways.FirstOrDefault(p => p.Name == paymentGatewayName);
        if (gateway == null) throw new Exception($"Payment gateway {paymentGatewayName} not found");
        return gateway;
    }
    
    public IEnumerable<string> GetAvailableGateways()
    {
        var paymentGateways = _serviceProvider.GetServices<IPaymentGateway>();
        return paymentGateways.Select(p => p.Name).ToList();
    }
}