using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace faka.Models;

public class Announcement : BaseEntity
{
    public string? Content { get; set; }
}