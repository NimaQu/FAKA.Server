namespace faka.Filters;

public class ResponseModel
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public object Data { get; set; } = new { };
}