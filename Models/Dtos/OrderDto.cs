using System.ComponentModel.DataAnnotations;

namespace faka.Models.Dtos;

public class OrderOutDto
{
    public int Id { get; init; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public List<GatewayOutDto>? Gateways { get; set; }
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
}