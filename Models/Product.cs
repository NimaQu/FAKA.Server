using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace faka.Models;

public class Product : BaseEntity
{
    [Required] public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Required] [Precision(10, 2)] public decimal Price { get; set; }

    public bool IsEnabled { get; set; } = true;
    public bool IsHidden { get; set; } = false;
    public int Stock { get; set; }
    public int ProductGroupId { get; set; }
    public ProductGroup? ProductGroup { get; set; }

    [JsonIgnore] public List<Order>? Orders { get; set; }

    [JsonIgnore] public List<Key>? Keys { get; set; }
}