namespace FAKA.Server.Models.Dtos;

public class KeyOutDto
{
    public int Id { get; init; }
    public string? Content { get; set; }

    public int ProductId { get; set; }
    public string? ProductName { get; set; }
}

public class KeyInDto
{
    public string? Content { get; set; }
    public string? Batch { get; set; }
    public int ProductId { get; set; }
}

public class KeyBatchInDto
{
    public IEnumerable<string> Contents { get; set; } = Array.Empty<string>();
    public string? Batch { get; set; }
    public int ProductId { get; set; }
}