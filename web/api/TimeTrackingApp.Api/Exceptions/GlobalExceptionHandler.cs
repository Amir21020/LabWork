using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace TimeTrackingApp.Api.Exceptions;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unhandled exception has occurred: {Message}", exception.Message);

        var (statusCode, title, details) = exception switch
        {
            ValidationException validation =>
                (StatusCodes.Status400BadRequest, "Validation Error", HandleValidationException(validation)),

            ArgumentException =>
                (StatusCodes.Status400BadRequest, "Bad Request",
                    new { Message = exception.Message }),

            KeyNotFoundException =>
                (StatusCodes.Status404NotFound, "Not Found",
                    new { Message = exception.Message }),

            InvalidOperationException =>
                (StatusCodes.Status400BadRequest, "Invalid Operation",
                    new { Message = exception.Message }),

            DbException =>
                (StatusCodes.Status503ServiceUnavailable, "Database Error",
                    new { Message = "A database error occurred while processing your request." }),

            OperationCanceledException => (StatusCodes.Status499ClientClosedRequest,
                "Request Cancelled", new { Message = "The request was cancelled." }),

            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", HandleUnknownException())
        };

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = $"https://httpstatuses.com/{statusCode}",
            Detail = details is string detailString ? detailString : null
        };

        if (details is not string)
        {
            problemDetails.Extensions["errors"] = details;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static object HandleValidationException(ValidationException exception)
    {
        return exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );
    }

    private static object HandleUnknownException()
    {
        return new
        {
            Message = "An unexpected internal server error has occurred."
        };
    }
}
