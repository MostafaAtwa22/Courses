using Application.Features.Progress.Commands.MarkComplete;
using Application.Features.Progress.Commands.MarkIncomplete;
using Application.Features.Progress.Queries.GetCourseProgress;
using Application.Features.Progress.Queries.GetMyCoursesProgress;
using Application.DTOs.Progress;

namespace API.Endpoints
{
    public class ProgressEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/progress")
                .WithTags("Progress");

            group.MapGet("/course/{courseId:guid}", GetCourseProgress)
                .WithName(nameof(GetCourseProgress))
                .Produces<CourseProgressDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status403Forbidden)
                .RequireAuthorization(policy =>
                    policy.RequireRole(Role.Student.ToString()));

            group.MapGet("/my-courses", GetMyCoursesProgress)
                .WithName(nameof(GetMyCoursesProgress))
                .Produces<IReadOnlyList<CourseProgressSummaryDto>>(StatusCodes.Status200OK)
                .RequireAuthorization(policy =>
                    policy.RequireRole(Role.Student.ToString()));

            group.MapPost("/complete", MarkContentComplete)
                .WithName(nameof(MarkContentComplete))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status403Forbidden)
                .RequireAuthorization(policy =>
                    policy.RequireRole(Role.Student.ToString()));

            group.MapPost("/incomplete", MarkContentIncomplete)
                .WithName(nameof(MarkContentIncomplete))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status403Forbidden)
                .RequireAuthorization(policy =>
                    policy.RequireRole(Role.Student.ToString()));
        }

        public static async Task<Ok<CourseProgressDto>> GetCourseProgress(Guid courseId, IMediator mediator)
        {
            var result = await mediator.Send(new GetCourseProgressQuery(courseId));
            return TypedResults.Ok(result);
        }

        public static async Task<Ok<IReadOnlyList<CourseProgressSummaryDto>>> GetMyCoursesProgress(IMediator mediator)
        {
            var result = await mediator.Send(new GetMyCoursesProgressQuery());
            return TypedResults.Ok(result);
        }

        public static async Task<NoContent> MarkContentComplete(MarkProgressRequestDto dto, IMediator mediator)
        {
            await mediator.Send(new MarkContentCompleteCommand(dto));
            return TypedResults.NoContent();
        }

        public static async Task<NoContent> MarkContentIncomplete(MarkProgressRequestDto dto, IMediator mediator)
        {
            await mediator.Send(new MarkContentIncompleteCommand(dto));
            return TypedResults.NoContent();
        }
    }
}
