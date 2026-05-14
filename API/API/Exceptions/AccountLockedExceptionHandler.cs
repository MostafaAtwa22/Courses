using Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Exceptions
{
    public class AccountLockedExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<AccountLockedExceptionHandler> logger)
        : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not AccountLockedException ex)
                return false;

            logger.LogWarning(ex, "Account locked exception occurred: {Message}", ex.Message);

            var problemDetails = new ProblemDetails
            {
                Title = "Account Locked.",
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
