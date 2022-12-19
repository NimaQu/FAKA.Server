using faka.Models;

namespace faka.Services;

public class ProductService
{
    private static List<Product> Products { get; }

    static ProductService()
    {
        Products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Price = 100 },
            new Product { Id = 2, Name = "Product 2", Price = 200, CategoryId = 0 },
            new Product { Id = 3, Name = "Product 3", Price = 300, IsEnabled = true },
            new Product { Id = 4, Name = "Product 4", Price = 400, IsHidden = true },
            new Product { Id = 5, Name = "Product 5", Price = 500, Description = "114514" },
            new Product { Id = 6, Name = "Product 6", Price = 600, Stock = 0 },
            new Product { Id = 7, Name = "Product 7", Price = 700 },
            new Product { Id = 8, Name = "Product 8", Price = 800 },
            new Product { Id = 9, Name = "Product 9", Price = 900 },
            new Product { Id = 10, Name = "Product 10", Price = 1000 },
        };
    }
    
    public static List<Product> GetAllProducts()
    {
        return Products;
    }
    
    public static Product? GetProductById(int id)
    {
        return Products.FirstOrDefault(p => p.Id == id);
    }
    
    public static void AddProduct(Product product)
    {
        Products.Add(product);
    }
    
    public static void UpdateProduct(Product product)
    {
        var index = Products.FindIndex(p => p.Id == product.Id);
        if (index != -1)
        {
            Products[index] = product;
        }
    }
    
    public static void DeleteProduct(int id)
    {
        var index = Products.FindIndex(p => p.Id == id);
        if (index != -1)
        {
            Products.RemoveAt(index);
        }
    }
}