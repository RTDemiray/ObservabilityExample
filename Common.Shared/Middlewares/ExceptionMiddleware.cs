using Common.Shared.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Common.Shared.Middlewares;

public static class ExceptionMiddleware
{
    public static void UseExceptionMiddleware(this WebApplication app)
    {
        app.UseExceptionHandler(config =>
        {
            config.Run(async context =>
            {
                var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var response = ResponseDto<string>.Fail(StatusCodes.Status500InternalServerError,
                    exceptionFeature!.Error.Message);
                
                await context.Response.WriteAsJsonAsync(response);
            });
        });
    }
}