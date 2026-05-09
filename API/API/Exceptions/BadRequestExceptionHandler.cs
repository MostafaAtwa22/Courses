using Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Exceptions
{
    public class BadRequestExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<BadRequestExceptionHandler> logger)
        : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not BadRequestException ex)
                return false;

            logger.LogWarning(ex, "Bad request exception occurred: {Message}", ex.Message);

            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message
            };

            if (ex.Errors.Any())
            {
                problemDetails.Extensions.Add("errors", ex.Errors);
            }

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });

            return true;
        }
    }
}
