namespace FAKA.Server.Models;

public class Gateway : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? FriendlyName { get; set; }
    public bool IsEnabled { get; set; } = true;
}