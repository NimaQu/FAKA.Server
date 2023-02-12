using Microsoft.AspNetCore.Identity;

namespace FAKA.Server.Models;

public class AssignedKey : BaseEntity
{
    public string? Content { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }
    
    public string? UserId { get; set; }
    public IdentityUser? User { get; set; }
}