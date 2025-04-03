using Microsoft.AspNetCore.Diagnostics;
using WebApi.Errors;
using WebApi.Extensions;

namespace WebApi.Middlewares;

public static class ExceptionHandlerMiddleware
{
    public static void UseResultExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(exceptionApp =>
        {
            exceptionApp.Run(async context =>
            {
                var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();

                // ✅ Check if exceptionFeature exists
                if (exceptionFeature?.Error is null)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
                    return;
                }

                var exception = exceptionFeature.Error;

                // ✅ Handle known `IError` exceptions
                if (exception is IError error)
                {
                    context.Response.StatusCode = error.StatusCode ?? StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(error.AsHttpResult());
                    return;
                }

                // ✅ Handle all other exceptions as 500 Internal Server Error
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });

            });
        });
    }
}