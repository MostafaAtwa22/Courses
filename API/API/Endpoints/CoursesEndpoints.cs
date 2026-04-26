using Application.Features.Courses.Queries.GetAll;
using Application.Features.Courses.Queries.GetById;
using Application.DTOs.Course;
using Application.Features.Courses.Commands.Create;
using Application.Features.Courses.Commands.Update;
using Application.Features.Courses.Commands.Delete;

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

            group.MapPost("/", CreateCourse)
                .WithName(nameof(CreateCourse))
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            
            group.MapPut("/{id:guid}", UpdateCourse)
                .WithName(nameof(UpdateCourse))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
            
            group.MapDelete("/{id:guid}", DeleteCourse)
                .WithName(nameof(DeleteCourse))
                .Produces(StatusCodes.Status204NoContent)
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

        public static async Task<Results<CreatedAtRoute<CoursesResponseDto>, BadRequest>> CreateCourse(
            CourseCreateDto request, IMediator mediator) 
        {
            var id = await mediator.Send(new CreateCourseCommand(request));
            var course = await mediator.Send(new GetCourseByIdQuery(id));

            return TypedResults.CreatedAtRoute(course!, nameof(GetCourseById), new {id});
        }

        public static async Task<Results<NoContent, NotFound>> UpdateCourse(
            Guid id, CourseUpdateDto request, IMediator mediator)
        {
            await mediator.Send(new UpdateCourseCommand(id, request));

            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, NotFound>> DeleteCourse(
            Guid id, IMediator mediator)
        {
            await mediator.Send(new DeleteCourseCommand(id));

            return TypedResults.NoContent();
        }
    }
}