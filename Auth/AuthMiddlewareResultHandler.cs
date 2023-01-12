using faka.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace faka.Auth;

public class AuthMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        var code = 500;
        var message = "Auth Middleware Unknown error";
        object data;
        if (authorizeResult.AuthorizationFailure != null)
            data = authorizeResult.AuthorizationFailure;
        else
            data = new { };

        if (authorizeResult.Succeeded)
        {
            await next(context);
            return;
        }

        if (authorizeResult.Forbidden)
        {
            code = 403;
            message = "无权访问的资源";
        }
        else if (authorizeResult.Challenged)
        {
            code = 401;
            message = "未授权的访问";
        }

        var response = new ResponseModel
        {
            Code = code,
            Message = message,
            Data = data
        };
        await context.Response.WriteAsJsonAsync(response);
    }
}