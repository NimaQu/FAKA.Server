using System.Text.Json.Serialization;

namespace FAKA.Server.Models;

public class Key : BaseEntity
{
    public string? Content { get; set; }
    public string? Batch { get; set; }
    public bool IsUsed { get; set; } = false;
    public int ProductId { get; set; }
    [JsonIgnore]
    public Product? Product { get; set; }
}