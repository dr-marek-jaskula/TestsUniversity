using Microsoft.AspNetCore.Mvc;
using Orders.Api.Exceptions;

namespace Orders.Api.Middlewares;
/// <summary>
/// Middleware that provides exception handling. Each request needs to be processed by the following try-catch block
/// </summary>
public class ErrorHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (NotFoundException notFoundException)
        {
            context.Response.StatusCode = 404;
            await HandleExceptionAsync(context, notFoundException);
        }
        catch (BadRequestException badRequestException)
        {
            context.Response.StatusCode = 400;
            await HandleExceptionAsync(context, badRequestException);
        }
        catch (Exception exception)
        {
            context.Response.StatusCode = 500;
            await HandleExceptionAsync(context, exception);
        }
    }

    /// <summary>
    /// Handler that sends the enough information to the user so that he would be able to identify the problem and explain the details to developer.
    /// However, the detail does not cover the whole problem, in order to prevent the user from getting the sensitive or unreadable data.
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <param name="exception">Request exception</param>
    /// <returns>Exception details in JSON format</returns>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        //Return machine-readable problem details. See RFC 7807 for details.
        var problemDetails = new ProblemDetails
        {
            Type = "https://MyWebApi.com/errors/internal-server-error",
            Title = "An unrecoverable error occurred",
            Status = context.Response.StatusCode,
            Detail = $"This is a demo error used to demonstrate problem details: {exception.Message}",
            Instance = context.Request.Path
        };
        //Then add additional data that can be used for filtering
        problemDetails.Extensions.Add("RequestId", context.TraceIdentifier);
        await context.Response.WriteAsJsonAsync(problemDetails, problemDetails.GetType(), null, contentType: "application/problem+json");
    }
}