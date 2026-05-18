using System.Net;

namespace TodoApi.Extensions;

public static class AspNetCoreExtensions
{
    public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"[Error]: {ex.Message}");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(new
                {
                    status = 500,
                    title = "Internal Server Error",
                    detail = ex.Message
                });
            }
        });
    }
}
