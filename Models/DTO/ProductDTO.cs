﻿namespace faka.Models.DTO;

public class ProductDto
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int Stock { get; set; }
}