using Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Exceptions
{
    public class EmailNotConfirmedExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<EmailNotConfirmedExceptionHandler> logger)
        : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not EmailNotConfirmedException ex)
                return false;

            logger.LogWarning(ex, "Email not confirmed exception occurred: {Message}", ex.Message);

            var problemDetails = new ProblemDetails
            {
                Title = "Email Not Confirmed.",
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
