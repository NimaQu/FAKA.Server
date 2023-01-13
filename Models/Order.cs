using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using faka.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace faka.Models;

public class Order : BaseEntity
{
    public string? AccessCode { get; set; }
    public int Quantity { get; set; }
    public string Email { get; set; } = null!;

    [Precision(10, 2)] public decimal Amount { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public int ProductId { get; set; }

    [JsonIgnore] public Product? Product { get; set; }

    public string? UserId { get; set; }

    [JsonIgnore] public IdentityUser? User { get; set; }
    
    public List<AssignedKey>? AssignedKeys { get; set; }

    public void GenerateAccessCode()
    {
        var random = new Random();
        const string s = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var randomString = new string(Enumerable.Range(0, 64).Select(i => s[random.Next(s.Length)]).ToArray());
        AccessCode = randomString;
    }

    public void Complete()
    {
        Status = OrderStatus.Completed;
    }
}

public enum OrderStatus
{
    [EnumMember(Value = "pending")] Pending,
    [EnumMember(Value = "completed")] Completed,
    [EnumMember(Value = "cancelled")] Canceled
}