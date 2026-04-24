using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Diagnostics;

namespace API.Exceptions
{
    public class PostgresExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<PostgresExceptionHandler> logger)
        : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not PostgresException ex || ex.SqlState != "23505")
                return false;

            logger.LogWarning(ex, "Database conflict occurred (Unique constraint violation): {Message}", ex.Message);

            var problemDetails = new ProblemDetails
            {
                Title = "Conflict occurred.",
                Status = StatusCodes.Status409Conflict,
                Detail = "A record with the same unique value (like Name or Slug) already exists.",
                Extensions = { ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier }
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
