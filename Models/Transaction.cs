using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace faka.Models;

public class Transaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? PaymentMethod { get; set; } = "";
    [Precision(10, 2)]
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; } = false;
    public string? GatewayTradeNumber { get; set; }
    public string TradeNumber { get; set; } = "";
    public string? Description { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? UpdatedAt { get; set; }
    
    
    public int? GatewayId { get; set; }
    [JsonIgnore]
    public Gateway? Gateway { get; set; }

    public int? OrderId { get; set; }
    [JsonIgnore]
    public Order? Order { get; set; }
    
    public string? UserId { get; set; }
    [JsonIgnore]
    public IdentityUser? User { get; set; }
}