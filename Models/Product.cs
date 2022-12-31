using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace faka.Models;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
    [Required]
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool IsHidden { get; set; } = false;
    public int Stock { get; set; }
    
    [JsonIgnore]
    public virtual List<Order>? Orders { get; set; }
    [JsonIgnore]
    public virtual List<Key>? Keys { get; set; }
}