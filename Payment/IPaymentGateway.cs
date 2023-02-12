namespace FAKA.Server.Payment;

public interface IPaymentGateway
{
    string Name { get; }
    string ConfigSection { get; }
    Task<GatewayResponse> CreateAsync(PaymentRequest request);
}