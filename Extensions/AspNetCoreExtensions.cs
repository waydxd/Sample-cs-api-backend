using System.Net;
using System.Text;

namespace TodoApi.Extensions;

/// <summary>Extension methods for configuring the ASP.NET Core request pipeline.</summary>
public static class AspNetCoreExtensions
{
    /// <summary>
    /// Adds a global exception-handling middleware that catches unhandled exceptions
    /// and returns RFC 7807 Problem Details (JSON) responses.
    /// </summary>
    /// <param name="app">The application builder.</param>
    public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next(context);
            }
            catch (BadHttpRequestException ex)
            {
                // Return a 400 Bad Request for malformed requests (e.g. invalid route data).
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(new
                {
                    status = 400,
                    title = "Bad Request",
                    detail = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                // Log the error to stderr and return a generic 500 response.
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

    /// <summary>
    /// Adds middleware that logs the request body of POST requests to stderr.
    /// Uses <c>EnableBuffering</c> so the body can still be read by downstream middleware.
    /// </summary>
    /// <param name="app">The application builder.</param>
    public static void UseRequestBodyLogging(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.ContentLength > 0 && context.Request.Method == HttpMethods.Post)
            {
                context.Request.EnableBuffering();
                var body = await new StreamReader(context.Request.Body, Encoding.UTF8).ReadToEndAsync();
                context.Request.Body.Position = 0;
                await Console.Error.WriteLineAsync($"[Request Body] {context.Request.Path}: {body}");
            }
            await next(context);
        });
    }
}
