using faka.Data;
using Microsoft.AspNetCore.SignalR;

namespace faka.Hubs;

public class PaymentHub : Hub
{
    private readonly fakaContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public Dictionary<string, string> _connectionIds = new();

    public PaymentHub(fakaContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetAccessCode(string accessCode)
    {
        _connectionIds[accessCode] = Context.ConnectionId;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        // 删除连接 ID
        var accessCode = _connectionIds.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
        _connectionIds.Remove(accessCode);
        return base.OnDisconnectedAsync(exception);
    }
}