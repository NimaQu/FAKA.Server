namespace faka.Models.DTO;

public class OrderDto
{
    public int Id { get; init; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
}