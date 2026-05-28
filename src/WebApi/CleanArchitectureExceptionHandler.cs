namespace Hydron.WebApi;

using Hydron.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

internal sealed class CleanArchitectureExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, errorCode) = exception switch
        {
            ApplicationException applicationException => (StatusCodes.Status400BadRequest, "An application error occurred", applicationException.ErrorCode),
            DomainException domainException => (StatusCodes.Status422UnprocessableEntity, "A domain error occurred", domainException.ErrorCode),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", "unexpected_error")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        problemDetails.Extensions["errorCode"] = errorCode;
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
