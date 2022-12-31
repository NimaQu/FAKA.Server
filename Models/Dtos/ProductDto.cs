namespace faka.Models.Dtos;

public class ProductOutDto
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int Stock { get; set; }
}

public class ProductInDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsHidden { get; set; }
    public int Stock { get; set; }
}