using Application.DTOs.InstructorDashboard;
using Application.Features.InstructorDashboard.Queries.GetInstructorStatistics;
using Application.Features.Instructors.Queries.GetCurrentInstructor;
using Domain.Enums.Identity;

namespace API.Endpoints
{
    public class InstructorDashboardEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/instructor/dashboard")
                .WithTags("InstructorDashboard")
                .RequireAuthorization(policy =>
                    policy.RequireRole(Role.Instructor.ToString()));

            group.MapGet("/statistics", GetInstructorStatistics)
                .WithName(nameof(GetInstructorStatistics))
                .Produces<InstructorStatisticsDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden);
        }

        public static async Task<IResult> GetInstructorStatistics(IMediator mediator)
        {
            // Get current instructor
            var currentInstructor = await mediator.Send(new GetCurrentInstructorQuery());
            if (currentInstructor is null)
            {
                return TypedResults.NotFound();
            }

            // Get instructor statistics
            var result = await mediator.Send(new GetInstructorStatisticsQuery(currentInstructor.Id));
            return TypedResults.Ok(result);
        }
    }
}