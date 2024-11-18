using Entities.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace TaskTreck.Extensions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpcontext, Exception exception, CancellationToken cancellation)
    {
        httpcontext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpcontext.Response.ContentType = "application/json";

        var message = exception switch
        {
            NotFoundException => ((NotFoundException)exception).Detail,
            BadRequestException => ((BadRequestException)exception).Detail,
            AccessViolationException => "Access violation error from the IExceptionHandler",
            _ => "Internal Server Error from the IExceptionHandler."
        };

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpcontext,
            ProblemDetails = 
            {
                Type = "An error occurred",
                Detail = message,
                Title = exception.GetType().Name,
                Status = (int)HttpStatusCode.InternalServerError,
                Instance = exception.Message,
            },
            Exception = exception
        });
    }
}
    