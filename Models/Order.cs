using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace faka.Models;

public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }
    public int Quantity { get; set; }
    public string Email { get; set; } = null!;
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? CreatedAt { get; set; }
    [Precision(10, 2)]
    public decimal Price { get; set; }
    public int ProductId { get; set; }
    [JsonIgnore]
    public Product? Product { get; set; }
    public string? UserId { get; set; }
    
    [JsonIgnore]
    public IdentityUser? User { get; set; }
}