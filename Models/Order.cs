using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

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
    public decimal Price { get; set; }

    public int ProductId { get; set; }
    [JsonIgnore]
    public virtual Product Product { get; set; } = null!;
    public string? UserId { get; set; }
    
    [JsonIgnore]
    public virtual IdentityUser? User { get; set; }
}