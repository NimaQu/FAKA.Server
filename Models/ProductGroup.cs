namespace FAKA.Server.Models;

public class ProductGroup : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }

    public List<Product>? Products { get; set; }
}