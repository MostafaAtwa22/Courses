using Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Exceptions
{
    public class ConflictExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<ConflictExceptionHandler> logger)
        : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not ConflictException ex)
                return false;

            logger.LogWarning(ex, "Conflict exception occurred: {Message}", ex.Message);

            var problemDetails = new ProblemDetails
            {
                Title = "Conflict occurred.",
                Status = StatusCodes.Status409Conflict,
                Detail = ex.Message
            };

            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });

            return true;
        }
    }
}