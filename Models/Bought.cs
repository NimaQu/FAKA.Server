using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace faka.Models;

public class Bought
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public int Id { get; init; }
    public int Quantity { get; set; }
    [JsonIgnore]
    public string Email { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? CreatedAt { get; set; }
    [JsonIgnore]
    public int ProductId { get; set; }
    public virtual Product Product { get; set; }
    [JsonIgnore]
    public string? UserId { get; set; }
    [JsonIgnore]
    public virtual IdentityUser? User { get; set; }
}