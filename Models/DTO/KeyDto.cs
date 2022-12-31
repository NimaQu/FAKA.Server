namespace faka.Models.DTO;

public class KeyDto
{
    public int Id { get; init; }
    public string? Content { get; set; }

    public int ProductId { get; set; }
    public string? ProductName { get; set; }
}