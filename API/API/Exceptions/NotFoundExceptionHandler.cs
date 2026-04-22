using Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Exceptions
{
    public class NotFoundExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<NotFoundExceptionHandler> logger)
        : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not NotFoundException ex)
                return false;

            logger.LogWarning(ex, "Not found exception occurred: {Message}", ex.Message);

            var problemDetails = new ProblemDetails
            {
                Title = "Resource not found.",
                Status = StatusCodes.Status404NotFound,
                Detail = ex.Message
            };

            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });

            return true;
        }
    }
}