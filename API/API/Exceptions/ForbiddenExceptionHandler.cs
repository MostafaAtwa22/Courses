using Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Exceptions
{
    public class ForbiddenExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<ForbiddenExceptionHandler> logger)
        : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not ForbiddenException ex)
                return false;

            logger.LogWarning(ex, "Forbidden exception occurred: {Message}", ex.Message);

            var problemDetails = new ProblemDetails
            {
                Title = "Forbidden.",
                Status = StatusCodes.Status403Forbidden,
                Detail = ex.Message
            };

            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });

            return true;
        }
    }
}