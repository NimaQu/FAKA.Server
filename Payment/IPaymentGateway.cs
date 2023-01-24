namespace faka.Payment;

public interface IPaymentGateway
{
    string Name { get; }
    string ConfigSection { get; }
    Task<GatewayResponse> CreateAsync(PaymentRequest request);
}