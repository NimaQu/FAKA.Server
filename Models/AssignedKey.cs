using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace faka.Models;

public class AssignedKey : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? Content { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }
    
    public string? UserId { get; set; }
    public IdentityUser? User { get; set; }
}