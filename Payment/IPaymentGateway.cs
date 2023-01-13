using faka.Models;

namespace faka.Payment;

public interface IPaymentGateway
{
    string Name { get; }
    Task<GatewayResponse> CreateAsync(Order order);
}