using System.ComponentModel.DataAnnotations;

namespace FAKA.Server.Models.Dtos;

public class OrderOutDto
{
    public int Id { get; init; }
    public int Quantity { get; set; }
    public string? AccessCode { get; set; }
    public decimal Amount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public int ProductId { get; set; }
    public ProductOutDto? Product { get; set; }
    public List<GatewayOutDto>? Gateways { get; set; }
    public List<AssignedKeyOutDto?>? AssignedKeys { get; set; }
}

public class OrderInDto
{
    public int Quantity { get; set; }
    public string Email { get; set; } = null!;
    public decimal Price { get; set; }
    public int ProductId { get; set; }
    public string? UserId { get; set; }
}

public class OrderSubmitDto
{
    public int Quantity { get; set; }

    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string? Email { get; set; }

    public int ProductId { get; set; }
}

public class OrderPayDto
{
    public int GatewayId { get; set; }
    public string? ReturnUrl { get; set; }
}