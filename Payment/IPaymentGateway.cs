namespace faka.Payment;

public interface IPaymentGateway
{
    string Name { get; }
    Task<string> CreatePaymentAsync(decimal amount);
}
