using Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Exceptions
{
    public class UnauthorizedExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<UnauthorizedExceptionHandler> logger)
        : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not UnauthorizedException ex)
                return false;

            logger.LogWarning(ex, "Unauthorized exception occurred: {Message}", ex.Message);

            var problemDetails = new ProblemDetails
            {
                Title = "Unauthorized.",
                Status = StatusCodes.Status401Unauthorized,
                Detail = ex.Message
            };

            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });

            return true;
        }
    }
}