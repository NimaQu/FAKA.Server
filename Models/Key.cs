using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace faka.Models;

public class Key : BaseEntity
{
    public string? Content { get; set; }
    public string? Batch { get; set; }
    public bool IsUsed { get; set; } = false;
    public int ProductId { get; set; }
    [JsonIgnore]
    public Product? Product { get; set; }
}