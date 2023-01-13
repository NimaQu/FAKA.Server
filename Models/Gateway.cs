using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace faka.Models;

public class Gateway : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? FriendlyName { get; set; }
    public bool IsEnabled { get; set; } = true;
}