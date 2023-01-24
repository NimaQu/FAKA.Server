using System.Text.Json.Serialization;

namespace faka.Payment;

public class GatewayResponse
{
    public GatewayStatus Status { get; set; }
    // 网关交易号
    [JsonIgnore]
    public string GatewayTradeNumber { get; set; }
    // 网关名字
    public string Gateway { get; set; }
    // 承载从网关返回的数据给前端处理
    public object Data { get; set; }
}

public enum GatewayStatus
{
    Success,
    Failed
}