using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Exceptions
{
    public class ValidationExceptionHandler(
            IProblemDetailsService problemDetailsService,
            ILogger<ValidationExceptionHandler> logger)
            : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not ValidationException ex)
                return false;

            logger.LogWarning(ex, "Validation exception occurred: {Message}", ex.Message);

            var problemDetails = new ValidationProblemDetails(
                ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray())
            )
            {
                Title = "Validation error occurred.",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message
            };

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