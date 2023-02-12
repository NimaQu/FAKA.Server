using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FAKA.Server.Models;

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

    public void SetComplete()
    {
        Status = OrderStatus.Completed;
    }
    
    public void SetPending()
    {
        Status = OrderStatus.Pending;
    }
    
    public void SetCancelled()
    {
        Status = OrderStatus.Canceled;
    }
    
    public void SetPaid()
    {
        Status = OrderStatus.Paid;
    }
}

public enum OrderStatus
{
    [EnumMember(Value = "pending")] Pending,
    [EnumMember(Value = "paid")] Paid,
    [EnumMember(Value = "completed")] Completed,
    [EnumMember(Value = "cancelled")] Canceled
}