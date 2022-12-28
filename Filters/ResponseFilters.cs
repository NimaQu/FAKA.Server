using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace faka.Filters;

public class CustomResultFilterAttribute  : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        //很傻逼的写法等一个有缘人重构
        switch (context.Result)
        {
            case ObjectResult result:
            {
                var statusCode = result.StatusCode;
                var message = string.Empty;
                switch (statusCode)
                {
                    case 200:
                        message = "0";
                        break;
                    case 400:
                        if (result.Value != null) message = result.Value.ToString() ?? "Bad Request";
                        break;
                    case 401:
                        if (result.Value != null) message = result.Value.ToString() ?? "Unauthorized";
                        break;
                    case 500:
                        if (result.Value != null) message = result.Value.ToString() ?? "Internal Server Error";
                        break;
                    default:
                        message = "记得提醒我少写了个 case";
                        break;
                }
            
                if (statusCode == 200)
                {
                    context.Result = new JsonResult(new
                    {
                        code = 0,
                        message = message,
                        data = result.Value
                    });
                }
                else
                {
                    context.Result = new JsonResult(new
                    {
                        code = statusCode,
                        message = message,
                        data = new {}
                    });
                }

                break;
            }
            case StatusCodeResult statusCodeResult:
            {
                var statusCode = statusCodeResult.StatusCode;
                var message = statusCode switch
                {
                    200 => "0",
                    400 => "Bad Request",
                    401 => "Unauthorized",
                    _ => "记得提醒我少写了个 case"
                };
                if (statusCode == 200) statusCode = 0;
                {
                    context.Result = new JsonResult(new
                    {
                        code = statusCode,
                        message = message,
                        data = new {}
                    });
                }
                break;
            }
            default:
                base.OnResultExecuting(context);
                break;
        }
    }
}