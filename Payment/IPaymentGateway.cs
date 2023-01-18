using faka.Models;
using faka.Models.Dtos;

namespace faka.Payment;

public interface IPaymentGateway
{
    string Name { get; }
    Task<GatewayResponse> CreateAsync(Order order, OrderPayDto orderPayDto);
}