using Application.Features.Courses.Queries.GetAll;
using Application.Features.Courses.Queries.GetById;
using Application.DTOs.Course;

namespace API.Endpoints
{
    public class CoursesEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/courses")
                .WithTags("Courses");

            group.MapGet("/", GetCourses)
                .WithName(nameof(GetCourses))
                .Produces<PaginatedResult<CoursesResponseDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/{id:guid}", GetCourseById)
                .WithName(nameof(GetCourseById))
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
        }

        public static async Task<Results<Ok<PaginatedResult<CoursesResponseDto>>, BadRequest>> GetCourses([AsParameters] QueryParams queryParams, 
            IMediator mediator)
        {
            var result = await mediator.Send(new GetCoursesQuery(queryParams));
            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok<CoursesResponseDto>, NotFound>> GetCourseById(Guid id, IMediator mediator)
        {
            var result = await mediator.Send(new GetCourseByIdQuery(id));
            return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
        }
    }
}