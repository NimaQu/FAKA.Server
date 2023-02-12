namespace FAKA.Server.Models.Dtos;

public class GatewayOutDto
{
    public int Id { get; set; }
    public string? FriendlyName { get; set; }
}

public class GatewayInDto
{
    public string Name { get; set; } = null!;
    public string? FriendlyName { get; set; }
}