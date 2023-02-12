namespace FAKA.Server.Models.Dtos;

public class AnnouncementOutDto
{
    public string? Content { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class AnnouncementInDto
{
    public string? Content { get; set; }
}