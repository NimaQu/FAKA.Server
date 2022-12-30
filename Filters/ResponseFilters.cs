using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NuGet.Protocol.Plugins;

namespace faka.Filters;

public class CustomResultFilterAttribute  : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        // 获取响应结果
        var result = context.Result;
        var message = string.Empty;

        switch (result)
        {
            case StatusCodeResult statusCodeResult:
            {
                // 如果是 StatusCodeResult，将其转换为 ApiResponse
                var apiResponse = new ResponseModel
                {
                    Code = statusCodeResult.StatusCode,
                    Message = message
                };

                context.Result = new JsonResult(apiResponse);
                break;
            }
            case ObjectResult objectResult:
            {
                var code = objectResult.StatusCode ?? StatusCodes.Status500InternalServerError;
                var data = objectResult.Value;
                if (code != StatusCodes.Status200OK)
                {
                    message = objectResult.Value?.ToString() ?? string.Empty;
                    data = new {};
                }
                var apiResponse = new ResponseModel
                {
                    Code = objectResult.StatusCode.GetValueOrDefault(),
                    Message = message,
                    Data = data ?? new {}
                };

                context.Result = new JsonResult(apiResponse);
                break;
            }
            default:
            {
                var response = new ResponseModel
                {
                    Code = 233,
                    Message = "未知错误",
                    Data = new {}
                };
                context.Result = new JsonResult(response);
                break;
            }
        }
    }
}