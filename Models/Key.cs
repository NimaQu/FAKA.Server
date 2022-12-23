using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace faka.Models;

public class Key
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public int Id { get; init; }
    public string? Content { get; set; }
    [JsonIgnore]
    public string? Batch { get; set; }
    public bool IsUsed { get; set; } = false;
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; }

    [JsonIgnore]
    public int? ProductId { get; set; }
    [JsonIgnore]
    public virtual Product? Product { get; set; }
}