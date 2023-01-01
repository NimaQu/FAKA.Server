namespace faka.Models.Dtos;

public class ProductGroupInDto
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public class ProductGroupOutDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    public List<ProductOutDto> Products { get; set; }
}