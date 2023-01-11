using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace faka.Models;

public class Order : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }
    public string? AccessCode { get; set; }
    public int Quantity { get; set; }
    public string Email { get; set; } = null!;
    [Precision(10, 2)]
    public decimal Price { get; set; }
    public int ProductId { get; set; }
    [JsonIgnore]
    public Product? Product { get; set; }
    public string? UserId { get; set; }
    
    [JsonIgnore]
    public IdentityUser? User { get; set; }
    
    public void GenerateAccessCode()
    {
        var random = new Random();
        const string s = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var randomString = new string(Enumerable.Range(0, 64).Select(i => s[random.Next(s.Length)]).ToArray());
        AccessCode = randomString;
    }
}